# 05-Backlog Audit Report

## Scope

Updated audit after sequencing, renames, and backlog rationalization changes in `docs/05-backlog`.

---

## Current File Inventory (Post-Change)

- `00-backlog.md`
- `01-roadmap.md`
- `04-backlog-tabletop-engine.md`
- `05-backlog-ui-presentation.md`
- `06-backlog-rules-framework.md`
- `08-backlog-networking.md`
- `09-backlog-content-import.md`
- `09-backlog-persistence-infrastructure.md`
- `20-backlog-checkers-reference.md`
- `21-checkers-roadmap.md`
- `60-backlog-testing-qa.md`
- `90-master-backlog-reference.md`
- `98-audit.md`
- `99-index.md`

---

## What Was Rationalized

1. Canonical/reference split is now clearer:
- `00-backlog.md` = canonical working backlog.
- `90-master-backlog-reference.md` = reference-only roll-up.

2. Roadmaps are sequenced and role-separated:
- `01-roadmap.md` = canonical platform roadmap.
- `21-checkers-roadmap.md` = scoped/reference roadmap.

3. Documentation-planning backlog was consolidated:
- `backlog-documentation-planning.md` was removed.
- durable DOC traceability was merged into `90-master-backlog-reference.md`.

4. Category backlog files are now mostly sequence-prefixed.

---

## Remaining Issues / Observations

1. **Non-strict sequencing in middle range**
- numbering skips (`02`, `03`, `07`, etc.) which is acceptable but not strict.

2. **Duplicate `09-*` prefixes**
- `09-backlog-content-import.md`
- `09-backlog-persistence-infrastructure.md`

This is valid but can be confusing in scan order.

3. **Index and cross-reference hygiene should remain active**
- maintain `99-index.md` as canonical map when filenames move.

---

## Recommended Next Normalization Pass (Optional)

If strict numeric uniqueness is desired:

- keep existing canonical docs as-is (`00`, `01`, `90`, `99`)
- reassign category prefixes to unique values (for example 10–16 range)

Example:
- `10-backlog-tabletop-engine.md`
- `11-backlog-ui-presentation.md`
- `12-backlog-rules-framework.md`
- `13-backlog-networking.md`
- `14-backlog-content-import.md`
- `15-backlog-persistence-infrastructure.md`
- `16-backlog-testing-qa.md`

---

## Conclusion

Backlog rationalization is materially improved and major confusion points were addressed (especially canonical vs reference and documentation-planning overlap). Remaining work is mostly optional naming strictness rather than structural ambiguity.
