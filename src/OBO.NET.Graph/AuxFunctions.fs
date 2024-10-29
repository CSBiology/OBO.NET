namespace OBO.NET


[<AutoOpen>]
module AuxFunctions =

    module Seq =

        let reducei (reduction : int -> 'T -> 'T -> 'T) source =
            let mutable i = 0
            Seq.reduce (
                fun a b -> 
                    i <- i + 1
                    reduction i a b
            ) source