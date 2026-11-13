# Build and toolchain policy

Paths mentioned in this policy are relative to the repository root unless a Markdown link specifies otherwise.

Read when: changing the SDK, a project, a package version, a warning rule or a Revit version.

A build is reproducible from a clean checkout with the SDK selected by the repository and the package
versions declared in the repository. It states rules, not build recipes: commands and stack facts
belong to the [repository guide](../repository.md).

## SDK selection

- Take the .NET SDK from the repository-root `global.json`; it is the only source of the SDK for
  local builds, CI and publishing.
- Keep `global.json` on a stable SDK `10.0.103` or later in the `10.0` major/minor line, with
  `rollForward: latestFeature` and `allowPrerelease: false`. Do not move to another major/minor line
  without changing this policy and `global.json` in the same change.
- Read the SDK in every GitHub Actions setup step with `global-json-file: global.json` after
  checkout. Do not install a floating or explicitly versioned SDK in the workflow instead.
- This policy does not change the target frameworks the SDK assigns to a consuming project; those
  follow the Revit version below.

## Projects

- `src/Toolkit.Revit.Sdk` is the SDK package: the MSBuild tasks (`GenerateAddinManifest`) and the
  `Sdk/*.props` and `Sdk/*.targets` files that a consuming project imports. It targets `net48` and
  `net10.0` and references `Microsoft.Build.Utilities.Core` for the task API; the build does not
  depend on the Revit API.
- `tests/Toolkit.Revit.Sdk.Tests` is the headless test project, targets `net10.0` and is not a
  package. It exercises the task logic directly and the `.props`/`.targets` logic on a temporary
  project.
- A new project arrives with its solution entry, its tests where the [testing policy](testing.md)
  requires them, and the matching CI step in the same change.
- Do not add a project that only exists to work around a build setting; fix the shared configuration
  instead.

## Revit versions

- The consuming project names the Revit version in its configuration, such as `Debug_2021.1.100` or
  `Release_2025.4.60`. `RevitVersionResolution.props` derives `RevitVersion` and `MajorRevitVersion`
  from it; do not read the version from anywhere else.
- `MajorRevitVersion` selects the target framework: `net48` for Revit 2021-2024, `net8.0-windows` for
  2025-2026 and `net10.0-windows` for 2027.
- `SetupRevitDefineConstants.props` defines `VERSION_<year>`, `IS<year>`, `BEFORE<year>` and
  `AFTER<year>`. `BEFORE<year>` means `MajorRevitVersion <= year` and `AFTER<year>` means
  `>= year`; both bounds are inclusive. Express a range as `AFTER<low>` plus `BEFORE<high>`, never as
  `AFTER<low - 1>`.
- Use the constants in code instead of comparing a raw Revit year. When no constant covers the
  boundary the change needs, add the constant pair in the same change and document it in the
  [repository guide](../repository.md) and the package README.
- Adding or dropping a supported Revit year touches the version resolution, the constants, the
  supported version list in the [repository guide](../repository.md) and [README.md](../../README.md),
  the [testing policy](testing.md) and the validator expectations together.

## Central package management

- Pin every package version in the nearest `Directory.Packages.props`. The root file serves the SDK;
  `tests/Directory.Packages.props` imports it and holds the test-only versions.
- Keep central package management enabled (`ManagePackageVersionsCentrally=true`); a
  `PackageReference` in a project carries no `Version`.
- Add repository-wide analyzer and generator packages through `GlobalPackageReference` with private
  assets. Do not repeat them per project.
- Change a package version in one place only. When a project resolves a version you did not expect,
  look for a nearer props file before editing the project.

## Warnings

- Treat compiler and analyzer warnings as delayed errors. Do not leave new warnings in code you
  touch; fix them in the same change.
- Suppress a deliberate false positive as narrowly as possible and add a nearby comment explaining
  why.
- Keep every build warning-free. `.github/workflows/build-and-test.yml` and `publish-nuget.yml` build
  with `-p:TreatWarningsAsErrors=true`, so a warning fails both the pull request and the publish.
- Do not rely on a local build to find warnings; build the configuration you touch before opening the
  pull request.

## Analyzers

- The root `Directory.Build.props` enables `AnalysisMode=AllEnabledByDefault` for every project.
  Treat the resulting diagnostics as part of the build contract, not as optional advice.
- A rule that is wrong for this SDK is disabled in `.editorconfig` with a comment naming the reason,
  never with a file-wide `#pragma warning disable` in source.
- A new analyzer package follows the central package rule above and is reviewed like production code:
  a diagnostic that fires on the existing code is fixed or explicitly disabled in the same change.

## Public API documentation

`GenerateDocumentationFile` is enabled repository-wide. The strict audit promoted `CS1591` to an
error runs in CI with `-p:EnforcePublicApiDocumentation=true`; the [documentation policy](documentation.md)
owns what a public API must document. For this package that surface is the public task types and the
properties and items a consuming project sets, not only the C# types.

The rest of the contributor workflow is defined by the [development policy](development.md).
