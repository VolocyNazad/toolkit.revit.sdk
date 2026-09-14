# Changelog

All notable changes to this project are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]


### Added

- Validate required repository files, navigation links, and Solution Items in CI.

### Fixed

- Configure the xUnit v3 test project as a Microsoft.Testing.Platform executable, use the current parallelization API, and document intentional MSBuild task packaging.

### Changed

- Update MSBuild dependencies, MinVer, and the test toolchain.
- Run SDK tests through xUnit v3 and Microsoft.Testing.Platform in CI and before publishing.
- Translate API and test documentation comments into English.
- Clean package output before validating and publishing the NuGet package.
- Standardize GitHub Actions workflow filenames and display names by responsibility.
- Establish a shared EditorConfig baseline and use the repository-policy validator as the single structural CI check.
- Complete solution items for repository documents, configuration, workflows and maintenance scripts; document the shared layout.
- Standardize local and CI SDK selection on stable .NET 10.0 through global.json, restrict roll-forward to that major/minor line, and configure setup-dotnet to read the file.
- Show documentation in Visual Studio Solution Explorer under a `docs` solution folder with matching subfolders.
- Move development policies and repository guidance from `AGENTS.md` to `docs/policies/development.md` and `docs/repository.md`; keep required reading links in `AGENTS.md` and add README navigation.
