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

//    member this.AreTermsEquivalent(term1 : OboTerm, term2 : OboTerm) =
//        term1.Xrefs 
//        |> List.exists (
//            fun x1 -> 
//                term2.Id = x1.Name
//                &&
//                this.TreatXrefsAsEquivalents
//                |> List.exists (fun t -> t = (term2.ToCvTerm()).RefUri)
//        )

    /// Returns all terms of the OboOntology that have equivalent terms in a given second OboOntology.
    member this.ReturnAllEquivalentTerms(ontology : OboOntology) =
        if List.exists (fun x -> x = ontology.Terms.Head.ToCvTerm().RefUri) this.TreatXrefsAsEquivalents then
            this.Terms
            |> Seq.filter (
                fun t ->
                    List.isEmpty t.Xrefs |> not
                    &&
                    t.Xrefs
                    |> List.exists (
                        fun x -> 
                            ontology.Terms
                            |> List.exists (
                                fun t2 -> 
                                    (DBXref.toCvTerm x).Accession = t2.Id
                            ) 
                    ) 
            )
        else Seq.empty

let testTerm1 = OboTerm.Create("test:001", Name = "Test1", Xrefs = [DBXref.parseDBXref "check:001"])
let testOntology1 = OboOntology.Create([testTerm1], [], "", TreatXrefsAsEquivalents = ["check"])
let testTerm2 = OboTerm.Create("check:001")

testOntology1.AreTermsEquivalent(testTerm1, testTerm2)

let testOntology2 = OboOntology.Create([testTerm2], [], "")

testOntology1.ReturnAllEquivalentTerms testOntology2

DBXref.ofString """test:1 "testDesc" {testMod}"""

open ARCTokenization.Terms

open type System.Environment

let expected = 
    $"namespace ARCTokenization.StructuralOntology{NewLine}{NewLine}    open ControlledVocabulary{NewLine}{NewLine}    module Investigation ={NewLine}{NewLine}        let Investigation_Metadata = CvTerm.create(\"INVMSO:00000001\", \"Investigation Metadata\", \"INVMSO\"){NewLine}{NewLine}        let ONTOLOGY_SOURCE_REFERENCE = CvTerm.create(\"INVMSO:00000002\", \"ONTOLOGY SOURCE REFERENCE\", \"INVMSO\"){NewLine}{NewLine}        let Term_Source_Name = CvTerm.create(\"INVMSO:00000003\", \"Term Source Name\", \"INVMSO\")"
    |> String.replace "\r" ""
let actual = 
    CodeGeneration.toSourceCode "Investigation" InvestigationMetadata.ontology 
    |> String.splitS NewLine 
    |> Array.take 11 
    |> String.concat "\n"
    |> String.replace "\r" ""

OBO.NET.OboOntology.toFile @"C:\Repos\CSBiology\OBO.NET\tests\OBO.NET.CodeGeneration.Tests\References\ReferenceOboFile.obo" InvestigationMetadata.ontology
// OBO.NET.OboOntology.toFile @"C:\Repos\CSBiology\OBO.NET\tests\OBO.NET.CodeGeneration.Tests\References\ReferenceOboFile.obo" InvestigationMetadata.ontology

CodeGeneration.toFile "InvestigationMetadata" InvestigationMetadata.ontology @"C:\Repos\CSBiology\OBO.NET\tests\OBO.NET.CodeGeneration.Tests\References\ReferenceSourceFile2.fs"


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