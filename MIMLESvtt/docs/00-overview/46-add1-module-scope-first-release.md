# AD&D1 Module Scope (First Release)

## Purpose

Define a practical first-release scope for the AD&D1 rules module so implementation remains achievable while still delivering useful tabletop gameplay support.

---

## Included Mechanics (First Release)

1. **Initiative baseline integration**
   - round/turn ordering driven through generic turn state
   - simple initiative value handling for participant order

2. **Attack resolution (first pass)**
   - attack request validation (actor/target/action timing)
   - attack roll via shared dice service
   - hit/miss outcome with clear result payload

3. **Saving throw resolution (first pass)**
   - save-type request and target threshold input
   - d20-based save check
   - success/failure result payload

4. **Damage application hook output**
   - structured damage effect output for engine application
   - no direct module-side mutation of session persistence state

5. **Status/condition hook output (minimal)**
   - effect flags for simple status outcomes (for example: prone/stunned markers)

---

## Deferred Mechanics (Not in First Release)

- full spell subsystem and spell timing exceptions
- advanced weapon-vs-armor matrices and situational modifiers catalog
- morale/reaction subsystem depth
- complete encumbrance/travel/timekeeping loop
- advanced condition stacking/expiry orchestration
- full monster manual rules coverage edge cases

---

## Scope Guardrails

- Keep module logic in rules layer; engine remains system-agnostic.
- Reuse shared dice/randomization service and roll-expression support.
- Prefer explicit, testable input/output models over implicit rule side effects.
- Focus on combat-core loop first: validate -> resolve -> apply -> log.

---

## Readiness Criteria

AD&D1 first-release scope is considered ready when:

- supported mechanics list is implemented and testable,
- deferred mechanics list is explicit in docs,
- integration with generic turn/combat hooks is clear,
- module can run in isolation through rules test harness inputs.

---

## Related Docs

- `docs/00-overview/08-rules-plugin-framework.md`
- `docs/00-overview/42-rules-context-and-action-model.md`
- `docs/00-overview/45-rpg-combat-workflow-hooks.md`
- `docs/05-backlog/06-backlog-rules-framework.md` (RF-006)
