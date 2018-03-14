namespace MyEventGenerator

[<AutoOpen>]
module Util =
    
    let inline sqr x = x * x

    let pi2 = 6.283185307179586476925286766559

    let (<<|) f (x, y) = (f x, f y)