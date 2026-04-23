# Roadmap

## Canonical Status

This is the **canonical platform implementation roadmap** for the project.

Related scoped roadmap:
- `docs/05-backlog/checkers-roadmap.md` (reference implementation / prototype track)

## File Location

docs/05-backlog/roadmap.md

---

## Purpose

This document defines the development phases of the Blazor VTT.

It provides a structured sequence for building the system from:

- core domain model
- to usable single-user table
- to multiplayer platform
- to extensible module system

This document does not list individual tasks.

It defines phases and goals.

---

## Core Principle

Build the engine first.

Do not build UI features ahead of domain stability.

---

## Phase 0 – Domain Foundation

### Goal

Establish a stable domain model.

### Includes

- 00-overview/98-glossary.md defined
- 00-overview/09-tabletop-state-model.md implemented
- 00-overview/06-global-action-system.md implemented
- core domain classes created
- basic validation rules defined

### Exit Criteria

- VttSession exists and is coherent
- actions mutate state through a single system
- state is serializable

---

## Phase 1 – Single-User Table

### Goal

Create a usable table in single-user mode.

### Includes

- create/load VttSession
- create surfaces
- create and place pieces
- move pieces
- rotate pieces
- update piece state
- basic UI rendering

### Exit Criteria

- user can create a session
- user can place and move pieces
- session can be saved and loaded

---

## Phase 2 – Persistence and Import/Export

### Goal

Stabilize persistence and data exchange.

### Includes

- session save/load
- scenario export/import
- gamebox export/import
- validation rules implemented
- versioning added

### Exit Criteria

- sessions persist reliably
- imports validate correctly
- exports produce consistent output

---

## Phase 3 – Multiplayer Core

### Goal

Enable shared sessions.

### Includes

- SignalR session connection
- join/leave session
- action dispatch to server
- server-side validation
- state broadcast to clients

### Exit Criteria

- multiple users see the same table state
- actions are applied consistently
- server is authoritative

---

## Phase 4 – Table Tools

### Goal

Add essential gameplay tools.

### Includes

- dice rolling
- markers
- basic notes
- simple turn tracking
- basic visibility controls

### Exit Criteria

- table supports common gameplay needs
- tools operate through action system

---

## Phase 5 – Module System

### Goal

Enable extensibility through modules.

### Includes

- module activation per session
- module state storage
- module action handling
- module isolation rules enforced

### Exit Criteria

- modules can be added without modifying core
- module state persists correctly

---

## Phase 6 – Rules Modules

### Goal

Implement system-specific logic.

### Includes

- AD&D1 module
- BRP module
- initiative systems
- rule-specific actions

### Exit Criteria

- multiple rulesets operate on same engine
- no duplication of core logic

---

## Phase 7 – Advanced Features

### Goal

Extend system capabilities.

### Includes

- fog of war
- visibility refinement
- replay support
- undo/redo (if implemented)
- advanced tools

### Exit Criteria

- advanced features integrate with core model
- no violation of domain rules

---

## Phase 8 – Content and Workflow Expansion

### Goal

Improve usability and content workflows.

### Includes

- scenario editors
- content library UI
- import pipelines
- asset management improvements

### Exit Criteria

- users can manage content efficiently
- workflows are stable

---

## Phase 9 – Stabilization

### Goal

Harden the system.

### Includes

- bug fixing
- performance improvements
- validation improvements
- test coverage expansion

### Exit Criteria

- stable session behavior
- consistent persistence
- reliable multiplayer operation

---

## Summary

- build domain first
- build table second
- add persistence
- add multiplayer
- add tools
- add modules
- add rules
- expand features
- stabilize system
```
