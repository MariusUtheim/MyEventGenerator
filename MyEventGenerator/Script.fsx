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
#load "PartonShower.fs"
#load "Decays.fs"
#load "Jets.fs"
#load "HeavyIon.fs"
open MyEventGenerator
open Particles


let masses = [ 100.; 60.; 20. ]
let [ p1; p2; p3] = PhaseSpace.sample 250. masses
printfn "%A" (p1, p2, p3)
printfn "%A" (p1 + p2 + p3)

HardProcess.produce Up 92.0
|> PartonShower.radiate 0.15 (0., 0.99) 1.0
|> Event.Print


let particles = [
    (-0.5, 10.0);
    (0.0, 80.0);
    (0.4, 60.0);
]

let R = 0.6;
printfn "kT: %A\nCambridge/Aachen: %A\nanti-kT: %A" <| Jets.kT R particles <| Jets.CambridgeAchen R particles <| Jets.antikT R particles
