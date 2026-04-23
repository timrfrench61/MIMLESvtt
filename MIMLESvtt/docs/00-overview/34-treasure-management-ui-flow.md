# Treasure Management UI Flow

## Purpose

Define the practical UI flow for treasure content management using a consistent list/detail/entry-edit pattern.

This flow is intended for GM/operator workflows and aligns with the content inventory baseline.

---

## Screen Set

### 1) Treasure List Screen

Primary responsibilities:

- show all treasure entries
- support filter/search/sort
- provide quick visibility for source/import status
- provide actions for detail and entry/edit

Expected operator actions:

- Select treasure -> open detail
- Create new treasure -> open entry/edit in create mode
- Edit existing treasure -> open entry/edit in edit mode

### 2) Treasure Detail Screen

Primary responsibilities:

- present full read-only treasure data
- show value/type/category and other structured fields consistently
- provide actions to return to list or enter edit mode

Expected operator actions:

- Review full treasure details
- Transition to edit
- Return to list

### 3) Treasure Entry/Edit Screen

Primary responsibilities:

- create new or edit existing treasure records
- validate required fields and data ranges before save
- show field-level and summary validation messages

Expected operator actions:

- Enter/update values
- Save valid record
- Cancel and return without mutation

---

## Validation and Feedback Expectations

- Missing required fields block save with explicit user-facing errors.
- Invalid values are surfaced inline and summarized at form level.
- Save success/failure feedback is explicit and visible.

---

## Navigation Flow (Concise)

1. Open Treasure List
2. Choose Detail or Entry/Edit path
3. Validate edits
4. Save or cancel
5. Return to List/Detail

---

## Related Docs

- `docs/00-overview/32-content-management-screen-inventory.md`
- `docs/05-backlog/05-backlog-ui-presentation.md` (UI-009)
