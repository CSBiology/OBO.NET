#I "src/OBO.NET/bin/Debug/netstandard2.0"
#I "src/OBO.NET/bin/Release/netstandard2.0"
#I "src/OBO.NET.CodeGeneration/bin/Debug/netstandard2.0"
#I "src/OBO.NET.CodeGeneration/bin/Release/netstandard2.0"

#r "OBO.NET.dll"
//#r "OBO.NET.CodeGeneration.dll"

open OBO.NET
open OBO.NET.DBXref
//open OBO.NET.CodeGeneration

#r "nuget: FSharpAux"
#r "nuget: ARCTokenization"

open FSharpAux
open FSharpAux.Regex
open ARCTokenization.Terms

open System

open type System.Environment


let term1 = OboTerm.Create ""
let term2 = OboTerm.Create ""
let r = term1.Xrefs |> List.exists (fun x1 -> term2.Xrefs |> List.exists (fun x2 -> x2.Name = x1.Name))

type OboOntology with

    member this.AreTermsEquivalent(term1 : OboTerm, term2 : OboTerm) =
        term1.Xrefs 
        |> List.exists (
            fun x1 -> 
                term2.Id = x1.Name
                &&
                this.TreatXrefsAsEquivalents
                |> List.exists (fun t -> term2.)
        )




// DEPRECATED


#I "src/FsOboParser/bin/Debug/netstandard2.0"
#I "src/FsOboParser/bin/Release/netstandard2.0"
#r "FsOboParser.dll"

#r "nuget: IsaDotNet"
//#r "nuget: FsOboParser"
#r "nuget: FSharpAux"


open FsOboParser

open FSharpAux

open System.IO


//let testPath = Path.Combine(__SOURCE_DIRECTORY__,  "./../../nfdi4plants/arc-validate/ErrorClassOntology.obo")

//OboEntries.fromFile true testPath
//let testOntology = OboOntology.fromFile true testPath

let testTerms = [
    OboTerm.Create("test:000", Name = "test0")
    OboTerm.Create("test:001", Name = "test1a", IsA = ["test:000"])
    OboTerm.Create("test:002", Name = "test2", IsA = ["test:001"; "test:000"])
    OboTerm.Create("test:003", Name = "test1b", IsA = ["test:000"])
    OboTerm.Create("test:004", Name = "test1aSyn", Synonyms = [TermSynonym.parseSynonym None 1 "\"test1a\" EXACT []"])
    //OboTerm.Create("test:004", Name = "test1aSyn", Synonyms = [TermSynonym.parseSynonym None 1 "test1a EXACT []"])
]

let testOntology = OboOntology.create testTerms []



//let termOfInterest = testOntology.Terms[5]
//let targetOfInterest = testOntology.Terms[7]

//let emptyTermRelation = Empty termOfInterest
//let targetMissingTermRelation = TargetMissing ("unconnected_to", termOfInterest)
//let targetTermRelation = Target ("connected_to", termOfInterest, targetOfInterest)

//// exemplary
//type MyRelation =
//    | IsA
//    | HasA
//    | PartOf
//    | ConnectedTo
//    | Unknown of string

//let targetTermRelation' = Target (ConnectedTo, termOfInterest, targetOfInterest)



///// Takes a given OboTerm and returns a sequence of scope * OboTerm if the synonym exists in the given OboOntology or scope * None if it does not.
//let tryGetSynonymTerms (term : OboTerm) (onto : OboOntology) =
//    term.Synonyms
//    |> Seq.map (
//        fun s -> 
//            s.Scope,
//            onto.Terms 
//            |> Seq.tryFind (
//                fun t -> 
//                    t.Name = String.replace "\"" "" s.Text
//            )
//    )

///// Takes a given OboTerm and returns a sequence of scope * OboTerm if the synonym exists in the given OboOntology.
//let getSynonymTerms (term : OboTerm) (onto : OboOntology) =
//    term.Synonyms
//    |> Seq.choose (
//        fun s -> 
//            let sto =
//                onto.Terms 
//                |> Seq.tryFind (
//                    fun t -> 
//                        t.Name = String.replace "\"" "" s.Text
//                )
//            match sto with
//            | Some st -> Some (s.Scope, st)
//            | None -> None
//    )

//String.replace "\"" "" (TermSynonym.parseSynonym None 1 "test1a EXACT []").Text

//tryGetSynonymTerms testTerms[4] testOntology
//getSynonymTerms testTerms[4] testOntology


//OboOntology.getRelations testOntology


//testOntology.GetChildOntologyAnnotations(testTerms.Head.Id)
//testOntology.GetChildOntologyAnnotations(testTerms.Head.Id, Depth = 1)
//testOntology.GetChildOntologyAnnotations(testTerms.Head.Id, Depth = 2)



//let performanceTerms = List.init 7000000 (fun i -> OboTerm.Create($"lol:{i}"))
//let performanceOboOntology = OboOntology.create performanceTerms []
//OboOntology.toFile @"C:\Repos\CSBiology\FsOboParser\performanceOntology.obo" performanceOboOntology

//let x = OboOntology.fromFile false @"C:\Repos\CSBiology\FsOboParser\performanceOntology.obo"


//let fileLines = File.ReadAllLines testPath

//OboTerm.fromLines true ((fileLines |> Seq.ofArray).GetEnumerator()) 0

//OboOntology.toFile "myOboOntology.obo" testOboOntology

//Path.Combine("myOboOntology.obo") |> FileInfo

//let myOboTerm = OboTerm.Create("TO:00000000", Name = "testTerm", CreatedBy = "myself")

//let myOboTerm = 
//    OboTerm.create 
//        "TO:00000000" 
//        (Some "testTerm") 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        (Some "myself") 
//        None

//OboTypeDef.Create

//let myOntology = OboOntology.create [myOboTerm] []

//OboOntology.toFile "myOboOntology.obo" myOntology

//let termOfInterest = testOntology.Terms[5]

//let isAs = testOntology.GetParentOntologyAnnotations(termOfInterest.Id)
// output is an ISADotNet.OntologyAnnotation list

//let isAsTerms = isAs |> List.map (fun oa -> testOntology.GetTerm(oa.TermAccessionString.ToString()))
// output is an OboTerm list