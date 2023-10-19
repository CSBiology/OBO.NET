namespace OBO.NET


open DBXref


//The value consists of a quote enclosed synonym text, a scope identifier, an optional synonym type name, and an optional dbxref list, like this:
//synonym: "The other white meat" EXACT MARKETING_SLOGAN [MEAT:00324, BACONBASE:03021]

//The synonym scope may be one of four values: EXACT, BROAD, NARROW, RELATED. If the first form is used to specify a synonym, the scope is assumed to be RELATED.

//The synonym type must be the id of a synonym type defined by a synonymtypedef line in the header. If the synonym type has a default scope, that scope is used regardless of any scope declaration given by a synonym tag.

//The dbxref list is formatted as specified in dbxref formatting. A term may have any number of synonyms.
type TermSynonymScope =
    | Exact   
    | Broad   
    | Narrow  
    | Related 

    static member ofString (line : int) (s : string) = 
        match s with
        | "EXACT"   -> Exact
        | "BROAD"   -> Broad
        | "NARROW"  -> Narrow
        | "RELATED" -> Related
        | _         ->  printfn "[WARNING@L %i]unable to recognize %s as synonym scope" line s
                        Related


type TermSynonym = {
    Text        : string
    Scope       : TermSynonymScope
    TypeName    : string
    DBXrefs     : DBXref list
}


module TermSynonym =

    let private synonymRegex = 
        System.Text.RegularExpressions.Regex("""(?<synonymText>^\"(.*?)\"){1}(\s?)(?<synonymScope>(EXACT|BROAD|NARROW|RELATED))?(\s?)(?<synonymDescription>\w*)(\s?)(?<dbxreflist>\[(.*?)\])?""")

    let parseSynonym (scopeFromDeprecatedTag : TermSynonymScope option) (line : int) (v : string) =
        let matches = synonymRegex.Match(v.Trim()).Groups
        {
            Text = matches.Item("synonymText").Value
            Scope =
                match scopeFromDeprecatedTag with
                | Some scope -> scope
                | _ ->  matches.Item("synonymScope").Value
                        |> TermSynonymScope.ofString line
            TypeName = matches.Item("synonymDescription").Value
            DBXrefs =
                let tmp = matches.Item("dbxreflist").Value
                match tmp.Replace("[", "").Replace("]", "") with
                | "" -> []
                | dbxrefs ->
                    dbxrefs.Split(',')
                    |> Array.map parseDBXref
                    |> Array.toList
        }