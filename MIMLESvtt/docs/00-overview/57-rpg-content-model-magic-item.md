# RPG Content Model: Magic Item

## Purpose

Define the baseline magic item content model for manual entry and import workflows.

The model captures identity and effect metadata while deferring rules interpretation to system modules.

---

## Core Magic Item Fields

Minimum baseline fields:

- `Id` (stable unique key)
- `Name` (display label)
- `ItemType` (weapon, armor, wondrous, potion, scroll, etc.)
- `Rarity` (common/uncommon/rare/etc. or system equivalent)
- `AttunementRequired` (bool/flag)
- `ChargesOrUses` (optional numeric usage field)
- `Description` (operator-facing effect text)
- `Source` (manual/import/pack/module source)
- `Tags` (search/filter grouping)

---

## Effect Metadata Attachment

Magic item effects should be attachable through structured extension metadata, for example:

- passive bonuses
- activated effects
- trigger conditions
- duration/expiry hints
- save/check references

This supports system-specific interpretation without coupling the core content contract to one ruleset.

---

## Rules-Specific Deferral

- Core model stores item identity and effect metadata.
- Rules modules interpret effect details at runtime.
- Import/manual-entry flows remain stable even as module behavior evolves.

---

## Validation Baseline

First-pass validation expectations:

- required: `Id`, `Name`, `ItemType`
- optional fields validated for type/range when present (`ChargesOrUses`, etc.)
- effect metadata accepted as extension payload unless malformed structurally

---

## Related Docs

- `docs/00-overview/53-content-library-framework.md`
- `docs/00-overview/36-magic-item-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-005)
