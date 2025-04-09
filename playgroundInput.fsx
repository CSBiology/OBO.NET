#I "src/OBO.NET/bin/Debug/netstandard2.0"
#I "src/OBO.NET/bin/Release/netstandard2.0"
#I "src/OBO.NET.CodeGeneration/bin/Debug/netstandard2.0"
#I "src/OBO.NET.CodeGeneration/bin/Release/netstandard2.0"

#r "OBO.NET.dll"
//#r "OBO.NET.CodeGeneration.dll"

#r "nuget: ARCtrl.Core"
#r "nuget: FSharpAux"

open OBO.NET
open OBO.NET.DBXref
open ARCtrl
open FSharpAux
//open OBO.NET.CodeGeneration


let res = OboOntology.fromFile true @"C:\Repos\CSBiology\OBO.NET\tests\OBO.NET.Tests\References\WhitespaceAfterTerm.obo"

let res2 = OboOntology.fromFile true @"C:\Repos\CSBiology\OBO.NET\tests\OBO.NET.Tests\References\WhitespaceAfterTypeDef.obo"