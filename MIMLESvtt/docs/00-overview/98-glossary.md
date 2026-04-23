# Domain Glossary

## Purpose

This document defines the **canonical vocabulary** for the Blazor VTT.

It exists to:
- eliminate ambiguity
- prevent overlapping concepts
- stabilize naming across code, UI, and persistence

If a term is not here, it should not be casually invented in code.

---

## Core Concepts

### Table Session
A **live instance of play**.

- Contains all runtime state
- Represents one active or saved session
- Is the root aggregate of the system

---

### Definition
A **reusable content object**.

Examples:
- monster template
- map template
- item template
- scenario template

Definitions are:
- reusable
- not tied to a specific session
- versionable

---

### Instance
A **runtime object created from a definition**.

Examples:
- a goblin on the map
- a placed treasure chest
- a loaded battle map

Instances:
- exist only within a Table Session
- contain mutable state

---

### Surface
A **playable space**.

Examples:
- map
- board
- hex grid
- abstract zone layout

A surface may:
- contain pieces
- define coordinate systems
- support layers

---

### Surface Definition
A reusable definition of a surface.

---

### Surface Instance
A runtime surface within a Table Session.

---

### Piece
A **generic object placed on a surface**.

Examples:
- token
- counter
- pawn
- marker
- vehicle
- creature

---

### Piece Definition
Reusable description of a piece.

Contains:
- name
- image/icon
- base attributes
- metadata

---

### Piece Instance
A live piece in a session.

Contains:
- position
- rotation/facing
- ownership/controller
- visibility state
- dynamic attributes (HP, status, etc.)

---

### Marker
A lightweight piece used for status or annotation.

Examples:
- “stunned”
- “hidden”
- “bloodied”

---

### Stack
A group of pieces occupying the same logical location.

Used for:
- counters in war games
- overlapping tokens
- card piles

---

### Container
A piece or structure that holds other pieces.

Examples:
- inventory
- deck
- bag
- hidden GM pool

---

### Coordinate
A position within a surface.

May be:
- grid-based (x,y)
- hex-based (q,r or cube)
- freeform (pixel/world coordinates)
- abstract (zone id)

---

### Location
A resolved placement of a piece.

Includes:
- surface reference
- coordinate or zone
- optional layer
- optional container/stack

---

### Layer
A visual or logical level on a surface.

Examples:
- background map
- tokens
- overlays
- fog

---

### Zone
A named area on a surface.

Examples:
- room
- region
- control zone
- card area

---

### Visibility
Controls what a participant can see.

Includes:
- visible
- hidden
- GM-only
- partially revealed (future)

---

### Participant
A user in a Table Session.

Roles:
- GM
- Player
- Observer

---

### Action
A **user or system intent that changes state**.

Examples:
- MovePiece
- RotatePiece
- RevealArea
- AddMarker

Actions are:
- explicit
- loggable
- the preferred way to mutate state

---

### Event (Future-Oriented)
A recorded result of an action.

Used for:
- replay
- undo
- audit

---

### Tool
A UI or system feature used during play.

Examples:
- selection tool
- dice roller
- turn tracker
- measurement tool

---

### Module
A pluggable subsystem.

Examples:
- AD&D1 rules module
- BRP rules module
- initiative system
- inventory system

Modules:
- extend behavior
- must not redefine the core model

---

### Scenario
A prepared setup.

Includes:
- surfaces
- placed pieces
- initial state
- optional rules configuration

---

### Gamebox
A packaged set of definitions and assets.

Used for:
- import/export
- sharing content

---

### Session Save
A snapshot of a Table Session.

Includes:
- all instances
- current state
- metadata

---

### Action Log
A record of actions taken in a session.

Used for:
- debugging
- replay
- auditing

---

### Workspace
A **user-facing working context** for editing or playing within the application.

A workspace typically includes:
- the current Table Session
- current file path
- current pending scenario plan
- current selection state
- current board interaction state
- current operation feedback
- current developer feature toggles (if exposed)

A workspace is not the same thing as a Table Session.

A Table Session is domain/runtime state.
A Workspace is application/UI state wrapped around that session.

---

### Workspace State
The mutable UI/application state associated with a Workspace.

Examples:
- active surface
- selected piece
- dirty flag
- operation history
- current mode (edit/play)
- board HUD settings
- stamp mode state
- queue state
- preset state

Workspace State is application state, not core persisted domain state unless explicitly saved as workspace recovery data.

---

### Pending Scenario Plan
An intermediate application object created when a scenario is imported but not yet activated.

It represents:
- a valid scenario that has been loaded
- a prepared plan for activation
- a state prior to creating or activating a live session candidate

It is not yet the live Table Session.

---

### Scenario Candidate
An isolated Table Session candidate produced from a Pending Scenario Plan.

It is:
- structurally valid for activation
- not yet necessarily the active live session

It is used as a controlled step before replacing the current live session.

---

### Active Surface
The surface currently shown or interacted with in the Workspace board UI.

This is a workspace/UI concept.
It references a Surface Instance within the current Table Session.

---

### Selected Piece
The piece currently selected in the Workspace UI.

This is a workspace/UI concept.
It references a Piece Instance within the current Table Session.

---

### Feature Toggle
A switch used to enable, disable, or stage a feature during development or pre-alpha use.

Feature toggles are:
- application-level controls
- not core domain concepts
- useful for incremental rollout and testing

---

## Naming Rules

- Use **Definition** for reusable content
- Use **Instance** for runtime objects
- Do not mix these terms
- Avoid synonyms like “entity,” “object,” or “item” unless explicitly defined