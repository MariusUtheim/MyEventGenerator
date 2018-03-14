namespace MyEventGenerator
open Particles

module Decays =

    let private exponentialDistributionSample lifetime = -lifetime * log(1. - MonteCarlo.rand())

    let private lifetimes = 
        dict [ 
            (Muon, 2.1969811E-6);
            (Tau, 290.3E-15);
        ]
    let lifetime particleType = 
        match lifetimes.TryGetValue particleType with
        | true, value -> value
        | false, _ -> infinity
       

    let private decayModess =
        dict [
            (Muon, [ ([Electron; ElectronNeutrino; MuonNeutrino], 1.) ]);
            (Tau, [ ([Electron; ElectronNeutrino; TauNeutrino], 0.506); 
                    ([Muon; MuonNeutrino; TauNeutrino], 0.494)
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
                                |> List.mapi (fun i t -> { Type = t;
                                                           Momentum = particle.Momentum; })
            printfn "Decay %A -> %A" particle.Type.PdgId (List.map (fun product -> product.Type.PdgId) decayProducts)
            Some decayProducts
        
        
   


    let decay detectorDistance (event : Event) = 
        event.ForAllIteratively (fun entry -> match particleDecay detectorDistance entry.Particle with
                                              | None -> ()
                                              | Some decayProducts -> for product in decayProducts do event.WriteEntry(product, entry) |> ignore
                                )
        event
                                


    let particleDecayEvent detectorDistance particleType =
        if detectorDistance < 0. then failwith "Detector distance is negative"
        if lifetime particleType = infinity then failwith "Stable particles cannot decay"
        
        let event = Event([{ Type = particleType; Momentum = Vec3.Zero }])
        decay detectorDistance event
        
   