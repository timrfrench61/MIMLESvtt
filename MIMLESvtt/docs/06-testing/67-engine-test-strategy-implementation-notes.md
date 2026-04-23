# Engine Test Strategy Implementation Notes

## Purpose

Translate the engine testing strategy into practical repo guidance for suite grouping, regression promotion, and execution expectations.

Source alignment:
- `docs/00-overview/67-engine-test-strategy.md`
- Backlog mapping target: `docs/05-backlog/70-backlog-testing-qa.md` (TQA-001)

---

## Test Taxonomy

### 1) Unit Tests
- Scope: isolated logic and data transformation behavior.
- Typical folders/files:
  - pure domain validators
  - mapper helpers
  - serializer field-level checks
- Rules:
  - deterministic inputs
  - no broad orchestration setup
  - no UI runtime dependency

### 2) Integration Tests
- Scope: interactions across code layers.
- Typical examples:
  - action orchestration + engine apply/log
  - import pipeline + repository persistence
  - packet loader + manifest/outcome validation
- Rules:
  - verify service handoff and propagated outcomes
  - avoid duplicating unit-level micro assertions

### 3) Regression Tests
- Scope: stabilized feature paths and fixed defects.
- Promotion triggers:
  - bug fix in domain/persistence/import logic
  - vertical slice marked stable and repeatedly relied upon
- Rules:
  - preserve deterministic fixtures
  - keep failure messages directly actionable

### 4) Manual QA Flows
- Scope: operator-visible end-to-end workflow checks.
- Location:
  - `docs/06-testing/98-pre-alpha-testing-packet.md`
  - `docs/06-testing/96-qa-run-log.md`
- Rules:
  - document toggles, environment, and expected outcomes per run

---

## Unit vs Integration Grouping Conventions

- Unit tests should target single classes/functions without file-system packet orchestration where possible.
- Integration tests should compose at least two code layers (for example mapper + pipeline + repository).
- UI component tests should focus on action dispatch/feedback behavior, not domain re-validation already covered by unit tests.

Suggested project-level grouping conventions:
- `*PageTests` => UI/component workflow behavior
- `*Pipeline*Tests` => import/persistence integration behavior
- `*Mapper*Tests` / `*Validation*Tests` => unit behavior
- `*Regression*Tests` => promoted stable behavior packs

### Representative strategy mapping examples (documented verification)

| Change type | Unit suite expectation | Integration suite expectation | Regression promotion trigger |
|---|---|---|---|
| Content mapper/validator edit | `*Mapper*Tests`, `*Validation*Tests` | `*Pipeline*Tests` for handoff and counts | Prior bug/edge case fixed in same area |
| Action orchestration edit | focused action validation/apply tests | orchestrator + log/dispatch interaction tests | Turn/order/state mutation bug resolved |
| Packet manifest/outcome edit | packet schema parser tests | packet loader + pipeline path tests | Previously failing packet format case stabilized |
| UI workflow edit (content pages) | component field validation tests | UI + service-stage integration checks if behavior changed | User-facing workflow defect fixed |

---

## Regression Promotion Checklist

Promote a test case to regression suite when all conditions hold:
1. Behavior is relied upon by active workflow docs/backlog slices.
2. A defect or high-risk branch was fixed for the behavior.
3. Fixture can be deterministic and local-run friendly.
4. Assertion messages are concise and diagnosis-ready.
5. Ownership layer is explicit (domain, import, UI, etc.).

---

## Automated Execution Guidance

For feature slices that affect engine/service behavior:
1. Run targeted unit tests for changed validators/mappers/models.
2. Run targeted integration tests for changed handoff code.
3. If behavior was bug-fix/stability sensitive, run matching regression tests.
4. Update docs cross references when adding/changing suite ownership.

For UI-only slices:
- run affected page/component tests,
- run adjacent integration tests if UI now triggers new service-stage behavior.

---

## Strategy Mapping Examples

- Import validation model changes:
  - Unit: optional/required parser checks
  - Integration: pipeline duplicate policy and summary behavior
  - Regression: previously broken malformed-row case

- Rules workflow changes:
  - Unit: module validation/rejection checks
  - Integration: orchestrator apply/log handoff
  - Regression: tie-break or deterministic roll interpretation edge case

---

## Cross References

- `docs/06-testing/97-testing-strategy.md`
- `docs/00-overview/68-subsystem-test-boundaries.md`
- `docs/06-testing/68-subsystem-test-boundaries-implementation-notes.md`
- `docs/00-overview/66-test-data-packet-format.md`
- `docs/06-testing/96-qa-run-log.md`
- `docs/06-testing/98-pre-alpha-testing-packet.md`
