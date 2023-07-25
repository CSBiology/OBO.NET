namespace FsOboParser

open DBXref

open System


/// Models the relationship between OBO Terms 
type OboTypeDef = 
    {
        ///The unique id of the current term. 
        //Cardinality: exactly one.
        Id : string
        // The id of a term, or a special reserved identifier, which indicates the domain for this relationship type. 
        // If a property P has domain D, then any term T that has a relationship of type P to another term is a subclass of D.
        // Note that this does not mean that the domain restricts which classes of terms can have a relationship of type P to another term. 
        // Rather, it means that any term that has a relationship of type P to another term is by definition a subclass of D. 
        // Cardinality: zero or one. If the intent is to declare a disjunctive domain, then a new class must be declared and defined using the union_of construct.
        Domain : string
        // The id of a term, or a special reserved identifier, which indicates acceptable range for this relationship type. 
        // If a property P has range R, then any term T that is the target of a relationship of type P is a subclass of R. 
        // Note that this does not mean that the range restricts which classes of terms can be the target of relationships of type P. 
        // Rather, it means that any term that is the target of a relationship of type P is by definition a subclass of R. 
        // Cardinality: zero or one. If the intent is to declare a disjunctive range, then a new class must be declared and defined using the union_of construct.
        Range : string 
        ///The term name. 
        //Any term may have only zero or one name defined. 
        //Cardinality: zero or one - If multiple term names are defined, it is a parse error. In 1.2 name was required. This has been relaxed in 1.4. This helps with OWL interoperability, as labels are optional in OWL
        Name : string 
        // The id of another relationship type that is the inverse of this relationship type.
        // If relation A is the inverse_of type B, and X has relationship A to Y, then it is implied that Y has relation B to X. 
        // In obof1.2 the semantics of inverse_of were unclear, as obof1.2 unofficially allowed type-level relations. 
        // In obof1.4, the semantics are identical to OWL. Cardinality: any.
        Inverse_of : string list
        // The id of another relationship type that this relationship type is transitive over. 
        // If P is transitive over Q, and the ontology has X P Y and Y Q Z then it follows that X P Z (term/type level). 
        // Equivalent to property chains in OWL2. Cardinality: any.
        Transitive_over : string list
        // Whether or not a cycle can be made from this relationship type. 
        // If a relationship type is non-cyclic, it is illegal for an ontology to contain a cycle made from user-defined or implied relationships of this type. 
        // Allowed values: true or false. Cardinality: zero or one.
        Is_cyclic : bool
        // Whether this relationship is reflexive. All reflexive relationships are also cyclic. 
        // Allowed values: true or false. Term/type level. Cardinality: zero or one.
        Is_reflexive : bool
        // Whether this relationship is symmetric. All symmetric relationships are also cyclic. 
        // Allowed values: true or false. Term/type level. Cardinality: zero or one.
        Is_symmetric : bool
        // Whether this relationship is anti-symmetric. 
        // Allowed values: true or false. Term/type level. Cardinality: zero or one.
        Is_anti_symmetric : bool
        // Whether this relationship is transitive.
        // Allowed values: true or false. Term/type level. Cardinality: zero or one.
        Is_transitive : bool
        // Whether this relationship is a metadata tag. 
        // Properties that are marked as metadata tags are used to record object metadata. 
        // Object metadata is additional information about an object that is useful to track, 
        // but does not impact the definition of the object or how it should be treated by a reasoner. 
        // Metadata tags might be used to record special term synonyms or structured notes about a term, for example. 
        // Cardinality: zero or one.
        Is_metadata_tag : bool
        // Whether this relation is a class-level relation. 
        // In OBO-Format, all relationship tags are taken by default to mean an all-some relationship over an instance level relation.
        // This tag is used for other cases, e.g. lacks_part. In OWL this is translated to a hasValue restriction. 
        // Cardinality: zero or one.
        Is_class_level : bool
    }

    /// Creates an OBO Type Def from its field values.
    static member make id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
        is_anti_symmetric is_transitive is_metadata_tag is_class_level =
        {              
                Id                  = id
                Domain              = domain
                Range               = range 
                Name                = name
                Inverse_of          = inverse_of 
                Transitive_over     = transitive_over
                Is_cyclic           = is_cyclic
                Is_reflexive        = is_reflexive
                Is_symmetric        = is_symmetric
                Is_anti_symmetric   = is_anti_symmetric
                Is_transitive       = is_transitive
                Is_metadata_tag     = is_metadata_tag
                Is_class_level      = is_class_level
        }

    /// Creates an OBO Type Def from its field values.
    static member Create (id, domain, range, ?Name, ?Inverse_of, ?Transitive_over, ?Is_cyclic, ?Is_reflexive, ?Is_symmetric,
        ?Is_anti_symmetric, ?Is_transitive, ?Is_metadata_tag, ?Is_class_level) =
        {
                Id                  = id
                Domain              = domain
                Range               = range
                Name                = Option.defaultValue "" Name
                Inverse_of          = Option.defaultValue [] Inverse_of
                Transitive_over     = Option.defaultValue [] Transitive_over
                Is_cyclic           = Option.defaultValue false Is_cyclic
                Is_reflexive        = Option.defaultValue false Is_reflexive
                Is_symmetric        = Option.defaultValue false Is_symmetric
                Is_anti_symmetric   = Option.defaultValue false Is_anti_symmetric
                Is_transitive       = Option.defaultValue false Is_transitive
                Is_metadata_tag     = Option.defaultValue false Is_metadata_tag
                Is_class_level      = Option.defaultValue false Is_class_level
            }

    /// Creates an OBO Type Def from its field values.
    static member create id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric is_anti_symmetric is_transitive is_metadata_tag is_class_level =
        {
                Id                  = id
                Domain              = domain
                Range               = range
                Name                = Option.defaultValue "" name
                Inverse_of          = Option.defaultValue [] inverse_of
                Transitive_over     = Option.defaultValue [] transitive_over
                Is_cyclic           = Option.defaultValue false is_cyclic
                Is_reflexive        = Option.defaultValue false is_reflexive
                Is_symmetric        = Option.defaultValue false is_symmetric
                Is_anti_symmetric   = Option.defaultValue false is_anti_symmetric
                Is_transitive       = Option.defaultValue false is_transitive
                Is_metadata_tag     = Option.defaultValue false is_metadata_tag
                Is_class_level      = Option.defaultValue false is_class_level
            }

    /// Reads an OBO Type Def from lines in "key:value" style.
    static member fromLines verbose (en : Collections.Generic.IEnumerator<string>) lineNumber 
        id domain range name (inverse_of : string list) transitive_over is_cyclic is_reflexive is_symmetric 
        is_anti_symmetric is_transitive is_metadata_tag is_class_level =

        if en.MoveNext() then
            let split = (en.Current |> trimComment).Split([|": "|], System.StringSplitOptions.None)
            match split.[0] with
            | "id"              -> 
                let v = split.[1..] |> String.concat ": "
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    v domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | "domain"              -> 
                let v = split.[1..] |> String.concat ": "
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id v range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level
        
            | "range"            -> 
                let v = split.[1..] |> String.concat ": "
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain v name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level
    
            | "name"            -> 
                let v = split.[1..] |> String.concat ": "
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range v inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | "inverse_of"    ->
                let v = split.[1..] |> String.concat ": "
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name (v::inverse_of) transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | "transitive_over"              -> 
                let v = split.[1..] |> String.concat ": "
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of (v::transitive_over) is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | "is_cyclic"              -> 
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over true is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | "is_reflexive"              -> 
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over is_cyclic true is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | "is_symmetric"              -> 
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over is_cyclic is_reflexive true 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | "is_anti_symmetric"              -> 
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    true is_transitive is_metadata_tag is_class_level

            | "is_transitive"              -> 
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric true is_metadata_tag is_class_level

            | "is_metadata_tag"              -> 
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive true is_class_level

            | "is_class_level"              -> 
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag true


            | "" -> 
                lineNumber,
                OboTypeDef.make id domain range name
                    (inverse_of |> List.rev)
                    (transitive_over |> List.rev)
                    is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

            | unknownTag -> 
                if verbose then printfn "[WARNING@L %i]: Found typedef tag <%s> that does not fit OBO flat file specifications 1.4. Skipping it..." lineNumber unknownTag
                OboTypeDef.fromLines verbose en (lineNumber + 1)
                    id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level

        else
            // Maybe check if id is empty
            lineNumber,
            OboTypeDef.make id domain range name
                (inverse_of |> List.rev)
                (transitive_over |> List.rev)
                is_cyclic is_reflexive is_symmetric 
                is_anti_symmetric is_transitive is_metadata_tag is_class_level
            //failwithf "Unexcpected end of file."

    /// Writes an OBO Type Def to lines in "key:value" style.
    static member toLines (typedef : OboTypeDef) =
        seq {
            "id: " + typedef.Id
            "domain: " + typedef.Domain
            "range: " + typedef.Range
            "name: " + typedef.Name
        }