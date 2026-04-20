## Updated Roadmap


# Roadmap

## Phase 1 — Foundation and Product Definition

### Goals
- Define the platform as a **general tabletop engine**, not only an RPG VTT
- Establish architecture, documentation, and subsystem boundaries
- Define core engine concepts that support RPGs, war games, and board games

### Deliverables
- Vision and Scope
- Subsystem Map
- Architecture Decision Record
- Master Backlog
- Initial domain glossary
- Core design principles

### Key Decisions
- Blazor Server for early development
- Modular rules/plugin approach
- Generic table-state model as the platform center

---

## Phase 2 — Core Table Engine

### Goals
- Build the generic engine for boards, spaces, pieces, players, and turn state
- Avoid hard-coding RPG assumptions into the foundation

### Deliverables
- Board definition model
- Coordinate system support (square first; hex later)
- Piece definition and piece instance model
- Turn/phase sequencing model
- Move/history log
- Session state model
- Save/load game state design

### Notes
This phase creates the reusable foundation for:
- checkers
- tactical combat
- strategic war games
- RPG encounters

---

## Phase 3 — Checkers Reference Implementation

### Goals
- Prove the engine with a simple complete game
- Validate board presentation, turn handling, move validation, capture, and state persistence

### Deliverables
- Checkers board UI
- Piece rendering
- Move rules
- Capture rules
- King promotion rules
- Turn tracking
- Win-state detection
- Save/load support for checkers sessions

### Why This Phase Matters
Checkers is the early proof that:
- board state works
- turn sequencing works
- networking can later sync state
- rules modules can be layered over the engine

---

## Phase 4 — Tactical Tabletop Expansion

### Goals
- Expand from simple board play to tactical tabletop support
- Prepare for miniatures, counters, and encounter-style play

### Deliverables
- Grid movement support
- Hex-grid support
- Terrain and zone concepts
- Piece attributes (movement, side, status)
- Stacking/grouping rules framework
- Range and adjacency helpers
- Line/path helper design
- Tactical scenario setup screen

### Notes
This phase bridges the gap between board games and RPG/wargame combat.

---

## Phase 5 — Content Libraries and Data Import

### Goals
- Create structured data support for reusable content
- Support both manual entry and CSV imports

### Deliverables
- Content library framework
- Manual GM entry screens
- CSV import pipeline
- Validation/error reporting
- Initial import specs for:
  - monsters
  - treasure
  - equipment
  - magic items
  - units/counters (if used for wargames)
- Test data packet format
- Sample seed/test files

### Notes
This phase establishes the data-driven content model needed for larger systems.

---

## Phase 6 — RPG Ruleset Support

### Goals
- Add RPG-focused mechanics as modules on top of the engine
- Keep rules logic isolated from generic table/piece/state logic

### Deliverables
- Ruleset plugin framework
- Dice and roll-expression services
- AD&D 1e rules module
- BRP rules module
- Initiative and combat workflow options
- Character/creature stat handling
- Damage/wound tracking
- Status/effect hooks

### Notes
RPG support becomes a specialization of the engine, not the foundation of the platform.

---

## Phase 7 — Wargame and Strategic Play Support

### Goals
- Extend the engine to support strategic and operational play
- Add larger-scale map/state concepts beyond tactical skirmish

### Deliverables
- Area or region map model
- Unit/counter metadata
- Supply/status markers framework
- Multi-turn sequence support
- Scenario objectives
- Reinforcement/event scheduling hooks
- Hidden/open information options
- Strategic scale turn phases

### Notes
This phase supports more traditional war game play without forcing RPG assumptions.

---

## Phase 8 — Networking and Live Session Sync

### Goals
- Support multiplayer sessions and shared state
- Enable GM-hosted and player-connected games

### Deliverables
- Session hosting model
- Player seat/role model
- SignalR-based live sync
- Shared move/event updates
- Reconnect/session recovery behavior
- Permissions model for GM vs player controls
- Multiplayer test cases

### Notes
By this point the table-state model should already be stable enough to synchronize.

---

## Phase 9 — Testing, Hardening, and Tooling

### Goals
- Improve reliability, maintainability, and developer workflow
- Expand automated and manual test coverage

### Deliverables
- Test packet library
- Import validation test suite
- Checkers regression tests
- Tactical movement test cases
- RPG combat test cases
- Networking sync tests
- Logging/error-handling standards
- Performance review
- Refactoring backlog

---

## Phase 10 — Advanced Platform Features

### Goals
- Add richer capabilities once the core engine is stable

### Possible Deliverables
- Player-facing UI views
- Fog of war
- Card/deck systems
- Hand/private information zones
- Replay/history viewer
- Scenario editor
- Rule scripting
- AI helpers
- Map overlays
- Printing/export tools

---

## Release Milestone View

### Milestone A — Engine Skeleton
- Foundation complete
- Core table engine defined

### Milestone B — First Working Game
- Checkers fully playable

### Milestone C — Tactical Sandbox
- Grid/hex tactical prototype working

### Milestone D — Data-Driven Content
- CSV imports and GM entry screens working

### Milestone E — First RPG Modules
- AD&D1 and BRP integrated

### Milestone F — Multiplayer Table
- Live synchronized session working

### Milestone G — Broader Tabletop Platform
- Wargame, RPG, and board-game support all proven

