# Pull request policy

Paths mentioned in this policy are relative to the repository root unless a Markdown link specifies otherwise.

Read when: preparing, reviewing or merging a pull request.

Every change reaches `main` and `dev` through a short-lived branch and a pull request. The
[branching policy](branching.md) owns branch kinds, base branches, merge strategy and protection
settings. The template is `.github/pull_request_template.md`; the required checks are the
`build-and-test` and `repository-policy` workflows.

## Source branch and target

| Source branch | Cut from | Pull request targets |
| --- | --- | --- |
| `feature/<ticket>-<slug>` | `dev` | `dev` |
| `fix/<ticket>-<slug>` | `dev` | `dev` |
| `chore/<slug>` | `dev` | `dev` |
| `release/<version>` | `dev` | `main` |
| `hotfix/<version>` | `main` | `main` |

- A documentation-only change is a `chore/<slug>` branch that targets `dev`, as the
  [branching policy](branching.md#branches) defines; there is no separate `docs/` branch kind.
- A hotfix is also merged back into `dev`, or into the open release branch when one exists.
- Never push directly to `main` or `dev`. Every change, including a release or hotfix preparation,
  arrives through a pull request that passes review and CI.

## Required checks

- `build-and-test` must pass. It builds the solution with warnings as errors, runs the headless tests
  through the project executable, and runs the strict public API documentation audit; a run that
  discovers no tests fails.
- `repository-policy` must pass. It runs `./scripts/Validate-Repository.ps1` for the required files,
  navigation links, solution items and action pins, `./scripts/Update-Changelog.ps1 -Verify` for the
  pending fragments, and `./scripts/Verify-Release.ps1` for the `CHANGELOG.md` structure.
- Merge only after both required workflows pass and the pull request has the required approval.

## Review

- A pull request needs at least one approval.
- Only maintainers merge into `main`; a release or hotfix pull request may use a single reviewer.
- Merge with a merge commit (`--no-ff`, **Create a merge commit**), not with squash or rebase;
  `git revert -m 1 <merge>` reverts the whole change.
- Update a branch by merging its target branch into it; do not rebase a branch that has been pushed
  and reviewed, and never force-push `main` or `dev`.

## Author checklist

Before opening a pull request, check every item that applies to the change:

- Assess whether tests are needed; add or update tests, and follow the test approval rule in the
  [development policy](development.md). MSBuild and SDK logic is covered by a test on a temporary
  project, as the [testing policy](testing.md) requires.
- Build the affected projects and configurations; resolve new compiler and analyzer warnings in
  touched code.
- Review compatibility with the supported Revit versions when the change touches the version
  resolution, the constants, the target framework selection or the manifest generation.
- Update XML documentation and the package README when a public task type, an MSBuild property or an
  item changes, as the [documentation policy](documentation.md) requires.
- Add the changelog fragment in `changelog.d/` when the change is observable by a consumer of the
  package, as the [changelog policy](changelog.md) requires.
- Run `./scripts/Validate-Repository.ps1` after changing repository-level files, documentation or the
  solution.
- Write commit messages as the [commit message policy](commits.md) requires.

## Release and hotfix pull requests

- A `release/<version>` pull request targets `main` and carries only fixes; new features wait for the
  next release. Compose, edit and finalize the release section with `./scripts/Update-Changelog.ps1`
  in the release branch before the tag, as the [changelog policy](changelog.md) requires.
- A `hotfix/<version>` pull request targets `main`. Merge the same branch back into `dev`, or into the
  open release branch when there is one.
- The tag is created on the merge commit on `main` after the pull request is merged; the branch and
  tag steps are in the [branching policy](branching.md) and the publishing steps in the
  [release policy](releases.md).

## Template

`.github/pull_request_template.md` is the default and applies automatically. It keeps three sections:

- **Summary** — what changed and why; link the issue when there is one.
- **Branch** — the source branch, its base and target, and confirmation that `main` and `dev` are not
  changed by a direct push.
- **Checklist** — the build and test result for the affected projects,
  `./scripts/Validate-Repository.ps1`, the changelog fragment when the change carries one, and the XML
  documentation update when the public API changed.

The Branch and Checklist items restate rules owned by the [branching policy](branching.md) and the
[changelog policy](changelog.md); complete them.

The rest of the contributor workflow is defined by the [development policy](development.md).
