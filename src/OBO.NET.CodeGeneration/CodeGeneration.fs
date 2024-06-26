﻿namespace OBO.NET.CodeGeneration


open OBO.NET
open FSharpAux
open type System.Environment


module CodeGeneration =

    [<Literal>]
    let baseString = """namespace ARCTokenization.StructuralOntology

    open ControlledVocabulary

    module <name> =

"""

    /// Takes an OboTerm and returns its name but with all spaces replaced by underscores.
    let toUnderscoredName (term : OboTerm) = 
        term.Name
        |> String.replace " " "_"

    /// Returns true if a string contains special characters or starts with a number.
    let checkForSpecialCharacters str =
        let spChs = System.Text.RegularExpressions.Regex(@"(^\d|[^a-zA-Z0-9_])")
        (spChs.Match str).Success

    /// Takes a string and returns it with back ticks ("``") at the beginning and the end.
    let addBackTicks str =
        $"``{str}``"

    /// Takes an OboTerm and returns its TermSourceRef as string.
    let toTermSourceRef (term : OboTerm) =
        term.Id
        |> String.takeWhile ((<>) ':')

    /// Takes an OboTerm and transforms it into an F# code string for structural ontology libraries.
    let toCodeString (term : OboTerm) = 
        let underscoredName = toUnderscoredName term
        let curatedName =
            if checkForSpecialCharacters underscoredName then
                addBackTicks underscoredName
            else underscoredName
        $"        let {curatedName} = CvTerm.create(\"{term.Id}\", \"{term.Name}\", \"{toTermSourceRef term}\"){NewLine}{NewLine}"

    /// Takes a module name and an OboOntology and returns the F# code of the whole term list for structural ontology libraries.
    let toSourceCode moduleName (onto : OboOntology) =
        let concattedSingleValues = String.init onto.Terms.Length (fun i -> $"{toCodeString onto.Terms[i]}")
        let updatedBaseString = String.replace "<name>" moduleName baseString
        $"{updatedBaseString}{concattedSingleValues}"

    /// Takes a module name and an OboOntology and writes the ontology's terms as F# code for structural ontology libraries as a source file at the given path.
    let toFile moduleName (onto : OboOntology) path =
        System.IO.File.WriteAllText(path, toSourceCode moduleName onto)

    /// Takes a module name and the path to an OBO file and writes the ontology's terms as F# code for structural ontology libraries as a source file at the given output path.
    let fromOboFileToSourceFile moduleName inputPath outputPath =
        OboOntology.fromFile false inputPath
        |> fun o -> toFile moduleName o outputPath