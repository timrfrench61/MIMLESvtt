# Backlog Additions from 00-overview/45

Source: `docs/00-overview/45-rpg-combat-workflow-hooks.md`
Target backlog: `docs/05-backlog/06-backlog-rules-framework.md`

## Code Project Additions

- [x] Add hook dispatcher contract for turn-start/validation/resolution/apply/progression/objective checks.
- [x] Add module hook registration model and default no-op implementation.
- [x] Add action pipeline integration points for combat hook invocation order.
- [x] Add tests asserting hook order and failure-path short-circuit behavior.
- [x] Add tests asserting engine-owned state apply/log behavior remains authoritative.
