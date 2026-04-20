# Vision and Scope

Related: [Design Packet Integration Report](design-packet-integration-report.md)

## Vision

Build a **Blazor-based Virtual Tabletop (VTT) and tabletop engine** for running:

- roleplaying games
- tactical encounters
- board games
- hex-and-counter war games
- scenario-driven tabletop play

The platform should be:

- **engine-first**, not locked to one game genre
- **state-centered**, with the live table as the core runtime model
- **modular**, so rules, tools, and UI workflows can be added without rewriting the foundation
- **fast in actual use**, especially for the Game Master or referee

The first practical target may still be GM-led play for systems such as AD&D 1st Edition and BRP, but those should be treated as **modules and workflows on top of the engine**, not as the engine itself.

---

## Product Position

This project is not just a rules helper or encounter tracker.

It is a **general tabletop platform** that should eventually support:

- reusable content definitions
- live table/session state
- maps, boards, and surfaces
- pieces, counters, and tokens
- turn/phase/initiative tools
- notes, handouts, and logs
- optional rules-aware modules
- future multiplayer synchronization

---

## Target Users

### Primary
- Game Masters
- Referees
- Scenario designers
- Solo operators testing encounters or game states

### Secondary
- Players in networked sessions
- Content authors building reusable game assets
- Designers building custom modules for specific systems

---

## Core Objectives

1. Provide a **strong runtime table model** as the center of the system
2. Support **fast table setup and manipulation**
3. Separate **definitions** from **live instances**
4. Allow **rules-specific modules** without hardwiring rules into the core
5. Support **save/load, import/export, and future multiplayer**
6. Keep GM workflows fast, explicit, and reliable

---

## Functional Scope

### Included (Core Platform)

#### Table and Session Management
- Create, load, save, and archive table sessions
- Track participants, roles, and session metadata
- Maintain authoritative live table state

#### Surface / Map / Board Support
- Support one or more play surfaces
- Support square, hex, or freeform spatial models over time
- Load map images or board definitions
- Maintain layers, zones, or regions as needed

#### Piece / Token / Counter Support
- Define reusable piece types
- Create live piece instances in a session
- Place, move, rotate, stack, hide, reveal, and annotate pieces
- Support markers, status indicators, and attachments

#### Content Definition and Reuse
- Define reusable content for:
  - creatures / monsters
  - characters / NPCs
  - items / equipment
  - treasure
  - magic items
  - markers
  - maps
  - scenarios
- Support structured import and export of these definitions

#### Table Tools
- Selection and inspection
- Turn / phase / initiative tools as optional modules
- Dice and action logging
- Notes / handouts / reference panels
- GM-side editing and overrides

#### Persistence and Exchange
- Session save/load
- Scenario export/import
- Content pack import/export
- Versioned serialization and migration support

---

## Functional Scope for Early Releases

### Early Releases Should Prioritize
- Single-user GM/referee workflow
- Table/session creation
- Surface loading
- Piece placement and movement
- Selection and editing
- Save/load
- Structured content import
- Optional combat/turn tracker tool
- AD&D1 and BRP starter modules built on top of the shared engine

### Early Releases May Include
- CSV import for:
  - monsters
  - treasure
  - equipment
  - magic items
- Encounter setup workflows
- Initiative and turn order tools
- Damage or status tracking panels

These are valid early features, but they should be implemented as **applications of the core model**, not substitutes for it.

---

## Deferred (Later Phases)

- Full player-facing UI
- Fog of war and visibility controls
- Real-time multiplayer collaboration
- Permissions by role and visibility layer
- Drawing and measurement tools
- Advanced map/grid rendering
- Mobile optimization
- Rule automation scripting
- AI assistant/referee tooling
- Campaign-level persistence
- Replay / undo / event timeline

---

## Non-Goals (Initial)

- 3D environments
- heavyweight animation engine
- full automation of every game rule
- marketplace or public content sharing platform
- deeply system-specific assumptions in the core model

---

## Technical Direction

- UI: **Blazor**
- Initial hosting direction: **Blazor Server** or Blazor Web App with server-authoritative state
- Backend: **ASP.NET Core services**
- Real-time sync: **SignalR-based session synchronization**
- Storage: SQL-backed metadata plus serialized session/content state
- Import/export: versioned JSON and package-based asset formats
- Architecture: separation of:
  - domain model
  - application/use cases
  - infrastructure
  - UI/presentation

---

## Success Criteria

### Core Platform Success
- A table can be created, saved, and reloaded reliably
- A map or surface can be loaded and manipulated
- Pieces can be added, moved, and edited with predictable behavior
- Live state remains coherent and debuggable
- Core workflows do not depend on one ruleset

### GM Workflow Success
- A GM can load content and build an encounter quickly
- Common actions take minimal clicks
- The system reduces table friction rather than adding ceremony

### Architecture Success
- AD&D1 and BRP can both be supported without duplicating core engine code
- New systems can be added by extending definitions, tools, and rules modules
- The core table model remains stable while modules evolve

---

## Guiding Principles

- **Engine-first, not RPG-first**
- **State-first design**
- **Definitions separate from live instances**
- **Rules modularity**
- **Fast UI over flashy UI**
- **Explicit over implicit behavior**
- **Minimal clicks during play**
- **Stable core, extensible edges**