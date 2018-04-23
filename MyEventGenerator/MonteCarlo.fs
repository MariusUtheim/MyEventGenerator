namespace MyEventGenerator

module MonteCarlo =

    let private _rng = System.Random()

    let rec rand () = 
        let x = _rng.NextDouble()
        if x <> 0. then x else rand()


    let exponential () = 1. - exp(-rand())

    let gaussian () = sqrt(-2. * log(rand())) * cos(pi2 * rand())

    let poisson mean () =
        let threshold = rand()
        let rec loop sum term n =
            if sum + term >= threshold then n
            else loop (sum + term) (term * mean / float(n + 1)) (n + 1)
        loop 0. (exp (-mean)) 0


    let sampler (distribution, envelope, envelopeSampler) =
        let rec f () =
            let x = envelopeSampler()
            if rand() * envelope(x) < distribution(x) then x else f()
        f


    let pick elements =
        let index = _rng.Next(List.length elements)
        List.item index elements


    let pickWeighted elementsWithWeight =
        let threshold = rand() * (elementsWithWeight |> List.sumBy snd)
        let rec accumulate cumulative lst =
            match lst with
            | (element, weight) :: remainingElements -> if cumulative + weight >= threshold then element
                                                        else accumulate (cumulative + weight) remainingElements
            | [] -> failwith "Error: Passed through all elements, but still couldn't reach threshold."
        accumulate 0.0 elementsWithWeight
