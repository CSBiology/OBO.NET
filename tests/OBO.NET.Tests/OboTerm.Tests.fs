namespace OBO.NET.Tests

open Expecto
open OBO.NET


module OboTermTests =

    [<Tests>]
    let oboTermTest =
        testList "OboTerm" [
            testList "deconstructRelationship" [
                testCase "is correctly deconstructed" <| fun _ ->
                    let actual = OboTerm.deconstructRelationship "part_of INVMSO:00000082"
                    let expected = "part_of", "INVMSO:00000082"
                    Expect.equal actual expected "is not equal"
            ]
            testList "GetRelatedTermIds" [
                testCase "returns correct related term IDs" <| fun _ ->
                    let testTerm = OboTerm.Create("id:1", Name = "testTerm1", Relationships = ["related_to id:2"; "unrelated_to id:3"])
                    let actual = testTerm.GetRelatedTermIds()
                    let expected = ["id:1", "related_to", "id:2"; "id:1", "unrelated_to", "id:3"]
                    Expect.sequenceEqual actual expected "is not equal"
            ]
        ]