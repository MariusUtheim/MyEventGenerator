namespace MyEventGenerator
open MyEventGenerator.Particles

type BranchingMode = { From : ParticleType; To : ParticleType * ParticleType; SplittingFunction : float -> float; }

module PartonShower =

    let private cube x = x * x * x
    let private branchingModes = [
        { From = Up; To = (Up, Gluon); SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z)) };
        { From = ~~Up; To = (~~Up, Gluon); SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z)) };
        { From = Down; To = (Down, Gluon); SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z)) };
        { From = ~~Down; To = (~~Down, Gluon); SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z)) };
        //{ From = Gluon; To = (Gluon, Gluon); SplittingFunction = fun z -> 3. * log(z / (1. - z)) - 6. * z + 1.5 * sqr z - cube z }
        //{ From = Gluon; To = (Up, Up); SplittingFunction = fun z -> 1./3. * (cube z - cube(1. - z)) }
        //{ From = Gluon; To = (Down, Down); SplittingFunction = fun z -> 1./3. * (cube z - cube(1. - z)) }
    ]


    let finalStateRadiation alpha (zmin, zmax) qmin (particle : Particle) =
        
        let rec f q particle =
            
            let branch iz =
                sqrt(q * q * (MonteCarlo.rand() ** (1. / iz)))
       
            let (thisType, thatType), nextQ =
                    branchingModes
                    |> List.choose(fun m -> if m.From <> particle.Type then None
                                            else Some (m.To, alpha / pi2 * (m.SplittingFunction zmax - m.SplittingFunction zmin)))
                    |> List.map(fun (t, iz) -> (t, branch iz))
                    |> List.maxBy snd
            
            if nextQ > q then failwith "Virtuality is increasing" 

            if nextQ < qmin then [ (particle, q) ]
            else //printfn "%A(%.2f) -> %i(%.2f) + %i(%.2f)" particle.Type.PdgId q thisType.PdgId nextQ thatType.PdgId (q - nextQ)
                 ({ Type = thatType; Momentum = (1. - nextQ / q) * particle.Momentum }, (q - nextQ))
                 :: (f nextQ { Type = thisType; Momentum = (nextQ / q) * particle.Momentum }); 
                               

        f particle.Energy particle
    

    let radiate alpha (zmin, zmax) qmin (event : Event) =
        let r = finalStateRadiation alpha (zmin, zmax) qmin
        event.ForAllOnce (fun entry -> if List.exists (fun mode -> mode.From = entry.Particle.Type) branchingModes then
                                           let products = r entry.Particle
                                           if products.Length > 1 then
                                               for (product, _) in products do
                                                   event.WriteEntry(product, entry) |> ignore)
        event