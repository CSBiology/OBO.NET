namespace OBO.NET.CodeGeneration.Tests

open Expecto
open OBO.NET
open OBO.NET.CodeGeneration
open FSharpAux
open ARCTokenization.StructuralOntology
open ARCTokenization.Terms
open type System.Environment


module CodeGenerationTests =

    let toUnderscoredNameTest =
        testList "toUnderscoredName" [
            testCase "returns correct underscored name" <| fun _ ->
                let expected = "Investigation_Metadata"
                let actual = List.head InvestigationMetadata.ontology.Terms |> CodeGeneration.toUnderscoredName
                Expect.equal actual expected "underscored name is not correct"
        ]

    let toTermSourceRefTest =
        testList "toTermSourceRef" [
            testCase "returns correct TermSourceRef" <| fun _ ->
                let expected = "INVMSO"
                let actual = List.head InvestigationMetadata.ontology.Terms |> CodeGeneration.toTermSourceRef
                Expect.equal actual expected "TermSourceRef is not correct"
        ]

    let toCodeStringTest =
        testList "toCodeString" [
            testCase "returns correct F# code" <| fun _ ->
                let expected = $"        let Investigation_Metadata = CvTerm.create(\"INVMSO:00000001\", \"Investigation Metadata\", \"INVMSO\"){NewLine}{NewLine}"
                let actual = List.head InvestigationMetadata.ontology.Terms |> CodeGeneration.toCodeString
                Expect.equal actual expected "F# code is not correct"
        ]

    let toSourceCodeTest =
        testList "toSourceCode" [
            testCase "returns correct source code" <| fun _ ->
                let expected = 
                    $"namespace ARCTokenization.StructuralOntology{NewLine}{NewLine}    open ControlledVocabulary{NewLine}{NewLine}    module Investigation ={NewLine}{NewLine}        let Investigation_Metadata = CvTerm.create(\"INVMSO:00000001\", \"Investigation Metadata\", \"INVMSO\"){NewLine}{NewLine}        let ONTOLOGY_SOURCE_REFERENCE = CvTerm.create(\"INVMSO:00000002\", \"ONTOLOGY SOURCE REFERENCE\", \"INVMSO\"){NewLine}{NewLine}        let Term_Source_Name = CvTerm.create(\"INVMSO:00000003\", \"Term Source Name\", \"INVMSO\")"
                    |> String.replace "\r" ""
                let actual = 
                    CodeGeneration.toSourceCode "Investigation" InvestigationMetadata.ontology 
                    |> String.splitS NewLine 
                    |> Array.take 11 
                    |> String.concat "\n"
                    |> String.replace "\r" ""
                Expect.equal actual expected "Source code is not correct"
        ]

    [<Tests>]
    let all = testList "CodeGeneration" [toUnderscoredNameTest; toTermSourceRefTest; toCodeStringTest; toSourceCodeTest]