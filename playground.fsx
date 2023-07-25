#I "src/FsOboParser/bin/Debug/netstandard2.0"
#r "FsOboParser.dll"

#r "nuget: IsaDotNet"

open FsOboParser

open System.IO


let testPath = Path.Combine(__SOURCE_DIRECTORY__,  "./../../nfdi4plants/arc-validate/ErrorClassOntology.obo")

let testOboOntology = OboOntology.fromFile true testPath

let fileLines = File.ReadAllLines testPath

OboTerm.fromLines true ((fileLines |> Seq.ofArray).GetEnumerator()) 0

OboOntology.toFile "myOboOntology.obo" testOboOntology

Path.Combine("myOboOntology.obo") |> FileInfo

let myOboTerm = OboTerm.Create("TO:00000000", Name = "testTerm", CreatedBy = "myself")

let myOboTerm = 
    OboTerm.create 
        "TO:00000000" 
        (Some "testTerm") 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        None 
        (Some "myself") 
        None

OboTypeDef.Create

let myOntology = OboOntology.create [myOboTerm] []

OboOntology.toFile "myOboOntology.obo" myOntology