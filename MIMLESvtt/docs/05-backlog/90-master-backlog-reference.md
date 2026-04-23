## Updated `master-backlog.md`

### Reference-only. Do not use as active planning source.

# Master Backlog

## Overview

This is the top-level backlog for the Blazor Tabletop Engine / VTT project.

The product is designed as a **general tabletop platform** supporting:

- board games
- tactical skirmish games
- strategic war games
- roleplaying games

AD&D 1e and BRP are early supported rulesets, but the platform itself must remain broader than any one game family.

---

## Legend

### Priority
- High
- Medium
- Low

### Status
- Pending
- In Progress
- Blocked
- Done

---

## Documentation and Planning Traceability (Reference)

This section captures the compact crosswalk previously expanded in the documentation-planning subsystem backlog.

Primary master IDs for documentation/planning governance:

- MB-002 Define product vision and scope
- MB-003 Define subsystem map
- MB-054 Create subsystem backlog set
- MB-055 Create roadmap and milestone tracking doc
- MB-056 Create developer onboarding and repo conventions doc

Compact DOC reference catalog:

| DOC ID | Master ID | Title | Priority |
|---|---|---|---|
| DOC-001 | MB-002 | Maintain vision and scope document | High |
| DOC-002 | MB-003 | Maintain subsystem map | High |
| DOC-003 | MB-055 | Maintain roadmap and milestone tracking | High |
| DOC-004 | MB-054 | Maintain master backlog reference | High |
| DOC-005 | MB-054 | Maintain subsystem backlog set | High |
| DOC-006 | MB-056 | Create developer onboarding guide | Medium |
| DOC-007 | MB-056 | Create repo conventions document | Medium |
| DOC-008 | MB-056 | Define module-addition workflow | Medium |
| DOC-009 | MB-055 | Create milestone review checklist | Medium |
| DOC-010 | MB-055 | Create architecture decision record index | Medium |
| DOC-011 | MB-055 | Maintain domain glossary | High |
| DOC-012 | MB-054 | Create dependency tracking notes across subsystems | Medium |
| DOC-013 | MB-055 | Create release packaging and milestone naming notes | Low |
| DOC-014 | MB-056 | Create documentation folder map | Medium |
| DOC-015 | MB-054 | Define backlog item writing standard | Medium |
| DOC-016 | MB-056 | Create contributor quick-start checklist | Low |
| DOC-017 | MB-055 | Maintain implementation notes and open questions log | Medium |
| DOC-018 | MB-054 | Create subsystem ownership matrix | Low |

Reference usage note:

- This file remains reference-only.
- Active execution planning should be maintained in `00-backlog.md` and category backlog docs.

Recent backlog intake references from `00-overview` (58-68):

- `docs/05-backlog/58-backlog-unit-counter-content-model.md`
- `docs/05-backlog/59-backlog-manual-gm-entry-workflow.md`
- `docs/05-backlog/60-backlog-gm-monster-entry-screen-slice.md`
- `docs/05-backlog/61-backlog-gm-treasure-entry-screen-slice.md`
- `docs/05-backlog/62-backlog-gm-equipment-entry-screen-slice.md`
- `docs/05-backlog/63-backlog-gm-magic-item-entry-screen-slice.md`
- `docs/05-backlog/64-backlog-reusable-csv-import-pipeline.md`
- `docs/05-backlog/65-backlog-import-validation-model.md`
- `docs/05-backlog/66-backlog-test-data-packet-format.md`
- `docs/05-backlog/67-backlog-engine-test-strategy.md`
- `docs/05-backlog/68-backlog-subsystem-test-boundaries.md`

---

## Backlog Items

---

## Foundation and Product Definition

### MB-001
**Title:** Establish solution structure  
**Subsystem:** Core / Architecture  
**Priority:** High  
**Status:** Done  

**Description:**  
Create the initial Blazor Server solution and supporting project structure for UI, domain, application services, infrastructure, and tests.

**Dependencies:** None

**Acceptance Criteria:**
- Solution builds successfully
- Project boundaries are defined
- Basic startup and navigation work
- Documentation folder structure is created

---

### MB-002
**Title:** Define product vision and scope  
**Subsystem:** Documentation / Planning  
**Priority:** High  
**Status:** Done  

