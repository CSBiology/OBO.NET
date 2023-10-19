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
let onto = ontologyToFGraph true obo

printGraph onto

toCyGraph matchRelationColorDefault true onto |> CyGraph.show
toCyGraph matchRelationColorDefault true onto |> CyGraph.withLayout(Layout.initBreadthfirst <| Layout.LayoutOptions.Cose()) |> CyGraph.show

let go = @"C:\Users\olive\Downloads\go.obo"
let gobo = OboOntology.fromFile false go
let gonto = ontologyToFGraph true gobo

toCyGraph matchRelationColorDefault true gonto |> CyGraph.withLayout(Layout.initBreadthfirst <| Layout.LayoutOptions.Cose()) |> CyGraph.show

let goSlimAgr = @"C:\Users\olive\Downloads\goslim_agr.obo"
let goSlimAgrObo = OboOntology.fromFile false goSlimAgr
let goSlimAgrOnto = ontologyToFGraph true goSlimAgrObo

toCyGraph matchRelationColorDefault false goSlimAgrOnto |> CyGraph.show