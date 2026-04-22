# Backlog

## File Location

docs/05-backlog/backlog.md

---

## Purpose

This document defines the working backlog for the Blazor VTT.

It translates the roadmap phases into actionable work items.

It is the primary list used to:

- guide development work
- track progress
- define implementation order

---

## Core Principle

Backlog items must align with the domain model.

No task should bypass:

- VttSession
- action system
- persistence rules

---

## Recent Vertical Slice Update

- Stamp mode add-at-click now includes a compact stamp-session control strip on Workspace.
- Next id preview supports explicit reset and +/- nudge controls for fast repeated placement.
- Id control remains visible and conservative; conflicts still fail without hidden auto-renumbering.
- Stamp presets now allow quick reuse of DefinitionId/owner/rotation for repeated placement workflows.
- Stamp presets are local to workspace UI state and are not persisted.
- Piece ids remain explicit and controlled separately from preset selection.
- Workspace now includes a compact multi-surface workflow strip for faster per-surface setup.
- Surface switching and default-preset assignment are now quicker and clearer across multiple surfaces.
- Per-surface default presets auto-apply on surface switch while remaining UI-only state.
- Workspace now supports a compact cross-surface stamping queue for rapid setup sequences.
- Queue items keep surface, preset, and next-id explicit and editable.
- Conflict handling remains conservative; queue ids are not silently auto-skipped.
- Queue templates now allow saving/reapplying multi-surface queue sequences within the same session.
- Queue templates are UI-only in-memory snapshots (not persisted to disk).
- Workspace now supports undo/redo for a conservative safe operation set.
- Undo/redo scope is intentionally limited to supported operations in current single-user workspace flow.
- Workspace board now supports conservative multi-select and group move operations for setup workflows.
- Multi-select/group move remains action-driven and intentionally avoids drag-selection/advanced transform frameworks.
- Workspace board now includes lightweight visibility/filter controls for readability as complexity grows.
- Visibility/filter controls are intentionally not fog-of-war or LOS systems.
- Workspace now supports saving current layout as a scenario for practical authoring/reuse flow.
- Scenario pending-plan pipeline is now connected to this authoring save/open workflow.
- Workspace now includes a lightweight piece palette sidebar driven by stamp presets for faster placement setup.
- Piece palette entries arm add-at-click placement but are not a full authored-definition editor.
- Workspace now includes a minimal turn/phase system (turn order, current turn index, current phase) for practical gameplay state.
- Turn tracking now includes turn number and previous-turn support for a fuller minimal gameplay loop.
- Workspace now includes a minimal participant/player system with add/remove operations and owner-aware display wiring.
- Participant workflows now include rename plus selected-piece owner assignment from participant list.
- Action processing now includes a lightweight pre-execution validation layer for conditional allow/block behavior.
- Validation checks now explicitly cover piece reference, surface reference, and payload-shape coherence for supported actions.
- Workspace now supports explicit Edit vs Play modes so authoring and gameplay constraints are separated.
- Play mode now enforces action validation before execution while Edit mode preserves permissive authoring workflows.
- Session controls now clearly represent game-state save/load flow, separate from scenario authoring/import flow.
- Game-state save/load now explicitly preserves live progress fields (participants, turn state, action log).
- Scenario load remains starting-layout focused and does not carry live game progress state.

---

## Backlog Structure

Backlog items are grouped by category:

- Domain
- Table
- Persistence
- Multiplayer
- Tools
- Modules
- Rules
- UI
- Testing

Each item should be small enough to implement independently.

---

## Domain

- [x] create VttSession class
- [x] create Participant model
- [x] create SurfaceInstance model
- [x] create PieceInstance model
- [x] create Location model
- [x] create Coordinate model
- [x] create VisibilityState model
- [x] create TableOptions model
- [x] create ActionRecord model
- [x] implement ModuleState structure
- [ ] enforce id uniqueness rules
- [~] enforce reference validation rules (implemented for key action paths such as piece/surface checks)

---

## Action System

- [x] create ActionRequest structure
- [x] implement action dispatch mechanism
- [x] implement validation step
- [x] implement execution step
- [x] implement ActionLog storage
- [~] implement core actions:
  - [ ] CreatePiece
  - [ ] DeletePiece
  - [x] MovePiece
  - [x] RotatePiece
  - [x] ChangePieceState
  - [x] AddMarker
  - [x] RemoveMarker
  - [ ] SetPieceVisibility
  - [ ] UpdateTableOptions

---

## Table

- [x] create table creation workflow
- [x] create table load workflow
- [ ] create surface creation
- [ ] create piece creation
- [ ] implement piece placement
- [x] implement piece movement
- [x] implement piece rotation
- [x] implement piece state editing
- [ ] implement selection model
- [ ] implement rendering of surfaces and pieces

---

## Persistence

