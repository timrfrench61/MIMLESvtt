# Action System

## File Location

docs/02-domain/action-system.md

---

## Related Domain Objects (Implementation Path)

ActionRecord → src/Domain/Actions/ActionRecord.cs  
ActionRequest → src/Domain/Actions/ActionRequest.cs

---

## Purpose

Defines how `TableSession` state changes.

---

## Core Rule

All state changes must go through actions.  
Do not mutate `TableSession` directly.

---

## Action

An action is a named operation that changes state.

Examples:
- MovePiece
- RotatePiece
- AddMarker
- RemoveMarker
- ChangePieceState

---

## ActionRequest

Represents an incoming request to change state.

Contains:
- ActionType
- ActorParticipantId
- Payload

---

## ActionRecord

Represents a logged, accepted action.

Stored in:
- TableSession.ActionLog

Contains:
- ActionType
- ActorParticipantId
- Timestamp
- Payload

---

## Action Flow

UI → ActionRequest → Validate → Apply → Log → Update

---

## Steps

### 1. Create ActionRequest

UI generates a request describing intended change.

---

### 2. Validate

Check:
- ids exist
- actor allowed
- payload valid

Invalid → reject, no change

---

### 3. Apply

Update only relevant state in TableSession.

---

### 4. Log

Append ActionRecord to TableSession.ActionLog.

---

### 5. Update

- single-user: refresh UI
- multiplayer: broadcast from server

---

## Core Actions

- CreatePiece
- DeletePiece
- MovePiece
- RotatePiece
- ChangePieceState
- AddMarker
- RemoveMarker
- SetPieceVisibility
- UpdateTableOptions

---

## Rules

- No direct mutation of TableSession
- Validate before apply
- Log only accepted actions
- One action = one meaningful change

---

## Module Actions

Modules may define additional actions but must use the same system.