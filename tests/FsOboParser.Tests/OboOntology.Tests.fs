namespace FsOboParser.Tests

open Expecto
open FsOboParser


module OboOntologyTests =

    [<Tests>]
    let oboOntologyTest =
        testList "OboOntology" [
            let testTerm1 = 
                OboTerm.Create(
                    "id:1", 
                    Name = "testTerm1", 
                    Relationships = ["related_to id:2"; "unrelated_to id:3"; "antirelated_to id:4"]
                )
            let testTerm2 = 
                OboTerm.Create(
                    "id:2", 
                    Name = "testTerm2", 
                    Relationships = ["related_to id:1"]
                )
            let testTerm3 = 
                OboTerm.Create(
                    "id:3", 
                    Name = "testTerm3", 
                    Relationships = ["unrelated_to id:1"],
                    IsA = ["id:1"; "id:2"]
                )
            let testTerm4 =
                OboTerm.Create(
                    "id:5",
                    Name = "testTerm4"
                )
            let testTerm5 =
                OboTerm.Create(
                    "id:6",
                    Name = "testTerm5",
                    Synonyms = [TermSynonym.parseSynonym None 0 "\"testTerm1\" EXACT []"; TermSynonym.parseSynonym None 1 "\"testTerm2\" BROAD []"; TermSynonym.parseSynonym None 2 "\"testTerm0\" NARROW []"]
                )
            let testOntology = OboOntology.create [testTerm1; testTerm2; testTerm3; testTerm4; testTerm5] []
            testList "GetRelatedTerms" [
                testCase "returns correct related terms" <| fun _ ->
                    let actual = testOntology.GetRelatedTerms(testTerm1)
                    let expected = [testTerm1, "related_to", Some testTerm2; testTerm1, "unrelated_to", Some testTerm3; testTerm1, "antirelated_to", None]
                    Expect.sequenceEqual actual expected "is not equal"
            ]
            testList "GetIsAs" [
                testCase "returns correct related terms" <| fun _ ->
                    let actual = testOntology.GetIsAs testTerm3
                    let expected = [testTerm3, Some testTerm1; testTerm3, Some testTerm2]
                    Expect.sequenceEqual actual expected "is not equal"
            ]
            testList "GetRelations" [
                testCase "returns correct TermRelations" <| fun _ ->
                    let actual = testOntology.GetRelations()
                    let expected = [
                        Target ("related_to", testTerm1, testTerm2)
                        Target ("unrelated_to", testTerm1, testTerm3)
                        TargetMissing ("antirelated_to", testTerm1)
                        Target ("related_to", testTerm2, testTerm1)
                        Target ("unrelated_to", testTerm3, testTerm1)
                        Target ("is_a", testTerm3, testTerm1)
                        Target ("is_a", testTerm3, testTerm2)
                        Empty testTerm4
                        Empty testTerm5
                    ]
                    Expect.sequenceEqual actual expected "is not equal"
            ]
            testList "GetSynonyms" [
                testCase "returns correct synonymous terms" <| fun _ ->
                    let actual = testOntology.GetSynonyms testTerm5
                    let expected = seq {Exact, testTerm1; Broad, testTerm2}
                    Expect.sequenceEqual actual expected "is not equal"
            ]
            testList "TryGetSynonyms" [
                testCase "returns correct synonymous terms" <| fun _ ->
                    let actual = testOntology.TryGetSynonyms testTerm5
                    let expected = seq {Exact, Some testTerm1; Broad, Some testTerm2; Narrow, None}
                    Expect.sequenceEqual actual expected "is not equal"
            ]
        ]