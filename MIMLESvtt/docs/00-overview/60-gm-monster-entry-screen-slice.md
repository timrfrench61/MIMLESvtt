# GM Monster Entry Screen Backlog Slice

## Purpose

Define the first concrete manual-content entry slice for Monster records.

This slice translates the manual GM workflow into an actionable Monster entry screen plan with explicit fields, validation, and save/edit behavior.

---

## Screen Modes

The Monster entry screen supports:

1. **Create mode**
   - blank/default field initialization
   - save creates new monster entry

2. **Edit mode**
   - loads existing monster by id
   - save updates existing record

3. **Cancel flow**
   - returns to list/detail without persisting changes

---

## Field List (First Pass)

### Required fields

- `Id`
- `Name`
- `Category`

### Core optional/structured fields

- `LevelOrThreat`
- `HitPoints`
- `Movement`
- `ArmorOrDefense`
- `AttackProfile`
- `Source`
- `Tags`
- `Description`

### Ruleset extension section

- key/value extension editor for AD&D1/BRP-specific fields
- extension entries are optional but must be structurally valid

---

## Validation Rules

Minimum validation requirements:

- required fields must be non-empty
- `Id` must be unique on create (duplicate blocked)
- numeric fields must parse and remain within practical ranges
- extension entries must use non-empty keys

Validation feedback expectations:

- inline field-level errors where possible
- form-level summary for blocked save
- explicit success message on save

---

## Save/Edit Behavior

### Save (create)

- validates all required and typed fields
- on success: persist new monster and return to list/detail context
- on failure: remain on form with visible errors

### Save (edit)

- validates modified values
- updates existing record while preserving id integrity
- on success: show confirmation and return to detail/list

### Cancel

- no mutation persisted
- user returns safely to previous navigation context

---

## Related Docs

- `docs/00-overview/54-rpg-content-model-monster.md`
- `docs/00-overview/59-manual-gm-entry-workflow.md`
- `docs/00-overview/33-monster-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-008)
