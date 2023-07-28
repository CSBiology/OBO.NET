namespace FsOboParser


/// Functions for working with OboEntries.
module OboEntries =

    /// Reads a collection of strings and parses them into a list of OboEntries.
    let fromLines verbose (input : seq<string>) =

        let en = input.GetEnumerator()
        let rec loop (en : System.Collections.Generic.IEnumerator<string>) entries lineNumber =

            match en.MoveNext() with
            | true ->
                match (en.Current |> DBXref.trimComment) with
                | "[Term]" -> 
                    let lineNumber, parsedTerm = OboTerm.fromLines verbose en lineNumber "" "" false [] "" "" [] [] [] [] [] [] [] [] false [] [] [] false "" ""
                    loop en (Term parsedTerm :: entries) lineNumber
                | "[Typedef]" -> 
                    let lineNumber, parsedTypeDef = OboTypeDef.fromLines verbose en lineNumber "" "" "" "" [] [] false false false false false false false
                    loop en (TypeDef parsedTypeDef :: entries) lineNumber
                | _ -> loop en entries (lineNumber + 1)
            | false -> entries

        loop en [] 1

    /// Reads an OBO file and returns a list of OboEntries.
    let fromFile verbose filepath =
        System.IO.File.ReadAllLines filepath
        |> fromLines verbose