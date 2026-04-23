# Magic Item Management UI Flow

## Purpose

Define the practical UI flow for magic item content management using a consistent list/detail/entry-edit pattern.

This flow supports GM/operator workflows and aligns with the shared content-management inventory.

---

## Screen Set

### 1) Magic Item List Screen

Primary responsibilities:

- show all magic item entries
- support filter/search/sort
- expose quick source/import status context
- provide actions for detail and entry/edit

Expected operator actions:

- Select magic item -> open detail
- Create new magic item -> open entry/edit in create mode
- Edit existing magic item -> open entry/edit in edit mode

### 2) Magic Item Detail Screen

Primary responsibilities:

- present full read-only magic item information
- show rarity/attunement/effect and structured fields consistently
- provide clear transitions back to list and into edit mode

Expected operator actions:

- Review complete magic item entry
- Transition to edit
- Return to list

### 3) Magic Item Entry/Edit Screen

Primary responsibilities:

- create new or edit existing magic item records
- validate required fields and value constraints before save
- show field-level and summary validation feedback

Expected operator actions:

- Enter/update values
- Save valid record
- Cancel and return without persisting changes

---

## Validation and Feedback Expectations

- Required field omissions block save and show explicit user-facing errors.
- Invalid structured values are shown inline and summarized at form level.
- Save success/failure feedback is explicit and visible.

---

## Navigation Flow (Concise)

1. Open Magic Item List
2. Choose Detail or Entry/Edit path
3. Validate edits
4. Save or cancel
5. Return to List/Detail

---

## Related Docs

- `docs/00-overview/32-content-management-screen-inventory.md`
- `docs/05-backlog/05-backlog-ui-presentation.md` (UI-011)
