namespace FsOboParser

open DBXref
//open OboTerm
//open OboTypeDef

open ISADotNet

open System


/// Ontology containing OBO Terms and OBO Type Defs (OBO 1.2).
type OboOntology =

    {
        Terms       : OboTerm list
        TypeDefs    : OboTypeDef list
    }

    static member create terms typedefs =
        {
            Terms = terms
            TypeDefs = typedefs
        }

    /// Reads an OBO Ontology containing term and type def stanzas from lines.
    static member fromLines verbose (input : seq<string>) =

        let en = input.GetEnumerator()
        let rec loop (en:System.Collections.Generic.IEnumerator<string>) terms typedefs lineNumber =

            match en.MoveNext() with
            | true ->             
                match (en.Current |> trimComment) with
                | "[Term]"    -> 
                    let lineNumber,parsedTerm = (OboTerm.fromLines verbose en lineNumber "" "" false [] "" "" [] [] [] [] [] [] [] [] false [] [] [] false "" "")
                    loop en (parsedTerm :: terms) typedefs lineNumber
                | "[Typedef]" -> 
                    let lineNumber,parsedTypeDef = (OboTypeDef.fromLines verbose en lineNumber "" "" "" "" [] [] false false false false false false false)
                    loop en terms (parsedTypeDef :: typedefs) lineNumber
                | _ -> loop en terms typedefs (lineNumber + 1)
            | false -> OboOntology.create (List.rev terms) (List.rev typedefs)

        loop en [] [] 1

    /// Reads an OBO Ontology containing term and type def stanzas from a file with the given path.
    static member fromFile verbose (path : string) =
        System.IO.File.ReadAllLines path
        |> OboOntology.fromLines verbose

    /// Writes an OBO Ontology to term and type def stanzas in line form.
    static member toLines (oboOntology : OboOntology) =
        seq {
            for term in oboOntology.Terms do
                yield "[Term]"
                yield! OboTerm.toLines term
                yield ""

            for typedef in oboOntology.TypeDefs do
                yield "[Typedef]"
                yield! OboTypeDef.toLines typedef
                yield ""
        }

    /// Writes an OBO Ontology to term and type def stanzas to a file in the given path.
    static member toFile (path : string) (oboOntology : OboOntology) =
        System.IO.File.WriteAllLines(path, OboOntology.toLines oboOntology)

    /// Writes an OBO Ontology to term and type def stanzas in line form.
    member this.ToLines() = 
        OboOntology.toLines this

    /// Writes an OBO Ontology to term and type def stanzas to a file in the given path.
    member this.ToFile(path : string) =
        OboOntology.toFile path this

    /// Finds OBO term by "TermSourceRef:TermAccessionNumber" style ID.
    member this.TryGetTerm(id : string) = 
        this.Terms
        |> List.tryFind (fun t ->
            t.Id = id
        )

    /// Finds OBO term by "TermSourceRef:TermAccessionNumber" style ID.
    member this.GetTerm(id : string) = 
        this.Terms
        |> List.find (fun t ->
            t.Id = id
        )

    /// Finds OBO term by "TermSourceRef:TermAccessionNumber" style ID and returns it as ISA OntologyAnnotation type.
    member this.TryGetOntologyAnnotation(id : string) = 
        this.Terms
        |> List.tryPick (fun t ->
            if t.Id = id then Some (OboTerm.toOntologyAnnotation t) else None
        )

    /// Finds OBO term by "TermSourceRef:TermAccessionNumber" style ID and returns it as ISA OntologyAnnotation type.
    member this.GetOntologyAnnotation(id : string) = 
        this.Terms
        |> List.pick (fun t ->
            if t.Id = id then Some (OboTerm.toOntologyAnnotation t) else None
        )

    /// Finds OBO term by its free text name if it exists. Else returns None.
    member this.TryGetTermByName(name : string) = 
        this.Terms
        |> List.tryFind (fun t ->
            t.Name = name
        )

    /// Finds OBO term by its free text name.
    member this.GetTermByName(name : string) = 
        this.Terms
        |> List.find (fun t ->
            t.Name = name
        )

    /// Finds OBO term by its free text name and returns it as ISA OntologyAnnotation type if it exists. Else returns None.
    member this.TryGetOntologyAnnotationByName(name : string) = 
        this.Terms
        |> List.tryPick (fun t ->
            if t.Name = name then Some (OboTerm.toOntologyAnnotation t) else None
        )

    /// Finds OBO term by it's free text name and return it as ISA OntologyAnnotation type.
    member this.GetOntologyAnnotationByName(name : string) = 
        this.Terms
        |> List.pick (fun t ->
            if t.Name = name then Some (OboTerm.toOntologyAnnotation t) else None
        )

    /// For a given ontology term, finsd all equivalent terms that are connected via XRefs.
    ///
    /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked.
    member this.GetEquivalentOntologyAnnotations(term : ISADotNet.OntologyAnnotation, ?Depth : int) =
            
        let rec loop depth (equivalents : ISADotNet.OntologyAnnotation list) (lastLoop : ISADotNet.OntologyAnnotation list) =
            if equivalents.Length = lastLoop.Length then equivalents
            elif Depth.IsSome && Depth.Value < depth then equivalents
            else
                let newEquivalents = 
                    equivalents
                    |> List.collect (fun t ->
                        let forward = 
                            match this.TryGetTerm t.ShortAnnotationString with
                            | Some term ->
                                term.Xrefs
                                |> List.map (fun xref ->
                                    let id = OntologyAnnotation.createShortAnnotation "" xref.Name
                                    match this.TryGetOntologyAnnotation id with
                                    | Some oa ->
                                        oa
                                    | None -> 
                                        OntologyAnnotation.fromString "" "" xref.Name
                                )
                            | None ->
                                []
                        let backward = 
                            this.Terms
                            |> List.filter (fun term ->
                                term.Xrefs
                                |> List.exists (fun xref ->
                                    t.Equals(xref.Name)
                                )
                            )
                            |> List.map (fun ot -> OboTerm.toOntologyAnnotation ot)
                        forward @ backward
                    )
                loop (depth + 1) (equivalents @ newEquivalents |> List.distinct) equivalents
        loop 1 [term] []
        |> List.filter ((<>) term)

    /// For a given ontology term, finds all equivalent terms that are connected via XRefs.
    ///
    /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked.
    member this.GetEquivalentOntologyAnnotations(termId : string, ?Depth) =
        match Depth with 
        | Some d ->
            OntologyAnnotation.fromAnnotationId termId         
            |> fun oa -> this.GetEquivalentOntologyAnnotations(oa, d)
        | None -> 
            OntologyAnnotation.fromAnnotationId termId    
            |> this.GetEquivalentOntologyAnnotations

    /// For a given ontology term, finds all terms to which this term points in a "isA" relationship.
    ///
    /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked.
    member this.GetParentOntologyAnnotations(term : ISADotNet.OntologyAnnotation, ?Depth) =
        let rec loop depth (equivalents : ISADotNet.OntologyAnnotation list) (lastLoop : ISADotNet.OntologyAnnotation list) =
            if equivalents.Length = lastLoop.Length then equivalents
            elif Depth.IsSome && Depth.Value < depth then equivalents
            else
                let newEquivalents = 
                    equivalents
                    |> List.collect (fun t ->
                        match this.TryGetTerm t.ShortAnnotationString with
                        | Some term ->
                            term.IsA
                            |> List.map (fun isA ->
                                match this.TryGetOntologyAnnotation isA with
                                | Some oa ->
                                    oa
                                | None -> 
                                    OntologyAnnotation.fromString "" "" isA
                            )
                        | None ->
                            []
                    )
                loop (depth + 1) (equivalents @ newEquivalents |> List.distinct) equivalents
        loop 1 [term] []
        |> List.filter ((<>) term)

    /// For a given ontology term, find all terms to which this term points in a "isA" relationship
    ///
    /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked.
    member this.GetParentOntologyAnnotations(termId : string, ?Depth) =
        match Depth with 
        | Some d ->
            OntologyAnnotation.fromAnnotationId termId
            |> fun oa -> this.GetParentOntologyAnnotations(oa, d)
        | None -> 
            OntologyAnnotation.fromAnnotationId termId
            |> this.GetParentOntologyAnnotations

    /// For a given ontology term, finds all terms which point to this term "isA" relationship.
    ///
    /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked.
    member this.GetChildOntologyAnnotations(term : ISADotNet.OntologyAnnotation, ?Depth) =
        let rec loop depth (equivalents : ISADotNet.OntologyAnnotation list) (lastLoop : ISADotNet.OntologyAnnotation list) =
            if equivalents.Length = lastLoop.Length then equivalents
            elif Depth.IsSome && Depth.Value < depth then equivalents
            else
                let newEquivalents = 
                    equivalents
                    |> List.collect (fun t ->
                        this.Terms
                        |> List.choose (fun pt -> 
                            let isChild = 
                                pt.IsA
                                |> List.exists (fun isA -> t.ShortAnnotationString = isA)
                            if isChild then
                                Some (OboTerm.toOntologyAnnotation(pt))
                            else
                                None

                        )
                    )
                loop (depth + 1) (equivalents @ newEquivalents |> List.distinct) equivalents
        loop 1 [term] []
        |> List.filter ((<>) term)

    /// For a given ontology term, finds all terms which point to this term "isA" relationship.
    ///
    /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked.
    member this.GetChildOntologyAnnotations(termId : string, ?Depth) =
        match Depth with 
        | Some d ->
            OntologyAnnotation.fromAnnotationId termId
            |> fun oa -> this.GetChildOntologyAnnotations(oa, d)
        | None -> 
            OntologyAnnotation.fromAnnotationId termId
            |> this.GetChildOntologyAnnotations


