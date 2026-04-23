# RPG Content Model: Equipment

## Purpose

Define the baseline equipment content model for manual entry and import workflows.

The model covers mundane gear plus weapons/armor and keeps rules-specific interpretation in module-level logic.

---

## Core Equipment Fields

Minimum baseline fields:

- `Id` (stable unique key)
- `Name` (display label)
- `EquipmentType` (weapon, armor, tool, gear, consumable, etc.)
- `Category` (higher-level grouping)
- `BaseCost` (normalized value)
- `Weight` (numeric weight value)
- `WeightUnit` (lb/kg/other)
- `Description` (operator-facing notes)
- `Source` (manual/import/pack/module source)
- `Tags` (search/filter grouping)

---

## Extensibility for Rules-Specific Details

Equipment should support optional extension metadata for system-specific details, such as:

- AD&D1:
  - weapon speed factors
  - damage by size notes
  - armor class adjustments
- BRP:
  - skill interactions
  - armor points / hit location interaction hints
  - trait flags

This allows rich module interpretation without destabilizing the shared content contract.

---

## Validation Baseline

First-pass validation expectations:

- required: `Id`, `Name`, `EquipmentType`
- numeric fields (`BaseCost`, `Weight`) must parse and be within practical range
- category/type values should be validated against known import/manual-entry options where available

---

## Related Docs

- `docs/00-overview/53-content-library-framework.md`
- `docs/00-overview/35-equipment-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-004)
