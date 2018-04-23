namespace MyEventGenerator
open Particles

module Decays =

    let private exponentialDistributionSample lifetime = -lifetime * log(1. - MonteCarlo.rand())

    let private lifetimes = 
        dict [ 
            (Muon, 2.1969811E-6);
            (~~Muon, 2.1969811E-6);
            (Tau, 290.3E-15);
            (~~Tau, 290.3E-15);
        ]
    let lifetime particleType = 
        match lifetimes.TryGetValue particleType with
        | true, value -> value
        | false, _ -> infinity
       

    let private decayModess =
        dict [

            (Muon, [ ([ElectronNeutrino; Electron], 1.) ]);

            (Tau, [ ([ElectronNeutrino; Electron], 0.506); 
                    ([MuonNeutrino; Muon], 0.494)
                  ]);

            (~~Muon, [ ([ElectronNeutrino; ~~Electron], 1.) ]);

            (~~Tau, [ ([ElectronNeutrino; ~~Electron], 0.506); 
                    ([MuonNeutrino; ~~Muon], 0.494)
                  ]);
        ]

    let decayModes particle = 
        match decayModess.TryGetValue particle.Type with
        | true, value -> value
        | false, _ -> []

        

    let private particleDecay distance (particle : Particle) =
        if lifetime particle.Type = infinity then None else
        let timeToDetector = if particle.Velocity = 0. then infinity else (particle.LorentzFactor * distance) / particle.Velocity
        let decayTime = exponentialDistributionSample <| lifetime particle.Type
        
        if decayTime > timeToDetector then 
            None
        else
            let decayProducts = MonteCarlo.pickWeighted <| decayModes particle
            let momenta = PhaseSpace.sample particle.Type.Mass <| List.map (fun p -> p.Mass) decayProducts
            if List.sum momenta <> { X = 0.; Y = 0.; Z = 0. } then
                printfn "Phase space sampled to overall momentum (%A)" <| List.sum momenta

            let comVelocity = Vec4.Velocity particle.FourMomentum
            
            let results = List.zip decayProducts momenta
                          |> List.map (fun (pt, p) -> let p4 = -Vec4.OfVector pt.Mass p
                                                               |> Vec4.Boost comVelocity
                                                      { Type = pt; Momentum = p4.Vec3 })
            
            Some results
        
        
   


    let decay detectorDistance (event : Event) = 
        event.ForAllIteratively (fun entry -> match particleDecay detectorDistance entry.Particle with
                                              | None -> ()
                                              | Some decayProducts -> for product in decayProducts do 
                                                                          event.WriteEntry(product, entry) |> ignore
                                )
        event
                                


    let particleDecayEvent detectorDistance particleType =
        if detectorDistance < 0. then failwith "Detector distance is negative"
        if lifetime particleType = infinity then failwith "Stable particles cannot decay"
        
        let event = Event([{ Type = particleType; Momentum = Vec3.Zero }])
        decay detectorDistance event
        
   