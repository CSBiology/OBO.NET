namespace FsOboParser

//open OboTerm


////########################################
//// Definition of OboGraph


//module FastOboGraph =

//    /// Obo Term as node
//    [<StructuredFormatDisplay("{PrettyString}")>]
//    type OboNode = { 
//        Id : int
//        Name : string
//        NameSpace : string
//        OntologyId : string // GO:
//        }
//        with
//        member this.PrettyString = sprintf "%s:%07i | %s {%s}" this.OntologyId this.Id this.Name this.NameSpace
//        interface INode<int>
//            with member this.Id = this.Id


//    /// Creates OboNode
//    let createOboNode id name nameSpace ontologyId =
//        {Id = id; Name = name; NameSpace = nameSpace; OntologyId = ontologyId; }



//    type OboEdgeType =
//        | Is_A
//        | Part_Of

//    [<StructuredFormatDisplay("{PrettyString}")>]
//    type OboEdge = { 
//        Id : int
//        SourceId :int
//        TargetId :int } 
//        with
//        member this.PrettyString =  if this.Id = this.SourceId then
//                                        sprintf "o---> %07i | (%i)" this.Id this.TargetId
//                                    else 
//                                        sprintf "%07i <---o | (%i)" this.Id this.TargetId
//        interface IEdge<int> with
//            member this.Id = this.Id
//            member this.SourceId = this.SourceId
//            member this.TargetId = this.TargetId
            

//    /// Creates OboEdge
//    let createOboEdge id sourceId targetId =
//        {Id = id; SourceId = sourceId; TargetId = targetId}


//    type oboAdjacencyNode = AdjacencyNode<OboNode,OboEdge,int>



//    /// Splits String s at ":", returns sa.[1]
//    let tryIdToInt str =
//        match str with
//        | Regex.RegexValue @"GO:(?<goId>[\d]+)" [ goId; ] -> Some( int goId )
//        | _ -> None

//    let idToInt str =
//        match tryIdToInt str with
//        | Some v -> v
//        | None   -> failwithf "%s invaild GO id" str

//    let private oboIdStringToInt s =
//        let sa = String.split ':' s
//        if sa.Length > 1 then
//            sa.[1] |> int
//        else
//            -1

//    /// Creates fromOboTerm from oboTerm startIndex
//    let fromOboTerm (obo: OboTerm) (startIndex: int) =
//        let nodeId = oboIdStringToInt obo.Id
//        let node   = createOboNode nodeId obo.Name obo.Namespace
//        let edges = 
//            obo.IsA
//            |> List.mapi (fun i edId -> let edgeTargetId = oboIdStringToInt edId
//                                        createOboEdge (i+startIndex) nodeId edgeTargetId
//                            )
//        (node,edges,(startIndex + obo.IsA.Length))


//    /// Creates OboEnumerator from oboNode oboEdge
//    let oboTermToOboGraph (input: seq<OboTerm>) = //: seq<oboAdjacencyNode> =
//        let en = input.GetEnumerator()
//        let rec loop (en:System.Collections.Generic.IEnumerator<OboTerm>) acc  =
//            seq { 
//                match en.MoveNext() with
//                | true -> let cNode,cEdges,cIndex = fromOboTerm en.Current acc

//                            yield (cNode,cEdges)
//                            yield! loop en cIndex
//                | false -> ()
//                }
//        loop en 0


//    /// Reads obo file 
//    let readFile path =
//        FileIO.readFile path
//        |> parseOboTerms
//        |> oboTermToOboGraph
//        |> Seq.toList