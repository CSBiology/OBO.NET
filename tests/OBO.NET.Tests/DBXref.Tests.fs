namespace OBO.NET.Tests


open OBO.NET

open Expecto
open ControlledVocabulary


module DBXref =

    [<Tests>]
    let dbxrefTests =
        testList "DBXref" [

            let testDBXref = DBXref.ofString """test:1 "testDesc" {testMod}"""

            testList "ofString" [
                testCase "returns correct DBXref" <| fun _ ->
                    let expected = {Name = "test:1"; Description = "\"testDesc\""; Modifiers = "{testMod}"}
                    Expect.equal testDBXref.Name expected.Name "Name does not match"
                    Expect.equal testDBXref.Description expected.Description "Description does not match"
                    Expect.equal testDBXref.Modifiers expected.Modifiers "Modifiers do not match"
            ]

            testList "ToCvTerm" [
                testCase "returns correct CvTerm" <| fun _ ->
                    let actual = testDBXref.ToCvTerm()
                    let expected = CvTerm.create("test:1", "", "test")
                    Expect.equal actual.Accession expected.Accession "TANs are not equal"
                    Expect.equal actual.RefUri expected.RefUri "TSRs are not equal"
                    Expect.equal actual.Name expected.Name "Names are not equal"
            ]

        ]