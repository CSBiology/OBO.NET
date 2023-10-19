namespace OBO.NET


open OBO.NET

open Graphoscope
open Cytoscape.NET

/// Functions for working with ontology-based FGraphs.
module Graph =

    /// Takes an OboOntology and returns an FGraph with OboTerms as nodes and their relations as Edges. The structure of the graph results from the TermRelations between the ontology's terms.
    let ontologyToFGraph onto =
        OboOntology.getRelations onto
        |> Seq.fold (
            fun acc termRel -> 
                match termRel with
                | Empty target -> acc
                | TargetMissing (rel,target) -> acc
                | Target (rel,sourceTerm,targetTerm) ->
                    FGraph.addElement sourceTerm.Id sourceTerm targetTerm.Id targetTerm rel acc
        ) FGraph.empty<string,OboTerm,string>


    /// Functions for visualizing OboTerm FGraphs.
    module Visualization =

        /// Takes an OboTerm FGraph and prints all its term names and their relations.
        let printGraph transformFunction (graph : FGraph<string,OboTerm,string>) =
            for (nk1,nd1,nk2,nd2,e) in FGraph.toSeq graph do
                let nk1s = sprintf "%s" nd1.Name
                let nk2s = sprintf "%s" nd2.Name
                printfn "%s ---%A---> %s" nk1s e nk2s

        /// Takes an OboTerm FGraph and returns a CyGraph according to its structure. Uses relationColor function to match a specific relation to a specific color.
        let toCyGraph (relationColor : string -> CyParam.CyStyleParam) (oboGraph : FGraph<string,OboTerm,string>) =
            CyGraph.initEmpty ()
            |> CyGraph.withElements [
                    for (nk1,nd1,nk2,nd2,e) in FGraph.toSeq oboGraph do
                        Elements.node nk1 [CyParam.label nd1.Name]
                        Elements.node nk2 [CyParam.label nd2.Name]
                        let edgeLabel = [
                            CyParam.label e
                            relationColor e
                        ]
                        Elements.edge (sprintf "%s_%s" nk1 nk2) nk1 nk2 edgeLabel
                ]
            |> CyGraph.withStyle "node"
                [
                    CyParam.content =. CyParam.label
                    CyParam.color "#A00975"
                ]
            |> CyGraph.withStyle "edge"
                [
                    //CyParam.content =. CyParam.label
                    CyParam.Line.color =. CyParam.color
                ]
            //|> CyGraph.withLayout(Layout.initCose <| Layout.LayoutOptions.Cose(ComponentSpacing = 40, EdgeElasticity = 100))
            //|> CyGraph.withLayout(Layout.initBreadthfirst <| Layout.LayoutOptions.Cose())

        /// Matches a term relation (as string) to a specific predefined color. When calling `toCyGraph`, use this for a simple edge visualization if you don't want to specify your own function.
        let matchRelationColorDefault rel =
            match rel with
            | "follows" -> CyParam.color "red"
            | "part_of" -> CyParam.color "blue"
            | "is_a" -> CyParam.color "orange"
            | "synonym" -> CyParam.color "purple"
            | "xref" -> CyParam.color "yellow"
            | _ -> CyParam.color "black"