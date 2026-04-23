# Piece Visual States

## Purpose

Define practical visual states for pieces rendered on the Workspace board so operators can quickly understand selection, ownership, status, and interaction context.

This document captures current implemented states and the near-term style contract for follow-on polish.

---

## Current Implemented Visual States

### 1) Default piece state

- Piece renders with base board-piece styling.
- Label is shown from piece id (or piece label context where available in panel/HUD).

### 2) Selected state

- Piece receives selected styling when included in the current selection set.
- Used for single-select and multi-select workflows.

### 3) Primary-selected state

- Primary selected piece receives distinct emphasis in addition to selected state.
- Used for operations that target one piece while preserving broader selection context.

### 4) Marker-indicated state

- When marker display is enabled, piece label shows marker count context.
- Supports quick visibility for status-like markers.

### 5) Ownership-visible state (panel/HUD companion)

- Owner assignment is surfaced in selected-piece details and lists.
- Board and panel together provide ownership clarity without requiring separate owner glyph system yet.

---

## Distinguishability Rules

Visual states must remain clearly distinguishable:

- default vs selected is always clear,
- selected vs primary-selected is always clear,
- marker context is visible without obscuring core piece identity.

The current CSS/state wiring (`board-piece`, `selected`, `primary`) satisfies baseline distinguishability for pre-alpha operations.

---

## Rules-System Influence Readiness

The model supports future rules-driven visual influence through:

- marker ids on pieces,
- piece state key/value bag,
- owner/participant links,
- visibility fields.

This allows future rule modules to drive visual overlays (disabled, elite/leader, promotion, hidden states) without rewriting base selection/placement rendering.

---

## Acceptance Mapping (UI-004)

- Visual-state list exists: **Yes**
- States are distinguishable: **Yes**
- Rules system can influence displayed state later: **Yes**

---

## Related Docs

- `docs/00-overview/39-board-presentation-rules.md`
- `docs/09-user-documentation/workspace-controls-api.md`
- `docs/05-backlog/05-backlog-ui-presentation.md` (UI-004)
