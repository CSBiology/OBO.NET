# FsOboParser

An OBO file format parser, written in F#.

## Usage

### Read an OBO file

```fsharp
open FsOboParser

let testOntology = OboOntology.fromFile true filepath
```

### Create OBO terms

OOP style (recommended):

```fsharp
let myOboTerm = 
	OboTerm.Create(
		"TO:00000000", 
		Name = "testTerm", 
		CreatedBy = "myself"
	)
```

Functional style:

```fsharp
let myOboTerm = 
	OboTerm.create 
		"TO:00000000" 
		(Some "testTerm") 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		None 
		(Some "myself") 
		None
```

### Create an OBO ontology

```fsharp
let myOntology = OboOntology.create [myOboTerm] []
```

### Save an OBO ontology

```fsharp
OboOntology.toFile "myOboOntology.obo" myOntology
```

### Get all `is_a`s of a Term recursively

```fsharp
let termOfInterest = testOntology.Terms[5]

let isAs = testOntology.GetParentOntologyAnnotations(termOfInterest.Id)
// output is an ISADotNet.OntologyAnnotation list

let isAsTerms = isAs |> List.map (fun oa -> testOntology.GetTerm(oa.TermAccessionString.ToString()))
// output is an OboTerm list
```

## Develop

### Build (QuickStart)

If not already done, install .NET SDK >= 6.

In any shell, run `build.cmd <target>` where `<target>` may be
- if `<target>` is empty, it just runs dotnet build after cleaning everything
- `RunTests` to run unit tests
	- `RunTestsWithCodeCov` to run unit tests with code coverage
- `ReleaseNotes semver:<version>` where `<version>` may be `major`, `minor`, or `patch` to update RELEASE_NOTES.md
- `Pack` to create a NuGet release
    - `PackPrelease` to create a NuGet prerelease
- `BuildDocs` to create docs
    - `BuildDocsPrerelease` to create prerelease docs
- `WatchDocs` to create docs and run them locally
- `WatchDocsPrelease` to create prerelease docs and run them locally
- `PublishNuget` to create a NuGet release and publish it
    - `PublishNugetPrelease` to create a NuGet prerelease and publish it