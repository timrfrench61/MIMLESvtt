# Backlog — Testing and QA

## Purpose

This backlog covers the testing and quality strategy for the Blazor Tabletop Engine / VTT project.

The goal is to ensure the platform is stable across:

- board-game style play
- tactical tabletop play
- strategic war-game play
- RPG rules modules
- future live/networked sessions

Testing must validate both the **generic engine** and **game-specific modules** without tightly coupling one to the other.

---

## Scope

Included in this subsystem:

- engine test strategy
- regression testing
- reference game test coverage
- tactical movement test coverage
- RPG mechanics test coverage
- networking sync test coverage
- import/data validation test coverage
- manual QA workflows
- future automation planning

Not included:

- core feature implementation
- production logging standards
- backlog governance outside testing artifacts

---

## Dependencies

Primary dependencies:

- `90-master-backlog-reference.md`
- `backlog-table-engine.md`
- `20-backlog-checkers-reference.md`
- `backlog-rules-framework.md`
- `09-backlog-content-import.md`
- `08-backlog-networking.md`
- `09-backlog-persistence-infrastructure.md`

Testing work depends on stable or semi-stable designs from those subsystems.

---

## Traceability to Master Backlog

This subsystem backlog primarily expands:

- MB-030 Design test data packet specification
- MB-049 Create engine test strategy
- MB-050 Create checkers regression test suite
- MB-051 Create tactical movement test cases
- MB-052 Create RPG rules test cases
- MB-053 Create networking synchronization test cases

It also supports quality verification for many other master items.

---

## Backlog Items

---

### TQA-001
**Master ID:** MB-049  
**Title:** Create engine test strategy  
**Priority:** High  
**Status:** Done  

**Description:**  
Define the overall testing approach for the platform, including unit, integration, regression, and manual QA layers.

**Dependencies:** None

**Acceptance Criteria:**
- Test strategy document exists
- Test categories are defined
- Unit vs integration boundaries are documented
- Engine-first testing principles are stated

---

### TQA-002
**Master ID:** MB-049  
**Title:** Define subsystem test boundaries  
**Priority:** High  
**Status:** Done  

**Description:**  
Specify what each subsystem is responsible for testing and where shared integration tests should live.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Test ownership is documented per subsystem
- Engine tests are separated from rules-module tests
- UI tests are separated from domain logic tests
- Cross-subsystem integration boundaries are defined

---

### TQA-003
**Master ID:** MB-049  
**Title:** Define regression testing policy  
**Priority:** High  
**Status:** Pending  

**Description:**  
Establish how stable features become part of the regression suite and how regressions are tracked.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Regression criteria are documented
- Core engine regression targets are listed
- Reference-game regression policy is defined
- Change-risk categories are identified

---

### TQA-004
**Master ID:** MB-050  
**Title:** Create checkers regression test inventory  
**Priority:** High  
**Status:** Pending  

**Description:**  
List the rule and state cases required for full regression coverage of the checkers reference implementation.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Test inventory exists
- Legal move cases are listed
- Illegal move cases are listed
- Capture, kinging, and win-state cases are listed

---

### TQA-005
**Master ID:** MB-050  
**Title:** Implement checkers legal-move tests  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create tests that verify ordinary legal moves for both sides.

**Dependencies:** TQA-004

**Acceptance Criteria:**
- Normal forward movement is tested
- Side-to-move enforcement is tested
- Board boundaries are respected
- Passing through occupied spaces is rejected

---

### TQA-006
**Master ID:** MB-050  
**Title:** Implement checkers illegal-move tests  
**Priority:** High  
**Status:** Pending  

**Description:**  
Verify invalid move attempts are rejected correctly.

**Dependencies:** TQA-004

**Acceptance Criteria:**
- Backward non-king movement is rejected
- Move to occupied square is rejected
- Out-of-turn moves are rejected
- Off-board moves are rejected

---

### TQA-007
**Master ID:** MB-050  
**Title:** Implement checkers capture and kinging tests  
**Priority:** High  
**Status:** Pending  

**Description:**  
Verify capture behavior, piece removal, and promotion logic.

**Dependencies:** TQA-004

**Acceptance Criteria:**
- Single capture is tested
- Multi-capture path behavior is tested if supported
- Promotion to king is tested
- King movement after promotion is tested

---

### TQA-008
**Master ID:** MB-050  
**Title:** Implement checkers end-state tests  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Verify win detection and no-legal-move scenarios.

**Dependencies:** TQA-004

**Acceptance Criteria:**
- No-pieces-left condition is tested
- No-legal-moves condition is tested
- Winner is recorded correctly
- Session state marks game complete

---

### TQA-009
**Master ID:** MB-051  
**Title:** Create tactical movement test inventory  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define test cases for square-grid and hex-grid movement, adjacency, and terrain interactions.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Square-grid cases are listed
- Hex-grid cases are listed
- Adjacency cases are listed
- Terrain-interaction placeholders are listed

---

### TQA-010
**Master ID:** MB-051  
**Title:** Implement square-grid tactical movement tests  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Test basic movement and range behavior for square-grid tactical scenarios.

**Dependencies:** TQA-009

