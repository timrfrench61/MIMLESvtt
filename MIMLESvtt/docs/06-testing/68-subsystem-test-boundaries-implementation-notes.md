# Subsystem Test Boundaries Implementation Notes

## Purpose

Define practical ownership mapping for subsystem test responsibilities and clarify where shared integration assertions should be placed.

Source alignment:
- `docs/00-overview/68-subsystem-test-boundaries.md`
- Backlog mapping target: `docs/05-backlog/70-backlog-testing-qa.md` (TQA-002)

---

## Subsystem Ownership Matrix

| Subsystem | Owns Tests For | Does Not Own |
|---|---|---|
| Engine | session/state models, action apply/validation, board-space-zone-piece transformations, turn/phase behavior | ruleset-specific legality logic, UI rendering behavior |
| Rules Framework | rules request/result contracts, module validation/rejection, resolution payloads, deterministic random usage | UI interaction tests, generic engine persistence tests |
| UI / Presentation | page/component transitions, route flow behavior, user-visible validation feedback | deep domain rule correctness, serializer invariants |
| Persistence / Import | serializer round-trip, file safety/recovery, parse-map-validate-report behavior, packet edge-case expectations | final UI interaction workflows (except handoff checks) |
| Networking | connection/session sync behavior, server-authoritative ordering, reconnect/rehydration behavior | (none in first pass) |

---

## Cross-Subsystem Integration Slices

Use integration suites to verify layer handoff contracts for:

1. **Engine + Persistence**
   - save/load/restore integrity
2. **Engine + Import**
   - import/apply outcomes and state mutation consistency
3. **Engine + Rules**
   - module validation/resolution handoff behavior
4. **UI + Workspace Services**
   - operator flow state and feedback propagation

Integration tests should verify interaction outcomes and avoid re-asserting every unit-level detail already covered in owned subsystem suites.

---

## Separation Rule Checks

Apply these checks during test authoring and review:

1. Engine tests remain separate from rules-module tests.
2. UI tests remain separate from domain logic tests.
3. Integration tests verify contract handoffs and should not duplicate unit-level assertions.

---

## Test Project Organization Guidance

Recommended grouping conventions:

- `*PageTests` => UI/presentation assertions
- `*Pipeline*Tests` => import/persistence integration assertions
- `*Mapper*Tests` / `*Validation*Tests` => unit-level transformation/validation assertions
- `*Regression*Tests` => promoted stable behavior assertions
- `*Orchestrator*Tests` => engine + rules/service handoff assertions

When adding new tests, explicitly identify the subsystem owner in method/class naming or doc comments so placement remains traceable.

---

## Boundary-Focused Example Placement

- **Engine-owned assertion example:** action rejection does not mutate session state.
- **Rules-owned assertion example:** module returns expected rejection/result payload for invalid phase.
- **Persistence-owned assertion example:** serialized session round-trip preserves identifiers and action log order.
- **UI-owned assertion example:** failed form validation renders visible blocked-save messaging.
- **Integration-owned assertion example:** import pipeline results propagate correctly into workflow summary messaging.

---

## Cross References

- `docs/06-testing/67-engine-test-strategy-implementation-notes.md`
- `docs/06-testing/97-testing-strategy.md`
- `docs/06-testing/99-index.md`
- `docs/05-backlog/68-backlog-subsystem-test-boundaries.md`
