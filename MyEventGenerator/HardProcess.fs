namespace MyEventGenerator
open Particles

module HardProcess =

    let eebar (energy : float) =
        let particles = [ Electron; Muon; Tau ]
                        |> List.map (fun p -> (p, PhaseSpace.size energy [p.Mass; p.Mass]))
        
        let particleType = MonteCarlo.pickWeighted particles
        let momentum = PhaseSpace.sample energy [particleType.Mass; particleType.Mass]

        Event([
                { Type = particleType; Momentum = momentum.[0]; };
                { Type = particleType; Momentum = momentum.[1]; }
        ])
            
                
