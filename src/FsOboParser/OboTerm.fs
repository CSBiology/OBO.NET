namespace FsOboParser

open DBXref
open TermSynonym

open ISADotNet

open System


/// Models the entities in an Obo Ontology.
type OboTerm = 
    {

    ///The unique id of the current term. 
    //Cardinality: exactly one.
    Id : string 
        
    ///The term name. 
    //Any term may have only zero or one name defined. 
    //Cardinality: zero or one - If multiple term names are defined, it is a parse error. In 1.2 name was required. This has been relaxed in 1.4. This helps with OWL interoperability, as labels are optional in OWL
    Name : string 
        
    ///Whether or not the current object has an anonymous id. 
    //Cardinality: zero or one. The semantics are the same as B-Nodes in RDF.
    IsAnonymous: bool
        
    ///Defines an alternate id for this term. 
    //Cardinality: any. A term may have any number of alternate ids.
    AltIds : string list
        
    ///The definition of the current term. 
    //Cardinality: zero or one. More than one definition for a term generates a parse error. The value of this tag should be the quote enclosed definition text, followed by a dbxref list containing dbxrefs that describe the origin of this definition (see dbxref formatting for information on how dbxref lists are encoded). An example of this tag would look like this:
    //definition: "The breakdown into simpler components of (+)-camphor, a bicyclic monoterpene ketone." [UM-BBD:pathway "", http://umbbd.ahc.umn.edu/cam/cam_map.html ""]
    Definition : string 
        
    ///A comment for this term. 
    //Cardinality: zero or one. There must be zero or one instances of this tag per term description. More than one comment for a term generates a parse error.
    Comment : string
        
    ///This tag indicates a term subset to which this term belongs. 
    //The value of this tag must be a subset name as defined in a subsetdef tag in the file header. 
    //If the value of this tag is not mentioned in a subsetdef tag, a parse error will be generated. 
    //Cardinality: any. A term may belong to any number of subsets.
    Subsets : string list

    ///This tag gives a synonym for this term, some xrefs to describe the origins of the synonym, and may indicate a synonym category or scope information. 
    //Cardinality: any.
    //A term may have any number of synonyms.
    Synonyms : TermSynonym list

    ///Cross references that describe analagous terms in another vocabularies. 
    //Cardinality: any. A term may have any number of xrefs.
    Xrefs : DBXref list

    //Describes a subclassing relationship between one term and another. The value is the id of the term of which this term is a subclass. 
    //A term may have any number of is_a relationships. This is equivalent to a SubClassOf axiom in OWL. Cardinality: any.
    //Parsers which support trailing modifiers may optionally parse the following trailing modifier tags for is_a:
    IsA  : string list // new

    //namespace <any namespace id>
    //derived true OR false

    //The namespace modifier allows the is_a relationship to be assigned its own namespace (independent of the namespace of the superclass or subclass of this is_a relationship).

    //The derived modifier indicates that the is_a relationship was not explicitly defined by a human ontology designer, but was created automatically by a reasoner, and could be re-derived using the non-derived relationships in the ontology.

    //This tag previously supported the completes trailing modifier. This modifier is now deprecated. Use the intersection_of tag instead.

    //Cardinality: EITHER zero OR two or more. This tag indicates that this term is equivalent to the intersection of several other terms. The value is either a term id, or a relationship type id, a space, and a term id. For example:
    //id: GO:0000085
    //name: G2 phase of mitotic cell cycle
    //intersection_of: GO:0051319 ! G2 phase
    //intersection_of: part_of GO:0000278 ! mitotic cell cycle

    //This means that GO:0000085 is equivalent to any term that is both a subtype of 'G2 phase' and has a part_of relationship to 'mitotic cell cycle' (i.e. the G2 phase of the mitotic cell cycle). Note that whilst relationship tags specify necessary conditions, intersection_of tags specify necessary and sufficient conditions.

    //A collection of intersection_of tags appearing in a term is also known as a cross-product definition (this is the same as what OWL users know as a defined class, employing intersectionOf constructs).

    //It is strongly recommended that all intersection_of tags follow a genus-differentia pattern. In this pattern, one of the tags is directly to a term id (the genus) and the other tags are relation term pairs. For example:

    //[Term]
    //id: GO:0045495 name: pole plasm
    //intersection_of: GO:0005737 ! cytoplasm
    //intersection_of: part_of CL:0000023 ! oocyte

    //These definitions can be read as sentences, such as a pole plasm is equivalent to a cytoplasm that is part_of an oocyte

    //If any intersection_of tags are specified for a term, at least two intersection_of tags need to be present or it is a parse error. The full intersection for the term is the set of all ids specified by all intersection_of tags for that term.

    //As of OBO 1.4, this tag may be applied in Typedef stanzas
    IntersectionOf : string list

    ///indicates that this term represents the union of several other terms. 
    //Cardinality: EITHER zero OR two or more.
    //The value is the id of one of the other terms of which this term is a union.
    //If any union_of tags are specified for a term, at least 2 union_of tags need to be present or it is a parse error. The full union for the term is the set of all ids specified by all union_of tags for that term.

    //This tag may not be applied to relationship types.

    //Parsers which support trailing modifiers may optionally parse the following trailing modifier tag for disjoint_from:

    //namespace <any namespace id>
    UnionOf: string list

    ///indicates that a term is disjoint from another, meaning that the two terms have no instances or subclasses in common. 
    //The value is the id of the term from which the current term is disjoint. This tag may not be applied to relationship types.
    //Cardinality: any.
    //Parsers which support trailing modifiers may optionally parse the following trailing modifier tag for disjoint_from:

    //namespace <any namespace id>
    //derived true OR false

    //The namespace modifier allows the disjoint_from relationship to be assigned its own namespace.

    //The derived modifier indicates that the disjoint_from relationship was not explicitly defined by a human ontology designer, but was created automatically by a reasoner, and could be re-derived using the non-derived relationships in the ontology.
    DisjointFrom : string list

    ///describes a typed relationship between this term and another term or terms. 
    ///relationship
    ///Cardinality: any.
    ///The value of this tag should be the relationship type id, and then the id of the target term, plus, optionally, other target terms. The relationship type 
    ///name must be a relationship type name as defined in a typedef tag stanza. The [Typedef] must either occur in a document in the current parse batch, or in a 
    ///file imported via an import header tag. If the relationship type name is undefined, a parse error will be generated. If the id of the target term cannot be 
    ///resolved by the end of parsing the current batch of files, this tag describes a "dangling reference"; see the parser requirements section for information 
    ///about how a parser may handle dangling references. If a relationship is specified for a term with an is_obsolete value of true, a parse error will be generated.
    ///Parsers which support trailing modifiers may optionally parse the following trailing modifier tags for relationships:

    ///namespace <any namespace id>
    ///inferred true OR false
    ///cardinality any non-negative integer
    ///maxCardinality any non-negative integer
    ///minCardinality any non-negative integer

    ///The namespace modifier allows the relationship to be assigned its own namespace (independant of the namespace of the parent, child, or type of the relationship).

    ///The inferred modifier indicates that the relationship was not explicitly defined by a human ontology designer, but was created automatically by a reasoner, and could be re-derived using the non-derived relationships in the ontology.

    ///Cardinality qualifiers can be used to specify constraints on the number of relations of the specified type any given instance can have. For example, in the stanza declaring a id: SO:0000634 ! polycistronic mRNA, we can say: relationship: has_part SO:0000316 {minCardinality=2} ! CDS which means that every instance of a transcript of this type has two or more CDS features such that they stand in a has_part relationship from the transcript.

    ///The semantics of a relationship tag is by default "all-some". Formally, in OWL this corresponds to an existential restriction - see the OWL section.
    Relationships : string list

    ///Whether or not this term is obsolete. 
    //Cardinality: zero or one. 
    //Allowable values are "true" and "false" (false is assumed if this tag is not present). Obsolete terms must have no relationships, and no defined is_a, inverse_of, disjoint_from, union_of, or intersection_of tags.
    IsObsolete : bool


    ///Gives a term which replaces an obsolete term. The value is the id of the replacement term. 
    //The value of this tag can safely be used to automatically reassign instances whose instance_of property points to an obsolete term.
    //Cardinality: any. 
    //The replaced_by tag may only be specified for obsolete terms. A single obsolete term may have more than one replaced_by tag. This tag can be used in conjunction with the consider tag.
    Replacedby : string list
        
    //A term which may be an appropriate substitute for an obsolete term, but needs to be looked at carefully by a human expert before the replacement is done.
    //Cardinality: any. 
    //This tag may only be specified for obsolete terms. A single obsolete term may have many consider tags. This tag can be used in conjunction with replaced_by.
    Consider : string list

    PropertyValues : string list

    ///Whether or not this term or relation is built in to the OBO format. 
    //Allowable values are "true" and "false" (false assumed as default). Rarely used. One example of where this is used is the OBO relations ontology, which provides a stanza for the is_a relation, even though this relation is axiomatic to the language.
    //builtin
    //Cardinality: zero or one. 
    BuiltIn : bool 


    ///Name of the creator of the term. May be a short username, initials or ID. 
    //Cardinality: zero or one. 
    //Example: dph
    //Note that although this tag is defined in obof1.4, it can be used in obof1.2 harmlessly
    CreatedBy : string


    ///Date of creation of the term specified in ISO 8601 format. Example: 2009-04-13T01:32:36Z
    //Cardinality: zero or one. 
    //Note that although this tag is defined in obof1.4, it can be used in obof1.2 harmlessly
    CreationDate : string

    }

    /// Create an Obo Term from its field values
    static member make id name isAnonymous altIds definition comment subsets synonyms xrefs isA         
        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider propertyValues builtIn    
        createdBy creationDate = {
        Id              = id
        Name            = name
        IsAnonymous     = isAnonymous 
        AltIds          = altIds
        Definition      = definition
        Comment         = comment
        Subsets         = subsets
        Synonyms        = synonyms
        Xrefs           = xrefs
        IsA             = isA
        IntersectionOf  = intersectionOf
        UnionOf         = unionOf
        DisjointFrom    = disjointFrom
        Relationships   = relationships
        IsObsolete      = isObsolete
        Replacedby      = replacedby
        Consider        = consider
        PropertyValues  = propertyValues
        BuiltIn         = builtIn
        CreatedBy       = createdBy
        CreationDate    = creationDate
    }

    /// Create an Obo Term from its field values
    static member Create (id, ?Name, ?IsAnonymous, ?AltIds, ?Definition, ?Comment, ?Subsets, ?Synonyms, ?Xrefs, ?IsA,
        ?IntersectionOf, ?UnionOf, ?DisjointFrom, ?Relationships, ?IsObsolete, ?Replacedby, ?Consider, ?PropertyValues, ?BuiltIn,
        ?CreatedBy, ?CreationDate) =

        {
            Id              = id
            Name            = Option.defaultValue "" Name
            IsAnonymous     = Option.defaultValue false IsAnonymous 
            AltIds          = Option.defaultValue [] AltIds
            Definition      = Option.defaultValue "" Definition
            Comment         = Option.defaultValue "" Comment
            Subsets         = Option.defaultValue [] Subsets
            Synonyms        = Option.defaultValue [] Synonyms
            Xrefs           = Option.defaultValue [] Xrefs
            IsA             = Option.defaultValue [] IsA
            IntersectionOf  = Option.defaultValue [] IntersectionOf
            UnionOf         = Option.defaultValue [] UnionOf
            DisjointFrom    = Option.defaultValue [] DisjointFrom
            Relationships   = Option.defaultValue [] Relationships
            IsObsolete      = Option.defaultValue false IsObsolete
            Replacedby      = Option.defaultValue [] Replacedby
            Consider        = Option.defaultValue [] Consider
            PropertyValues  = Option.defaultValue [] PropertyValues
            BuiltIn         = Option.defaultValue false BuiltIn
            CreatedBy       = Option.defaultValue "" CreatedBy
            CreationDate    = Option.defaultValue "" CreationDate
        }

    /// Read an Obo Term from lines in "key:value" style
    static member fromLines verbose (en:Collections.Generic.IEnumerator<string>) lineNumber 
        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
        propertyValues builtIn createdBy creationDate =

        if en.MoveNext() then
            let split = (en.Current |> trimComment).Split([|": "|], System.StringSplitOptions.None)
            match split.[0] with
            | "id"              -> 
                let v = split.[1..] |> String.concat ": "
                OboTerm.fromLines verbose  en (lineNumber + 1)
                    v name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate
        
            | "name"            -> 
                let v = split.[1..] |> String.concat ": "
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id v isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "is_anonymous"    ->
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name true altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "alt_id"              -> 
                let v = split.[1..] |> String.concat ": "
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous (v::altIds) definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "def"              -> 
                let v = split.[1..] |> String.concat ": "
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds v comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "comment"             -> 
                let v = split.[1..] |> String.concat ": "
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition v subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "subset"              -> 
                let v = split.[1..] |> String.concat ": "
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment (v::subsets) synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | synonymTag when synonymTag.Contains("synonym")              -> 
                let scope =
                    match synonymTag with
                    | "exact_synonym"   -> Some Exact
                    | "narrow_synonym"  -> Some Narrow
                    | "broad_synonym"   -> Some Broad
                    | _                 -> None
                let v = parseSynonym scope lineNumber (split.[1..] |> String.concat ": ")
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets (v::synonyms) xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "xref" | "xref_analog" | "xref_unk" -> 
                let v = (split.[1..] |> String.concat ": ") |> parseDBXref
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms (v::xrefs) isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "is_a"              -> 
                let v = (split.[1..] |> String.concat ": ")
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs (v::isA)
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "intersection_of"              -> 
                let v = (split.[1..] |> String.concat ": ")
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    (v::intersectionOf) unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "union_of"              -> 
                let v = (split.[1..] |> String.concat ": ")
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf (v::unionOf) disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate
                    
            | "disjoint_from"              -> 
                let v = (split.[1..] |> String.concat ": ")
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf (v::disjointFrom) relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate
                    
            | "relationship"              -> 
                let v = (split.[1..] |> String.concat ": ")
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom (v::relationships) isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

            | "is_obsolete"             -> 
                let v = ((split.[1..] |> String.concat ": ").Trim()) 
                let v' = v = "true"

                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships v' replacedby consider 
                    propertyValues builtIn createdBy creationDate            

            | "replaced_by"             -> 
                let v = (split.[1..] |> String.concat ": ")

                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete (v::replacedby) consider 
                    propertyValues builtIn createdBy creationDate


            | "consider" | "use_term"            -> 
                let v = (split.[1..] |> String.concat ": ")

                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby (v::consider)
                    propertyValues builtIn createdBy creationDate


            | "builtin"             -> 
                let v = ((split.[1..] |> String.concat ": ").Trim()) 
                let v' = v = "true"

                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues v' createdBy creationDate

            | "property_value"             -> 
                let v = (split.[1..] |> String.concat ": ")

                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    (v::propertyValues) builtIn createdBy creationDate

            | "created_by"             -> 
                let v = (split.[1..] |> String.concat ": ")

                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn v creationDate


            | "creation_date"             -> 
                let v = (split.[1..] |> String.concat ": ")

                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy v

            | "" -> 
                lineNumber,
                OboTerm.make id name isAnonymous 
                    (altIds |> List.rev) 
                    definition comment 
                    (subsets        |> List.rev)
                    (synonyms       |> List.rev)
                    (xrefs          |> List.rev)
                    (isA            |> List.rev)
                    (intersectionOf |> List.rev)
                    (unionOf        |> List.rev)
                    (disjointFrom   |> List.rev)
                    (relationships  |> List.rev)
                    isObsolete 
                    (replacedby     |> List.rev)
                    (consider       |> List.rev)
                    (propertyValues |> List.rev)
                    builtIn 
                    createdBy creationDate

            | unknownTag -> 
                if verbose then printfn "[WARNING@L %i]: Found term tag <%s> that does not fit OBO flat file specifications 1.4. Skipping it..." lineNumber unknownTag
                OboTerm.fromLines verbose en (lineNumber + 1)
                    id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                    intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                    propertyValues builtIn createdBy creationDate

        else
            // Maybe check if id is empty
            lineNumber,
            OboTerm.make id name isAnonymous 
                (altIds |> List.rev) 
                definition comment 
                (subsets        |> List.rev)
                (synonyms       |> List.rev)
                (xrefs          |> List.rev)
                (isA            |> List.rev)
                (intersectionOf |> List.rev)
                (unionOf        |> List.rev)
                (disjointFrom   |> List.rev)
                (relationships  |> List.rev)
                isObsolete 
                (replacedby     |> List.rev)
                (consider       |> List.rev)
                (propertyValues |> List.rev)
                builtIn 
                createdBy creationDate
            //failwithf "Unexcpected end of file."

    /// Write an Obo Term to lines in "key:value" style
    static member toLines (term : OboTerm) =
        seq {
            yield "id: " + term.Id
            if term.IsAnonymous then yield "is_anonymous"
            yield "name: " + term.Name
            for altid in term.AltIds do yield $"alt_id: {altid}"
            if term.Definition = "" |> not then yield $"def: {term.Definition}"
            if term.Comment = "" |> not then yield $"comment: {term.Comment}"
            for subset in term.Subsets do yield $"subset: {subset}"
            for synonym in term.Synonyms do yield $"synonym: {synonym}"
            for xref in term.Xrefs do yield $"xref: {xref.Name}"
            if term.BuiltIn then yield "builtin"
            for property_value in term.PropertyValues do yield $"property_value: {property_value}"
            for is_a in term.IsA do yield $"is_a: {is_a}"
            for intersection in term.IntersectionOf do yield $"intersection_of: {intersection}"
            for union in term.UnionOf do yield $"union_of: {union}"
            //for equivalent in term.equ do yield $"equivalent_to: {equivalent}"
            for disjoint in term.DisjointFrom do yield $"disjoint_from: {disjoint}"
            for relationship in term.Relationships do yield $"relationship: {relationship}"
            for created_by in term.CreatedBy do yield $"created_by: {created_by}"
            if term.CreationDate = "" |> not then yield $"creation_date: {term.CreationDate}"
            if term.IsObsolete then yield "is_obsolete"
            for replaced_by in term.Replacedby do yield $"replaced_by: {replaced_by}"
            for consider in term.Consider do yield $"consider: {consider}"
        }

    /// Translates a OBO `term` into an ISADotNet `OntologyAnnotation`
    static member toOntologyAnnotation (term : OboTerm) =
        let ref,num = OntologyAnnotation.splitAnnotation term.Id
        OntologyAnnotation.fromString term.Name ref term.Id

    /// Translates an ISADotNet `OntologyAnnotation` into a OBO `term`
    static member ofOntologyAnnotation (term : OntologyAnnotation) =
        OboTerm.Create(term.ShortAnnotationString,term.NameText)