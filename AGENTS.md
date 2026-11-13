# AGENTS.md

This repository keeps its rules and facts in documents, not in this file. Load only what your change
needs: the table routes you, and each document says what it owns.

| Your change touches | Read first | Then, only when the change crosses that area |
| --- | --- | --- |
| Anything | [Development policy](docs/policies/development.md) — the asking rule, the stack priority, code basics | The [policy index](docs/policies/README.md): pick the one policy that covers the area |
| Project structure, dependencies, technology stack, build, Revit versions | [Repository guide](docs/repository.md) | [Build and toolchain policy](docs/policies/build.md) when the SDK, a project, a package, a Revit version or a warning rule changes |
| A public task type, an MSBuild property or item | [Documentation policy](docs/policies/documentation.md) | [Release policy](docs/policies/releases.md) when it ships, and the [changelog policy](docs/policies/changelog.md) for its fragment |
| Release, version, NuGet package, `CHANGELOG.md` | [Release policy](docs/policies/releases.md) and [changelog policy](docs/policies/changelog.md) | [Build and toolchain policy](docs/policies/build.md) |
| A pull request | [Pull request policy](docs/policies/pull-requests.md) | [Commit message policy](docs/policies/commits.md) and [branching policy](docs/policies/branching.md) |

## How to load context

- Open sections, not whole files. The [repository guide](docs/repository.md) is a reference for the
  area you touch; reading it end to end is not required.
- Documents under [docs/policies/](docs/policies/) are binding rules; the [policy
  index](docs/policies/README.md) says what each one owns. Read the policy that covers your area and
  the [development policy](docs/policies/development.md), not all of them.
- `CHANGELOG.md` keeps `## [Unreleased]` empty; a change carries its entry as a fragment in
  `changelog.d/`, which the release preparation composes.
- Do not read `changelog.d/` for context: open it only to add a fragment or to compose the release
  section.
- Do not read generated or ignored directories for context: `artifacts/`, `bin/`, `obj/`, `.vs/`,
  `.idea/`, `TestResults/`.
- Verify before finishing: [scripts/Validate-Repository.ps1](scripts/Validate-Repository.ps1) checks
  links, anchors, required files, solution items and action pins;
  [scripts/Update-Changelog.ps1](scripts/Update-Changelog.ps1) checks the pending changelog
  fragments; [scripts/Verify-Release.ps1](scripts/Verify-Release.ps1) checks the `CHANGELOG.md`
  structure.