**Description:**  
Document the product as a general tabletop engine rather than an RPG-only VTT.

**Dependencies:** None

**Acceptance Criteria:**
- Vision document is written
- Scope includes board, tactical, strategic, and RPG use cases
- Non-goals are documented
- Early supported rulesets are identified

---

### MB-003
**Title:** Define subsystem map  
**Subsystem:** Documentation / Planning  
**Priority:** High  
**Status:** Done  

**Description:**  
Create and maintain the subsystem breakdown for the platform.

**Dependencies:** MB-002

**Acceptance Criteria:**
- Subsystem list is documented
- Responsibilities are defined for each subsystem
- High-level dependencies are mapped

---

### MB-004
**Title:** Record initial architecture decision  
**Subsystem:** Architecture  
**Priority:** High  
**Status:** Done  

**Description:**  
Document the use of Blazor Server for early-phase development, with later networking and modular service growth in mind.

**Dependencies:** MB-001, MB-002

**Acceptance Criteria:**
- Architecture decision record exists
- Rationale for Blazor Server is documented
- Known tradeoffs are listed
- Future migration considerations are noted

---

## Core Table Engine

### MB-005
**Title:** Define core domain glossary  
**Subsystem:** Domain / Documentation  
**Priority:** High  
**Status:** Done  

**Description:**  
Create standard definitions for core engine concepts such as board, map, zone, space, piece, token, counter, player seat, turn state, scenario, and session.

**Dependencies:** MB-003

**Acceptance Criteria:**
- Glossary exists
- Terms are non-overlapping where possible
- Terms support both RPG and wargame usage

---

### MB-006
**Title:** Define board and table-state model  
**Subsystem:** Domain / Table Engine  
**Priority:** High  
**Status:** Done  

**Description:**  
Design the core state model for boards, spaces, zones, and tables.

**Dependencies:** MB-005

**Acceptance Criteria:**
- BoardDefinition model exists
- Space or cell model exists
- Zone support is designed
- Model supports square boards initially

---

### MB-007
**Title:** Define piece definition and piece instance model  
**Subsystem:** Domain / Table Engine  
**Priority:** High  
**Status:** Done  

**Description:**  
Create the distinction between a reusable piece definition and a live in-session piece instance.

**Dependencies:** MB-005

**Acceptance Criteria:**
- PieceDefinition model exists
- PieceInstance model exists
- Ownership/controller fields are defined
- Position and state fields are defined

---

### MB-008
**Title:** Define player seat and role model  
**Subsystem:** Domain / Session  
**Priority:** High  
**Status:** Done  

**Description:**  
Create a model for GM, player, observer, side/faction, and seat ownership.

**Dependencies:** MB-005

**Acceptance Criteria:**
- PlayerSeat model exists
- Role/permission concepts are defined
- Supports one GM and multiple players
- Supports open or assigned sides

---

### MB-009
**Title:** Define turn, phase, and sequence model  
**Subsystem:** Domain / Turn Engine  
**Priority:** High  
**Status:** Done  

**Description:**  
Create a generic turn structure that can support board games, tactical games, and RPG initiative systems.

**Dependencies:** MB-005

**Acceptance Criteria:**
- TurnState model exists
- Phase/step concepts are documented
- Sequence supports variable game types
- Initiative can later plug into this model

---

### MB-010
**Title:** Define move history and event log model  
**Subsystem:** Domain / Table Engine  
**Priority:** High  
**Status:** Done  

**Description:**  
Design the event and move log for replay, audit, undo exploration, and multiplayer sync.

**Dependencies:** MB-006, MB-007, MB-009

**Acceptance Criteria:**
- Move/event record model exists
- Basic event categories are defined
- Log is compatible with future sync

---

### MB-011
**Title:** Define save/load session-state model  
**Subsystem:** Domain / Persistence  
**Priority:** High  
**Status:** Done  

**Description:**  
Design how a live table session is persisted and restored.

**Dependencies:** MB-006 through MB-010

**Acceptance Criteria:**
- SessionState model exists
- Save/load boundaries are documented
- Supports board, pieces, players, and turn state

---

## Checkers Reference Implementation

### MB-012
**Title:** Implement checkers board definition  
**Subsystem:** Reference Game / Checkers  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create the first concrete board implementation using an 8x8 checkers board.

**Dependencies:** MB-006

