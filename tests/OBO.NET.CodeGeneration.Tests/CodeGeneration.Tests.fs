namespace OBO.NET.CodeGeneration.Tests

open Expecto
open OBO.NET
open OBO.NET.CodeGeneration
open FSharpAux
open ARCTokenization.StructuralOntology
open ARCTokenization.Terms
open System.IO
open type System.Environment


module CodeGenerationTests =

    let refObo = OboOntology.fromFile false (Path.Combine(__SOURCE_DIRECTORY__, "References/ReferenceOboFile.obo"))
    let refSF = File.ReadAllText (Path.Combine(__SOURCE_DIRECTORY__, "References/ReferenceSourceFile.fs"))

    let toUnderscoredNameTest =
        testList "toUnderscoredName" [
            testCase "returns correct underscored name" <| fun _ ->
                let expected = "Investigation_Metadata"
                let actual = List.head refObo.Terms |> CodeGeneration.toUnderscoredName
                Expect.equal actual expected "underscored name is not correct"
        ]

    let toTermSourceRefTest =
        testList "toTermSourceRef" [
            testCase "returns correct TermSourceRef" <| fun _ ->
                let expected = "INVMSO"
                let actual = List.head refObo.Terms |> CodeGeneration.toTermSourceRef
                Expect.equal actual expected "TermSourceRef is not correct"
        ]

    let toCodeStringTest =
        testList "toCodeString" [
            testCase "returns correct F# code" <| fun _ ->
                let expected = $"        let Investigation_Metadata = CvTerm.create(\"INVMSO:00000001\", \"Investigation Metadata\", \"INVMSO\"){NewLine}{NewLine}"
                let actual = List.head refObo.Terms |> CodeGeneration.toCodeString
                Expect.equal actual expected "F# code is not correct"
        ]

    let toSourceCodeTest =
        testList "toSourceCode" [
            testCase "returns correct source code" <| fun _ ->
                let expected = refSF.ReplaceLineEndings()
                let actual = (CodeGeneration.toSourceCode "Investigation" refObo).ReplaceLineEndings()
                Expect.equal actual expected "Source code is not correct"
        ]

    [<Tests>]
    let all = testList "CodeGeneration" [toUnderscoredNameTest; toTermSourceRefTest; toCodeStringTest; toSourceCodeTest]