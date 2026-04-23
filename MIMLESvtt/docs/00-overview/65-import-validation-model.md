# Import Validation Model

## Purpose

Define the shared validation model for CSV/content imports, including severity levels, required/optional behavior, and duplicate-handling policy.

---

## Validation Levels

Validation runs at two levels:

1. **File-level validation**
   - schema/header integrity
   - import type compatibility
   - required column presence

2. **Row-level validation**
   - required field presence
   - type/range checks
   - reference/key checks
   - duplicate key detection

---

## Severity Model

Validation issues use explicit severity:

- `Info` – non-blocking note
- `Warning` – import may proceed with caution/policy
- `Error` – row or file is blocked from persistence

Recommended issue shape:

- severity
- stage (`Parse`, `Map`, `Validate`, `Persist`)
- row index (optional)
- field/key (optional)
- message

---

## Required vs Optional Fields

- **Required fields** must be present and valid; otherwise produce `Error`.
- **Optional fields** may be blank, but if provided they must pass type/format checks.
- Optional extension metadata is allowed unless structurally malformed.

---

## Duplicate Handling Policy

Duplicate strategy is policy-driven per import run:

- `RejectDuplicate` – duplicates produce `Error` and are not persisted
- `SkipDuplicate` – duplicates produce `Warning` and are skipped
- `UpdateDuplicate` – duplicates produce `Info/Warning` and update existing entries

Policy choice must be reflected in import summary counts.

---

## Output Expectations

Validation model feeds import result summary with:

- created/updated/skipped/failed counts
- aggregated warnings/errors
- row-level issue diagnostics for retry/fix workflow

---

## Related Docs

- `docs/00-overview/64-reusable-csv-import-pipeline.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-013)
