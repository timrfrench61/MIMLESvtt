# Shared Dice and Randomization Service

## Purpose

Define the shared dice/randomization layer for rules modules.

This service is engine-adjacent utility logic that can be used by multiple rules modules (checkers extensions, AD&D1, BRP, and future systems).

---

## Supported Dice

Current shared service supports:

- d4
- d6
- d8
- d10
- d12
- d20
- d100

It also supports generic `RollDie(sides, count, modifier)` for extensibility.

---

## Structured Result Shape

Each roll returns a structured `DiceRollResult` containing:

- `Sides`
- `Count`
- `Rolls` (individual die outcomes)
- `Modifier`
- `Total`

This supports rule transparency and easier test assertions.

---

## Consumption Pattern

Rules modules should:

- request dice outcomes through the shared service,
- keep rule-specific interpretation in the module,
- avoid direct ad-hoc randomization calls scattered across module code.

---

## Validation Rules

- `sides` must be > 1
- `count` must be > 0
- invalid requests fail clearly with argument exceptions

---

## Related Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-003)
