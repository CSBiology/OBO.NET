#I "src/OBO.NET/bin/Debug/netstandard2.0"
#I "src/OBO.NET/bin/Release/netstandard2.0"
#r "OBO.NET.dll"
#I "src/OBO.NET.Graph/bin/Debug/netstandard2.0"
#I "src/OBO.NET.Graph/bin/Release/netstandard2.0"
#r "OBO.NET.Graph.dll"

#r "nuget: Graphoscope"
#r "nuget: Isadotnet"
#r "nuget: Cytoscape.NET"


open OBO.NET
open OBO.NET.Graph

open Graphoscope
open Cytoscape.NET


let oboPath = @"C:\Repos\nfdi4plants\ARCTokenization\src\ARCTokenization\structural_ontologies\investigation_metadata_structural_ontology.obo"
let obo = OboOntology.fromFile false oboPath
let onto = OBO.NET.Graph.ontologyToFGraph obo

let printGraph transformFunction (graph : FGraph<_,_,_>) =
    for (nk1,nd1,nk2,nd2,e) in FGraph.toSeq graph do
        let nk1s = sprintf "%s" (transformFunction nd1)
        let nk2s = sprintf "%s" (transformFunction nd2)
        printfn "%s ---%A---> %s" nk1s e nk2s

printGraph (fun (t : OboTerm) -> t.Name) onto

let toFullCyGraph nodeKeyTransformer nodeDataTransformer edgeTransformer (fGraph : FGraph<_,_,_>) =
    CyGraph.initEmpty ()
    |> CyGraph.withElements [
            for (nk1,nd1,nk2,nd2,e) in FGraph.toSeq fGraph do
                let nk1s = nodeKeyTransformer nk1
                let nk2s = nodeKeyTransformer nk2
                Elements.node nk1s [CyParam.label <| nodeDataTransformer nd1]
                Elements.node nk2s [CyParam.label <| nodeDataTransformer nd2]
                Elements.edge (sprintf "%s_%s" nk1s nk2s) nk1s nk2s (edgeTransformer e)
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
    //|> CyGraph.withLayout (Layout.initCose <| Layout.LayoutOptions.Cose(ComponentSpacing = 40, EdgeElasticity = 100))

let ontoGraphToFullCyGraph graph =
    toFullCyGraph 
        (sprintf "%s") 
        (fun (d : OboTerm) -> d.Name)
        (fun e -> 
            [
                CyParam.label e
                match e with
                | "follows" -> CyParam.color "red"
                | "part_of" -> CyParam.color "blue"
                | _ -> CyParam.color "black"
                //| x when x = ArcRelation.PartOf + ArcRelation.Follows -> CyParam.color "purple"
            ]
        )
        graph
    //|> CyGraph.withLayout (Layout.initCose <| Layout.LayoutOptions.Cose(ComponentSpacing = 40, EdgeElasticity = 100))
    |> CyGraph.withLayout(Layout.initBreadthfirst <| Layout.LayoutOptions.Cose())

ontoGraphToFullCyGraph onto |> CyGraph.show