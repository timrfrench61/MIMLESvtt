# BRP Percentile and Skill Resolution (First Pass)

## Purpose

Define first-pass BRP percentile and skill resolution flow with explicit inputs/outputs for module implementation and isolated testing.

---

## Percentile Check Flow

### Inputs

- actor/subject id
- check type/category
- target value (typically skill/attribute threshold)
- optional modifier aggregate
- current turn/phase context

### Validation Steps

1. Actor/subject exists in session context.
2. Target value is present and in valid range.
3. Check type is recognized by BRP module scope.
4. Modifier payload values are valid.

If validation fails:

- check request is rejected,
- no state mutation occurs,
- explicit user-facing reason is returned.

### Resolution Steps

1. Roll d100 via shared dice service.
2. Apply modifier policy for first-pass scope.
3. Compare final roll against target value.
4. Produce success/failure outcome.

### Outputs

- `IsSuccess` (resolution executed)
- `Passed`
- roll details (`DiceRollResult` / `RollExpressionResult`)
- target and modifier summary
- optional effect hints
- user-facing summary string

---

## Skill Resolution Flow

### Inputs

- actor id
- skill id/name
- skill target value
- optional situational modifiers
- optional opposed target context

### Validation Steps

1. Skill reference is present/recognized.
2. Actor and target values are valid.
3. Opposed context (if present) is complete enough for first-pass comparison.

If validation fails:

- skill action is rejected,
- no state mutation occurs,
- clear validation reason is returned.

### Resolution Steps

1. Execute percentile check for acting skill.
2. For opposed checks, execute counterpart check and compare outcomes.
3. Apply deterministic tie policy (first-pass stable rule).
4. Produce final skill outcome payload.

### Outputs

- `IsSuccess` (resolution executed)
- `SkillPassed` / opposed outcome
- roll details and modifiers
- optional downstream effect payload
- user-facing summary string

---

## Engine Integration Notes

- BRP module computes legality and outcome payloads.
- Engine applies effects and logs action records.
- Generic turn progression remains engine-managed through hook points.

---

## Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-010)
- Depends on RF-009 scope and RF-003/RF-004 shared dice support
