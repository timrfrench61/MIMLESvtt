# Module Architecture

## File Location

docs/00-overview/20-module-architecture.md

---

## Purpose

This document defines how optional systems (modules) extend the Blazor VTT without modifying the core engine.

It establishes:

- how modules interact with TableSession
- how modules store data
- how modules participate in actions
- how modules are enabled or disabled
- how modules remain isolated from core domain structures

---

## Core Principle

Modules extend behavior.

Modules do not redefine the core model.

---

## What Is a Module

A module is an optional subsystem that adds behavior or rules.

Examples:

- AD&D1 rules
- BRP rules
- initiative tracker
- turn/phase system
- inventory system
- card/deck system
- wargame phase system

Modules operate on top of:

- TableSession
- SurfaceInstance
- PieceInstance

---

## What Modules Can Do

Modules may:

- define new actions
- validate actions
- react to actions
- store module-specific state
- add derived behavior
- interpret piece state

---

## What Modules Cannot Do

Modules must not:

- change the structure of TableSession
- modify core domain classes directly
- bypass the action system
- store state outside of defined module storage
- assume all sessions use that module

---

## Module State

Modules store runtime data inside:

```plaintext
TableSession.ModuleState
````

This is a key-value structure.

Each module owns its own key.

---

## Module State Rules

* each module uses a unique key
* module state must be serializable
* module state must not conflict with other modules
* module state must not duplicate core state

---

## Example Module State

Examples of module data:

* initiative order
* turn index
* phase state
* BRP hit locations
* spell timers
* card deck order

---

## Module Activation

Modules are enabled per session.

A session may have:

* no modules
* one module
* multiple modules

Module activation should be stored in TableSession configuration.

---

## Module and Actions

Modules interact with the action system.

Modules may:

* define new action types
* validate action payloads
* react to accepted actions

---

## Module Action Rules

* module actions must follow the same action flow
* module actions must be validated
* module actions must be logged
* module actions must not bypass core rules

---

## Module Isolation

Modules must be isolated from each other.

Rules:

* no direct access to another module’s state
* no shared mutable structures outside TableSession
* interaction must occur through:

  * actions
  * core state

---

## Module Dependency

Modules should not require other modules unless explicitly declared.

If dependencies exist:

* they must be explicit
* they must be validated during module activation

---

## Module Lifecycle

A module participates in the session lifecycle:

1. enabled for session
2. initializes its state
3. handles actions
4. updates ModuleState
5. persists with session
6. restores on load

---

## Module Initialization

When a module is enabled:

* it may initialize its state
* it may register handlers
* it may validate session compatibility

---

## Module Persistence

Module state is persisted as part of TableSession.

It must:

* serialize cleanly
* restore cleanly
* survive version changes

---

## Module Versioning

Modules may evolve independently.

Module state should include version information if needed.

---

## Module UI

Modules may provide UI elements such as:

* panels
* controls
* overlays

These must:

* use the action system
* not directly mutate core state

---

## Module Examples

### Initiative Module

* tracks turn order
* defines AdvanceTurn action
* stores order in ModuleState

### BRP Combat Module

* interprets hit locations
* updates piece state
* defines ApplyDamage action

### Wargame Phase Module

* tracks phase progression
* defines AdvancePhase action
* stores phase state

---

## Module Boundaries

Core engine owns:

* TableSession
* surfaces
* pieces
* visibility
* actions

Modules extend behavior but do not redefine these.

---

## Summary

* modules extend behavior
* modules store state in ModuleState
* modules use the action system
* modules remain isolated
* modules do not modify core structures

