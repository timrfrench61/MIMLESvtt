# Backlog — Table Engine

## Purpose

This backlog covers the generic tabletop foundation:

- boards
- spaces/cells
- zones
- pieces
- piece instances
- turn/phase sequencing
- move history
- session state

This subsystem must remain system-agnostic and support:

- board games
- tactical skirmish games
- strategic war games
- RPG table play

Related master backlog items:
- MB-005 through MB-011
- MB-018 through MB-021
- MB-036 through MB-037

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

## Items

### TE-001
**Title:** Define core domain glossary  
**Master Ref:** MB-005  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create the standard terminology for the engine.

**Acceptance Criteria:**
- Glossary doc exists
- Board, map, zone, space, piece, token, counter, seat, turn, phase, scenario, and session are defined
- Terms are broad enough for RPG, board game, and wargame use

---

### TE-002
**Title:** Define board definition model  
**Master Ref:** MB-006  
**Priority:** High  
**Status:** Pending  

**Description:**  
Design the reusable model describing a board or play surface.

**Acceptance Criteria:**
- BoardDefinition exists
- Supports dimensions and board type
- Supports square-grid first
- Can later support hex, region, or freeform layouts

---

### TE-003
**Title:** Define space/cell model  
**Master Ref:** MB-006  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define the smallest board-addressable play unit for square and later hex boards.

**Acceptance Criteria:**
- Space model exists
- Position identity is unique
- Occupancy support is considered
- Adjacency model can be added

---

### TE-004
**Title:** Define zone model  
**Master Ref:** MB-006, MB-020  
**Priority:** High  
**Status:** Pending  

**Description:**  
Support logical areas on a board such as rooms, regions, terrain sectors, deployment zones, or private areas.

**Acceptance Criteria:**
- Zone model exists
- Zone can reference multiple spaces
- Zone can hold metadata
- Zones can be optional

---

### TE-005
**Title:** Define piece definition model  
**Master Ref:** MB-007  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create the reusable piece template for counters, units, monsters, characters, and tokens.

**Acceptance Criteria:**
- PieceDefinition exists
- Shared presentation metadata is supported
- Piece category/type is supported
- Ruleset-specific extensions are anticipated

---

### TE-006
**Title:** Define piece instance model  
**Master Ref:** MB-007  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create the live in-session instance of a piece, separate from the reusable definition.

**Acceptance Criteria:**
- PieceInstance exists
- Position is tracked
- Owner/controller is supported
- Runtime state is supported

---

### TE-007
**Title:** Define player seat model  
**Master Ref:** MB-008  
**Priority:** High  
**Status:** Pending  

**Description:**  
Model the seat or role a participant occupies during a session.

**Acceptance Criteria:**
- PlayerSeat exists
- GM and player roles are supported
- Side/faction assignment is supported
- Observer role is possible

---

### TE-008
**Title:** Define turn state model  
**Master Ref:** MB-009  
**Priority:** High  
**Status:** Pending  

**Description:**  
Design the generic turn container for all game types.

**Acceptance Criteria:**
- TurnState exists
- Current side/seat can be tracked
- Phase/step is supported
- Round/turn counters can be stored

---

### TE-009
**Title:** Define phase and sequence model  
**Master Ref:** MB-009, MB-037  
**Priority:** High  
**Status:** Pending  

**Description:**  
Support configurable phases for games that do not use identical round structures.

**Acceptance Criteria:**
- Phase model exists
- Sequence is configurable
- Works for board turns, RPG initiative rounds, and wargame phases

---

### TE-010
**Title:** Define move and event log model  
**Master Ref:** MB-010  
**Priority:** High  
**Status:** Pending  

**Description:**  
Record state changes in a structured way for audit, replay, and networking.

**Acceptance Criteria:**
- Event record format exists
- Move events are distinct from admin/system events
- Events can reference pieces, zones, turns, and actors

---

### TE-011
**Title:** Define session state aggregate  
**Master Ref:** MB-011  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create the root aggregate for a live tabletop session.

**Acceptance Criteria:**
- SessionState exists
- Board, pieces, seats, and turn state are included
- Designed for save/load and sync

---

### TE-012
**Title:** Add square-grid coordinate helpers  
**Master Ref:** MB-018  
**Priority:** High  
**Status:** Pending  

**Description:**  
Implement coordinate helpers for square-grid indexing, adjacency, and range basics.

**Acceptance Criteria:**
- Coordinate helper API exists
- Adjacency can be computed
- Distance basics are supported

---

### TE-013
**Title:** Add hex-grid coordinate model  
**Master Ref:** MB-019  
**Priority:** High  
**Status:** Pending  

**Description:**  
Extend board coordinates to support hex play.

**Acceptance Criteria:**
- Hex coordinate type exists
- Neighbor relationships are defined
- Hex board storage approach is documented

---

### TE-014
**Title:** Add terrain metadata support  
**Master Ref:** MB-020  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support terrain properties at space or zone level.

**Acceptance Criteria:**
- Terrain metadata can be attached
- Rules engine can read terrain data later
- Core model stays generic

---

### TE-015
**Title:** Add tactical piece attribute bag  
**Master Ref:** MB-021  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support movement, facing, status, and tactical tags without hard-coding one ruleset.

**Acceptance Criteria:**
- Attribute extension model exists
- Supports movement and side tags
- Supports future facing/stacking tags

---

### TE-016
**Title:** Define strategic map abstraction  
**Master Ref:** MB-036  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Extend the table engine to support region/area maps in addition to grid maps.

**Acceptance Criteria:**
- Strategic map abstraction exists
- Region/area adjacency can be represented
- Model does not assume cells only

---

### TE-017
**Title:** Implement session save/load contract  
**Master Ref:** MB-011  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define the serialization boundary for session state.

**Acceptance Criteria:**
- Save/load contract exists
- Core session aggregate can be serialized
- Versioning concerns are documented