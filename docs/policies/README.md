# Repository policies

Read the policy for your area before changing it; the [development policy](development.md) is the
entry point and links the rest.

Facts about the repository belong in the [repository guide](../repository.md), what the package
offers in [README.md](../../README.md), and the pending changelog entries in `changelog.d/`.

## Conventions

- Policies are written in English.
- Every policy starts with the sentence "Paths mentioned in this policy are relative to the
  repository root unless a Markdown link specifies otherwise.".
- Every policy has a `Read when:` line after the paths sentence.
- Policy links are relative Markdown links; an anchor must point at an existing heading, checked by
  `scripts/Validate-Repository.ps1`.
- A policy ends by pointing to the [development policy](development.md).
- A policy owns exactly one subject. Rules owned by another policy are linked, not repeated.

## Policy index

| Policy | Covers | Enforced by |
| --- | --- | --- |
| [Development policy](development.md) | Entry point: stack priority, asking before a change, code basics, links to every policy | Review |
| [Branching policy](branching.md) | Branch kinds, base branches, merge strategy, protected branches | Review; branch protection is configured in GitHub, not in the repository |
| [Commit message policy](commits.md) | Conventional Commits, types, scope, body and footers | Review |
| [Changelog policy](changelog.md) | `CHANGELOG.md` format, `changelog.d/` fragments, the compose and finalize tool, release section | `scripts/Update-Changelog.ps1` and `scripts/Verify-Release.ps1` in `.github/workflows/repository-policy.yml`; `Verify-Release.ps1` in `.github/workflows/publish-nuget.yml` |
| [Testing policy](testing.md) | Test kinds, what needs a test, determinism, running tests | `.github/workflows/build-and-test.yml` |
| [Benchmark policy](benchmarks.md) | When to measure, how, how results are reported | Review; benchmarks deliberately do not run in CI |
| [Pull request policy](pull-requests.md) | Source branch and target, required checks, review, author checklist | Review; the `build-and-test` and `repository-policy` workflows are the required checks; branch protection is configured in GitHub |
| [Release policy](releases.md) | Release channels, tags, versioning, package artifacts, publish automation | `scripts/Verify-Release.ps1` in `.github/workflows/publish-nuget.yml` |
| [Documentation policy](documentation.md) | Document genres, single source of truth, updates in the same change, XML documentation, links | Review; the repository validator checks links and anchors |
| [Build and toolchain policy](build.md) | .NET SDK selection, projects, central packages, warnings, analyzers, Revit version constants | `.github/workflows/build-and-test.yml` and `publish-nuget.yml` (warnings as errors) |
| [Repository maintenance policy](repository-maintenance.md) | Solution items, required root files, repository validation, formatting baseline | `scripts/Validate-Repository.ps1` through `.github/workflows/repository-policy.yml` |

## Adding or changing a policy

1. Add or edit the file in `docs/policies/`. Keep one subject per policy.
2. Add the row to the [policy index](#policy-index) above.
3. Add the file to the `/docs/policies/` solution folder in `Toolkit.Revit.Sdk.slnx`, or the
   validator fails.
4. Link the policy from [development policy](development.md), [CONTRIBUTING.md](../../CONTRIBUTING.md)
   or the area guide; keep no second copy of the full list.
5. Run `./scripts/Validate-Repository.ps1`.

When an automated check replaces a policy rule, say so in the "Enforced by" column. Fix a policy that
is no longer enforced instead of leaving a dead rule.
