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
#load "Jets.fs"
open MyEventGenerator
open Particles

let partons = [
    (0.4, 5.0);
    (1.05, 12.0);
    (1.5, 5.05);
    (1.8, 10.5);
    (2.15, 8.0);
    (2.6, 13.0);
    (2.95, 7.5);
    (3.4, 18.0);
    (3.75, 4.0);
    (3.85, 0.25);
]

Jets.kT 1.0 partons
Jets.antikT 1.0 partons
Jets.CambridgeAchen 1.0 partons
