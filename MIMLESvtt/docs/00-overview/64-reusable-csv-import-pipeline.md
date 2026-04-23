# Reusable CSV Import Pipeline

## Purpose

Define a reusable CSV import pipeline that can be applied across content types (Monster, Treasure, Equipment, Magic Item, Unit/Counter).

This pipeline is designed to keep import behavior consistent while preserving content-specific validation rules.

---

## Pipeline Stages

### 1) Upload

- Select CSV file and import target type.
- Capture source metadata (file name, timestamp, operator context if applicable).

### 2) Parse

- Parse CSV headers and rows into normalized row objects.
- Record parse-level failures (bad format, missing headers, encoding issues).

### 3) Map

- Map parsed rows into content-type candidate models.
- Capture row-level mapping issues without mutating persistent content.

### 4) Validate

- Run file-level and row-level validation.
- Produce warnings vs errors using shared severity model.
- Apply duplicate-handling policy input where required.

### 5) Persist

- Persist valid candidate rows according to import mode/policy.
- Skip or reject invalid rows per validation policy.

### 6) Summarize

- Return structured import summary and diagnostics.
- Include actionable details for retry/fix workflows.

---

## Import Result Object (Conceptual)

A reusable import result should include:

- `IsSuccess`
- `TotalRows`
- `CreatedCount`
- `UpdatedCount`
- `SkippedCount`
- `FailedCount`
- `Messages` (high-level status/warnings)
- `Issues` (row-level diagnostics with severity)

---

## Error Collection and Reporting

Issue records should support:

- severity (`Info`, `Warning`, `Error`)
- stage (`Parse`, `Map`, `Validate`, `Persist`)
- row reference (if row-level)
- field/key context (if available)
- user-facing message

This structure supports UI error review and deterministic test assertions.

---

## Reusability Rules

- Stage flow remains common across content categories.
- Mapping and validation rules are content-type specific plugins/adapters.
- Shared reporting model is reused by all import categories.

---

## Related Docs

- `docs/00-overview/53-content-library-framework.md`
- `docs/00-overview/37-import-workflow-ui.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-012)
