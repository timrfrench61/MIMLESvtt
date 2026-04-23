# Rules Context and Action Model

## Purpose

Define the concrete data/flow contract used when rules modules evaluate and resolve actions.

This document complements the rules plugin framework overview with explicit context, request, and outcome shapes for implementation and testing.

---

## Rules Evaluation Flow

1. Engine receives an action request.
2. Engine builds a rules context from current session state.
3. Rules module validates the request.
4. If valid, rules module resolves outcome/effects.
5. Engine applies resulting effects and records action log entry.

---

## Rules Context (Conceptual)

A rules context should include:

- current session aggregate reference
- current turn state/phase info
- acting participant/seat identity
- selected rules module id/version
- optional scenario metadata/objective state
- policy/toggle inputs (for staged validation behavior)

Context is read-oriented for rules evaluation; final mutation remains engine-managed.

---

## Action Request Model (Conceptual)

Action request should contain:

- `ActionType` (engine-level action intent)
- `ActorParticipantId`
- typed payload for action-specific data

Examples:

- MovePiece payload (piece + destination)
- RotatePiece payload (piece + rotation)
- Marker/state payloads

Rules modules may interpret additional domain action types, but request dispatch remains engine-routed.

---

## Validation Result Model (Conceptual)

Validation response should support:

- `IsValid` boolean
- user-facing message when invalid
- optional warning/info payload for non-blocking rule notes

Invalid actions must not mutate session state.

---

## Resolution/Outcome Model (Conceptual)

Resolution result should support:

- success/failure status
- engine-applicable effect set (or equivalent outcome payload)
- optional diagnostics/messages
- optional turn/phase progression hints

Engine remains responsible for applying outcomes to session state and logging final action records.

---

## Responsibility Split

### Rules module responsibilities

- interpret request in rules context
- validate legality
- determine rule outcome/effects

### Engine responsibilities

- construct context
- invoke rules module
- apply effects to session state
- persist/log operation history and action log

---

## Testability Guidance

Rules modules should be testable with:

- fixed synthetic session context
- deterministic request inputs
- expected validation/result assertions

This allows isolated rules tests without UI coupling.

---

## Related Docs

- `docs/00-overview/08-rules-plugin-framework.md`
- `docs/05-backlog/06-backlog-rules-framework.md` (RF-001, RF-002)
