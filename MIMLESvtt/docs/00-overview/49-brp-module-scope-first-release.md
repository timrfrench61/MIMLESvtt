# BRP Module Scope (First Release)

## Purpose

Define a practical first-release scope for the BRP rules module so delivery remains realistic while enabling usable percentile-based gameplay flows.

---

## Included Mechanics (First Release)

1. **Percentile core checks**
   - d100 roll against target skill/attribute threshold
   - pass/fail result shape for engine consumption

2. **Skill resolution baseline**
   - skill id + actor + target value input
   - optional first-pass modifier handling
   - structured result payload for apply/log phases

3. **Opposed check baseline (simple)**
   - first-pass comparison flow for two participants/entities
   - deterministic tie handling policy

4. **Combat-adjacent check support hooks**
   - attack/defense style percentile checks through generic combat hook points
   - no deep BRP subsystem expansion in first pass

5. **Outcome payload contract**
   - explicit success/failure + roll details + modifier summary
   - optional effect hints for engine apply phase

---

## Deferred Mechanics (Not in First Release)

- full BRP subsystem variants and supplements
- deep hit location/wound severity trees
- advanced opposed matrix and special success tiers beyond baseline
- extensive downtime/improvement progression loops
- full condition catalog with nuanced stacking/expiry behavior

---

## Scope Guardrails

- Keep BRP logic in module layer; engine remains rules-agnostic.
- Reuse shared dice/randomization and minimal roll-expression support.
- Prefer explicit, testable input/output contracts over implicit side effects.
- Focus first on repeatable percentile and skill resolution loops.

---

## Readiness Criteria

BRP first-release scope is ready when:

- supported mechanics are documented and testable,
- deferred mechanics are explicitly listed,
- integration points with generic hooks are clear,
- module behavior can be exercised via isolated rules harness inputs.

---

## Related Docs

- `docs/00-overview/08-rules-plugin-framework.md`
- `docs/00-overview/42-rules-context-and-action-model.md`
- `docs/00-overview/45-rpg-combat-workflow-hooks.md`
- `docs/05-backlog/06-backlog-rules-framework.md` (RF-009)
