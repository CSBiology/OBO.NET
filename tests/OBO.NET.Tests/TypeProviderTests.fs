namespace OBO.NET.Tests

open OBO.NET
open OboProvider

open Expecto

open System
open System.IO

module TypeProviderTests =
    
    // use this for in-IDE support.
    //let [<Literal>] oboPath = @"tests/OBO.NET.Tests/References/go.obo"

    // use this for performing the test execution
    let [<Literal>] oboPath = @"References/go.obo"

    type go = OboTermsProvider<oboPath>

    [<Tests>]
    let oboOntologyTest =
        testList "OboProvider" [
            test "go term 'adult gena' is provided" {
                let actual = go.``adult gena``
                Expect.equal actual.Id "TGMA:0000031" "Expected term ID for 'adult gena' to be TGMA:0000031"
                Expect.equal actual.Name "adult gena" "Expected term name for 'adult gena' to be 'adult gena'"
                Expect.equal actual.Synonyms.Length 7 "Expected 'adult gena' to have 7 synonyms"
            }
        ]