# Table State Model

## Purpose

This document defines the **core runtime state model** for the Blazor VTT.

It answers these questions:

- What is the root live object during play?
- What data belongs to the live session?
- What data is referenced from reusable definitions?
- How do surfaces, pieces, participants, visibility, and module state fit together?
- What rules must always be true?

This document is meant to guide:

- domain model design
- persistence design
- SignalR/session synchronization
- UI binding and rendering
- future import/export and replay work

It is not a screen design document.

---

## Core Principle

> The `VttSession` is the single source of truth for live play.

The table session contains the current runtime state of one live or saved tabletop session.

Everything the UI shows should be derived from this state or from reusable definitions referenced by this state.

---

## Design Goals

The model must support:

- RPG encounters
- board games
- tactical combat
- hex-and-counter war games
- scenario-driven play

The model must **not** assume:

- initiative always exists
- combat always exists
- every piece is a creature
- every session uses one map
- every game uses grids
- every rule belongs in the core engine

---

## Design Rules

### 1. Separate Definitions from Instances

Reusable content is not stored directly inside runtime state.

Examples of reusable content:

- monster definitions
- map definitions
- marker definitions
- item definitions
- scenario definitions

Runtime state stores references such as:

- `DefinitionId`
- module keys
- asset ids

The session stores the current live instance values only.

---

### 2. The Session Owns Live State

The `VttSession` owns:

- participants
- active surfaces
- live pieces
- options
- visibility state
- action log
- module-specific runtime data

It does not own the master content library.

---

### 3. Runtime State Must Be Explicit

If something matters to:

- rendering
- persistence
- synchronization
- undo/replay later
- permissions or visibility

then it must be represented explicitly in runtime state.

Do not hide important state in component-only fields.

---

## Top-Level Aggregate

## VttSession

`VttSession` is the root aggregate for one live or saved game table.

### Responsibilities

It is responsible for:

- identifying the session
- tracking participants
- holding all active surfaces
- holding all active pieces
- exposing table-wide options
- exposing visibility information
- storing module-specific runtime state
- recording action history

### Example Shape

```csharp
class VttSession
{
    string Id;
    string Title;

    List<Participant> Participants;

    List<SurfaceInstance> Surfaces;
    List<PieceInstance> Pieces;

    TableOptions Options;

    VisibilityState Visibility;

    List<ActionRecord> ActionLog;

    Dictionary<string, object> ModuleState;
}