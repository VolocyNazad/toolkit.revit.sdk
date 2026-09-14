# Development policy

Paths mentioned in this policy are relative to the repository root unless a Markdown link specifies otherwise.

The stack documented in [Repository guide](../repository.md) is the default and takes priority over
whatever an agent might otherwise reach for. Prefer what's already in use
over introducing an alternative. If a deviation seems necessary, say so
explicitly to the user and get confirmation before adding it.

If a change affects the folder structure or the tech stack (a new/removed project, a new dependency, a version bump worth recording, a new convention), update the relevant policy and [repository guide](../repository.md) accordingly as part of the same change - don't leave it to a later pass.

Before changing existing tests or writing new ones, ask the user first – confirm what should be covered and how (or that the change is trivial enough not to need it) rather than deciding unilaterally.

Commit messages follow Conventional Commits (`<type>(<scope>): <description>`, e.g. `feat(manifest): ...`, `fix(...): ...`, `docs(agents): ...`, `test(...): ...`, `chore(...): ...`, `refactor(...): ...`) - scope optional but preferred when it clarifies what changed.

Don't `git push` - commit locally and leave pushing to the user, unless they explicitly ask for a push.

Keep `CHANGELOG.md` up to date, in [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) format - add an entry under `## [Unreleased]` (in the right category: Added/Changed/Fixed/Removed/...) as part of the same change, not as a separate follow-up.

## .NET SDK selection

Use the repository-root `global.json` for local builds and CI: SDK `10.0.103` or a later stable SDK in the `10.0` major/minor line (`rollForward: latestFeature`, `allowPrerelease: false`). Do not roll forward to another major/minor line without updating this policy and `global.json` together. GitHub Actions setup steps must read `global-json-file: global.json` after checkout. Additional SDKs may be installed to supply runtimes for older test targets; they do not replace the SDK selected by `global.json`. This policy does not change project target frameworks.

## Solution items

Keep the root solution in sync with existing repository-level documentation, license, Git/editor settings, SDK/NuGet/versioning and shared MSBuild configuration, and root maintenance scripts. Expose root files under `solutionItems`, `.github/` and `scripts/` under matching subfolders, and documentation under `docs/` with its directory hierarchy. Add, rename or remove solution links together with the corresponding files. Include only existing files, once each; exclude local settings, secrets and generated output. Solution templates must follow the same convention.
## Repository validation

Run `./scripts/Validate-Repository.ps1` after changing repository-level files,
documentation or the solution. CI runs the same validator. It checks the required
files and navigation links and verifies that managed repository files are present
exactly once in Solution Items. Update the solution together with added, renamed or
removed managed files. Solution templates follow the same contract.

## Formatting baseline

Keep the root `.editorconfig` in the solution and apply its repository-wide encoding, line-ending and trailing-whitespace rules. Repository-specific sections may add stricter language rules. Update the file deliberately when formatting conventions change; do not replace specialized rules with the shared minimum.
