# AGENTS.md

## Policy

The stack documented below is the default and takes priority over
whatever an agent might otherwise reach for. Prefer what's already in use
over introducing an alternative. If a deviation seems necessary, say so
explicitly to the user and get confirmation before adding it.

If a change affects the folder structure or the tech stack (a new/removed project, a new dependency, a version bump worth recording, a new convention), update this file accordingly as part of the same change - don't leave it to a later pass.

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

A single small project, no separate `tests/`:

```
.
└── src/
    └── Toolkit.Revit.Sdk/    the SDK itself (MSBuild tasks/targets)
```

## Tech stack

- .NET, multi-targets `net48;net10.0`
- `Microsoft.Build.Utilities.Core` (MSBuild task API)
- MinVer (git-tag-based versioning)
- PolySharp (C# language polyfills)
