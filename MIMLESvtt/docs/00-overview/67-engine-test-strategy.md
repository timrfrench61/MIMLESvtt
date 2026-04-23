# Engine Test Strategy

## Purpose

Define the platform-wide testing approach for the tabletop engine stack.

This strategy separates engine verification from rules-module and UI-specific verification while keeping integration coverage explicit.

---

## Test Categories

1. **Unit tests**
   - Focus on isolated logic and data transformation behavior.
   - Examples: coordinate helpers, action validation helpers, serializer field-level behavior.

2. **Integration tests**
   - Exercise interactions across code layers without full UI runtime dependence.
   - Examples: workspace service + persistence round-trip, import/apply workflows, packet evaluation flows.

3. **Regression tests**
   - Stable feature paths promoted to permanent verification set.
   - Examples: session save/load, join-code registry behavior, action-processing invariants.

4. **Manual QA flows**
   - Operator-facing workflow checks for pre-alpha UX and state consistency.
   - Examples: game selector paths, workspace setup flows, import error-review behavior.

---

## Unit vs Integration Separation

### Unit layer

- deterministic, fast, isolated,
- minimal setup and no broad runtime orchestration,
- validates single-purpose behavior.

### Integration layer

- composes multiple services and state objects,
- verifies interaction contracts and result propagation,
- includes persistence/import/recovery paths.

---

## Engine-First Testing Principles

- Validate engine state mutation and logging behavior independently from specific rulesets.
- Keep shared services (persistence/import/randomization) testable without UI coupling.
- Keep rules-module tests isolated from UI and focused on request/result contracts.
- Promote completed vertical slices into regression tests once behavior stabilizes.

---

## Execution Guidance

- Run unit and integration suites on every feature slice that changes domain/service behavior.
- Add regression cases when a bug is fixed or a workflow reaches stable expected behavior.
- Track manual QA scenarios in user-facing workflow docs and convert repeatable paths to automated tests where practical.

---

## Related Docs

- `docs/05-backlog/60-backlog-testing-qa.md` (TQA-001)
- `docs/00-overview/52-rules-module-test-harness-design.md`
- `docs/00-overview/64-reusable-csv-import-pipeline.md`
- `docs/00-overview/66-test-data-packet-format.md`
