# FsOboParser

An OBO file format parser, written in F#.

## Usage



## Develop

### Build (QuickStart)

If not already done, install .NET SDK.

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