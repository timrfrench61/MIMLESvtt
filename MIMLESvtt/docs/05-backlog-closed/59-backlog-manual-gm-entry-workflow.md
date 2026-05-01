# Backlog Additions from 00-overview/59

Source: `docs/00-overview/59-manual-gm-entry-workflow.md`
Target backlog: `docs/05-backlog/09-backlog-content-import.md`

## Code Project Additions

- [x] Add shared manual GM entry workflow shell supporting list -> create/edit -> detail/review -> return flow.
- [x] Add content-type-specific workflow adapters for Monster, Treasure, Equipment, and Magic Item while preserving a reusable UX pattern.
- [x] Add save/cancel behavior contracts with explicit no-mutation cancel guarantees and success/failure status messaging.
- [x] Add validation feedback model for inline field errors plus form-level blocked-save summaries.
- [x] Add workflow tests covering list-to-entry transitions, create/edit persistence paths, and cancel return behavior.