- [x] implement session save
- [x] implement session load
- [x] implement scenario export
- [x] implement scenario import
- [x] implement content pack export
- [x] implement content pack import
- [x] implement action log export
- [x] implement version field handling
- [x] implement validation during import
- [x] add import dispatcher/orchestration boundary
- [x] add import apply-intent boundary
- [x] add runtime apply executor boundary (ReplaceVttSession)
- [x] add runtime apply policy controls
- [x] add pending scenario application plan support (non-mutating)
- [x] add scenario candidate activation boundary (DryRun/Activate policy-controlled)
- [x] add scenario activation orchestration service
- [x] add thin file persistence services for VttSession/Scenario/ContentPack/ActionLog
- [x] add snapshot file extension constants helper
- [x] add snapshot file workflow orchestration with extension guardrails
- [x] add single-file import/apply workflow from path
- [x] add known/recent snapshot file library service (caller-managed, non-scanning)
- [x] add snapshot file library list persistence (known paths only)
- [x] add runtime workspace/session lifecycle service (new/open/save/save-as/import-scenario/activate-scenario)
- [x] add workspace operation history tracking for lifecycle operations
- [x] integrate workspace dirty-state updates for successful action processing
- [x] add workspace restore/recovery state file (rehydrate current file + pending scenario context)
- [x] harden workspace-state persistence with temp-write/replace + single backup fallback
- [x] add structured workspace recovery diagnostics (main vs backup, restored flags, warnings/errors)
- [x] add restore options for strict vs best-effort pending-scenario recovery (default strict)

---

## Multiplayer

- [ ] create SignalR hub
- [ ] implement session join
- [ ] implement session leave
- [ ] send ActionRequest from client to server
- [ ] validate actions on server
- [ ] apply actions on server
- [ ] broadcast updated state
- [ ] handle reconnect scenarios

---

## Tools

- [~] implement marker system (core add/remove marker actions complete)
- [ ] implement dice rolling
- [ ] implement note system
- [~] implement basic turn tracking (minimal turn/phase + next/previous-turn + turn number implemented; richer tracker UX/rules pending)
- [ ] implement basic visibility controls

---

## Modules

- [ ] implement module registration
- [ ] implement module activation per session
- [x] implement module state storage
- [ ] allow modules to define actions
- [ ] allow modules to validate actions
- [ ] enforce module isolation

---

## Rules

- [ ] implement AD&D1 module
- [ ] implement BRP module
- [ ] implement initiative logic
- [ ] implement damage application
- [ ] implement rule-specific state updates

---

## UI

- [~] build table view (board panel renders per-active-surface pieces with click/keyboard interactions; broader UX polish pending)
- [ ] build side panel for piece editing
- [~] build surface controls (active surface switch, compact surface workflow strip, quick add/duplicate surface)
- [~] build tool controls (stamp mode, preset lifecycle/defaults, cross-surface stamp queue)
- [~] build participant controls (add/remove/rename panel and selected-piece owner assignment wired; broader UX/validation pending)
- [~] build gameplay mode controls (Edit/Play toggle and indicator wired; policy depth pending)
- [ ] build module UI hooks
- [~] build session management UI (create/open/save/save-as/import/activate flows wired; production UX polish pending)

---

## Testing

- [x] test action validation
- [x] test action execution
- [x] test persistence save/load
- [x] test import validation
- [x] test file persistence boundaries and extension guardrails
- [x] test scenario activation workflow (dry-run/activate/policy)
- [x] test known-files library operations and persistence
- [x] test workspace lifecycle operations
- [x] test workspace operation history
- [x] test workspace action-driven dirty-state behavior
- [x] test workspace save/restore recovery state round-trip and rehydration behavior
- [x] test workspace-state file safety behavior (temp replace, backup fallback, corruption handling)
- [x] test restore diagnostics flags and strict vs best-effort restore options behavior
- [x] test board interaction vertical slices (movement/rotation/keyboard, stamp mode, preset lifecycle/defaults, surface strip, cross-surface queue)
- [x] test participant workflows and ownership consistency
- [x] test turn/phase system and workspace mode behavior
- [x] test game-state save/load vs scenario-start separation behavior
- [x] test action validation payload-shape failures and no-mutation failure behavior
- [ ] test multiplayer synchronization
- [ ] test module behavior
- [~] test domain invariants (covered for current action/persistence boundaries, full invariant suite pending)

---

## Near-Term Focus (next block)

- [ ] expand domain invariant enforcement (id uniqueness + broader reference checks)
- [ ] complete remaining core actions (CreatePiece, DeletePiece, SetPieceVisibility, UpdateTabletopOptions)
- [ ] add current-session missing-file restore option (strict default, optional best-effort) with matrix tests
- [ ] persist workspace mode in workspace recovery state round-trip
- [ ] tighten owner-reference rules for all piece create/update paths (optionally gated to Play mode)

---

## Backlog Rules

- each item must be implementable
- each item must align with domain model
- no UI-only features without domain support
- no direct state mutation tasks
- actions must be used for all changes

---

## Summary

- backlog follows roadmap
- work grouped by system area
- all work aligns to domain model
- actions and VttSession remain central

- Workspace board add-at-click now supports **Stamp Mode** for repeated placement on the active surface.
- Piece ids remain explicit and visible through an editable next-id preview that auto-advances after successful placement.
- Id conflicts remain conservative: duplicate ids fail clearly and do not create a piece.
