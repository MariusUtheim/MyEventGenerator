namespace MyEventGenerator
open Particles

module HardScattering =



    let eebar (energy : float) =
        let particles = [ Electron; Muon; Tau ]
                        |> List.map (fun p -> (p, PhaseSpace.size energy [p.Mass; p.Mass]))
        let particleSampler () = MonteCarlo.pickWeighted particles

        fun () ->
            let particleType = particleSampler()
            let momentum = PhaseSpace.sample energy [ particleType.Mass; particleType.Mass ]
            Event([
                    { Type = particleType; Momentum = momentum.[0]; };
                    { Type = particleType; Momentum = momentum.[1]; }
            ])
            
                
