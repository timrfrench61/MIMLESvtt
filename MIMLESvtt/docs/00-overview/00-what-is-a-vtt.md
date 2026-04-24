# What Is a VTT (Project Overview + Vision and Scope)
##### Note 00-overview actually means Models and code
## Definition

A **Virtual Tabletop (VTT)** is:

> A software platform that maintains and renders a shared game state representing a tabletop play surface, allowing users to interact with game elements through a digital interface.

---

## Core Concept

A VTT replaces a physical tabletop environment (board, pieces, dice, and players) with a digital equivalent.

At its simplest, it is a **shared interactive surface** where:

- a game state exists
- users can view that state
- users can modify that state through actions

---

## Core Responsibilities

A minimal VTT consists of these fundamental responsibilities:

### 1. Shared State
The system maintains a single authoritative game state, including:

- board/map
- pieces/tokens
- game data and attributes

### 2. Interaction System
Users interact with state through actions:

- move pieces
- update values (health/status)
- trigger game-specific behaviors

### 3. Rendering / UI
The system presents current state visually:

- displays board and pieces
- reflects changes immediately
- provides input mechanisms

### 4. Persistence
The system supports saving and loading:

- session state
- game progress

### 5. Optional Networking
In multiplayer scenarios:

- synchronizes state across clients
- ensures consistency between participants

---

## Project Vision (MIMLESvtt)

Build a **Blazor-based VTT and tabletop engine** for:

- roleplaying games
- tactical encounters
- board games
- hex-and-counter war games
- scenario-driven tabletop play

The platform is intended to be:

- **engine-first** (not tied to one game genre)
- **state-centered** (live table state is central)
- **modular** (rules/tools/workflows added without rewriting core)
- **fast in use** for GM/referee workflows

---

## Product Position

This project is not just a rules helper or encounter tracker.

It is a **general tabletop platform** that supports:

- reusable content definitions
- live table/session state
- maps/surfaces
- pieces/counters/tokens
- turn/phase tools
- notes/logging
- optional rules-aware modules
- future multiplayer synchronization

---

## Scope

### Included Core Scope

- table/session management
- surface/map support
- piece/token/counter support
- reusable content definitions
- table tools
- persistence and data exchange

### Early Release Priorities

- single-user GM/referee workflow
- session creation and setup
- piece placement and movement
- save/load
- structured import
- optional turn-tracking tooling

### Deferred (Later)

- full player-facing UI
- fog/visibility refinement
- real-time multiplayer collaboration
- advanced rendering/tools
- campaign-level persistence

### Non-Goals (Initial)

- 3D environments
- heavy animation engine
- full automation of every rule
- marketplace/public sharing platform

---

## What a VTT Is Not

A VTT does **not inherently include**:

- one specific game ruleset
- a mandatory rules engine
- RPG-only assumptions

These are optional features layered on top of the core engine.

---

## Guiding Design Principle

For maintainability and flexibility:

> Build as **state + actions + rendering**, then add rules/tools/networking as optional layers.

This keeps the core reusable across multiple game types.

---

## Success Criteria (High Level)

- sessions can be created, saved, and reloaded reliably
- surfaces can be loaded and manipulated
- pieces can be added/moved/edited predictably
- workflows remain explicit and low-friction
- core model stays ruleset-agnostic

---

## Summary

In this project, a VTT is:

- a **state container**
- an **interaction engine**
- a **rendering system**

with optional feature layers (rules modules, advanced tools, multiplayer) added incrementally.
