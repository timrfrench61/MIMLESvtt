# Checkers Test Cases

## Purpose

Define the initial test cases for the Checkers Rules Module.

These tests serve as the first regression baseline for the Tabletop Engine and verify that:

- board state changes correctly
- move validation works
- turn order works
- captures work
- promotion works
- win detection works

This document is intentionally focused on engine and rules behavior, not UI appearance.

---

## Scope

Included:

- legal move cases
- illegal move cases
- capture cases
- promotion cases
- turn progression
- end-state detection
- persistence sanity cases

Not included:

- animation
- AI behavior
- network transport behavior
- advanced draw rules
- optional rule variants unless added later

---

## Test Data Assumptions

Unless otherwise stated:

- board is 8x8
- two sides exist: Red and Black
- Red moves first
- normal pieces move forward only
- kings move forward and backward
- destination square must be empty
- first implementation may allow non-capture moves even when a capture exists

---

## Test Categories

1. Board setup
2. Legal move validation
3. Illegal move rejection
4. Capture resolution
5. Promotion
6. Turn switching
7. Win detection
8. Save/load integrity

---

## Board Setup Tests

### CHK-001
**Title:** Standard opening setup creates correct piece count  
**Priority:** High  

**Preconditions:**
- new standard checkers session created

**Expected Results:**
- Red has 12 pieces
- Black has 12 pieces
- total pieces = 24
- all pieces are on valid dark squares only
- no pieces overlap

---

### CHK-002
**Title:** Standard opening setup starts with Red to move  
**Priority:** High  

**Preconditions:**
- new standard checkers session created

**Expected Results:**
- TurnState sequence type = AlternatingTurn
- CurrentSideId = Red
- session is not ended

---

## Legal Move Validation Tests

### CHK-003
**Title:** Red piece may make one-step forward diagonal move  
**Priority:** High  

**Preconditions:**
- Red piece at valid starting square
- target diagonal square is empty
- Red to move

**Action:**
- MovePiece from starting square to forward diagonal square

**Expected Results:**
- move is accepted
- piece appears at destination
- origin becomes empty

---

### CHK-004
**Title:** Black piece may make one-step forward diagonal move on its turn  
**Priority:** High  

**Preconditions:**
- Black piece positioned for legal move
- Black to move
- destination empty

**Action:**
- MovePiece to legal forward diagonal square

**Expected Results:**
- move is accepted
- piece moves correctly
- origin becomes empty

---

### CHK-005
**Title:** King may move one step diagonally backward  
**Priority:** High  

**Preconditions:**
- king piece exists
- correct side to move
- destination empty

**Action:**
- MovePiece backward diagonally one square

**Expected Results:**
- move is accepted
- king moves correctly

---

## Illegal Move Rejection Tests

### CHK-006
**Title:** Piece cannot move when it is not that side's turn  
**Priority:** High  

**Preconditions:**
- Red to move
- Black piece has legal-looking move available

**Action:**
- attempt to move Black piece

**Expected Results:**
- move is rejected
- board state is unchanged
- turn state is unchanged

---

### CHK-007
**Title:** Non-king piece cannot move backward  
**Priority:** High  

**Preconditions:**
- non-king piece positioned with backward diagonal square open
- correct side to move

**Action:**
- attempt backward move

**Expected Results:**
- move is rejected
- piece remains in original square

---

### CHK-008
**Title:** Piece cannot move to occupied destination  
**Priority:** High  

**Preconditions:**
- legal diagonal path exists
- destination square already contains any piece

**Action:**
- attempt move into occupied square

**Expected Results:**
- move is rejected
- board state unchanged

---

### CHK-009
**Title:** Piece cannot move straight forward  
**Priority:** High  

**Preconditions:**
- piece has open square directly ahead
- correct side to move

**Action:**
- attempt non-diagonal move

**Expected Results:**
- move is rejected

---

### CHK-010
**Title:** Piece cannot move more than allowed distance without capture rule support  
**Priority:** High  

**Preconditions:**
- open path exists
- correct side to move

**Action:**
- attempt move more than two diagonal squares

**Expected Results:**
- move is rejected

---

### CHK-011
**Title:** Piece cannot move off board  
**Priority:** High  

**Preconditions:**
- piece adjacent to board edge
- correct side to move

**Action:**
- attempt move beyond board boundary

**Expected Results:**
- move is rejected

---

## Capture Resolution Tests

### CHK-012
**Title:** Piece may capture opposing piece by jumping diagonally  
**Priority:** High  

**Preconditions:**
- moving piece positioned two diagonal squares away from empty landing square
- opposing piece occupies middle diagonal square
- correct side to move

**Action:**
- MovePiece with capture jump

**Expected Results:**
- move is accepted
- moving piece lands in destination square
- opposing piece is removed from play
- origin square becomes empty

---

### CHK-013
**Title:** Piece cannot capture if middle square is empty  
**Priority:** High  

**Preconditions:**
- destination two diagonal squares away is empty
- no piece in middle square
- correct side to move

**Action:**
- attempt capture jump

