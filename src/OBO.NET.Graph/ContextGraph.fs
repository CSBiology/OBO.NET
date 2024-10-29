namespace OBO.NET


open Graphoscope
open FSharpAux


module ContextGraph =

    let ontologiesToContextGraph (ontologies : OboOntology seq) =

        //let graphedOntos = ontologies |> Seq.map (fun onto -> onto.Ontology, Graph.ontologyToFGraph true onto)
        //let gOwithNameNodes = 
        //    graphedOntos 
        //    |> Seq.mapi (
        //        fun i (name,ontoGr) -> 
        //            let defName = Option.defaultValue $"unnamed ontology {i}" name
        //            let nodeKeys, _ = FGraph.getNodes ontoGr |> Seq.unzip
        //            FGraph.addNode defName (OboTerm.Create defName) ontoGr |> ignore
        //            let nodeKeysAndEdges = nodeKeys |> Seq.map (fun nk -> defName, nk, "part_of")
        //            FGraph.addEdges nodeKeysAndEdges ontoGr
        //    )

        let allTerms = ontologies |> Seq.collect (fun o -> o.Terms)

        let graph = FGraph.empty<string,OboTerm,string>

        ontologies
        |> Seq.iter (
            fun oo ->
                oo.Terms
                |> Seq.iter (fun ot -> FGraph.addNode ot.Id ot |> ignore)
        )

        FGraph.mapNodeData graph (
            fun nd -> 
                nd.Relationships
                |> Seq.map (
                    fun r -> 
                        OboTerm.deconstructRelationship r
                )
        )

        //let contextNode = FGraph.addNode "ontology context" (OboTerm.Create "ontology context") (Seq.head gOwithNameNodes)
        
        //gOwithNameNodes
        //|> Seq.reduce (
        //    fun onto1 onto2 ->
        //        FGraph.
        //)