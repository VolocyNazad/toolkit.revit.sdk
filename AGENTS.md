# AGENTS.md

## Policy

The stack documented below is the default and takes priority over
whatever an agent might otherwise reach for. Prefer what's already in use
over introducing an alternative. If a deviation seems necessary, say so
explicitly to the user and get confirmation before adding it.

If a change affects the folder structure or the tech stack (a new/removed project, a new dependency, a version bump worth recording, a new convention), update this file accordingly as part of the same change - don't leave it to a later pass.

Before changing existing tests or writing new ones, ask the user first – confirm what should be covered and how (or that the change is trivial enough not to need it) rather than deciding unilaterally.

Commit messages follow Conventional Commits (`<type>(<scope>): <description>`, e.g. `feat(manifest): ...`, `fix(...): ...`, `docs(agents): ...`, `test(...): ...`, `chore(...): ...`, `refactor(...): ...`) - scope optional but preferred when it clarifies what changed.

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
- xunit (unit tests, in `tests/Toolkit.Revit.Sdk.Tests`, targets `net10.0` only), two styles:
  - task logic (`GenerateAddinManifest`) is tested by instantiating the task directly with
    `Microsoft.Build.Utilities.TaskItem` inputs and asserting on the generated `.addin` XML
  - `.props`/`.targets` logic (version parsing, `DefineConstants`, `RemoveRevitAPICopyLocal`,
    `ValidateMajorRevitVersion`) is tested by importing the real files into a throwaway temp project
    and evaluating/building it via `Microsoft.Build.Evaluation`/`Microsoft.Build.Execution`, bootstrapped
    with `Microsoft.Build.Locator` (`MSBuildLocator.RegisterDefaults()` in a `[ModuleInitializer]`) so
    the SDK-import chain (`Sdk="Microsoft.NET.Sdk"`) resolves against the real installed .NET SDK -
    no real C# compile or Revit is involved
