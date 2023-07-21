open Expecto


[<EntryPoint>]
let main argv = Tests.runTestsWithCLIArgs [] argv FsOboParser.Tests.TestTests.testTest