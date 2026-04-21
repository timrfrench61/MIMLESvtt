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

- TableSession
- action system
- persistence rules

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

- [x] create TableSession class
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
- [x] add runtime apply executor boundary (ReplaceTableSession)
- [x] add runtime apply policy controls
- [x] add pending scenario application plan support (non-mutating)
- [x] add scenario candidate activation boundary (DryRun/Activate policy-controlled)
- [x] add scenario activation orchestration service
- [x] add thin file persistence services for TableSession/Scenario/ContentPack/ActionLog
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
- [ ] implement basic turn tracking
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

- [ ] build table view
- [ ] build side panel for piece editing
- [ ] build surface controls
- [ ] build tool controls
- [ ] build module UI hooks
- [ ] build session management UI

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
- [ ] test multiplayer synchronization
- [ ] test module behavior
- [~] test domain invariants (covered for current action/persistence boundaries, full invariant suite pending)

---

## Near-Term Focus (next block)

- [ ] expand domain invariant enforcement (id uniqueness + broader reference checks)
- [ ] complete remaining core actions (CreatePiece, DeletePiece, SetPieceVisibility, UpdateTableOptions)
- [ ] add current-session missing-file restore option (strict default, optional best-effort) with matrix tests

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
- actions and TableSession remain central
