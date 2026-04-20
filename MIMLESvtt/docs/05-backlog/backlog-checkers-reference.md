# Backlog — Checkers Reference Game

## Scope

This backlog defines the first complete proof-of-engine implementation using checkers.

Its purpose is to validate:

- board representation
- piece representation
- turns
- legal move handling
- capture handling
- promotion
- win-state detection
- save/load
- regression testing

---

## CHK-001
**Title:** Define checkers scenario setup  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define the initial checkers setup including board size, piece count, sides, and starting placement.

**Dependencies:** TE-001, TE-006

**Acceptance Criteria:**
- Standard 8x8 setup is documented
- Piece ownership is defined
- Starting positions are reproducible

---

## CHK-002
**Title:** Create checkers piece definition  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define standard and king piece states for checkers.

**Dependencies:** TE-005, TE-007

**Acceptance Criteria:**
- Standard piece is defined
- King state is represented
- Visual distinction is anticipated

---

## CHK-003
**Title:** Render checkers board  
**Priority:** High  
**Status:** Pending  

**Description:**  
Render the checkers board using the table engine UI.

**Dependencies:** CHK-001, TE-012

**Acceptance Criteria:**
- 8x8 board renders correctly
- Dark/light square distinction is visible
- Playable area is clear

---

## CHK-004
**Title:** Render checkers pieces  
**Priority:** High  
**Status:** Pending  

**Description:**  
Render checkers pieces in starting positions.

**Dependencies:** CHK-002, CHK-003, TE-013

**Acceptance Criteria:**
- Both sides render correctly
- Pieces appear in valid starting locations
- King state can later display distinctly

---

## CHK-005
**Title:** Implement side-to-move tracking  
**Priority:** High  
**Status:** Pending  

**Description:**  
Track which side may act.

**Dependencies:** TE-009, CHK-001

**Acceptance Criteria:**
- Side-to-move is visible in state
- Illegal same-side repeated moves are blocked

---

## CHK-006
**Title:** Implement normal move validation  
**Priority:** High  
**Status:** Pending  

**Description:**  
Validate legal non-capturing moves for standard pieces.

**Dependencies:** CHK-004, CHK-005, TE-014

**Acceptance Criteria:**
- Legal diagonal forward moves are allowed
- Illegal moves are rejected
- Move validation respects current side

---

## CHK-007
**Title:** Implement capture validation  
**Priority:** High  
**Status:** Pending  

**Description:**  
Validate legal captures.

**Dependencies:** CHK-006

**Acceptance Criteria:**
- Jump captures are recognized
- Invalid captures are rejected
- Captured piece identity is known

---

## CHK-008
**Title:** Implement capture resolution  
**Priority:** High  
**Status:** Pending  

**Description:**  
Apply capture results to board state.

**Dependencies:** CHK-007, TE-015

**Acceptance Criteria:**
- Capturing piece lands correctly
- Captured piece is removed
- Event log reflects capture

---

## CHK-009
**Title:** Implement king promotion  
**Priority:** High  
**Status:** Pending  

**Description:**  
Promote pieces reaching the far row.

**Dependencies:** CHK-006, CHK-008

**Acceptance Criteria:**
- Promotion occurs at correct row
- Piece state changes to king
- Rendering reflects promoted state

---

## CHK-010
**Title:** Implement king movement rules  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Allow kings to move according to chosen checkers variant rules.

**Dependencies:** CHK-009

**Acceptance Criteria:**
- King movement is validated
- Rule assumptions are documented
- Standard and king movement are distinct

---

## CHK-011
**Title:** Implement optional multi-capture support  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Support chained captures if included in the selected rules variant.

**Dependencies:** CHK-008, CHK-010

**Acceptance Criteria:**
- Multiple capture sequence can be represented
- Rule decision is documented
- State updates remain correct

---

## CHK-012
**Title:** Implement end-state detection  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Detect win/loss or no-legal-move conditions.

**Dependencies:** CHK-006 through CHK-010

**Acceptance Criteria:**
- No-piece condition is detected
- No-legal-move condition is detected
- Winner is recorded in session state

---

## CHK-013
**Title:** Implement save/load for checkers session  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Persist and restore a checkers game.

**Dependencies:** TE-016, CHK-012

**Acceptance Criteria:**
- Saved game restores correctly
- Board and turn state match prior session
- Piece states restore correctly

---

## CHK-014
**Title:** Create checkers regression test set  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create repeatable regression tests for legal moves, captures, promotion, and end-state detection.

**Dependencies:** CHK-006 through CHK-013

**Acceptance Criteria:**
- Legal move cases exist
- Illegal move cases exist
- Capture cases exist
- Promotion and win-state cases exist