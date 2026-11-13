# Documentation policy

Paths mentioned in this policy are relative to the repository root unless a Markdown link specifies otherwise.

Read when: writing, moving or restructuring contributor documentation or XML documentation.

Every document has one genre and one subject, so each fact has one home and the rest of the repository
links to it instead of restating it.

## Documents and genres

| Document | Genre | Answers |
| --- | --- | --- |
| `docs/policies/*.md` (index: [repository policies](README.md)) | Binding rules | What a change must do in one area |
| [Repository guide](../repository.md) | Facts about the repository | How the solution, projects, tooling and documents are arranged |
| [README.md](../../README.md) | Product description | What the package offers, how to install and use it |
| [CONTRIBUTING.md](../../CONTRIBUTING.md) | Contributor workflow | How a change is prepared, checked and submitted |
| [CHANGELOG.md](../../CHANGELOG.md) and `changelog.d/` | Release history | What one release changed; what is pending (see the [changelog policy](changelog.md)) |
| [AGENTS.md](../../AGENTS.md) | Entry point | Which documents a contributor must read before changing the repository |

The genre decides the content: do not move a rule into a guide, and do not record a procedure as a
policy rule.

## Single source of truth

- Each fact, rule or procedure has exactly one owning document. Another document that needs it links
  to it.
- Do not copy a section into a second document; keep the full text in the owning document and link to
  it.
- When a section is renamed, moved or split, update every document that references it in the same
  change. Never leave a link pointing at a heading that no longer exists.
- A summary in a guide links to the owning document and does not become a second rule.

## Updating documentation in the same change

- A change that affects the folder structure, the technology stack, a dependency, a recorded version,
  a supported Revit year or a new convention updates the relevant policy and the
  [repository guide](../repository.md) in the same change, never as a later follow-up.
- A change that alters the package behaviour, an MSBuild property, an item or a target updates
  [README.md](../../README.md) in the same change.
- A change that adds or removes a public API updates its XML documentation in the same change.
- A change covered by the [changelog policy](changelog.md) adds its fragment in `changelog.d/`.
- A new documentation file is added as a solution item together with the file; the
  [repository maintenance policy](repository-maintenance.md) defines the solution items contract.
- Documentation is not deferred.

## XML documentation

- XML documentation is mandatory for public APIs in `src/`; test projects are exempt.
- Documentation explains the observable contract rather than restating the identifier.
- Use `<remarks>` for behaviour that is not evident from the signature: implicit fallback, inferred
  defaults, side effects, ordering, caching, thread requirements, file-system ownership and
  intentionally ignored failures.
- Keep the note next to the affected API or configuration setting, not in a distant guide.
- If a type is public only accidentally and has no external or cross-project consumer, reduce its
  visibility instead of documenting it as an API.
- Run the strict audit with
  `dotnet build Toolkit.Revit.Sdk.slnx --configuration Release -p:Platform=x64 -p:EnforcePublicApiDocumentation=true`.
  The switch promotes `CS1591` to an error, and `.github/workflows/build-and-test.yml` runs it on
  every pull request and push.

## Documentation language

- Policies and guides are written in English.
- The package README is written in English; it is shipped inside the package and is the consumer's
  first contact with the SDK.
- Identifiers follow the naming rules of the [development policy](development.md).

## Links and anchors

- Link between repository documents with relative Markdown links. Do not use absolute repository URLs
  in repository documents.
- An anchor must point at a heading that exists.
- Do not leave a broken or ambiguous link. If a target no longer exists, update the text instead of
  deleting only the link.
- `scripts/Validate-Repository.ps1` checks the required repository files, the navigation links and
  their anchors, and the completeness of the solution items. The
  [repository maintenance policy](repository-maintenance.md) defines when it must run.

## Adding a policy document

A new policy follows the procedure in the [policy index](README.md#adding-or-changing-a-policy): one
file per subject in `docs/policies/`, a row in the index, a solution item and a link from the place
that needs the rule. Write it like the other policies — the paths sentence, the rules, and the closing
pointer to the [development policy](development.md) — and run `./scripts/Validate-Repository.ps1`.

The rest of the contributor workflow is defined by the [development policy](development.md).
