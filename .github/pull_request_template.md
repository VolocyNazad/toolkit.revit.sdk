## Summary

<!-- What changed and why. Link the issue when there is one. -->

## Branch

- [ ] The source branch follows the [branching policy](../docs/policies/branching.md):
  `feature/<ticket>-<slug>`, `fix/<ticket>-<slug>` or `chore/<slug>` from `dev`,
  `release/<version>` from `dev`, or `hotfix/<version>` from `main`.
- [ ] The pull request targets `dev` for a feature, a fix or repository maintenance, and
  `main` for a release or a hotfix. A hotfix is also merged back into `dev` or into the
  open release branch.
- [ ] `main` and `dev` are not changed by a direct push.

## Checklist

- [ ] Build and tests for the affected projects and Revit configurations pass.
- [ ] `./scripts/Validate-Repository.ps1` passes after changing repository-level
  files, documentation or the solution.
- [ ] `./scripts/Update-Changelog.ps1 -Verify` passes, and a consumer-visible change
  carries its fragment in `changelog.d/` as the
  [changelog policy](../docs/policies/changelog.md) requires.
- [ ] XML documentation and the package README are updated when a public task type, an
  MSBuild property or an item changes.
