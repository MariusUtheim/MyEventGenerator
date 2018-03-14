namespace MyEventGenerator

module Hadronize =

    let private kappa = 0.197
    let private breakRate = 1.

    let rec private _artruMennessier p1 p2 stringEnergy =
        let comBoost = (p1.Momentum + p2.Momentum) / (p1.Energy + p2.Energy)
        let p1, p2 = Vec4.Boost comBoost <<| (p1.FourMomentum, p2.FourMomentum)

        let totalEnergy = stringEnergy + p1.T + p2.T
        let leg = totalEnergy / kappa / sqrt 2.

        let n = MonteCarlo.poisson (breakRate * leg * leg)
        let t0 = stringEnergy / 2.
        failwith "Not implemented"
        (*
        match Seq.init n (fun _ -> MonteCarlo.rand() * leg, MonteCarlo.rand() * leg)
              |> Seq.map(fun (r, l) -> ((r + l) / sqrt 2., (r - l) / sqrt 2.))
              |> Seq.sortBy fst
              |> Seq.skipWhile (fun (t, _) -> t0 > t)
              |> Seq.tryHead
              with
        | Some (t, z) -> if t < totalEnergy / 2. then
                            ()
                         let left = _artruMennessier 
                         failwith "Not implemented"
        | None -> ()
        *)

    let stringFragmentation (q : Particle) (qbar : Particle) =
        if not q.Type.IsQuark then failwith "Only quarks can participate in string models"
        _artruMennessier q qbar 0.

