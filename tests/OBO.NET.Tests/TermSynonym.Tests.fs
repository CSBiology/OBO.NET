namespace OBO.NET.Tests

open Expecto
open OBO.NET

module TermSynonymTests =

    [<Tests>]
    let main =
        testList "TermSynonym" [
            testCase "create" <| fun _ ->
                let actual = TermSynonym.create("\"Bacillus coli\"", Related, "synonym", [])
                Expect.equal actual.Text "\"Bacillus coli\"" "text"
                Expect.equal actual.Scope Related "scope"
                Expect.equal actual.TypeName "synonym" "typeName"
                Expect.equal actual.DBXrefs [] "dbxrefs"
            testCase "ToOboString" <| fun _ ->
                let actual = TermSynonym.create("\"Bacillus coli\"", Related, "synonym", []).ToLine()
                let expected = "\"Bacillus coli\" RELATED synonym []"
                Expect.equal actual expected "is not equal"
        ]