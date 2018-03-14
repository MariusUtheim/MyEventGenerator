﻿namespace MyEventGenerator
open System

module PhaseSpace =

    let private polarAngleSampler =
        MonteCarlo.sampler((fun x -> 1. + sqr(cos x)),
                           (fun x -> 2.),
                           (fun () -> (2. * Math.PI * MonteCarlo.rand())))

    let private azimuthalSampler () = 2. * Math.PI * MonteCarlo.rand()                        

    let private kallen (x, y, z) = sqr(x - y - z) - 4. * y * z

    let dbl x = (x, x)

    let private uniformSample M masses () =
        let partialSums, massSum = List.mapFold (fun s e -> dbl(s + e)) 0. masses
        let D = M - massSum
        let rs = List.init (masses.Length - 2) (fun _ -> MonteCarlo.rand()) |> List.sort

        ()


    let sampleUnitSphere () =
        let polar = polarAngleSampler()
        let azimuth = azimuthalSampler()
        { 
            X = cos azimuth * cos polar; 
            Y = sin azimuth * cos polar;
            Z = sin polar 
        }

    let size energy masses =
        match masses with
        | [] | [ _ ] -> 0.
        | [ m1; m2 ] -> if m1 + m2 >= energy then 0.
                        else let (m1sqr, m2sqr, ssqr) = (m1 * m1, m2 * m2, pown energy 4)
                             let psqr = sqr(m1sqr - m2sqr) / (4. * ssqr) + 2. * (m1sqr + m2sqr) + ssqr / 4.
                             sqrt(psqr) / (4. * Math.PI * energy)
        | _ -> failwith "Not implemented"


    let sample energy masses =
        match masses with
        | [] -> []
        | [ m ] when energy = 0. -> [{ X = 0.; Y = 0.; Z = 0. }]
        | [ m ] when energy <> 0. -> failwith "Available phase space is empty"
        | [ m1; m2 ] -> if m1 + m2 >= energy then failwith "Unable to sample phase space"
                        else let s = energy * energy
                             let pnorm = sqrt <| sqr(m1 * m1 - m2 * m2) / (4. * s * s) 
                                                 + 2. * (m1 * m1 + m2 * m2) + s * s / 4.
                             let p = pnorm * sampleUnitSphere()
                             [ p; -p ]
        | masses -> failwith "Not implemented"
                    

    

    let private _toCenterOfMass vectors =
        let momentumSum = List.sum vectors
        vectors |> List.map (Vec3.ToFourVector 0. >> Vec4.Boost momentumSum)

    let private _scaleTo energy vectors =
        let scaleFactor = energy / sqrt(vectors |> List.sumBy Vec4.Square)
        vectors |> List.map ((*) scaleFactor)

    let randomMomentaBoost energy nVectors () = 
        List.init nVectors (fun _ -> MonteCarlo.exponential() * sampleUnitSphere())
        |> _toCenterOfMass
        |> _scaleTo energy
        