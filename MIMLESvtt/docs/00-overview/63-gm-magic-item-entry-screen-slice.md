# GM Magic Item Entry Screen Backlog Slice

## Purpose

Define the manual-entry Magic Item screen slice with explicit fields, validation, and save/edit behavior.

This slice follows the shared GM manual-entry pattern while covering magic-item specific effect metadata needs.

---

## Screen Modes

1. **Create mode**
   - initializes blank/default magic-item fields
   - save creates new magic-item record

2. **Edit mode**
   - loads existing magic-item by id
   - save updates existing record

3. **Cancel flow**
   - exits without persisting changes

---

## Field List (First Pass)

### Required fields

- `Id`
- `Name`
- `ItemType`

### Core optional fields

- `Rarity`
- `AttunementRequired`
- `ChargesOrUses`
- `Description`
- `Source`
- `Tags`

### Effect metadata section

- optional structured effect entries for passive/activated effects
- trigger/condition notes
- duration/expiry hints
- effect entries must be structurally valid

---

## Validation Rules

Minimum validation requirements:

- required fields must be non-empty
- `Id` must be unique on create
- optional numeric fields (`ChargesOrUses`) must parse and remain in practical range
- effect metadata entries require non-empty keys

Validation feedback expectations:

- inline field errors where possible
- form-level summary for blocked save
- explicit success message on successful save

---

## Save/Edit Behavior

### Save (create)

- validate required and typed fields
- on success: persist new magic item and return to list/detail context
- on failure: remain on form and show errors

### Save (edit)

- validate modified values
- update existing magic item while preserving id integrity
- on success: show confirmation and return to detail/list

### Cancel

- no persisted mutation
- return to previous navigation context safely

---

## Related Docs

- `docs/00-overview/57-rpg-content-model-magic-item.md`
- `docs/00-overview/59-manual-gm-entry-workflow.md`
- `docs/00-overview/36-magic-item-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-011)
