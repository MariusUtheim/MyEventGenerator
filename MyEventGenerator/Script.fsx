// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Util.fs"
#load "Vector.fs"
#load "Matrix.fs"
#load "ParticleType.fs"
#load "Particle.fs"
#load "Event.fs"
#load "MonteCarlo.fs"
#load "PhaseSpace.fs"
#load "HardProcess.fs"
#load "FinalStateRadiation.fs"
#load "Decays.fs"
#load "Jets.fs"
#load "HeavyIon.fs"
open MyEventGenerator
open Particles


HardProcess.produce Up 92.0
|> FinalStateRadiation.partonShower 0.15 (0., 0.99) 1.0
|> Event.Print


let particles = [
    (-0.5, 10.0);
    (0.0, 80.0);
    (0.4, 60.0);
]

let R = 0.6;
printfn "kT: %A\nCambridge/Aachen: %A\nanti-kT: %A" <| Jets.kT R particles <| Jets.CambridgeAchen R particles <| Jets.antikT R particles


let f z = -2./3. * (z * (z + 2.) + 4. * log(1. - z))
f 0.99999