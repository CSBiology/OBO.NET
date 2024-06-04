namespace OBO.NET


open FSharpAux
open ControlledVocabulary

open System


/// Representation of DBXrefs.
type DBXref = 
    {
        Name        : string
        Description : string
        Modifiers   : string
    }

    /// Parses a given string to a DBXref
    static member ofString (v : string) =
        let xrefRegex = Text.RegularExpressions.Regex("""(?<xrefName>^([^"{])*)(\s?)(?<xrefDescription>\"(.*?)\")?(\s?)(?<xrefModifiers>\{(.*?)}$)?""")
        let matches = xrefRegex.Match(v.Trim()).Groups
        {
            Name        = matches.Item("xrefName")          .Value |> String.trim
            Description = matches.Item("xrefDescription")   .Value |> String.trim
            Modifiers   = matches.Item("xrefModifiers")     .Value |> String.trim
        }

    /// Returns the corresponding CvTerm of the DBXref with empty name.
    member this.ToCvTerm() = {
        Name        = ""
        Accession   = this.Name
        RefUri      = 
            String.split ':' this.Name
            |> Array.head
            |> String.trim
    }


/// Functions for working with DBXrefs.
module DBXref =

    //Dbxref definitions take the following form:

    //<dbxref name> {optional-trailing-modifier}

    //or

    //<dbxref name> "<dbxref description>" {optional-trailing-modifier}

    //The dbxref is a colon separated key-value pair. The key should be taken from GO.xrf_abbs but this is not a requirement. 
    //If provided, the dbxref description is a string of zero or more characters describing the dbxref. 
    //DBXref descriptions are rarely used and as of obof1.4 are discouraged.

    //Dbxref lists are used when a tag value must contain several dbxrefs. Dbxref lists take the following form:

    //[<dbxref definition>, <dbxref definition>, ...]

    //The brackets may contain zero or more comma separated dbxref definitions. An example of a dbxref list can be seen in the GO def for "ribonuclease MRP complex":

    //def: "A ribonucleoprotein complex that contains an RNA molecule of the snoRNA family, and cleaves the rRNA precursor as part of rRNA transcript processing. It also has other roles: In S. cerevisiae it is involved in cell cycle-regulated degradation of daughter cell-specific mRNAs, while in mammalian cells it also enters the mitochondria and processes RNAs to create RNA primers for DNA replication." [GOC:sgd_curators, PMID:10690410, Add to Citavi project by Pubmed ID PMID:14729943, Add to Citavi project by Pubmed ID PMID:7510714] Add to Citavi project by Pubmed ID

    //Note that the trailing modifiers (like all trailing modifiers) do not need to be decoded or round-tripped by parsers; trailing modifiers can always be optionally ignored. However, all parsers must be able to gracefully ignore trailing modifiers. It is important to recognize that lines which accept a dbxref list may have a trailing modifier for each dbxref in the list, and another trailing modifier for the line itself.

    // EXAMPLE (taken from GO_Slim Agr): "xref: RO:0002093"

    let trimComment (line : string) = 
        line.Split('!').[0].Trim()

    [<Obsolete "Use `DBXref.ofString` instead">]
    let parseDBXref (v : string) =
        DBXref.ofString v

    /// Creates a CvTerm (with an empty name) of a given DBXref.
    let toCvTerm (dbxref : DBXref) =
        dbxref.ToCvTerm()