type OboTermDef = 
    {
        Id           : string
        Name         : string
        IsTransitive : string
        IsCyclic     : string
    }

    static member make id name  isTransitive isCyclic =
        {Id = id; Name = name; IsTransitive = isTransitive; IsCyclic = isCyclic}

    //parseTermDef
    static member fromLines (en:Collections.Generic.IEnumerator<string>) id name isTransitive isCyclic =     
        if en.MoveNext() then                
            let split = (en.Current |> trimComment).Split([|": "|], System.StringSplitOptions.None)
            match split.[0] with
            | "id"            -> OboTermDef.fromLines en (split.[1..] |> String.concat ": ") name isTransitive isCyclic
            | "name"          -> OboTermDef.fromLines en id (split.[1..] |> String.concat ": ") isTransitive isCyclic 
            | "is_transitive" -> OboTermDef.fromLines en id name (split.[1..] |> String.concat ": ") isCyclic
            | "is_cyclic"     -> OboTermDef.fromLines en id name isTransitive (split.[1..] |> String.concat ": ")
            | ""              -> OboTermDef.make id name isTransitive isCyclic
                  
            | _               -> OboTermDef.fromLines en id name isTransitive isCyclic
        else
            // Maybe check if id is empty
            OboTermDef.make id name isTransitive isCyclic
            //failwithf "Unexcpected end of file."


/// Functions for parsing and querying an OBO ontology.
module OboOntology =

    /// Parses OBO Terms [Term] from seq<string>.
    [<Obsolete("Use static method fromLines instead")>]
    let parseOboTerms verbose (input:seq<string>)  =

        let en = input.GetEnumerator()
        let rec loop (en:System.Collections.Generic.IEnumerator<string>) lineNumber =
            seq {
                match en.MoveNext() with
                | true ->
                    match (en.Current |> trimComment) with
                    | "[Term]" -> 
                        let lineNumber,parsedTerm = (OboTerm.fromLines verbose en lineNumber "" "" false [] "" "" [] [] [] [] [] [] [] [] false [] [] [] false "" "")
                        yield parsedTerm
                        yield! loop en lineNumber
                    | _ -> yield! loop en (lineNumber + 1)
                | false -> ()
            }
        loop en 1