namespace MyEventGenerator
open MyEventGenerator.Particles

type BranchingMode = { From : ParticleType; 
                       To : ParticleType * ParticleType; 
                       SplittingFunction : float -> float; 
                       Samplez : (float * float) -> float }

module FinalStateRadiation =

    let private cube x = x * x * x

    let private Pqqg (zmin, zmax) =
        MonteCarlo.sampler ((fun z -> (1. + z * z) / (1. - z)),
                            (fun z -> 2. / (1. - z)),
                            (fun () -> (log(1. - MonteCarlo.rand()) - log(1. - zmin)) / (log(1. - zmax) - log(1. - zmin)))) ()

    let private Pggg (zmin, zmax) =
        MonteCarlo.sampler ((fun z -> sqr(1. - z * (1. - z)) / (z * (1. - z))),
                            (fun z -> 1. / (z * (1. - z))),
                            (fun () -> let H r = 1. / (1. - exp r)
                                       (H (MonteCarlo.rand()) - H zmin) / (H zmax - H zmin))
                           ) ()

    let private Pgqq (zmin, zmax) =
        let q z = z ** (1./3.)
        (q(MonteCarlo.rand()) - q(zmin)) / (q(zmax) - q(zmin))

    let private branchingModes = [
        { From = Up; To = (Up, Gluon); 
          SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z));
          Samplez = Pqqg };

        { From = ~~Up; To = (~~Up, Gluon); 
          SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z));
          Samplez = Pqqg };

        { From = Down; To = (Down, Gluon); 
          SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z));
          Samplez = Pqqg };

        { From = ~~Down; To = (~~Down, Gluon); 
          SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z));
          Samplez = Pqqg };

        { From = Gluon; To = (Gluon, Gluon); 
          SplittingFunction = fun z -> 3. * log(z / (1. - z)) - 6. * z + 1.5 * sqr z - cube z;
          Samplez = Pggg }

        { From = Gluon; To = (Up, ~~Up);
          SplittingFunction = fun z -> 1./3. * (cube z - cube(1. - z));
          Samplez = Pgqq }

        { From = Gluon; To = (Down, ~~Down);
          SplittingFunction = fun z -> 1./3. * (cube z - cube(1. - z));
          Samplez = Pgqq }
    ]


    let branch alphaS (zmin, zmax) qmin (particle : Particle) =
        if particle.Energy <= qmin
            then None
        else
        let branchingModes = branchingModes |> List.where(fun mode -> mode.From = particle.Type)
        let Qs = [ 
            for mode in branchingModes do 
                let z = mode.Samplez (zmin, zmax)
                let Iz = alphaS / pi2 * (mode.SplittingFunction zmax - mode.SplittingFunction zmin)
                let Qnext = particle.Energy * MonteCarlo.rand() ** (0.5 / Iz)
                if Qnext > qmin then yield Qnext, (z, mode)
        ] 
        if Qs.IsEmpty then 
            None
        else
            let Q, (z, chosenMode) = Qs |> List.maxBy fst
            let t1, t2 = chosenMode.To
            let pp1, pp2 = z * particle.Momentum, (1. - z) * particle.Momentum
            let pTamount = sqrt(z * (1. - z) * sqr particle.Type.Mass + (1. - z) * sqr t1.Mass + z * sqr t2.Mass)
            let pT = pTamount * MonteCarlo.perpendicularDirection particle.Momentum

            let p1, p2 = pp1 + pT, pp2 - pT

            if p1.Magnitude >= particle.Momentum.Magnitude || p2.Magnitude >= particle.Momentum.Magnitude then
                printfn "Warning: Energy is increasing in %d -> %d + %d (z=%.3f, Q=%.3f)" 
                        chosenMode.From.PdgId t1.PdgId t2.PdgId z Q

            Some ({ Type = t1; Momentum = pp1 + pT}, { Type = t2; Momentum = pp2 - pT })



    let partonShower alphaS (zmin, zmax) qmin (event : Event) =
        let brancher = branch alphaS (zmin, zmax) qmin
        event.ForAllIteratively (fun entry -> 
            match brancher entry.Particle with
            | None -> ()
            | Some (p1, p2) -> event.WriteEntry(p1, entry) |> ignore
                               event.WriteEntry(p2, entry) |> ignore
        )
        event
