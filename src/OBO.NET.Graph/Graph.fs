namespace OBO.NET


open OBO.NET

open Graphoscope

/// Functions for working with ontology-based FGraphs.
module Graph =

    ///// Takes a TermRelation of string and returns the approbriate ArcRelation plus source term and target term as a triple. If the TermRelation is Empty or TargetMissing, returns None.
    //let tryToArcRelation termRelation =
    //    match termRelation with
    //    | Empty t -> None
    //    | TargetMissing (r,t) -> None
    //    | Target (r,st,tt) -> Some (toArcRelation r,st,tt)

    /// Takes an OboOntology and returns an FGraph with OboTerms as nodes and ArcRelations as Edges. The structure of the graph results from the TermRelations between the ontology's terms.
    let ontologyToFGraph onto =
        OboOntology.getRelations onto
        //|> Seq.choose tryToArcRelation
        //|> Seq.groupBy (
        //    fun (r,st,tt) -> 
        //        st, tt
        //)
        //|> Seq.map (
        //    fun (k,v) -> 
        //        v 
        //        |> Seq.reduce (
        //            fun (ar1,st1,tt1) (ar2,st2,tt2) -> 
        //                ar1 + ar2, st1, tt1
        //        )
        //)
        |> Seq.fold (
            fun acc termRel -> 
                match termRel with
                | Empty target -> acc
                | TargetMissing (rel,target) -> acc
                | Target (rel,sourceTerm,targetTerm) ->
                    FGraph.addElement sourceTerm.Id sourceTerm targetTerm.Id targetTerm rel acc
        ) FGraph.empty<string,OboTerm,string>