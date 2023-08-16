namespace FsOboParser.Tests

open Expecto
open FsOboParser


module OboTermTests =

    [<Tests>]
    let testTest =
        testList "OboTerm" [
            testList "deconstructRelationship" [
                testCase "is correctly deconstructed" <| fun _ ->
                    let actual = OboTerm.deconstructRelationship "part_of INVMSO:00000082"
                    let expected = "part_of", "INVMSO:00000082"
                    Expect.equal actual expected "is not equal"
            ]
        ]