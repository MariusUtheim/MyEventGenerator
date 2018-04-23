// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Util.fs"
#load "Vector.fs"
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


let particles = [
    (-0.5, 10.0);
    (0.0, 80.0);
    (0.4, 60.0);
]

let R = 0.5;
Jets.kT R particles, Jets.antikT R particles, Jets.CambridgeAchen R particles
