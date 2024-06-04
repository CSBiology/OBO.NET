namespace OBO.NET.Tests

open Expecto
open OBO.NET
open ControlledVocabulary


module OboTermTests =

    let testTerm1 = OboTerm.Create("test:001", Name = "TestTerm1")

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
            testList "ToLines" [
                testCase "synonym" <| fun _ ->
                    let actual = 
                        OboTerm.Create("NCBITaxon:562", Name="Escherichia coli", Synonyms=[TermSynonym.create("\"Bacillus coli\"", Related, "synonym", [])]) 
                        |> OboTerm.toLines
                        |> List.ofSeq
                    let expected = [
                        "id: NCBITaxon:562"
                        "name: Escherichia coli"
                        "synonym: \"Bacillus coli\" RELATED synonym []"
                    ]
                    Expect.sequenceEqual actual expected ""
            ]
            testList "ToCvTerm" [
                testCase "returns correct CvTerm" <| fun _ ->
                    let actual = OboTerm.toCvTerm testTerm1
                    let expected = CvTerm.create("test:001", "TestTerm1", "test")
                    Expect.equal actual.RefUri expected.RefUri "TSRs are different"
                    Expect.equal actual.Name expected.Name "Names are different"
                    Expect.equal actual.Accession expected.Accession "TANs are different"
            ]
        ]