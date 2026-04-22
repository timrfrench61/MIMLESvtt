# Tabletop Screen - Use Cases and Actions

## Purpose

Define what the user can **do** on the Tabletop (Workspace) screen and the corresponding **actions** the system must support.

The Tabletop screen is where users **view and manipulate the shared game state**.

---

## Primary Use Cases

### 1. View Table State
- User sees the current board, pieces, and state.
- Includes zoom, pan, and layer visibility.

---

### 2. Select Piece
- User selects a piece to inspect or manipulate.
- Selection may drive available actions (move, rotate, etc.).

---

### 3. Move Piece
- User moves a piece from one position to another.
- May be constrained by game rules (if active).

---

### 4. Modify Piece
- User updates piece attributes.
- Examples:
  - label/name
  - status flags
  - game-specific values

---

### 5. Create Piece
- User adds a new piece to the board.
- Placement occurs at a chosen position.

---

### 6. Remove Piece
- User deletes a piece from the board.

---

### 7. Change Visibility
- User hides or reveals a piece.
- Supports fog-of-war or GM-only visibility.

---

### 8. Execute Game Move
- User performs a game-defined move (e.g., checkers move, chess move).
- Delegates to game rules for validation.

---

### 9. End Turn
- User completes their turn and passes control.

---

### 10. Undo / Redo
- User reverts or reapplies recent actions.

---

### 11. Inspect Game State
- User views details about:
  - selected piece
  - game status
  - turn state

---

### 12. Save / Load Session
- User saves current state or loads another state.

---

### 13. Adjust View
- User pans, zooms, or re-centers the board.

---

## Core Actions (Engine-Level)

These are the **atomic operations** the Tabletop must support.

- SelectPieceAction — Select a piece for interaction
- CreatePieceAction — Add a new piece to the table
- RemovePieceAction — Remove a piece from the table
- MovePieceAction - Change a piece's position
- UpdatePieceAction — Modify piece attributes
- ChangeVisibilityAction — Show or hide a piece
- ExecuteGameMoveAction — Perform a validated game move
- EndTurnAction — Advance the turn state
- UndoAction — Revert last action
- RedoAction — Reapply reverted action

---

## Supporting Actions (Non-State)

These do not change TableState but affect the user experience.

- PanViewAction — Move viewport
- ZoomViewAction — Adjust zoom level
- SelectToolAction — Change active tool (move, select, measure, etc.)
- OpenPanelAction — Show UI panels (inventory, game info, etc.)

---

## Interaction Model

All state-changing interactions should follow:

```text
User Input → TableAction → Validation → Apply to TableState → Render Update