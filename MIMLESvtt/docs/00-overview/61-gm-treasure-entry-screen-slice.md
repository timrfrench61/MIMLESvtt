# GM Treasure Entry Screen Backlog Slice

## Purpose

Define the manual-entry Treasure screen slice with explicit fields, validation, and save/edit behavior.

This is the Treasure counterpart to the first Monster entry slice and follows the same practical list/detail/edit workflow style.

---

## Screen Modes

1. **Create mode**
   - initializes blank/default treasure fields
   - save creates new treasure record

2. **Edit mode**
   - loads existing treasure by id
   - save updates existing record

3. **Cancel flow**
   - exits without persisting changes

---

## Field List (First Pass)

### Required fields

- `Id`
- `Name`
- `TreasureType`

### Core optional fields

- `BaseValue`
- `CurrencyOrValueUnit`
- `Quantity`
- `Description`
- `Source`
- `Tags`

### Composition section

- optional treasure component rows:
  - component id/name
  - component type
  - quantity
  - value contribution

---

## Validation Rules

Minimum validation requirements:

- required fields must be non-empty
- `Id` must be unique on create
- numeric fields (`BaseValue`, `Quantity`) must parse and be in practical range
- component rows validate independently and report row-level errors

Validation feedback expectations:

- inline field errors where possible
- form-level summary for blocked save
- explicit success message on successful save

---

## Save/Edit Behavior

### Save (create)

- validate all required and typed fields
- on success: persist new treasure and return to list/detail context
- on failure: remain on form and show errors

### Save (edit)

- validate modified values
- update existing treasure while preserving id integrity
- on success: show confirmation and return to detail/list

### Cancel

- no persisted mutation
- return to previous navigation context safely

---

## Related Docs

- `docs/00-overview/55-rpg-content-model-treasure.md`
- `docs/00-overview/59-manual-gm-entry-workflow.md`
- `docs/00-overview/34-treasure-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-009)