**Acceptance Criteria:**
- Orthogonal movement cases exist
- Diagonal movement policy is tested if supported
- Distance calculations are verified
- Occupancy constraints are tested

---

### TQA-011
**Master ID:** MB-051  
**Title:** Implement hex-grid tactical movement tests  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Test adjacency, movement range, and coordinate handling for hex maps.

**Dependencies:** TQA-009

**Acceptance Criteria:**
- Hex adjacency is tested
- Movement step counts are tested
- Coordinate translation is verified
- Edge/corner map cases are covered

---

### TQA-012
**Master ID:** MB-052  
**Title:** Create RPG rules test inventory  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define the first pass of rules-module tests for AD&D1 and BRP.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- AD&D1 test categories are listed
- BRP test categories are listed
- Shared dice-service dependencies are identified
- System-specific vs shared tests are separated

---

### TQA-013
**Master ID:** MB-052  
**Title:** Implement AD&D1 rules tests  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Add tests for initial AD&D1 attack, saving throw, and initiative behavior.

**Dependencies:** TQA-012

**Acceptance Criteria:**
- Basic attack-resolution test cases exist
- Saving throw cases exist
- Initiative hook behavior is tested
- Rules module can be tested in isolation

---

### TQA-014
**Master ID:** MB-052  
**Title:** Implement BRP rules tests  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Add tests for BRP percentile checks, skill checks, and related rule behavior.

**Dependencies:** TQA-012

**Acceptance Criteria:**
- Percentile roll cases exist
- Pass/fail threshold behavior is tested
- Skill resolution cases exist
- Rules module can be tested in isolation

---

### TQA-015
**Master ID:** MB-053  
**Title:** Create networking sync test inventory  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define the synchronization test set for shared board state, shared events, reconnects, and update ordering.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Multi-client test categories are listed
- Reconnect cases are listed
- Ordering/conflict cases are listed
- Session recovery cases are listed

---

### TQA-016
**Master ID:** MB-053  
**Title:** Implement shared-state synchronization tests  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Verify that board and piece state remain consistent across clients.

**Dependencies:** TQA-015

**Acceptance Criteria:**
- Piece movement propagates correctly
- Turn-state updates propagate correctly
- Event log consistency is verified
- Desync conditions are detectable

---

### TQA-017
**Master ID:** MB-053  
**Title:** Implement reconnect and recovery tests  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Verify that reconnecting clients regain accurate current session state.

**Dependencies:** TQA-015

**Acceptance Criteria:**
- Reconnected client receives board state
- Reconnected client receives turn state
- Reconnect after mid-turn change is covered
- Recovery behavior is documented

---

### TQA-018
**Master ID:** MB-030  
**Title:** Create import validation test inventory  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define the validation cases for CSV import, including good data, bad data, missing fields, and malformed data.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Happy-path import cases are listed
- Missing required field cases are listed
- Type mismatch cases are listed
- Duplicate/conflict cases are listed

---

### TQA-019
**Master ID:** MB-030  
**Title:** Build sample test data packet library  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create reusable import test packets for automated and manual testing.

**Dependencies:** TQA-018

**Acceptance Criteria:**
- At least one valid packet exists
- At least one invalid/error packet exists
- Expected outcomes are documented
- Packets are versionable and reusable

---

### TQA-020
**Master ID:** MB-049  
**Title:** Define manual QA workflow checklist  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Create a simple manual test checklist for milestone reviews.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Checklist exists
- Covers engine, UI, import, and persistence basics
- Can be run by a developer or tester
- Pass/fail notes can be recorded

---

### TQA-021
**Master ID:** MB-049  
**Title:** Define test fixture and naming conventions  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Standardize test names, fixture structure, and seed/state naming so the suite remains understandable over time.

**Dependencies:** TQA-001

**Acceptance Criteria:**
- Naming conventions are documented
- Fixture structure is documented
- Scenario naming rules are documented
- Conventions work across multiple game styles

---

### TQA-022
**Master ID:** MB-049  
**Title:** Define CI-ready test execution grouping  
**Priority:** Low  
**Status:** Pending  

**Description:**  
Prepare test grouping conventions for future automated build/test execution.

**Dependencies:** TQA-002, TQA-021

**Acceptance Criteria:**
- Fast vs slow test grouping is defined
- Unit vs integration grouping is defined
- Reference-game regression grouping is defined
- Network-sensitive tests are identified

---

## Near-Term Execution Order

Recommended first execution band:

1. TQA-001 Create engine test strategy  
2. TQA-002 Define subsystem test boundaries  
3. TQA-003 Define regression testing policy  
4. TQA-004 Create checkers regression test inventory  
5. TQA-018 Create import validation test inventory  
6. TQA-019 Build sample test data packet library  
7. TQA-020 Define manual QA workflow checklist  
8. TQA-021 Define test fixture and naming conventions  

After that, implement the test suites in this order:

1. checkers regression tests  
2. import validation tests  
3. tactical movement tests  
4. RPG rules tests  
5. networking synchronization tests

---

## Notes

- Checkers is the first stable reference game and should become the first durable regression baseline.
- Test data packets should be reusable across both automated and manual QA.
- Networking tests should come after the underlying state/event model is stable enough to avoid churn.