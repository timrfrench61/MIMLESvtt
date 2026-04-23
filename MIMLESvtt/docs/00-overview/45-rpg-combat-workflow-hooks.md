# RPG Combat Workflow Hook Points

## Purpose

Define where RPG combat mechanics connect to generic session/turn state so rules modules can implement initiative, attacks, saves, damage, and status changes without hard-coding one system into engine code.

---

## Hook Point Overview

Combat hook points are evaluated in this sequence:

1. **Turn Start Hook**
2. **Action Validation Hook**
3. **Action Resolution Hook**
4. **Outcome Application Hook**
5. **Turn/Phase Advancement Hook**
6. **Objective/Victory Check Hook**

The engine controls state mutation and logging; rules modules provide legality checks and outcome semantics.

---

## Hook Definitions

### 1) Turn Start Hook

Triggered when a new active seat/participant turn begins.

Rules module can:

- determine initiative-dependent order adjustments,
- apply start-of-turn effects,
- enforce phase-specific constraints.

Consumes:

- session turn state,
- active seat/participant id,
- phase sequence context.

---

### 2) Action Validation Hook

Triggered before an action mutates state.

Rules module can validate:

- actor eligibility,
- target legality,
- phase restrictions,
- resource/condition prerequisites.

Returns:

- valid/invalid result,
- user-facing rejection reason where invalid.

---

### 3) Action Resolution Hook

Triggered for valid combat actions (attack/save/damage/status effects).

Rules module can resolve:

- attack checks,
- saving throw checks,
- damage/effect outcomes,
- conditional side effects.

May consume shared dice/randomization service and roll expression support.

Returns:

- structured outcomes/effects for engine application.

---

### 4) Outcome Application Hook

Engine applies outcomes produced by rules module.

Typical effects:

- hit point/state changes,
- marker/status updates,
- piece/participant condition changes,
- action log/event record enrichment.

Rules module does not directly mutate persistent state outside this controlled application path.

---

### 5) Turn/Phase Advancement Hook

Triggered after action resolution/application completes.

Rules module can:

- advance phase index,
- rotate active seat,
- trigger round increment behavior,
- enforce initiative reroll/reorder logic where applicable.

Consumes generic `TurnState` and module-specific metadata.

---

### 6) Objective/Victory Check Hook

Triggered after major combat outcomes or at configured progression points.

Rules module can evaluate:

- elimination conditions,
- objective completion,
- scenario victory/defeat states.

Returns:

- continuation vs terminal state,
- optional winner/objective context.

---

## Turn Engine Integration Summary

- Generic turn state remains engine-owned (`TurnState`, `TurnOrder`, `TurnNumber`, `CurrentPhase`).
- Rules modules interpret and influence progression through hook outputs.
- Engine ensures deterministic apply/log progression for persistence/replay compatibility.

---

## Non-Hardcoding Constraint

This hook model is intentionally system-neutral:

- AD&D-style attack/save integration,
- BRP percentile/skill combat integration,
- future tactical/war-game combat overlays

all use the same hook sequence with module-specific evaluation logic.

---

## Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-005)
- Depends on RF-001 and RF-002 foundations
