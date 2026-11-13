# Release policy

Paths mentioned in this policy are relative to the repository root unless a Markdown link specifies otherwise.

Read when: preparing a release tag or publishing the package.

A release turns a tagged commit into a NuGet package on nuget.org. The workflow is
`.github/workflows/publish-nuget.yml`; it consumes the tag and the release section of
`CHANGELOG.md`. The [changelog policy](changelog.md) owns that section's format, the
[branching policy](branching.md) owns release and hotfix branches, and the
[testing policy](testing.md) owns what must be green before the tag.

## Release channels

- A **stable release** is a `vMAJOR.MINOR.PATCH` tag and the only channel the automation publishes.
  The `v` belongs to the tag; the NuGet package version drops it: `v1.2.0` publishes `1.2.0`.
- A **pre-release** is a `vMAJOR.MINOR.PATCH-<channel>.<number>` tag such as `v1.3.0-beta.1`. NuGet
  accepts it as a pre-release and package managers do not select it for a plain version range.
  Current automation publishes stable releases only: create a pre-release manually with
  `dotnet nuget push` after the same verification, or extend the workflow first.
- A package version that was published is never reused, not even for a pre-release with the same
  number. `--skip-duplicate` protects a retried workflow run; it is not permission to republish.

## Versioning

- `MinVer` 8 derives the version from the tag on the released commit. A stable tag is the package
  version without the `v`; commits after a tag receive a `dev.<count>` pre-release suffix and are not
  published.
- The tag is the version source. The SDK package version is not coupled to a Revit year; a change that
  adds a Revit year is a minor release of the same package, not a new package line.
- Version the package by SemVer: a consumer-visible breaking change ships in a major version, a new
  behaviour in a minor version and a defect fix in a patch version. Renaming an MSBuild property, item
  or target is a breaking change.

## Preparing a release

Before creating the tag:

1. Confirm `build-and-test` and `repository-policy` are green on `dev` and on the release branch.
2. Compose the pending fragments with `./scripts/Update-Changelog.ps1 -Compose` and edit the collected
   entries under `## [Unreleased]`: merge related entries, drop what a consumer cannot observe and fix
   the wording. Commit the result.
3. Close the section with `./scripts/Update-Changelog.ps1 -Finalize <version>`, where `<version>` is
   the tag without its `v`, and commit the result together with the deleted fragments.
4. Confirm `## [Unreleased]` is empty and `changelog.d/` holds no fragment.
5. Merge the release branch into `main` with a merge commit, create the tag on that merge commit and
   push the tag.

The tagged commit already contains the release section in `CHANGELOG.md` with an empty
`## [Unreleased]` above it and no pending fragment; the workflow refuses to publish otherwise.

## Release metadata rules

- Never mark a stable version as a pre-release, and never give a pre-release a stable version number.
- Never delete, move or force-push a pushed release tag. A pushed tag is immutable; publish the next
  version instead. If a mistake is found before the tag is pushed and nothing was published, fix the
  commit, move the local tag and push it once.
- A package that was already published with a wrong version stays published; correct the content in
  the next version and explain it in the changelog.

## Package artifacts

- The SDK packs on build into `artifacts/`: one `VolocyNazad.Revit.Sdk.<version>.nupkg`.
- The package carries `README.md`, `LICENSE.md`, the XML documentation file and the package metadata
  declared in `src/Toolkit.Revit.Sdk/Toolkit.Revit.Sdk.csproj`, plus the `Sdk/` props and targets a
  consuming project imports.
- The workflow verifies the `.nuspec` package id and version against the tag before pushing; a
  mismatch fails the release.

## Publish automation

The workflow runs on `workflow_dispatch`; a tag push alone does not publish. It:

- checks out with full history, because `MinVer` reads the tags;
- runs `./scripts/Verify-Release.ps1 -FromHeadTag -AncestorOf main`: `HEAD` must carry exactly one
  stable `v` tag, `CHANGELOG.md` must have an empty `## [Unreleased]` and exactly one
  `## [VERSION] - YYYY-MM-DD` section for the tag directly below it, and `HEAD` must be an ancestor
  of `origin/main`;
- builds the solution in `Release` with warnings as errors and runs the headless tests;
- verifies the package id and version, then pushes it to nuget.org.

After the pipeline, inspect the package on nuget.org: the version, the dependencies and the README.

The rest of the contributor workflow is defined by the [development policy](development.md).
