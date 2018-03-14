// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Util.fs"
#load "Vector.fs"
#load "ParticleType.fs"
#load "Particle.fs"
#load "Event.fs"
#load "MonteCarlo.fs"
#load "PhaseSpace.fs"
#load "HardScattering.fs"
#load "PartonShower.fs"
#load "Decays.fs"
open MyEventGenerator
open Particles

[ for _ in 1 .. 100000 -> MonteCarlo.poisson 2. () ] |> List.map float |> List.average


PartonShower.finalStateRadiation 0.15 (0.01, 0.99) (92.0, 1.0) Up 
|> List.map (fun (p, q) -> (p.PdgId, q))

Decays.particleDecayEvent 1.0 Particles.Tau
|> Event.Print


 