# Repository guide

Paths in this document are relative to the repository root.
Read and follow the [development policy](policies/development.md) alongside this guide.

## About

This repo is the source for `VolocyNazad.Revit.Sdk` (published under the
`VolocyNazad` org), a custom MSBuild SDK for Autodesk Revit add-in
projects: supports Revit 2021-2027, picks `net48` vs `net8.0-windows`
automatically based on the target Revit version encoded in the build
configuration name, defines version conditional-compilation constants
(`VERSION_2025`, `IS2025`, `BEFORE2026`, `AFTER2024`, ...), excludes
`RevitAPI.dll`/`RevitAPIUI.dll` from output, generates the `.addin`
manifest, and can merge dependencies via ILRepack. It's what
`impact.revitaddinapi/revit` and every `toolkit.revit.*` repo build
against (`<Project Sdk="VolocyNazad.Revit.Sdk/...">`).

## Repository structure

```
.
├── src/
│   └── Toolkit.Revit.Sdk/          the SDK itself (MSBuild tasks/targets)
└── tests/
    └── Toolkit.Revit.Sdk.Tests/    xunit tests for the task logic (e.g. GenerateAddinManifest)
```

## Tech stack

- .NET, multi-targets `net48;net10.0`
- `Microsoft.Build.Utilities.Core` (MSBuild task API)
- MinVer (git-tag-based versioning)
- PolySharp (C# language polyfills)
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

- `AGENTS.md` links to the required repository guidance.
- `docs/policies/development.md` contains the development policy.
- `docs/repository.md` describes the project, repository structure, and technology stack.

The root solution exposes the documentation files under a `docs` solution folder in Visual Studio, preserving their subfolder structure. When adding documentation files, also add them as solution items; solution folders do not automatically include new files.

The root `global.json` selects stable .NET SDK 10.0 (minimum `10.0.103`, `rollForward: latestFeature`). CI and publishing install the SDK from this file. Additional SDK installations may provide older test runtimes. See the [SDK selection policy](policies/development.md#net-sdk-selection).

## Solution items

The root solution exposes repository-level documents and configuration under `solutionItems`, GitHub files and maintenance scripts in matching subfolders, and documentation under `docs/`. The list is explicit, not a filesystem glob; keep links up to date when files change. See the [solution items policy](policies/development.md#solution-items).
## Repository validation

`scripts/Validate-Repository.ps1` enforces the required repository documents,
their navigation links, and complete, valid Solution Items. The
`.github/workflows/repository-policy.yml` workflow runs it for pushes and pull
requests. See the [development policy](policies/development.md#repository-validation).

## Formatting

The root `.editorconfig` defines the portable formatting baseline. Existing repositories may add stricter C# or analyzer-specific settings. See the [development policy](policies/development.md#formatting-baseline).

## Versioning and release tags

The SDK package uses MinVer 8 with stable tags in the `vMAJOR.MINOR.PATCH` format. The tag without its `v` prefix is the NuGet package version; unlike runtime Revit libraries, the package version is not rewritten per Revit year. Manual publishing accepts exactly one matching tag at HEAD and verifies the package ID and version before pushing to NuGet.
