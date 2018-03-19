namespace MyEventGenerator
open MyEventGenerator.Particles

type BranchingMode = { From : ParticleType; To : ParticleType * ParticleType; SplittingFunction : float -> float; }

module PartonShower =

    let private cube x = x * x * x
    let private branchingModes = [
        { From = Up; To = (Up, Gluon); SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z)) };
        { From = Down; To = (Down, Gluon); SplittingFunction = fun z -> -2./3. * (z * (z + 2.) + 4. * log(1. - z)) };
        { From = Gluon; To = (Gluon, Gluon); SplittingFunction = fun z -> 3. * log(z / (1. - z)) - 6. * z + 1.5 * sqr z - cube z }
        { From = Gluon; To = (Up, Up); SplittingFunction = fun z -> 1./3. * (cube z - cube(1. - z)) }
        { From = Gluon; To = (Down, Down); SplittingFunction = fun z -> 1./3. * (cube z - cube(1. - z)) }
    ]


    let finalStateRadiation alpha (zmin, zmax) (qmax, qmin) particle =
        
        let rec f q particle =
        
            let branch iz =
                sqrt(q * q * (MonteCarlo.rand() ** (1. / iz)))
       
            let (this, that), nextQ =
                    branchingModes
                    |> List.choose(fun m -> if m.From <> particle then None
                                            else Some (m.To, alpha / pi2 * (m.SplittingFunction zmax - m.SplittingFunction zmin)))
                    |> List.map(fun (t, iz) -> (t, branch iz))
                    |> List.maxBy snd
        
            if nextQ < qmin then [ (particle, q) ]
            else printfn "%A(%.2f) -> %i(%.2f) + %i(%.2f)" particle.PdgId q this.PdgId nextQ that.PdgId (q - nextQ)
                 List.concat [ f nextQ this; f (q - nextQ) that ]

        f qmax particle
        