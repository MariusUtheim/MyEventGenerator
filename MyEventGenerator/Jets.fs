namespace MyEventGenerator


module Jets =

    type private Jet = { Subjets : System.Collections.Generic.List<float * float> }



    let rec private jetAlgorithm distanceMeasure (R : float) particles =
        match particles with
        | [] -> []
        | [ p ] -> [ p ]
        | _ -> let dij = particles
                         |> List.pairwise
                         |> List.map(fun ((y1, p1), (y2, p2)) -> distanceMeasure (p1, p2) * sqr(y1 - y2) / sqr R)
                         |> List.indexed
                         |> List.minBy snd

               let diB = particles |> List.map snd |> List.map sqr |> List.indexed |> List.minBy snd

               if snd dij < snd diB then
                   let left, right = List.splitAt (fst dij) particles
                   let (y1, p1), (y2, p2), right = right.Head, right.Tail.Head, right.Tail.Tail
                   let m = ((y1 * p1 + y2 * p2) / (p1 + p2), p1 + p2)
                   jetAlgorithm distanceMeasure R (left @ (m :: right))
               else 
                   let left, right = List.splitAt (fst diB) particles
                   let m, right = right.Head, right.Tail
                   (jetAlgorithm distanceMeasure R left) @ (m :: (jetAlgorithm distanceMeasure R right))
     

    let private dkT (p1, p2) = min (p1 * p1) (p2 * p2)
    let kT R particles = jetAlgorithm dkT R particles


    let private dantikT (p1, p2) = 1. / max (p1 * p1) (p2 * p2)
    let antikT R particles = jetAlgorithm dantikT R particles

    let CambridgeAchen R particles = jetAlgorithm (fun _ -> 1.) R particles