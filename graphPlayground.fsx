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
open OBO.NET.Graph.Visualization

open Graphoscope
open Cytoscape.NET


let oboPath = @"C:\Repos\nfdi4plants\ARCTokenization\src\ARCTokenization\structural_ontologies\investigation_metadata_structural_ontology.obo"
let obo = OboOntology.fromFile false oboPath
let onto = ontologyToFGraph obo

printGraph (fun (t : OboTerm) -> t.Name) onto

toCyGraph matchRelationColorDefault onto |> CyGraph.show
toCyGraph matchRelationColorDefault onto |> CyGraph.withLayout(Layout.initBreadthfirst <| Layout.LayoutOptions.Cose()) |> CyGraph.show

let go = @"C:\Users\olive\Downloads\go.obo"
let gobo = OboOntology.fromFile false go
let gonto = ontologyToFGraph gobo

toCyGraph matchRelationColorDefault gonto |> CyGraph.withLayout(Layout.initBreadthfirst <| Layout.LayoutOptions.Cose()) |> CyGraph.show