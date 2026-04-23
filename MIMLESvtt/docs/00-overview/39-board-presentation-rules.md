# Board Presentation Rules

## Purpose

Define practical board/table rendering behavior for Workspace so piece visibility, selection, and placement/movement feedback stay consistent for operators.

This document records the current presentation rules implemented in the Blazor Workspace and provides the baseline for follow-on visual-state work.

---

## Rendering Layers

Current board presentation is composed from these visible layers:

1. **Tabletop fallback layer**
   - Default x-y grid
   - Missing tabletop message
   - Page-not-loading message
2. **Board grid overlay** (when enabled by visibility controls)
3. **Rendered pieces** (active filtering applied)
4. **Selection emphasis** (selected vs primary-selected styling)
5. **Workspace right-slide controls** (operator panel over tabletop)

---

## Piece Rendering Rules

### Visibility and filtering

Piece rendering respects board visibility controls:

- show/hide surfaces
- show/hide pieces
- show/hide markers
- active-surface-only filter
- definition id filter
- owner participant id filter

### Piece text/marker summary

- Piece label is shown on-board.
- Marker count is shown inline when marker display is enabled.

---

## Selection State Rules

Selection state is explicit and multi-tiered:

- **Selected**: piece is in current selection set.
- **Primary selected**: one piece is designated as the primary selection.

Expected behavior:

- clear visual differentiation between selected and primary-selected states.
- selection summary remains visible in controls/HUD.
- clear selection action is always available.

---

## Placement and Move Indicators

### Placement indicators

- Last board click coordinate is shown in controls.
- Add-at-click and stamp mode expose next-id and active context indicators.
- Active surface context is visible in strip and board HUD.

### Move indicators

- Position updates are visible immediately on board and reflected in piece data/list values.
- Group move uses explicit delta inputs and applies consistently to selected set.
- Rotation updates are visible in rendered piece transform and reflected in piece data.

---

## Board Interaction Rules

- Board focus state is explicit for keyboard-driven nudge/rotate workflows.
- Keyboard movement/rotation behavior is bounded by current configuration (nudge step, clamp toggle).
- Interaction paths should fail with clear feedback when disabled by dev toggles.

---

## Acceptance Mapping (UI-003)

- Rendering patterns are documented: **Yes**
- Selection state is defined: **Yes**
- Piece placement and move indicators are defined: **Yes**

---

## Related Docs

- `docs/09-user-documentation/workspace-controls-api.md`
- `docs/09-user-documentation/workspace-use-cases.md`
- `docs/05-backlog/05-backlog-ui-presentation.md` (UI-003)