**Acceptance Criteria:**
- 8x8 board is defined
- Starting layout is supported
- Board renders correctly in UI

---

### MB-013
**Title:** Implement checkers piece rendering  
**Subsystem:** UI / Reference Game  
**Priority:** High  
**Status:** Pending  

**Description:**  
Render checkers pieces with side ownership and king state.

**Dependencies:** MB-007, MB-012

**Acceptance Criteria:**
- Pieces render on board
- Sides are visually distinguishable
- King state is visible

---

### MB-014
**Title:** Implement checkers move validation  
**Subsystem:** Rules / Reference Game  
**Priority:** High  
**Status:** Pending  

**Description:**  
Implement legal-move rules for standard checkers.

**Dependencies:** MB-012, MB-013, MB-009

**Acceptance Criteria:**
- Legal moves are validated
- Illegal moves are rejected
- Side-to-move is enforced

---

### MB-015
**Title:** Implement checkers capture and kinging rules  
**Subsystem:** Rules / Reference Game  
**Priority:** High  
**Status:** Pending  

**Description:**  
Implement capture, multi-capture if supported, and promotion to king.

**Dependencies:** MB-014

**Acceptance Criteria:**
- Capture rules work
- Promotion rules work
- State updates correctly after move resolution

---

### MB-016
**Title:** Implement checkers win-state detection  
**Subsystem:** Rules / Reference Game  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Detect when one side has won or no legal moves remain.

**Dependencies:** MB-014, MB-015

**Acceptance Criteria:**
- End-of-game conditions are detected
- Winner is recorded in session state

---

### MB-017
**Title:** Implement checkers save/load support  
**Subsystem:** Reference Game / Persistence  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Persist and restore checkers sessions as the first end-to-end state test.

**Dependencies:** MB-011, MB-012 through MB-016

**Acceptance Criteria:**
- Saved checkers game reloads correctly
- Board, turn, and piece state match prior session

---

## Tactical Tabletop Expansion

### MB-018
**Title:** Add square-grid tactical movement support  
**Subsystem:** Table Engine / Tactical  
**Priority:** High  
**Status:** Pending  

**Description:**  
Extend board support to tactical square-grid movement and measurement.

**Dependencies:** MB-006, MB-007

**Acceptance Criteria:**
- Pieces can occupy square-grid positions
- Movement distance can be computed
- Tactical movement rules can hook into engine

---

### MB-019
**Title:** Add hex-grid board support  
**Subsystem:** Table Engine / Tactical  
**Priority:** High  
**Status:** Pending  

**Description:**  
Support hex coordinates and rendering for war games and tactical systems.

**Dependencies:** MB-006

**Acceptance Criteria:**
- Hex board model exists
- Hex adjacency is defined
- UI can render a simple hex board

---

### MB-020
**Title:** Define terrain and zone model  
**Subsystem:** Domain / Tactical  
**Priority:** High  
**Status:** Pending  

**Description:**  
Support terrain, area effects, zones, and board features.

**Dependencies:** MB-006

**Acceptance Criteria:**
- Terrain concept exists
- Zones can be assigned to map areas
- Terrain can influence rules later

---

### MB-021
**Title:** Define piece attributes for tactical and war-game play  
**Subsystem:** Domain / Tactical  
**Priority:** High  
**Status:** Pending  

**Description:**  
Support movement, side, status, facing, and other piece-level metadata.

**Dependencies:** MB-007

**Acceptance Criteria:**
- Attribute model exists
- Attributes can be extended by rulesets
- Core engine remains generic

---

### MB-022
**Title:** Create tactical scenario setup workflow  
**Subsystem:** UI / Tactical  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Allow setup of a tactical scenario with board, sides, and initial forces.

**Dependencies:** MB-018, MB-019, MB-020, MB-021

**Acceptance Criteria:**
- Scenario can be created
- Initial pieces can be placed
- Sides/factions can be assigned

---

## Content Libraries and Data Import

### MB-023
**Title:** Define content library framework  
**Subsystem:** Content / Domain  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create a generic model for reusable game content across systems.

**Dependencies:** MB-005

**Acceptance Criteria:**
- Content categories are defined
- Shared content principles are documented
- Framework can support RPG and wargame assets

---

