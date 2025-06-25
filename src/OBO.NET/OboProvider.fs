namespace OBO.NET

module OboProvider =

    open System
    open System.Reflection
    open ProviderImplementation.ProvidedTypes // This comes from ProvidedTypes.fs/.fsi
    open Microsoft.FSharp.Core.CompilerServices

    // To make this assembly recognized as containing Type Providers.
    [<assembly: TypeProviderAssembly>] do ()

    module Expressions =

        open Microsoft.FSharp.Quotations
        open Microsoft.FSharp.Reflection

        let quoteStringOption (valueOpt: string option) : Expr =
            let optionType = typeof<string option>
            let unionCases = FSharpType.GetUnionCases(optionType)
    
            match valueOpt with
            | Some v ->
                let someCase = unionCases |> Array.find (fun c -> c.Name = "Some")
                Expr.NewUnionCase(someCase, [ Expr.Value(v, typeof<string>) ])
            | None ->
                let noneCase = unionCases |> Array.find (fun c -> c.Name = "None")
                Expr.NewUnionCase(noneCase, [])

        let quoteDateTimeOption (valueOpt: DateTime option) : Expr =
            let optionType = typeof<DateTime option>
            let unionCases = FSharpType.GetUnionCases(optionType)
    
            match valueOpt with
            | Some v ->
                let someCase = unionCases |> Array.find (fun c -> c.Name = "Some")
                Expr.NewUnionCase(someCase, [ Expr.Value(v, typeof<DateTime>) ])
            | None ->
                let noneCase = unionCases |> Array.find (fun c -> c.Name = "None")
                Expr.NewUnionCase(noneCase, [])

        let quoteDbxref (x: DBXref) =
            let name = x.Name
            let desc = x.Description
            let mods = x.Modifiers
            <@@ { Name = name; Description = desc; Modifiers = mods } @@>

        let quoteTermSynonymScope (s: TermSynonymScope) =
                match s with
                | Exact   -> <@ Exact @>
                | Broad   -> <@ Broad @>
                | Narrow  -> <@ Narrow @>
                | Related -> <@ Related @>

        let quoteTermSynonym (s: TermSynonym) =
            let text = s.Text
            let scopeExpr = quoteTermSynonymScope s.Scope
            let typename = s.TypeName

            let dbxrefsExpr =
                s.DBXrefs
                |> List.map quoteDbxref
                |> fun a -> Expr.NewArray(typeof<DBXref>, a)

            <@@ { Text = text; Scope = %%scopeExpr; TypeName = typename; DBXrefs = %%dbxrefsExpr |> List.ofArray } @@>

    
        // Quote list of strings as array expressions
        let quoteStringList (xs: string list) =
            xs |> List.map (fun s -> <@@ s @@>) |> fun a -> Expr.NewArray(typeof<string>, a)

        let quoteOboTerm (term: OboTerm) =

            // Quote all synonyms and xrefs as array expressions, and convert them to lists afterwards
            let synonymsExpr =
                term.Synonyms
                |> List.map quoteTermSynonym
                |> fun a -> Expr.NewArray(typeof<TermSynonym>, a)

            let xrefsExpr =
                term.Xrefs
                |> List.map quoteDbxref
                |> fun a -> Expr.NewArray(typeof<DBXref>, a)

            let altIdsExpr         = quoteStringList term.AltIds
            let subsetsExpr        = quoteStringList term.Subsets
            let isaExpr            = quoteStringList term.IsA
            let intersExpr         = quoteStringList term.IntersectionOf
            let disjointExpr       = quoteStringList term.DisjointFrom
            let unionExpr          = quoteStringList term.UnionOf
            let relExpr            = quoteStringList term.Relationships
            let replExpr           = quoteStringList term.Replacedby
            let considerExpr       = quoteStringList term.Consider
            let propValsExpr       = quoteStringList term.PropertyValues

            // Basic values
            let id            = term.Id
            let name          = term.Name
            let isAnon        = term.IsAnonymous
            let definition    = term.Definition
            let comment       = term.Comment
            let isObsolete    = term.IsObsolete
            let builtIn       = term.BuiltIn
            let createdBy     = term.CreatedBy
            let creationDate  = term.CreationDate

            // Final quotation
            <@@ {
                Id              = id
                Name            = name
                IsAnonymous     = isAnon
                AltIds          = %%altIdsExpr |> List.ofArray
                Definition      = definition
                Comment         = comment
                Subsets         = %%subsetsExpr |> List.ofArray
                Synonyms        = %%synonymsExpr |> List.ofArray
                Xrefs           = %%xrefsExpr |> List.ofArray
                IsA             = %%isaExpr |> List.ofArray
                IntersectionOf  = %%intersExpr |> List.ofArray
                DisjointFrom    = %%disjointExpr |> List.ofArray
                UnionOf         = %%unionExpr |> List.ofArray
                Relationships   = %%relExpr |> List.ofArray
                IsObsolete      = isObsolete
                Replacedby      = %%replExpr |> List.ofArray
                Consider        = %%considerExpr |> List.ofArray
                PropertyValues  = %%propValsExpr |> List.ofArray
                BuiltIn         = builtIn
                CreatedBy       = createdBy
                CreationDate    = creationDate
            } @@>

        let quoteOboTypeDef (typeDef: OboTypeDef) =

            let inverse_ofExpr        = quoteStringList typeDef.Inverse_of       
            let transitive_overExpr   = quoteStringList typeDef.Transitive_over  

            // Basic values
            let id                = typeDef.Id               
            let domain            = typeDef.Domain           
            let range             = typeDef.Range            
            let name              = typeDef.Name             
            let is_cyclic         = typeDef.Is_cyclic        
            let is_reflexive      = typeDef.Is_reflexive     
            let is_symmetric      = typeDef.Is_symmetric     
            let is_anti_symmetric = typeDef.Is_anti_symmetric
            let is_transitive     = typeDef.Is_transitive    
            let is_metadata_tag   = typeDef.Is_metadata_tag  
            let is_class_level    = typeDef.Is_class_level   

            <@@ {
                Id                = id
                Domain            = domain
                Range             = range
                Name              = name
                Is_cyclic         = is_cyclic
                Is_reflexive      = is_reflexive
                Is_symmetric      = is_symmetric
                Is_anti_symmetric = is_anti_symmetric
                Is_transitive     = is_transitive
                Is_metadata_tag   = is_metadata_tag
                Is_class_level    = is_class_level
                Inverse_of        = %%inverse_ofExpr |> List.ofArray
                Transitive_over   = %%transitive_overExpr |> List.ofArray
            } @@>

        let quoteOboOntology (ont: OboOntology) =

            let termsExpr =
                ont.Terms
                |> List.map quoteOboTerm
                |> fun a -> Expr.NewArray(typeof<OboTerm>, a)
            let typeDefsExpr =
                ont.TypeDefs
                |> List.map quoteOboTypeDef
                |> fun a -> Expr.NewArray(typeof<OboTypeDef>, a)

            let dataVersionExpr                 = quoteStringOption ont.DataVersion                                  
            let ontologyExpr                    = quoteStringOption ont.Ontology                                     
            let savedByExpr                     = quoteStringOption ont.SavedBy                                      
            let autoGeneratedByExpr             = quoteStringOption ont.AutoGeneratedBy                              
            let defaultRelationshipIdPrefixExpr = quoteStringOption ont.DefaultRelationshipIdPrefix                  

            let dateExpr                        = quoteDateTimeOption ont.Date                                         

            let idMappingsExpr                                    = quoteStringList ont.IdMappings                                   
            let remarksExpr                                       = quoteStringList ont.Remarks                                      
            let treatXrefsAsEquivalentsExpr                       = quoteStringList ont.TreatXrefsAsEquivalents                      
            let treatXrefsAsGenusDifferentiasExpr                 = quoteStringList ont.TreatXrefsAsGenusDifferentias                
            let treatXrefsAsRelationshipsExpr                     = quoteStringList ont.TreatXrefsAsRelationships                    
            let treatXrefsAsIsAsExpr                              = quoteStringList ont.TreatXrefsAsIsAs                             
            let relaxUniqueIdentifierAssumptionForNamespacesExpr  = quoteStringList ont.RelaxUniqueIdentifierAssumptionForNamespaces 
            let relaxUniqueLabelAssumptionForNamespacesExpr       = quoteStringList ont.RelaxUniqueLabelAssumptionForNamespaces      
        
            let subsetdefsExpr      = quoteStringList ont.Subsetdefs                                   
            let importsExpr         = quoteStringList ont.Imports                                      
            let synonymtypedefsExpr = quoteStringList ont.Synonymtypedefs                              
            let idspacesExpr        = quoteStringList ont.Idspaces                                     

            let formatVersion   = ont.FormatVersion                                 

            <@@ { 
                Terms                                        = %%termsExpr |> List.ofArray
                TypeDefs                                     = %%typeDefsExpr |> List.ofArray
                FormatVersion                                = formatVersion
                DataVersion                                  = %%dataVersionExpr
                Ontology                                     = %%ontologyExpr
                Date                                         = %%dateExpr
                SavedBy                                      = %%savedByExpr
                AutoGeneratedBy                              = %%autoGeneratedByExpr
                Subsetdefs                                   = %%subsetdefsExpr |> List.ofArray
                Imports                                      = %%importsExpr |> List.ofArray
                Synonymtypedefs                              = %%synonymtypedefsExpr |> List.ofArray
                Idspaces                                     = %%idspacesExpr |> List.ofArray
                DefaultRelationshipIdPrefix                  = %%defaultRelationshipIdPrefixExpr
                IdMappings                                   = %%idMappingsExpr |> List.ofArray
                Remarks                                      = %%remarksExpr |> List.ofArray
                TreatXrefsAsEquivalents                      = %%treatXrefsAsEquivalentsExpr |> List.ofArray
                TreatXrefsAsGenusDifferentias                = %%treatXrefsAsGenusDifferentiasExpr |> List.ofArray
                TreatXrefsAsRelationships                    = %%treatXrefsAsRelationshipsExpr |> List.ofArray
                TreatXrefsAsIsAs                             = %%treatXrefsAsIsAsExpr |> List.ofArray
                RelaxUniqueIdentifierAssumptionForNamespaces = %%relaxUniqueIdentifierAssumptionForNamespacesExpr |> List.ofArray
                RelaxUniqueLabelAssumptionForNamespaces      = %%relaxUniqueLabelAssumptionForNamespacesExpr |> List.ofArray
            } @@>

    module TypeDefinitions =

        // Add a static property to the generated type that holds the transformed string
        let buildStaticTermProperty (term: OboTerm) = 
            let providedProperty = 
                ProvidedProperty(
                    propertyName = $"{term.Name}",
                    propertyType = typeof<OboTerm>,
                    isStatic = true,
                    getterCode = fun _ -> Expressions.quoteOboTerm term
                )

            providedProperty.AddXmlDoc $"""
{term.Name}

{term.Definition}
"""
            providedProperty

        // Add a static property to the generated type that holds the transformed string
        let buildStaticTypeDefProperty (typeDef: OboTypeDef) = 
            let providedProperty = 
                ProvidedProperty(
                    propertyName = $"{typeDef.Name}",
                    propertyType = typeof<OboTypeDef>,
                    isStatic = true,
                    getterCode = fun _ -> Expressions.quoteOboTypeDef typeDef
                )

            providedProperty.AddXmlDoc $"""
{typeDef.Name}
"""
            providedProperty

        let buildTermTypes (assembly:Assembly) (providerNamespace: string) (terms: OboTerm list) (dynamicTypeName: string) =
            let providedType = ProvidedTypeDefinition(
                assembly,
                providerNamespace,
                dynamicTypeName,
                Some typeof<obj>
            )

            for term in terms do
                providedType.AddMember (buildStaticTermProperty term)

            providedType

        let buildTypeDefTypes (assembly:Assembly) (providerNamespace: string) (typeDefs: OboTypeDef list) (dynamicTypeName: string) =
            let providedType = ProvidedTypeDefinition(
                assembly,
                providerNamespace,
                dynamicTypeName,
                Some typeof<obj>
            )

            for typeDef in typeDefs do
                providedType.AddMember (buildStaticTypeDefProperty typeDef)

            providedType

        let buildOboTermsType (assembly:Assembly) (providerNamespace: string) =
            let oboTermsType = ProvidedTypeDefinition(
                assembly,
                providerNamespace,
                "OboTermsProvider", // The name of the type you'll use to access the provider, e.g., Ontology<"myObo.obo">
                Some typeof<obj>
            )

            oboTermsType.AddXmlDoc """
A typeprovider for term stanzas in obo flat file formatted ontologies.
Use it by providing a string literal that points to an obo file like this:
let [<Literal>] oboPath = @"path/to/your/oboFile.obo"
type myTerms = OboTermsProvider<oboPath>
"""

            // Define the static parameters the 'Transform' type accepts.
            // In this case, a single string parameter named "input".
            let staticParameters =
                [ ProvidedStaticParameter("path", typeof<string>) ]
            do oboTermsType.DefineStaticParameters(
                parameters = staticParameters,
                instantiationFunction = (fun typeNameWithArgs suppliedArguments ->
                    match suppliedArguments with
                    | [| :? string as path |] ->
                        let ont = OboOntology.fromFile false path
                        buildTermTypes assembly providerNamespace ont.Terms typeNameWithArgs
                    | _ ->
                        // This will result in a compile-time error if the arguments are incorrect.
                        failwith "Invalid static arguments. This Type Provider expects a single string argument."
                )
            )
            oboTermsType

        let buildOboTypeDefsType (assembly:Assembly) (providerNamespace: string) =
            let oboTypeDefsType = ProvidedTypeDefinition(
                assembly,
                providerNamespace,
                "OboTypeDefsProvider", // The name of the type you'll use to access the provider, e.g., Ontology<"myObo.obo">
                Some typeof<obj>
            )

            oboTypeDefsType.AddXmlDoc """
A typeprovider for typeDef stanzas in obo flat file formatted ontologies.
Use it by providing a string literal that points to an obo file like this:
let [<Literal>] oboPath = @"path/to/your/oboFile.obo"
type myTerms = OboTermsProvider<oboPath>
"""

            let staticParameters =
                [ ProvidedStaticParameter("path", typeof<string>) ]
            do oboTypeDefsType.DefineStaticParameters(
                parameters = staticParameters,
                instantiationFunction = (fun typeNameWithArgs suppliedArguments ->
                    match suppliedArguments with
                    | [| :? string as path |] ->
                        let ont = OboOntology.fromFile false path
                        buildTypeDefTypes assembly providerNamespace ont.TypeDefs typeNameWithArgs
                    | _ ->
                        // This will result in a compile-time error if the arguments are incorrect.
                        failwith "Invalid static arguments. This Type Provider expects a single string argument."
                )
            )
            oboTypeDefsType

        //
        // TO-DO: Implement a type that provides the whole ontology, not just terms or typeDefs.
        //
        //let buildOboOntologyType (assembly:Assembly) (providerNamespace: string) =
        //    let oboTermsType = ProvidedTypeDefinition(
        //        assembly,
        //        providerNamespace,
        //        "OboOntologyProvider", // The name of the type you'll use to access the provider, e.g., Ontology<"myObo.obo">
        //        Some typeof<obj>
        //    )
        

    open System.IO

    [<TypeProvider>]
    type public OboOntologyProvider(config: TypeProviderConfig) as this =
        inherit TypeProviderForNamespaces(config) // In many other online examples this is a parameterless constructor.
        let providerNamespace = "OboProvider"
        let thisAssembly = Assembly.GetExecutingAssembly()

        // Automatically resolve referenced assemblies from config
        do
            AppDomain.CurrentDomain.add_AssemblyResolve(fun _ args ->
                let requestedName = AssemblyName(args.Name).Name

                // Search among referenced assemblies passed to the provider
                let matchPath = 
                    config.ReferencedAssemblies
                    |> Array.tryFind (fun path -> 
                        let filename = Path.GetFileNameWithoutExtension(path)
                        String.Equals(filename, requestedName, StringComparison.OrdinalIgnoreCase)
                    )

                match matchPath with
                | Some path -> 
                    try 
                        Assembly.LoadFrom(path)
                    with ex ->
                        printfn "Failed to load: %s\n%s" path ex.Message
                        null
                | None -> 
                    printfn "Could not resolve assembly: %s" requestedName
                    null
            )

        do this.AddNamespace(
            providerNamespace, 
            [
                TypeDefinitions.buildOboTermsType thisAssembly providerNamespace
                TypeDefinitions.buildOboTypeDefsType thisAssembly providerNamespace
            ]
        )