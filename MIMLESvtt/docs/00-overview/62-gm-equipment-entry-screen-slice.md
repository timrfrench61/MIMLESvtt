# GM Equipment Entry Screen Backlog Slice

## Purpose

Define the manual-entry Equipment screen slice with explicit fields, validation, and save/edit behavior.

This slice follows the shared GM manual-entry pattern and applies equipment-specific constraints.

---

## Screen Modes

1. **Create mode**
   - initializes blank/default equipment fields
   - save creates new equipment record

2. **Edit mode**
   - loads existing equipment by id
   - save updates existing record

3. **Cancel flow**
   - exits without persisting changes

---

## Field List (First Pass)

### Required fields

- `Id`
- `Name`
- `EquipmentType`

### Core optional fields

- `Category`
- `BaseCost`
- `Weight`
- `WeightUnit`
- `Description`
- `Source`
- `Tags`

### Ruleset extension section

- optional extension key/value entries for AD&D1/BRP equipment details
- extension entries must be structurally valid (non-empty keys)

---

## Validation Rules

Minimum validation requirements:

- required fields must be non-empty
- `Id` must be unique on create
- numeric fields (`BaseCost`, `Weight`) must parse and remain in practical range
- extension rows require non-empty keys

Validation feedback expectations:

- inline field errors where possible
- form-level summary for blocked save
- explicit success message on successful save

---

## Save/Edit Behavior

### Save (create)

- validate all required and typed fields
- on success: persist new equipment and return to list/detail context
- on failure: remain on form and show errors

### Save (edit)

- validate modified values
- update existing equipment while preserving id integrity
- on success: show confirmation and return to detail/list

### Cancel

- no persisted mutation
- return to previous navigation context safely

---

## Related Docs

- `docs/00-overview/56-rpg-content-model-equipment.md`
- `docs/00-overview/59-manual-gm-entry-workflow.md`
- `docs/00-overview/35-equipment-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-010)
