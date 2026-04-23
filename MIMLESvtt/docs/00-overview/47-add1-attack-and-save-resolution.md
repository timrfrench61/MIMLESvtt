# AD&D1 Attack and Save Resolution (First Pass)

## Purpose

Define first-pass AD&D1 attack and saving throw resolution flow with explicit inputs/outputs suitable for rules-module implementation and test harness execution.

---

## Attack Resolution Flow

### Inputs

- acting participant/seat id
- attacker piece/entity id
- target piece/entity id
- attack context metadata:
  - attack type (melee/ranged/other)
  - relevant modifiers (situational, temporary)
  - optional weapon/profile reference id
- current turn/phase context

### Validation Steps

1. Actor can act in current turn/phase.
2. Attacker and target references are valid in current session state.
3. Target is legal under current rule constraints.
4. Request payload fields are complete enough for first-pass resolution.

If validation fails:

- action is rejected,
- no state mutation occurs,
- user-facing reason is returned.

### Resolution Steps

1. Roll attack check (first pass: d20 + aggregate modifier if applicable).
2. Compare against target threshold logic configured by module policy.
3. Produce hit/miss outcome.
4. If hit, produce damage effect payload (damage value can come from module policy + dice service).

### Outputs

Attack resolution output should include:

- `IsSuccess` (resolution executed)
- `IsHit`
- roll details (`RollExpressionResult`/`DiceRollResult` data)
- computed modifiers summary
- optional damage effect payload
- optional status/marker effect payload
- user-facing summary string

---

## Saving Throw Flow

### Inputs

- acting or defending entity id (based on save trigger)
- save type/category (module-defined enum/string)
- target save threshold
- modifier inputs
- current turn/phase context

### Validation Steps

1. Save target entity exists and is valid in current session.
2. Save type is recognized by module scope.
3. Threshold/modifier payload values are valid.

If validation fails:

- save action/result is rejected,
- no state mutation occurs,
- clear validation reason is returned.

### Resolution Steps

1. Roll saving throw check (first pass: d20 + modifier).
2. Compare against threshold.
3. Produce success/failure outcome.
4. Attach downstream effect hints (for example negate damage, halve damage, apply status).

### Outputs

Saving throw output should include:

- `IsSuccess` (resolution executed)
- `SavePassed`
- roll details (`RollExpressionResult`/`DiceRollResult` data)
- threshold and modifier summary
- optional effect adjustment payload
- user-facing summary string

---

## Engine Integration Notes

- Rules module computes legality and outcomes.
- Engine applies resulting effects and logs action record.
- Turn progression remains managed through generic turn hooks.
- No rules-module direct persistence mutation outside returned outcomes.

---

## Testability Notes

Both flows should be testable with deterministic synthetic inputs by:

- fixed session context setup,
- explicit request payload fixtures,
- expected validation/rejection assertions,
- expected resolution output assertions.

---

## Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-007)
- Depends on RF-006 scope and RF-003/RF-004 shared dice support
