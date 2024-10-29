namespace OBO.NET


open Graphoscope
open FSharpAux


module ContextGraph =

    let ontologiesToContextGraph (ontologies : OboOntology seq) =

        let graphedOntos = ontologies |> Seq.map (fun onto -> onto.Ontology, Graph.ontologyToFGraph true onto)
        let gOwithNameNodes = 
            graphedOntos 
            |> Seq.mapi (
                fun i (name,ontoGr) -> 
                    let defName = Option.defaultValue $"unnamed ontology {i}" name
                    let nodeKeys, _ = FGraph.getNodes ontoGr |> Seq.unzip
                    FGraph.addNode defName (OboTerm.Create defName) ontoGr |> ignore
                    let nodeKeysAndEdges = nodeKeys |> Seq.map (fun nk -> defName, nk, "part_of")
                    FGraph.addEdges nodeKeysAndEdges ontoGr
            )

        ontologies
        |> Seq.reducei (
            fun onto1 onto2 ->
                FGraph.addNode (onto.Ontology |> Option.defaultValue $"unnamed Ontology {i}")
        ) headGraph