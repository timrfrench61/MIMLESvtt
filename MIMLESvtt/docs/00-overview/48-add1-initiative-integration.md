# AD&D1 Initiative Integration (First Pass)

## Purpose

Define how AD&D1-style initiative integrates with the generic turn/phase model without hard-coding system-specific behavior into engine code.

---

## Integration Goals

- Keep initiative logic in rules-module code.
- Reuse generic turn state (`TurnState`, turn order, phase sequence, round counters).
- Preserve deterministic engine apply/log behavior.

---

## Initiative Data Requirements

Rules-module initiative evaluation should consume/provide:

- participant/seat ids eligible for initiative
- initiative roll results per participant/side
- tie-break policy indicator
- round number and phase context
- optional metadata for temporary modifiers

Suggested turn-state metadata keys:

- `InitiativeMode` (for example `Side` or `Individual`)
- `InitiativeRolls` (module-owned structured payload)
- `TieBreakPolicy`

---

## Round Handling Flow

### 1) Round initialization

- At round start hook, AD&D1 module determines initiative participants.
- Module requests dice outcomes from shared randomization service.
- Initiative ordering result is returned as module outcome payload.

### 2) Turn order projection

- Engine applies resulting order to generic turn structures.
- Active seat/participant is set from resolved initiative sequence.
- Current phase is set to combat/action phase according to module configuration.

### 3) In-round progression

- Actions execute using normal validate -> resolve -> apply flow.
- Turn advancement follows initiative order generated for the round.

### 4) Round completion

- At end-of-order, module decides if initiative rerolls each round (first-pass policy toggle).
- Engine increments round counters and re-enters round initialization path.

---

## Integration Points (Hook Mapping)

- **Turn Start Hook:** evaluate/initiate round initiative if needed.
- **Turn/Phase Advancement Hook:** advance through initiative sequence.
- **Action Validation Hook:** ensure actor is current initiative actor.
- **Objective/Victory Hook:** evaluate terminal conditions after initiative-driven actions.

---

## First-Pass Policy Choices

To keep scope practical:

- Support side-based or individual initiative modes via module metadata.
- Use simple tie-break policy (stable order fallback) unless module overrides.
- Defer complex segment/weapon-speed style initiative variants.

---

## Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-008)
- Depends on RF-005 combat hook model and RF-006 AD&D1 first-release scope
