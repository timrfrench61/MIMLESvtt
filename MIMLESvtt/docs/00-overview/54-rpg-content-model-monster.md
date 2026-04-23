# RPG Content Model: Monster

## Purpose

Define the baseline monster content model used for manual entry and import flows.

The model must support both AD&D1 and BRP usage while keeping system-specific interpretation in rules modules.

---

## Core Monster Fields

Minimum core fields:

- `Id` (stable unique key)
- `Name` (display label)
- `Category` (monster type/classification)
- `LevelOrThreat` (generic difficulty indicator)
- `HitPoints` (or baseline vitality value)
- `Movement` (base movement profile)
- `ArmorOrDefense` (generic defense slot)
- `AttackProfile` (summary attack descriptor)
- `Source` (manual/import/module/content-pack source)
- `Tags` (search/filter grouping)

These fields provide a practical cross-system baseline for list/detail/edit and import validation.

---

## Optional Ruleset-Specific Extensions

Ruleset-specific data should be stored in an extension payload (for example `Extensions` metadata bag), such as:

- AD&D1:
  - THAC0-like or table-reference data
  - morale
  - alignment
  - special attacks/defenses text
- BRP:
  - characteristic values (STR/CON/etc.)
  - skill percentages
  - damage bonus notes
  - hit location profile references

This keeps the core model stable while allowing module-specific depth.

---

## AD&D1 and BRP Compatibility Notes

- AD&D1 compatibility is achieved through core combat fields + extension metadata for TSR-era specifics.
- BRP compatibility is achieved through core identity/combat fields + extension metadata for percentile/stat detail.
- Rules modules consume/interpret extensions without changing the shared core contract.

---

## Validation Baseline

Validation expectations for first pass:

- required: `Id`, `Name`, `Category`
- numeric fields must parse and remain in practical range
- extension keys must be accepted as optional metadata (not blocked unless malformed)

---

## Related Docs

- `docs/00-overview/53-content-library-framework.md`
- `docs/03-persistence/24-csv-monster-spec.md`
- `docs/00-overview/33-monster-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-002)
