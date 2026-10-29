# Contributing

Before making a change, read the [development policy](docs/policies/development.md)
and the [repository guide](docs/repository.md). They define the repository-specific
stack, testing expectations and maintenance rules.

Branches follow the [branching policy](docs/policies/branching.md): `main` holds
the released state, `dev` integrates the next release, and every change arrives
through a short-lived `feature/`, `fix/`, `release/` or `hotfix/` branch and a
pull request.

Commit messages follow the [commit message policy](docs/policies/commits.md).

Keep changes focused and update [CHANGELOG.md](CHANGELOG.md) under `Unreleased`
when the development policy requires it. Run the repository build and the policy
validator before submitting a pull request:

```powershell
./scripts/Validate-Repository.ps1
```