### MB-024
**Title:** Define RPG content models  
**Subsystem:** Content / RPG  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define models for monsters, treasure, equipment, and magic items.

**Dependencies:** MB-023

**Acceptance Criteria:**
- Models are documented
- Shared item concepts are normalized where practical
- Ruleset-specific extensions are allowed

---

### MB-025
**Title:** Define unit and counter content models  
**Subsystem:** Content / Wargame  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define reusable content for units, counters, formations, or similar wargame pieces.

**Dependencies:** MB-023

**Acceptance Criteria:**
- Unit/counter model exists
- Side/faction metadata is supported
- Model fits tactical and strategic use

---

### MB-026
**Title:** Create manual GM entry screens  
**Subsystem:** UI / Content  
**Priority:** High  
**Status:** Pending  

**Description:**  
Build data-entry screens for content creation and editing.

**Dependencies:** MB-024

**Acceptance Criteria:**
- Manual entry works for monsters
- Manual entry works for treasure
- Manual entry works for equipment
- Manual entry works for magic items

---

### MB-027
**Title:** Build CSV import framework  
**Subsystem:** Import / Infrastructure  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create a reusable CSV import pipeline with mapping, validation, and result reporting.

**Dependencies:** MB-023

**Acceptance Criteria:**
- CSV upload works
- Mapping pipeline exists
- Validation pipeline exists
- Error report format is defined

---

### MB-028
**Title:** Define CSV specifications for RPG content  
**Subsystem:** Import / Documentation  
**Priority:** High  
**Status:** Pending  

**Description:**  
Write import specifications for monsters, treasure, equipment, and magic items.

**Dependencies:** MB-024, MB-027

**Acceptance Criteria:**
- Required fields are documented
- Optional fields are documented
- Validation rules are listed
- Example files are included

---

### MB-029
**Title:** Define CSV specifications for unit/counter content  
**Subsystem:** Import / Documentation  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Write import specifications for wargame/tactical units or counters if adopted.

**Dependencies:** MB-025, MB-027

**Acceptance Criteria:**
- Unit import format is documented
- Validation rules are documented
- Example files are included

---

### MB-030
**Title:** Design test data packet specification  
**Subsystem:** Testing / Import  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create a standard format for test data packets, including sample CSVs, metadata, expected outcomes, and edge-case cases.

**Dependencies:** MB-027, MB-028

**Acceptance Criteria:**
- Packet structure is defined
- Sample seed packet exists
- Error-case packet exists
- Expected import results are documented

---

## Rules and Mechanics Framework

### MB-031
**Title:** Define rules plugin framework  
**Subsystem:** Rules / Architecture  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create an extensible rules layer so different game systems can attach mechanics to the shared engine.

**Dependencies:** MB-005, MB-006, MB-007, MB-009

**Acceptance Criteria:**
- Rules service abstraction exists
- Game-specific modules can be plugged in
- Engine core stays system-agnostic

---

### MB-032
**Title:** Build dice and randomization services  
**Subsystem:** Rules / Shared  
**Priority:** High  
**Status:** Pending  

**Description:**  
Support common dice mechanics and result reporting.

**Dependencies:** MB-031

**Acceptance Criteria:**
- d4, d6, d8, d10, d12, d20, d100 supported
- Structured roll result is returned
- Rules modules can consume service

---

### MB-033
**Title:** Implement AD&D 1e rules module  
**Subsystem:** Rules / RPG  
**Priority:** High  
**Status:** Pending  

**Description:**  
Implement early AD&D1 mechanics such as attacks, initiative hooks, and saving throws.

**Dependencies:** MB-031, MB-032, MB-024

**Acceptance Criteria:**
- Module is isolated from core engine
- Basic attack resolution is supported
- Saving throw handling is supported
- Initiative hooks are defined

---

### MB-034
**Title:** Implement BRP rules module  
**Subsystem:** Rules / RPG  
**Priority:** High  
**Status:** Pending  

**Description:**  
Implement BRP percentile checks, skill handling, and related mechanics.

**Dependencies:** MB-031, MB-032, MB-024

**Acceptance Criteria:**
- Percentile checks are supported
- Skill resolution works
- Module is isolated from core engine

---

### MB-035
**Title:** Integrate initiative and combat workflow options  
**Subsystem:** Rules / RPG / Turn Engine  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Allow RPG combat workflows to plug into the general turn/sequence system.