**Expected Results:**
- move is rejected

---

### CHK-014
**Title:** Piece cannot capture own side's piece  
**Priority:** High  

**Preconditions:**
- same-side piece occupies middle square
- landing square empty
- correct side to move

**Action:**
- attempt capture jump

**Expected Results:**
- move is rejected
- no pieces removed

---

### CHK-015
**Title:** Capture removes exactly one opposing piece in simple jump  
**Priority:** High  

**Preconditions:**
- valid simple capture setup
- correct side to move

**Action:**
- perform capture

**Expected Results:**
- one opponent piece removed
- no additional pieces removed
- moving piece remains in destination square

---

## Promotion Tests

### CHK-016
**Title:** Red piece promotes to king on reaching final rank  
**Priority:** High  

**Preconditions:**
- Red non-king piece is one legal move from promotion row
- correct side to move
- destination empty

**Action:**
- move piece into promotion row

**Expected Results:**
- move is accepted
- piece state IsKing = true

---

### CHK-017
**Title:** Black piece promotes to king on reaching final rank  
**Priority:** High  

**Preconditions:**
- Black non-king piece is one legal move from promotion row
- correct side to move
- destination empty

**Action:**
- move piece into promotion row

**Expected Results:**
- move is accepted
- piece state IsKing = true

---

### CHK-018
**Title:** Piece not on final rank does not promote  
**Priority:** Medium  

**Preconditions:**
- non-king piece makes ordinary legal move not ending on promotion row

**Action:**
- perform move

**Expected Results:**
- IsKing remains false

---

## Turn Switching Tests

### CHK-019
**Title:** Turn switches after legal ordinary move  
**Priority:** High  

**Preconditions:**
- Red to move
- legal Red move available

**Action:**
- Red performs legal move

**Expected Results:**
- CurrentSideId becomes Black
- turn counter advances if implemented

---

### CHK-020
**Title:** Turn switches after legal capture  
**Priority:** High  

**Preconditions:**
- legal capture available
- correct side to move

**Action:**
- perform capture

**Expected Results:**
- capture resolves
- CurrentSideId switches to opponent
- turn counter advances if implemented

---

### CHK-021
**Title:** Turn does not switch after rejected move  
**Priority:** High  

**Preconditions:**
- current side attempts illegal move

**Action:**
- perform illegal move attempt

**Expected Results:**
- move rejected
- CurrentSideId unchanged
- board unchanged

---

## Win Detection Tests

### CHK-022
**Title:** Game ends when one side has no pieces remaining  
**Priority:** High  

**Preconditions:**
- session reduced to one final capturable piece for losing side
- correct side to move

**Action:**
- perform final capture

**Expected Results:**
- HasEnded = true
- winning side recorded
- losing side has zero active pieces

---

### CHK-023
**Title:** Game ends when side to move has no legal moves  
**Priority:** Medium  

**Preconditions:**
- one side has at least one piece
- none of its pieces have legal moves
- turn belongs to that side

**Action:**
- evaluate game state / advance as required by implementation

**Expected Results:**
- HasEnded = true
- opposing side recorded as winner

---

## Persistence Sanity Tests

### CHK-024
**Title:** Saved session restores board positions exactly  
**Priority:** High  

**Preconditions:**
- session contains non-opening arrangement
- session saved

**Action:**
- load saved session

**Expected Results:**
- all piece positions match saved state
- removed pieces remain removed
- no extra pieces appear

---

### CHK-025
**Title:** Saved session restores current side to move  
**Priority:** High  

**Preconditions:**
- session saved mid-game

**Action:**
- load saved session

**Expected Results:**
- CurrentSideId matches saved value
- turn state is valid

---

### CHK-026
**Title:** Saved session restores king status correctly  
**Priority:** Medium  

**Preconditions:**
- one or more kings exist
- session saved

**Action:**
- load saved session

**Expected Results:**
- promoted pieces remain kings
- non-kings remain non-kings

---

## Optional Future Tests

These may be added when the corresponding rules exist.

### CHK-F01
**Title:** Mandatory capture prevents ordinary move  
**Priority:** Low  

### CHK-F02
**Title:** Multi-capture chain continues before turn ends  
**Priority:** Low  

### CHK-F03
**Title:** Draw condition detected after repeated non-progress states  
**Priority:** Low  

### CHK-F04
**Title:** Replay log reconstructs checkers game correctly  
**Priority:** Low  

---

## Recommended Initial Automated Regression Set

Start by automating these first:

- CHK-001
- CHK-002
- CHK-003
- CHK-006
- CHK-007
- CHK-008
- CHK-012
- CHK-015
- CHK-016
- CHK-019
- CHK-021
- CHK-022
- CHK-024
- CHK-025

This gives a strong early regression baseline without overbuilding the first suite.

---

## Summary

These test cases validate the first complete rules module in the platform.

If these pass consistently, the engine has proven:

- board state handling
- piece movement
- turn sequencing
- rules validation
- state persistence

That makes checkers the first reliable proof-of-engine game.