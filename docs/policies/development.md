# Development policy

Paths mentioned in this policy are relative to the repository root unless a Markdown link specifies otherwise.

Read when: you start a change; it owns the stack priority, the asking rule and the code basics.

The [policy index](README.md) lists every policy with the area it covers and what enforces it. This
policy is the entry point: it states the rules that apply to any change and routes the rest.

The stack documented in the [repository guide](../repository.md) is the default and takes priority
over whatever an agent or a contributor might otherwise reach for. Prefer what is already in use
over introducing an alternative. If a deviation seems necessary, say so explicitly to the user and
get confirmation before adding it.

If a change affects the folder structure or the technology stack (a new or removed project, a new
dependency, a version bump worth recording, a new convention), update the relevant policy and the
[repository guide](../repository.md) in the same change, never as a later follow-up.

Before changing existing tests or writing new ones, ask the user first: confirm what should be
covered and how, or that the change is trivial enough not to need it.

Do not `git push` — commit locally and leave the push to the user unless they explicitly ask.

## Where the rules live

| Topic | Policy |
| --- | --- |
| Branch, base branch, merge strategy | [Branching policy](branching.md) |
| Commit message | [Commit message policy](commits.md) |
| `CHANGELOG.md` and `changelog.d/` | [Changelog policy](changelog.md) |
| Pull request, review, required checks | [Pull request policy](pull-requests.md) |
| Tests | [Testing policy](testing.md) |
| Benchmarks | [Benchmark policy](benchmarks.md) |
| Release, tag, NuGet package | [Release policy](releases.md) |
| Contributor documentation and XML docs | [Documentation policy](documentation.md) |
| .NET SDK, projects, packages, warnings, Revit constants | [Build and toolchain policy](build.md) |
| Solution items, validator, formatting | [Repository maintenance policy](repository-maintenance.md) |

## Code basics

Do not invent abbreviations in identifiers, file names or documentation. Use complete words such as
`Configuration`, `Manifest` and `TargetFramework`; established terms such as `API`, `XML`, `Guid`,
`Id` and `Uri` remain valid.

Use `global using` only when most files in that project need the namespace. Keep specialized
namespaces local, and do not repeat namespaces already supplied by `ImplicitUsings`. Apply this rule
to new and touched code; clean up existing files only within the task scope.

The SDK is consumed as an MSBuild package, so its property, item and target names are public API.
Renaming one is a consumer-visible change: update the documentation in the same change and follow the
[release policy](releases.md).

Keep `CHANGELOG.md` in the state the [changelog policy](changelog.md) requires: regular development
leaves `## [Unreleased]` empty, and a change carries its entry as a fragment in `changelog.d/`.
