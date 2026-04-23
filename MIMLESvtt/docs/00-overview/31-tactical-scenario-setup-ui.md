# Tactical Scenario Setup UI

## Purpose

Define the tactical scenario setup screen flow used to prepare a playable starting layout with sides, placements, and save behavior.

This flow targets practical pre-alpha operation from the existing Workspace + scenario save/import controls.

---

## Screen Intent

The tactical scenario setup experience is a focused authoring flow for:

- choosing/confirming board surface context,
- assigning sides/factions for participants,
- placing initial units/pieces,
- saving setup as a reusable scenario snapshot.

---

## Setup Workflow

### 1) Scenario setup entry

- Start from Home/Launch and open or create a session.
- Navigate to Workspace.
- Open right-side controls panel.

### 2) Board and surface preparation

- Add or select the active surface.
- Confirm coordinate system and board context.
- Optional: use strip-based surface activation for multi-surface scenarios.

### 3) Side and participant assignment

- Add participants/roles in session controls.
- Define practical side ownership via participant assignment and PlayerSeat model semantics.
- Confirm turn/phase initialization if scenario depends on a starting order.

### 4) Initial unit placement

- Add pieces with definition id and owner.
- Place via direct Add Piece fields or board click/stamp mode.
- Optional: use presets and queue templates for repeated tactical setup.

### 5) Scenario save behavior

- Use **Save Current Layout As Scenario** with:
  - scenario file path,
  - scenario export title.
- Saved scenario snapshot contains board/surface/piece layout for reuse.
- Live-only progression fields (turn progression/action log) are excluded from scenario snapshot by design.

---

## Validation Expectations

- Missing or invalid paths return explicit user-visible messages.
- Required setup context (session/surface/piece target validity) fails clearly without silent mutation.
- Scenario activation workflow remains two-step (pending plan -> activate) when importing.

---

## Operator Outcome

A GM/operator can build and save a tactical starting state, then later import and activate it into a live session with predictable behavior.

---

## Related Docs

- `docs/09-user-documentation/workspace-use-cases.md` (Use Case 5 and setup workflows)
- `docs/09-user-documentation/workspace-controls-api.md` (control reference)
- `docs/03-persistence/01-session-state-persistence.md` (snapshot semantics)
