# Checkers Rules Module

## Purpose

Define the first concrete implementation of the Rules Plugin Framework using **standard checkers**.

This module serves as:

- a reference implementation of rules integration
- a validation of the table state model
- a baseline for testing and regression

This is not just a game — it is the **first proof that the engine works**.

---

## Scope

Implements:

- piece movement
- turn order
- capture rules
- promotion (kinging)
- win conditions

Does NOT include:

- UI rendering
- animations
- AI opponent
- advanced variants

---

## Game Overview

Standard checkers rules:

- 8x8 board
- two sides: Red and Black
- pieces move diagonally
- captures are mandatory (optional in first pass)
- pieces promote to king on last rank

---

## Required Engine Components

This module depends on:

- VttSession
- Board (square grid)
- PieceDefinition / PieceInstance
- TurnState (alternating turn)
- Command → Validate → Resolve → Apply pipeline

---

## Piece Definitions

### Red Checker

- Category: Piece
- Side: Red
- Attributes:
  - IsKing = false

---

### Black Checker

- Category: Piece
- Side: Black
- Attributes:
  - IsKing = false

---

## Piece Instance State

Each piece tracks:

- position (board coordinate)
- side (Red / Black)
- isKing (true/false)
- removedFromPlay (true/false)

---

## Turn Model

### Sequence Type

- Alternating Turn

### Rules

- Red starts first (configurable)
- players alternate turns
- only current side may act

---

## Commands

### MovePiece

Move a piece from one coordinate to another.

Fields:

- fromPosition
- toPosition

---

## Validation Rules

A MovePiece command is valid only if:

### Turn Validation
- piece belongs to current side

### Movement Validation
- move is diagonal
- move distance is:
  - 1 space (normal move)
  - 2 spaces (capture)

### Direction Rules
- non-king pieces move forward only
- king pieces move both directions

### Occupancy Rules
- destination space is empty

### Capture Rules
- if moving 2 spaces:
  - there must be an opponent piece between
- optional (first pass): do not enforce mandatory capture

---

## Resolution Rules

After a valid move:

### Normal Move
- move piece to destination

### Capture Move
- move piece to destination
- remove captured piece

### Promotion (Kinging)
- if piece reaches last rank:
  - set IsKing = true

---

## Effects

Resolution produces effects such as:

- PieceMoved
- PieceCaptured
- PiecePromoted
- TurnAdvanced

---

## Turn Advancement

After a valid move:

- switch current side
- increment turn number

Optional future enhancement:

- multi-capture continuation before turn ends

---

## Win Conditions

A side wins if:

- opponent has no remaining pieces
- opponent has no legal moves

### End State

Set:

- HasEnded = true
- WinnerSideId

---

## Initial Simplifications

For first implementation:

- do NOT enforce mandatory capture
- do NOT enforce multi-capture chains
- do NOT support draw conditions
- do NOT support timers

Add later after base system is stable.

---

## Example Move

### Command

MovePiece:
- from: C3
- to: D4

### Validation

- correct side → valid
- diagonal → valid
- destination empty → valid

### Resolution

- piece moved

### Effects

- PieceMoved
- TurnAdvanced

---

## Example Capture

### Command

MovePiece:
- from: C3
- to: E5

### Validation

- diagonal → valid
- distance = 2 → capture
- opponent at D4 → valid

### Resolution

- move piece to E5
- remove piece at D4

### Effects

- PieceMoved
- PieceCaptured
- TurnAdvanced

---

## Example Promotion

### Condition

- Red piece reaches row 8

### Resolution

- set IsKing = true

### Effect

- PiecePromoted

---

## Integration with Engine

### Engine Responsibilities

- store board state
- store piece instances
- enforce turn ownership (via validation)
- apply effects
- update TurnState
- persist session

---

### Rules Module Responsibilities

- validate moves
- resolve moves
- determine captures
- determine promotion
- determine win condition

---

## Minimal Data Requirements

To run checkers:

- 8x8 board
- 24 piece instances
- 2 sides
- turn state

No additional systems required.

---

## Testing Targets

This module should support:

- legal move tests
- illegal move rejection
- capture behavior
- promotion behavior
- turn switching
- win detection

These tests become the **first regression suite**.

---

## Future Enhancements

Possible additions:

- forced capture rules
- multi-capture chains
- draw detection
- alternate rule variants
- AI opponent
- move history and replay

---

## Summary

The Checkers Rules Module:

- is the first real rules implementation
- validates the engine architecture
- provides a stable test target
- proves command → validation → resolution → effect flow

If this works cleanly, the engine is viable.