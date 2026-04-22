# 00-overview Audit

## Scope

Audit of `docs/00-overview` after file renumbering and module documentation move.

---

## Directory Inventory (Current)

Observed files:

1. `00-what-is-a-vtt.md`
2. `01-core-class-list.md`
3. `02-root-classes.md`
4. `03-workspace-launch-contract.md`
5. `04-workspace-use-case-flows.md`
6. `05-tabletop-screen-use-cases-and-actions.md`
7. `06-global-action-system.md`
8. `07-piece-definition-model.md`
9. `08-rules-plugin-framework.md`
10. `09-tabletop-state-model.md`
11. `10-turn-sequence-model.md`
12. `20-module-architecture.md`
13. `21-checkers-class-list.md`
14. `22-checkers-rules-module.md`
15. `97-design-packet-integration-report.md`
16. `98-glossary.md`
17. `99-index.md`

General observation:
- Numbered structure is clearer and mostly coherent.
- `00-overview` now effectively hosts both overview and core-model documentation.

---

## Cross-Reference Cleanup Applied

Completed in this pass:

1. `99-index.md`
- Replaced stale unnumbered file references with current numbered filenames.
- Expanded index to include all current overview docs.

2. `20-module-architecture.md`
- Updated File Location from old `docs/04-modules/module-architecture.md` to current `docs/00-overview/20-module-architecture.md`.

3. `97-design-packet-integration-report.md`
- Updated stale references to:
  - old overview filenames (`table-state-model.md`, `domain-glossary.md`, etc.)
  - old module location (`docs/04-modules/module-architecture.md`)
  - old persistence policy file path (`docs/03-import-export/persistence-and-import-export.md`)
- Updated roadmap wording to reflect canonical vs scoped roadmap labeling.
- Marked old build-failure statement as historical context.

---

## Problems Identified

## 1) Stale references likely still exist outside 00-overview

Because docs were moved/renumbered, additional stale links may exist in:
- `docs/99-admin/*`
- backlog docs
- user docs

Recommendation:
- run a whole-docs link sweep for old names:
  - `domain-glossary.md`
  - `table-state-model.md`
  - `turn-sequence-model.md`
  - `piece-definition-model.md`
  - `rules-plugin-framework.md`
  - `session-state-persistence.md`
  - `module-architecture.md` (old path)
  - `persistence-and-import-export.md` (old split file)

## 2) Terminology drift in legacy report documents

`97-design-packet-integration-report.md` is historical and still includes assumptions that no longer match current code/doc status in every section.

Recommendation:
- either mark it explicitly as archival snapshot
- or create a short `97b-current-status-notes.md` with current-state deltas

## 3) Encoding artifact in one heading

`05-tabletop-screen-use-cases-and-actions.md` heading appears with malformed dash character in one inventory readout (`â€”`).

Recommendation:
- normalize to standard UTF-8 em dash or plain hyphen for consistency.

## 4) Optional duplication risk

`01-core-class-list.md` and `02-root-classes.md` now overlap in areas (class-oriented explanation).

Recommendation:
- keep both only if split intent is explicit:
  - `01` = subsystem-to-class map
  - `02` = API-style root classes

---

## Suggested Next Steps

1. Do a repository-wide docs link normalization pass (all docs folders). DONE
2. Add explicit “archival/historical” banner at top of `97-design-packet-integration-report.md`. DELETED
3. Normalize heading punctuation/encoding in `05-tabletop-screen-use-cases-and-actions.md`.
4. Keep `99-index.md` as canonical map and update it whenever files are renumbered/moved.

---

## Summary

`00-overview` is now in a much better state after consolidation and numbering. The primary remaining risk is stale references in other docs folders and historical-document drift, not structural failure of the overview directory itself.