**Dependencies:** MB-009, MB-033, MB-034

**Acceptance Criteria:**
- Turn model can support initiative ordering
- Combat rounds can be advanced
- Ruleset-specific initiative logic is supported

---

## Strategic and Wargame Support

### MB-036
**Title:** Define strategic map model  
**Subsystem:** Domain / Wargame  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support maps made of regions, areas, or larger operational spaces in addition to tactical grids.

**Dependencies:** MB-006

**Acceptance Criteria:**
- Area/region map concept exists
- Non-grid movement can be modeled
- Strategic scenarios can reference map regions

---

### MB-037
**Title:** Define multi-turn strategic sequence support  
**Subsystem:** Turn Engine / Wargame  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support turn phases common in war games such as movement, combat, reinforcement, supply, and administration.

**Dependencies:** MB-009, MB-036

**Acceptance Criteria:**
- Multi-phase turn support exists
- Sequence is configurable by game type
- Engine is not hard-coded to RPG rounds

---

### MB-038
**Title:** Define scenario objectives and victory condition framework  
**Subsystem:** Rules / Scenario  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support scenario objectives, scoring, and win conditions for different game styles.

**Dependencies:** MB-031, MB-036, MB-037

**Acceptance Criteria:**
- Objective model exists
- Victory evaluation hooks exist
- Supports both simple and layered objectives

---

## Networking and Live Session Sync

### MB-039
**Title:** Define networking/session hosting model  
**Subsystem:** Networking / Architecture  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Design how live sessions are hosted and joined in a Blazor Server-first architecture.

**Dependencies:** MB-004, MB-008, MB-011

**Acceptance Criteria:**
- Session host model is documented
- Connection assumptions are documented
- GM/player relationship is defined

---

### MB-040
**Title:** Implement real-time session synchronization  
**Subsystem:** Networking  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Synchronize table-state changes across connected participants.

**Dependencies:** MB-010, MB-011, MB-039

**Acceptance Criteria:**
- Shared state updates propagate
- Move/event log syncs correctly
- Board and piece state remain consistent

---

### MB-041
**Title:** Define permissions and visibility model  
**Subsystem:** Networking / Security  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support GM controls, player-limited views, observer access, and future hidden/private information.

**Dependencies:** MB-008, MB-039

**Acceptance Criteria:**
- Role-based visibility rules are documented
- GM authority is defined
- Future private information model is anticipated

---

### MB-042
**Title:** Implement reconnect and session recovery behavior  
**Subsystem:** Networking / Reliability  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support client reconnects and restoration of live session state.

**Dependencies:** MB-040

**Acceptance Criteria:**
- Reconnected client receives current state
- Session continuity is preserved
- Failure cases are logged

---

## UI and Presentation

### MB-043
**Title:** Define UI shell layout  
**Subsystem:** UI  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create the baseline application shell with navigation, top bar, workspace, and side panels.

**Dependencies:** MB-001

**Acceptance Criteria:**
- Shell layout is documented
- Basic navigation is implemented
- Main workspace is reserved for table/board views

---

### MB-044
**Title:** Define board presentation patterns  
**Subsystem:** UI / Table Presentation  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define how boards, grids, zones, pieces, overlays, and selection states are displayed.

**Dependencies:** MB-006, MB-007, MB-043

**Acceptance Criteria:**
- Board rendering patterns are documented
- Selection state is defined
- Piece display rules are defined

---

### MB-045
**Title:** Define content-management screen inventory  
**Subsystem:** UI / Content  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Document the GM screens needed for content entry, edit, list, import, and review.

**Dependencies:** MB-026

**Acceptance Criteria:**
- Screen inventory exists
- Workflow order is documented
- CRUD and import screens are identified

---

## Persistence and Infrastructure

### MB-046
**Title:** Select and define persistence strategy  
**Subsystem:** Infrastructure / Persistence  
**Priority:** High  
**Status:** Pending  

**Description:**  
Choose and document the database and persistence approach for content and live session state.

**Dependencies:** MB-004, MB-011

**Acceptance Criteria:**
- Persistence strategy is documented
- Content persistence is defined
- Session-state persistence direction is defined

---

### MB-047
**Title:** Implement initial repository/service boundaries  
**Subsystem:** Infrastructure  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define clean boundaries between UI, application services, domain logic, and data access.

