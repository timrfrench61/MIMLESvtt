# Checkers Reference UI Flow

## Purpose

Define the practical reference-game UI flow for checkers-style play in the tabletop workspace.

This flow is lightweight and intentionally focused on validating generic piece-selection, movement, turn indication, and game-end messaging patterns.

---

## Core Interaction Flow

### 1) Select piece

- User selects a movable piece on the board.
- Selected and primary-selected visual states indicate current focus.
- Selection summary confirms the active piece context.

### 2) Move piece

- User applies movement via board interaction controls (click/move context) or move controls.
- Piece position updates immediately on board and in piece/session data views.
- Action log records movement event details.

### 3) Turn progression

- Turn indicator is visible via turn/phase controls and roster summary.
- Next/previous turn controls update current active participant context.
- Current phase and turn number are visible while stepping through play.

### 4) Win-state message area (planned slot)

- A dedicated message area is reserved in the gameplay/summary region for reference-game win/terminal-state messages.
- Current implementation already supports operation/status messages; win-state messaging can plug into this visible feedback region.

---

## Reference UI Expectations

- Piece selection and movement are explicit and traceable.
- Turn indication remains visible during interaction.
- End-of-game/win-state messaging has a clear UI location and does not require hidden diagnostics.

---

## Acceptance Mapping (UI-005)

- Flow for selecting and moving a piece is documented: **Yes**
- Turn indication is visible: **Yes**
- Win-state message area is planned: **Yes**

---

## Related Docs

- `docs/00-overview/39-board-presentation-rules.md`
- `docs/00-overview/40-piece-visual-states.md`
- `docs/09-user-documentation/workspace-use-cases.md`
- `docs/05-backlog/05-backlog-ui-presentation.md` (UI-005)
