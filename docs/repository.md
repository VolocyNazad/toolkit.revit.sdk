# Repository guide

Paths in this document are relative to the repository root.
Read and follow the [development policy](policies/development.md) alongside this guide.

## About

This repo is the source for `VolocyNazad.Revit.Sdk` (published under the
`VolocyNazad` org), a custom MSBuild SDK for Autodesk Revit add-in
projects: supports Revit 2021-2027, picks `net48` (`2021`-`2024`) vs
`net8.0-windows` (`2025`-`2026`) vs `net10.0-windows` (`2027`)
automatically based on the target Revit version encoded in the build
configuration name, defines version conditional-compilation constants
(`VERSION_2025`, `IS2025`, `BEFORE2026`, `AFTER2024`, ..., up to `AFTER2027`),
validates that the target Revit version is within 2021-2027, excludes
`RevitAPI.dll`/`RevitAPIUI.dll` from output, generates the `.addin`
manifest, and can merge dependencies via ILRepack. It's what
`impact.revitaddinapi/revit` and every `toolkit.revit.*` repo build
against (`<Project Sdk="VolocyNazad.Revit.Sdk/...">`).

## Repository structure

```
.
├── Directory.Packages.props           central package versions
├── src/
│   └── Toolkit.Revit.Sdk/            the SDK itself (MSBuild tasks/targets)
└── tests/
    ├── Directory.Packages.props       test-only package versions
    └── Toolkit.Revit.Sdk.Tests/      xunit tests for the task logic (e.g. GenerateAddinManifest)
```

## Tech stack

- .NET, multi-targets `net48;net10.0`
- `Microsoft.Build.Utilities.Core` (MSBuild task API)
- MinVer (git-tag-based versioning)
- PolySharp (C# language polyfills)
- Package versions use central package management (`Directory.Packages.props`, `ManagePackageVersionsCentrally=true`); test-only versions live in `tests/Directory.Packages.props`, which imports the root file.
- xUnit v3 through Microsoft.Testing.Platform (unit tests in `tests/Toolkit.Revit.Sdk.Tests`, targeting `net10.0` only), in two styles:
  - task logic (`GenerateAddinManifest`) is tested by instantiating the task directly with
    `Microsoft.Build.Utilities.TaskItem` inputs and asserting on the generated `.addin` XML
  - `.props`/`.targets` logic (version parsing, `DefineConstants`, `RemoveRevitAPICopyLocal`,
    `ValidateMajorRevitVersion`) is tested by importing the real files into a throwaway temp project
    and evaluating/building it via `Microsoft.Build.Evaluation`/`Microsoft.Build.Execution`, bootstrapped
    with `Microsoft.Build.Locator` (`MSBuildLocator.RegisterDefaults()` in a `[ModuleInitializer]`) so
    the SDK-import chain (`Sdk="Microsoft.NET.Sdk"`) resolves against the real installed .NET SDK -
    no real C# compile or Revit is involved

## Documentation layout

- `AGENTS.md` links to the required repository guidance and routes a change to the document that owns it.
- `docs/policies/` contains the repository policies — the rules a change must follow. The
  [policy index](policies/README.md) says what each policy covers.
- `docs/repository.md` describes the project, repository structure, and technology stack.
- `changelog.d/` holds the pending changelog fragments: a consumer-visible change adds one, and the
  release preparation composes them into `CHANGELOG.md`; the rules are in the
  [changelog policy](policies/changelog.md). Git does not track empty directories, so the folder
  exists only while a fragment is pending.
- `scripts/` holds the repository validator, the changelog tool and the release verification.

The root solution exposes the documentation files under a `docs` solution folder in Visual Studio, preserving their subfolder structure. When adding documentation files, also add them as solution items; solution folders do not automatically include new files.

The root `global.json` selects stable .NET SDK 10.0 (minimum `10.0.103`, `rollForward: latestFeature`). CI and publishing install the SDK from this file. Additional SDK installations may provide older test runtimes. See the [SDK selection policy](policies/build.md#sdk-selection).

## Solution items

The root solution exposes repository-level documents and configuration under `solutionItems`, GitHub files and maintenance scripts in matching subfolders, and documentation under `docs/`. The list is explicit, not a filesystem glob; keep links up to date when files change. See the [solution items policy](policies/repository-maintenance.md#solution-items).

## Repository validation

`scripts/Validate-Repository.ps1` enforces the required repository documents,
their navigation links, the complete and valid Solution Items, and the workflow action pins. The
`.github/workflows/repository-policy.yml` workflow runs it, the pending fragment check
(`Update-Changelog.ps1 -Verify`) and the release structure check (`Verify-Release.ps1`) for pushes
and pull requests. See the [repository maintenance policy](policies/repository-maintenance.md#repository-validation).

## Formatting

The root `.editorconfig` defines the portable formatting baseline. Existing repositories may add stricter C# or analyzer-specific settings. See the [repository maintenance policy](policies/repository-maintenance.md#formatting-baseline).

## Continuous integration

`build-and-test` builds the solution in `Release` with `-p:Platform=x64`, warnings as errors and
documentation warnings promoted to errors, then runs the headless tests and uploads the package.
`repository-policy` runs the validator, the pending fragment check and the release structure check.
`publish-nuget` is manual and refuses a commit without a single stable tag and a matching changelog
section.

## Versioning and release tags

The SDK package uses MinVer 8 with stable tags in the `vMAJOR.MINOR.PATCH` format. The tag without its `v` prefix is the NuGet package version; unlike runtime Revit libraries, the package version is not rewritten per Revit year. `scripts/Verify-Release.ps1 -FromHeadTag` accepts exactly one matching tag at `HEAD`, and the publish workflow verifies the package ID and version before pushing to NuGet.

The tagged commit must already contain the release section in `CHANGELOG.md` with an empty `Unreleased` section above it and no pending fragment; the publish workflow stops otherwise. See the [release policy](policies/releases.md) and the [changelog policy](policies/changelog.md#release).
