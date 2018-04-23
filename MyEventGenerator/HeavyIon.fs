namespace MyEventGenerator


module HeavyIon =

    let private a = 0.546
    let private R = 6.62
    let private nParticles = 208
    let private dsqr = 6.4 / 3.14159
    type private Nucleon = { mutable Wounded : bool; Offset : Vec3 }

    let rDistribution r =
        if r > 13. then 0.
        else r * r * (1. + (r * r) / (R * R)) / (1. + exp((r - R) / a))     
    
    let envelope r =
        210. / sqrt(2.8 * pi2) * exp(-sqr(r - 6.) / (2.8 * 2.8))

    let rec envelopeSampler () =
        let f = (MonteCarlo.gaussian() * 2.8) + 6.
        if f >= 0. then f else envelopeSampler()

    let sampleb () = R * sqrt(MonteCarlo.rand())
       
    let sampleR = MonteCarlo.sampler (rDistribution, envelope, envelopeSampler)

    let private sampleNucleon b =
        let r = sampleR()
        let d = r * PhaseSpace.sampleUnitSphere()
        { Wounded = false; Offset = { d with X = d.X + b } }

    let GlauberPbPb b =
        let left = [ for _ in 1 .. nParticles -> sampleNucleon 0. ] |> List.sortBy (fun n -> n.Offset.Z)
        //let b = sampleb()
        let right = [ for _ in 1 .. nParticles -> sampleNucleon b ] |> List.sortByDescending (fun n -> n.Offset.Z)

        let mutable count = 0

        let pluck n =
            match right |> List.tryFind (fun n' -> not n'.Wounded && sqr(n.Offset.X - n'.Offset.X) + sqr(n.Offset.Y - n'.Offset.Y) < dsqr) with
            | Some n' -> n'.Wounded <- true
                         1
            | None -> 0

        left
        |> List.map pluck
        |> List.sum