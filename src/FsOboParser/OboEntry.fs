namespace FsOboParser


/// Model of raw OboEntries, divided into Terms (as `OboTerm`s) and TypeDefs (as `OboTypeDef`s).
type OboEntry =
    | Term of OboTerm
    | TypeDef of OboTypeDef