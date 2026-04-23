# Test Data Packet Format

## Purpose

Define how import test data packets are structured, named, and validated so import behavior can be smoke-tested and regression-tested consistently.

---

## Packet Structure

A test data packet should contain:

1. **Manifest file**
   - packet id/name/version
   - packet intent (`ValidBaseline`, `EdgeCase`, `Mixed`)
   - included data files list
   - expected high-level outcomes

2. **Data files**
   - one or more CSV files by content type:
     - monsters
     - treasure
     - equipment
     - magic items
     - optional unit/counter

3. **Expected outcome file** (optional but recommended)
   - expected created/updated/skipped/failed counts
   - expected warnings/errors by file/row category

---

## Naming and Packaging

Recommended naming pattern:

- `packet-<name>-v<version>`
- examples:
  - `packet-baseline-valid-v1`
  - `packet-edge-cases-v1`

Recommended layout:

- packet root folder
  - `manifest.json`
  - `monsters.csv`
  - `treasure.csv`
  - `equipment.csv`
  - `magic-items.csv`
  - `expected-outcome.json` (optional)

---

## Manifest Fields (Suggested)

Suggested manifest schema:

- `PacketId`
- `Name`
- `Version`
- `Intent`
- `Files` (array)
- `Notes`
- `ExpectedOutcomeRef` (optional file name)

---

## Validation Expectations

Packet validation should verify:

- required packet metadata exists
- referenced files exist and are readable
- included CSV files map to known import specs
- expected outcome document (if present) is structurally valid

---

## Outcome Storage Guidance

Expected outcomes should support storing:

- total rows per file
- created/updated/skipped/failed counts
- expected warning/error categories
- optional row-level expectations for deterministic test cases

This enables packet-driven smoke and regression checks in CI/local runs.

---

## Related Docs

- `docs/00-overview/64-reusable-csv-import-pipeline.md`
- `docs/00-overview/65-import-validation-model.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-019)
