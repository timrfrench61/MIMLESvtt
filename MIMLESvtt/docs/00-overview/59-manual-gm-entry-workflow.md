# Manual GM Entry Workflow

## Purpose

Define how a GM/operator manually creates, edits, lists, and reviews content entries across supported content types.

This workflow covers the common manual path for:

- Monster
- Treasure
- Equipment
- Magic Item

---

## Workflow Stages

### 1) List

- Open the content-type list screen.
- Use filter/search/sort to find existing entries.
- Choose to open detail or begin create/edit.

### 2) Create / Edit

- Open entry screen in create or edit mode.
- Enter or update field values.
- Apply required-field and type/range validations.
- Save changes or cancel without persisting.

### 3) Detail / Review

- Open detail screen for full read-only review.
- Verify key metadata and content fields.
- Transition to edit when corrections are needed.

### 4) Return / Continue

- Return to list for next operation.
- Repeat create/edit/review for additional entries.

---

## Content-Type Separation

Manual entry remains separated by content type:

- Monster workflow and forms are distinct from Treasure/Equipment/Magic Item.
- Shared UX pattern is reused (list -> detail -> entry/edit), but fields and validations are content-specific.

---

## Validation Expectations

- Required fields must block save with clear user-facing messages.
- Numeric and structured fields must validate with explicit error feedback.
- Save success/failure must be shown visibly to operator.

---

## Related Docs

- `docs/00-overview/33-monster-management-ui-flow.md`
- `docs/00-overview/34-treasure-management-ui-flow.md`
- `docs/00-overview/35-equipment-management-ui-flow.md`
- `docs/00-overview/36-magic-item-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-007)
