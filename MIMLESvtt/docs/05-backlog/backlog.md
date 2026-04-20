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

- create TableSession class
- create Participant model
- create SurfaceInstance model
- create PieceInstance model
- create Location model
- create Coordinate model
- create VisibilityState model
- create TableOptions model
- create ActionRecord model
- implement ModuleState structure
- enforce id uniqueness rules
- enforce reference validation rules

---

## Action System

- create ActionRequest structure
- implement action dispatch mechanism
- implement validation step
- implement execution step
- implement ActionLog storage
- implement core actions:
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

## Table

- create table creation workflow
- create table load workflow
- create surface creation
- create piece creation
- implement piece placement
- implement piece movement
- implement piece rotation
- implement piece state editing
- implement selection model
- implement rendering of surfaces and pieces

---

## Persistence

- implement session save
- implement session load
- implement scenario export
- implement scenario import
- implement content pack export
- implement content pack import
- implement action log export
- implement version field handling
- implement validation during import

---

## Multiplayer

- create SignalR hub
- implement session join
- implement session leave
- send ActionRequest from client to server
- validate actions on server
- apply actions on server
- broadcast updated state
- handle reconnect scenarios

---

## Tools

- implement marker system
- implement dice rolling
- implement note system
- implement basic turn tracking
- implement basic visibility controls

---

## Modules

- implement module registration
- implement module activation per session
- implement module state storage
- allow modules to define actions
- allow modules to validate actions
- enforce module isolation

---

## Rules

- implement AD&D1 module
- implement BRP module
- implement initiative logic
- implement damage application
- implement rule-specific state updates

---

## UI

- build table view
- build side panel for piece editing
- build surface controls
- build tool controls
- build module UI hooks
- build session management UI

---

## Testing

- test action validation
- test action execution
- test persistence save/load
- test import validation
- test multiplayer synchronization
- test module behavior
- test domain invariants

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
