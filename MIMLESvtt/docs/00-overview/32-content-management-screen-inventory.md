# Content Management Screen Inventory

## Purpose

Define the baseline screen inventory for content-management workflows used by GMs/operators.

This inventory covers list/detail/create/edit/import/review flows and groups them by practical content area.

---

## Screen Groups

### 1) Monster Management

- Monster List
  - filter/search/sort
  - quick status indicators
- Monster Detail
  - read-only full stat/reference view
- Monster Entry/Edit
  - create new monster
  - edit existing monster
  - validate required fields before save

### 2) Treasure Management

- Treasure List
- Treasure Detail
- Treasure Entry/Edit

### 3) Equipment Management

- Equipment List
- Equipment Detail
- Equipment Entry/Edit

### 4) Magic Item Management

- Magic Item List
- Magic Item Detail
- Magic Item Entry/Edit

---

## Shared Workflow Screens

### Import Workflow

- Import Upload
  - file selection
  - import type selection
- Validation Preview
  - row/item-level validation results
  - warnings and errors
- Import Result Summary
  - counts for created/updated/skipped/failed
- Error Review
  - actionable error details for correction

---

## Grouping Rationale

- Group content by operator intent (monster/treasure/equipment/magic item).
- Keep list/detail/edit patterns consistent across groups.
- Reuse import/validation/review steps to reduce UI inconsistency.

---

## Minimum Inventory Checklist

- Monster: list/detail/entry-edit
- Treasure: list/detail/entry-edit
- Equipment: list/detail/entry-edit
- Magic Item: list/detail/entry-edit
- Import: upload/validation/error-review/success-summary

---

## Related Backlog Mapping

- UI-007: content-management screen inventory (this document)
- UI-008: monster flow detail
- UI-009: treasure flow detail
- UI-010: equipment flow detail
- UI-011: magic item flow detail
- UI-012: import workflow detail
