# Objective and Victory Evaluation Hooks

## Purpose

Define generic hook points for objective and victory evaluation across game styles while keeping engine code rules-agnostic.

This model supports simple pass/fail objectives and layered multi-condition victory logic.

---

## Evaluation Trigger Points

Objective/victory evaluation can be triggered by:

1. **State-change trigger**
   - after meaningful state mutations (capture, objective marker change, zone control update)
2. **Turn/phase progression trigger**
   - at end of turn, end of phase, or end of round
3. **Explicit evaluation trigger**
   - operator/module-issued evaluation command for immediate check

---

## Objective Consumption Model

Rules module consumes objective inputs from scenario/session context including:

- objective id/type metadata
- ownership/control state
- progression counters
- optional deadline/round constraints

Objective evaluation output should include:

- objective id
- current status (`NotMet`, `InProgress`, `Met`, `Failed`)
- summary message
- optional diagnostics payload

---

## Victory Evaluation Hook

Victory evaluation aggregates objective outcomes and other module-defined win conditions.

Suggested result shape:

- `IsTerminal`
- `WinnerParticipantId` or `WinnerSide`
- `VictoryType` (for example `Objective`, `Elimination`, `Points`)
- `SatisfiedConditions` list
- `Message`

---

## Simple vs Layered Conditions

### Simple conditions

Examples:

- first side to capture objective zone
- eliminate all opposing pieces

### Layered conditions

Examples:

- hold objective A for N rounds AND retain unit threshold
- aggregate points from multiple objectives with tie-break policy

Both are supported by the same evaluation hook contract; only module evaluation logic differs.

---

## Engine Integration Notes

- Rules module computes objective/victory outcomes.
- Engine applies terminal-state handling and publishes user-facing status.
- Evaluation results should be loggable in action/event history for replay/audit contexts.

---

## Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-011)
- Depends on RF-001 and RF-002