**Dependencies:** MB-046

**Acceptance Criteria:**
- Service boundaries are documented
- Repository abstractions exist where needed
- Core domain does not depend directly on UI

---

### MB-048
**Title:** Implement logging and error-handling standards  
**Subsystem:** Core / Infrastructure  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define a consistent approach to errors, diagnostics, and import failure reporting.

**Dependencies:** MB-027, MB-046

**Acceptance Criteria:**
- Logging strategy is documented
- Import errors are user-visible
- Unexpected failures are traceable

---

## Testing and Quality

### MB-049
**Title:** Create engine test strategy  
**Subsystem:** Testing  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define automated and manual testing strategy for engine behavior across board, tactical, RPG, and networked use.

**Dependencies:** MB-006 through MB-011

**Acceptance Criteria:**
- Test categories are defined
- Unit vs integration boundaries are documented
- Regression approach is documented

---

### MB-050
**Title:** Create checkers regression test suite  
**Subsystem:** Testing / Reference Game  
**Priority:** High  
**Status:** Pending  

**Description:**  
Use checkers as the first stable regression pack for turn logic, move validation, and state transitions.

**Dependencies:** MB-012 through MB-017

**Acceptance Criteria:**
- Core checkers rule cases are covered
- Illegal and legal move tests exist
- End-state tests exist

---

### MB-051
**Title:** Create tactical movement test cases  
**Subsystem:** Testing / Tactical  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Build scenario-based tests for tactical grid and hex movement.

**Dependencies:** MB-018, MB-019, MB-020, MB-021

**Acceptance Criteria:**
- Square-grid cases exist
- Hex-grid cases exist
- Terrain interactions can be tested later

---

### MB-052
**Title:** Create RPG rules test cases  
**Subsystem:** Testing / RPG  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Build system-specific tests for AD&D1 and BRP mechanics.

**Dependencies:** MB-033, MB-034, MB-035

**Acceptance Criteria:**
- AD&D1 test cases exist
- BRP test cases exist
- Rules modules can be tested independently

---

### MB-053
**Title:** Create networking synchronization test cases  
**Subsystem:** Testing / Networking  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Verify synchronized state behavior, reconnect handling, and update ordering.

**Dependencies:** MB-040, MB-042

**Acceptance Criteria:**
- Sync consistency tests exist
- Reconnect tests exist
- Multi-client update tests exist

---

## Documentation and Project Control

### MB-054
**Title:** Create subsystem backlog set  
**Subsystem:** Documentation / Planning  
**Priority:** High  
**Status:** Pending  

**Description:**  
Split the master backlog into subsystem-specific backlog documents.

**Dependencies:** MB-003

**Acceptance Criteria:**
- Subsystem backlog files exist
- Items are assigned by subsystem
- Priorities remain traceable to master backlog

---

### MB-055
**Title:** Create roadmap and milestone tracking doc  
**Subsystem:** Documentation / Planning  
**Priority:** High  
**Status:** Pending  

**Description:**  
Maintain the roadmap and milestone tracking as implementation proceeds.

**Dependencies:** MB-002, MB-003

**Acceptance Criteria:**
- Roadmap exists
- Milestones are defined
- Major phase transitions are traceable

---

### MB-056
**Title:** Create developer onboarding and repo conventions doc  
**Subsystem:** Documentation  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Document project structure, coding expectations, naming conventions, and how to add new modules.

**Dependencies:** MB-001, MB-047

**Acceptance Criteria:**
- Repo conventions are documented
- New contributors can locate major project areas
- Module-addition workflow is described

---

## Near-Term Execution Focus

The recommended first execution band is:

1. MB-001 Establish solution structure  
2. MB-002 Define product vision and scope  
3. MB-003 Define subsystem map  
4. MB-004 Record initial architecture decision  
5. MB-005 Define core domain glossary  
6. MB-006 Define board and table-state model  
7. MB-007 Define piece definition and piece instance model  
8. MB-009 Define turn, phase, and sequence model  
9. MB-012 Implement checkers board definition  
10. MB-013 Implement checkers piece rendering  
11. MB-014 Implement checkers move validation  
12. MB-015 Implement checkers capture and kinging rules  

This sequence proves the engine before content-heavy RPG workflows begin.

