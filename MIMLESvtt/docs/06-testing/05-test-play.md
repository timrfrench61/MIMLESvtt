# Test — Play

## Purpose

Define gameplay-path QA checks that validate interactive play behavior across setup, action flow, feedback, and end-state handling.

## Scope

Included:
- baseline play loop (setup -> action -> feedback -> continuation)
- invalid-action rejection without state corruption
- turn/phase and objective outcome visibility where available

Not included:
- deep subsystem-specific rule correctness catalogs (see dedicated rule/module test docs)

## Preconditions

- Session created and at least one playable surface/piece context available.
- Relevant feature toggles recorded for run.

## Test Cases

### PLAY-001
**Title:** Baseline play loop executes without dead ends  
**Priority:** High

**Steps:**
1. Create/load session.
2. Perform setup actions.
3. Execute a legal play action.

**Expected:**
- Action succeeds.
- State updates are visible.
- User can continue next action path.

### PLAY-002
**Title:** Invalid action is rejected without mutation  
**Priority:** High

**Steps:**
1. Trigger known invalid action.

**Expected:**
- Visible error feedback appears.
- No unintended state mutation occurs.

### PLAY-003
**Title:** Success and error feedback are both user-visible  
**Priority:** High

**Steps:**
1. Perform one valid and one invalid action.

**Expected:**
- Success path shows visible positive feedback.
- Failure path shows actionable error feedback.

### PLAY-004
**Title:** Turn/phase progression remains coherent where enabled  
**Priority:** Medium

**Steps:**
1. Execute actions that should advance turn/phase state.

**Expected:**
- Progression state updates coherently.
- No contradictory turn indicators appear.

### PLAY-005
**Title:** Save/load during play preserves actionable state  
**Priority:** Medium

**Steps:**
1. Perform several actions.
2. Save session.
3. Reload session.

**Expected:**
- Core play state is preserved.
- Continued play from restored state is possible.

## Cross References

- `21-checkers-test-cases.md`
- `97-testing-strategy.md`
- `98-pre-alpha-testing-packet.md`
- `96-qa-run-log.md`