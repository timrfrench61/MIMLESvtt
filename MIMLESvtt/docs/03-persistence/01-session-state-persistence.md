# Session State Persistence

## Purpose

Define how a live game session is saved, restored, and synchronized.

This document answers:

- What must be stored to restore a game
- How session data is structured for persistence
- How save/load works across different game types

The goal is **reliable, simple persistence first**, with room for future expansion.

---

## Core Principle

A session must be restorable from persisted data.

At minimum, persistence must capture:

- table state
- pieces and positions
- turn state
- participants

---

## What Is a Session

A session is a live instance of play.

It includes:

- play surface (board/map)
- placed pieces
- current turn state
- player seats
- game rules context

A session is NOT:

- reusable content (definitions)
- CSV-imported libraries

---

## Persistence Scope

Persist only runtime state.

Do NOT persist:

- duplicate definition data
- UI-only state
- temporary rendering state

---

## Required Persisted Components

---

### 1. Session Metadata

Fields:

- `SessionId`
- `ScenarioId` (optional)
- `RulesetId`
- `CreatedAt`
- `LastUpdatedAt`
- `IsCompleted`

---

### 2. Play Surface

Persist:

- board type
- dimensions
- map identifier (if external)
- zones (if used)

Do NOT persist:

- static map art (reference it)

---

### 3. PieceInstances

Persist all live pieces:

- `InstanceId`
- `DefinitionId`
- `Position`
- `OwnerSeatId`
- `SideId`
- `CurrentAttributes`
- `StatusFlags`
- `Hidden`
- `RemovedFromPlay`

Do NOT duplicate:

- PieceDefinition data

---

### 4. TurnState

Persist current sequence state:

- `SequenceType`
- `TurnNumber`
- `RoundNumber` (if used)
- `CurrentSideId`
- `CurrentActorId`
- `CurrentPhase`
- `HasEnded`

This is critical for restoring gameplay correctly.

---

### 5. Seats

Persist:

- `SeatId`
- `Role`
- `SideId`

Optional:

- player identity (future networking)

---

### 6. Sides

Persist:

- `SideId`
- `Name`

---

### 7. EventLog (Optional Early)

For initial version:

- optional or minimal

Later:

- full event history for replay/debug

---

## Minimal Persistence Model (Phase 1)

To get started, store:

- Session metadata
- Board definition reference
- PieceInstances
- TurnState
- Seats
- Sides

This is enough to:

- save a checkers game
- reload it exactly

---

## Save Strategy

### Snapshot-Based (Recommended Initial)

Save full session state as a single object.

Pros:
- simple
- reliable
- easy to debug

Cons:
- larger storage size
- no replay

---

### Event-Based (Future)

Store:

- initial snapshot
- event log

Pros:
- replay capability
- audit trail
- smaller updates

Cons:
- more complex
- harder to debug early

---

## Save Triggers

Define when a session is saved:

Options:

- manual save (button)
- auto-save on every command (recommended)
- periodic save (timer)

Initial recommendation:

- save after each valid command

---

## Load Behavior

When loading a session:

1. Load session record
2. Rebuild VttSession object
3. Restore:
   - board/map
   - pieces
   - turn state
   - seats
4. Validate state consistency
5. Resume play

---

## Data Storage Options

### Option A — JSON Storage (Recommended Early)

- store entire session as JSON
- file-based or DB blob

Pros:
- simple
- flexible
- easy to version

---

### Option B — Relational Tables

- normalized tables for pieces, sessions, etc.

Pros:
- queryable
- scalable

Cons:
- more complex mapping

---

### Recommendation

Start with:

- JSON snapshot stored in database

Refactor later if needed.

---

## Versioning

Add versioning to session data.

Example:

- `SchemaVersion`

Purpose:

- support future model changes
- allow migration logic

---

## Consistency Rules

A valid persisted session must:

- reference valid PieceDefinitions
- have valid positions for all pieces
- have a valid TurnState
- not contain orphaned references

Validation should run on load.

---

## Networking Considerations

In Blazor Server:

- server holds authoritative session state
- clients receive updates

Persistence should:

- store server state
- not rely on client copies

---

## Performance Considerations

Early phase:
- prioritize correctness over optimization

Later:
- partial updates
- event-based persistence
- caching

---

## UI Considerations

UI should support:

- Save session
- Load session
- Display session metadata
- List available sessions

---

## Minimal Checkers Example

Persist:

- 8x8 board reference
- 24 piece instances
- current side to move
- turn number

Reload:
- board appears exactly as before
- correct player resumes

---

## Future Extensions

Later enhancements may include:

- replay system
- undo/redo
- branching game states
- cloud sync
- multi-session campaigns

---

## Summary

Session persistence:

- captures all runtime state
- enables save/load
- supports networking
- must remain simple initially

Start with full snapshots.
Expand later if needed.