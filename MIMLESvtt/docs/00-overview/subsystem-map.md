# Subsystem Map

## Purpose

This document defines the major subsystems of the Blazor VTT.

It exists to prevent architectural drift and to keep the project centered on a stable tabletop engine rather than a growing collection of screens and special-case features.

The VTT should be understood as a set of cooperating subsystems built around a single live runtime concept:

> **The Table Session and its state are the center of the system.**

---

## Top-Level View

The major subsystems are:

1. Table Session Core
2. Surface / Map / Board System
3. Piece / Token / Counter System
4. Content Library and Definitions
5. Interaction and Tooling
6. Turn / Phase / Flow Tools
7. Notes / Chat / Dice / Logs
8. Visibility and Knowledge Model
9. Persistence and Serialization
10. Networking and Synchronization
11. Rules and Automation Extensions
12. Asset Pipeline
13. Administration and Configuration

---

## 1. Table Session Core

### Purpose
The runtime heart of the platform.

### Responsibilities
- represent a live table/session
- hold authoritative state
- track participants and roles
- coordinate active surfaces
- coordinate live piece instances
- expose table-level configuration
- maintain session metadata
- provide the root for save/load and synchronization

### Contains
- session identity
- session title and metadata
- participants
- table options
- active tools/modules
- references to definitions used by the session
- action history or event log hooks

### Notes
This subsystem is the center of the platform. All other live systems should connect to it rather than bypassing it.

---

## 2. Surface / Map / Board System

### Purpose
Represent playable spaces.

### Responsibilities
- define and instantiate boards, maps, and surfaces
- support square, hex, freeform, or zone-based layouts over time
- manage map dimensions, coordinates, layers, and regions
- support overlays and terrain markers later

### Examples
- dungeon map
- battle map
- hex war map
- checkerboard
- area movement board
- card layout surface

### Notes
Do not hardcode assumptions that all play happens on one rectangular grid.

---

## 3. Piece / Token / Counter System

### Purpose
Represent objects placed on surfaces.

### Responsibilities
- define reusable piece types
- instantiate live pieces in a session
- manage position, orientation, ownership, and state
- support stacking, grouping, attachment, and markers
- support hidden vs visible state

### Examples
- monster token
- player token
- counter
- marker
- treasure chest
- vehicle
- status badge
- attached condition marker

### Notes
This subsystem should work equally well for RPG tokens, war-game counters, or board-game pieces.

---

## 4. Content Library and Definitions

### Purpose
Store reusable authored content separately from live session state.

### Responsibilities
- manage definitions for:
  - monsters
  - NPCs
  - characters
  - items
  - treasure
  - equipment
  - magic items
  - markers
  - map definitions
  - scenarios
- support import/export
- support authoring and editing workflows
- support tagging, grouping, and metadata

### Notes
This is where AD&D1 and BRP content belongs at the definition level, not in the session core.

---

## 5. Interaction and Tooling

### Purpose
Handle how the user manipulates the table.

### Responsibilities
- selection
- drag/drop
- snapping
- hover and inspection
- context actions
- multi-select
- side-panel editing
- tool modes
- map navigation such as pan/zoom

### Notes
This subsystem should be designed for speed and clarity. It is one of the main user-experience drivers.

---

## 6. Turn / Phase / Flow Tools

### Purpose
Provide optional structures for sequence and control of play.

### Responsibilities
- initiative tracking
- turn order
- phase sequencing
- round markers
- activation tools
- custom flow systems for specific games

### Notes
This subsystem must be optional and pluggable. The table engine should not require initiative or rounds in order to function.

---

## 7. Notes / Chat / Dice / Logs

### Purpose
Provide support information around play.

### Responsibilities
- dice rolling and roll history
- action logs
- notes
- handouts
- GM-only annotations
- reference panels
- chat later in multiplayer phases

### Notes
Logging is especially important because it supports debugging, auditing, replay, and future automation.

---

## 8. Visibility and Knowledge Model

### Purpose
Represent what different roles or participants can see or know.

### Responsibilities
- hidden vs visible pieces
- GM-only data
- player-visible subsets
- revealed/unrevealed areas
- fog-of-war foundations
- future line-of-sight integration

### Notes
Visibility should be modeled explicitly, not left to UI accident.

---

## 9. Persistence and Serialization

### Purpose
Save, load, export, import, and version state safely.

### Responsibilities
- serialize session state
- deserialize session state
- archive sessions
- package reusable content
- support schema versioning
- support migration of saved data

### Export Types
- content packs
- scenario exports
- session saves
- action logs

### Notes
Persistence should be planned early, not added after the state model drifts.

---

## 10. Networking and Synchronization

### Purpose
Support shared live table play.

### Responsibilities
- session join/leave
- participant presence
- client synchronization
- authoritative action handling
- reconnect behavior
- state reconciliation
- permissions enforcement

### Notes
Even if multiplayer comes later, the session core should be designed with synchronization in mind.

---

## 11. Rules and Automation Extensions

### Purpose
Allow rules-aware behavior without embedding one ruleset in the engine core.

### Responsibilities
- optional validation hooks
- optional computed values
- optional automation modules
- rules-specific services
- system adapters for AD&D1, BRP, and future rulesets

### Notes
AD&D1 and BRP should live here and in content definitions, not in the engine foundation.

---

## 12. Asset Pipeline

### Purpose
Handle maps, token images, icons, packaged media, and derived assets.

### Responsibilities
- file upload
- asset metadata
- thumbnails
- image normalization
- package ingestion
- export packaging

### Notes
This subsystem can also support external preprocessing tools or Python-based asset digestion if needed.

---

## 13. Administration and Configuration

### Purpose
Control application-wide and table-wide settings.

### Responsibilities
- user and role configuration
- table defaults
- module enable/disable settings
- storage configuration
- import/export settings
- future provider settings for AI or automation helpers

---

## Cross-Cutting Concerns

These affect nearly all subsystems:

- identity and permissions
- validation
- logging
- schema versioning
- import/export compatibility
- performance
- testability

---

## Architectural Rule

The VTT should be designed so that:

- definitions are not live instances
- rules modules are not the session core
- UI screens are not the domain model
- persistence format is not the domain model
- networking messages do not become the domain model by accident

The **Table Session Core** remains authoritative, while other subsystems attach to or operate through it.