# Monster Management UI Flow

## Purpose

Define the practical UI flow for monster content management using a consistent list/detail/entry-edit pattern.

This flow targets GM/operator workflows and aligns with the content inventory baseline.

---

## Screen Set

### 1) Monster List Screen

Primary responsibilities:

- show all available monster entries
- support filter/search/sort
- show quick status indicators (for example: source/import status)
- provide actions to open detail or begin edit/create

Expected operator actions:

- Select monster -> open detail
- Create new monster -> open entry/edit in create mode
- Edit existing monster -> open entry/edit in edit mode

### 2) Monster Detail Screen

Primary responsibilities:

- present full read-only monster information
- show derived/structured fields in a consistent layout
- link back to list and forward to edit mode

Expected operator actions:

- Review complete monster entry
- Transition to edit
- Return to list

### 3) Monster Entry/Edit Screen

Primary responsibilities:

- create new or edit existing monster entries
- validate required fields before save
- surface field-level and summary validation feedback

Expected operator actions:

- Enter/update values
- Save valid record
- Cancel and return without saving

---

## Validation and Feedback Expectations

- Required fields must block save with clear user-visible messages.
- Invalid values should be shown inline where possible and summarized at form level.
- Save success/failure should provide explicit status messaging.

---

## Navigation Flow (Concise)

1. Open Monster List
2. Choose Detail or Entry/Edit path
3. Validate edits
4. Save or cancel
5. Return to List/Detail

---

## Related Docs

- `docs/00-overview/32-content-management-screen-inventory.md`
- `docs/05-backlog/05-backlog-ui-presentation.md` (UI-008)
