namespace FsOboParser.Tests

open Expecto


module TestTests =

    [<Tests>]
    let testTest =
        testList "testTestList" [
            testCase "testTestCase" <| fun _ ->
                Expect.isTrue true "testErrorMessage"
        ]