# Backlog Additions from 00-overview/64

Source: `docs/00-overview/64-reusable-csv-import-pipeline.md`
Target backlog: `docs/05-backlog/09-backlog-content-import.md`

## Code Project Additions

- [x] Add reusable CSV import pipeline orchestration contract with explicit staged flow (`Upload`, `Parse`, `Map`, `Validate`, `Persist`, `Summarize`).
- [x] Add shared import result model with created/updated/skipped/failed counts and high-level message collection.
- [x] Add stage-aware issue diagnostics model (`severity`, `stage`, `row`, `field`, `message`) for UI review and deterministic testing.
- [x] Add content-type adapter/plugin hooks for map/validate rules while preserving shared pipeline reporting behavior.
- [x] Add tests for stage progression, row-level issue capture, and import summary aggregation across mixed valid/invalid inputs.
