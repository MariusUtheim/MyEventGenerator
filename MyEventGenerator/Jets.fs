namespace MyEventGenerator


module Jets =

    type private Jet = { Subjets : System.Collections.Generic.List<float * float> }



    let rec private jetAlgorithm alpha (R : float) particles =
        match particles with
        | [] -> []
        | [ p ] -> [ p ]
        | _ -> let (ij, dij) = particles
                               |> List.pairwise
                               |> List.map(fun ((y1, p1), (y2, p2)) -> min (p1 ** (2. * alpha)) (p2 ** (2. * alpha)) 
                                                                        * sqr(y1 - y2) / sqr R)
                               |> List.indexed
                               |> List.minBy snd

               let (iB, diB) = particles
                               |> List.map(fun (y, p) -> p ** (2. * alpha))
                               |> List.indexed
                               |> List.minBy snd

               if dij < diB then
                   let left, right = particles |> List.splitAt ij
                   let (y1, p1), (y2, p2), right = right.[0], right.[1], right.Tail.Tail
                   let m = ((y1 * p1 + y2 * p2) / (p1 + p2), p1 + p2)
                   printfn "Recombining %A x %A" left right
                   jetAlgorithm alpha R (left @ (m :: right))
               else 
                   let left, right = List.splitAt iB particles
                   let m, right = right.Head, right.Tail
                   printfn "Fixing %A" m
                   (jetAlgorithm alpha R left) @ (m :: (jetAlgorithm alpha R right))
     

    let kT R particles = jetAlgorithm 1. R particles


    let antikT R particles = jetAlgorithm -1. R particles

    let CambridgeAchen R particles = jetAlgorithm 0. R particles