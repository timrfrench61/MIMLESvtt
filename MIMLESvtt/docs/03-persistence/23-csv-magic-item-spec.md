# Magic Item CSV Specification

## Purpose

Define the CSV format for importing magic item definitions.

Each row creates one magic item definition.

---

## Required Columns

- DefinitionId
- Name
- Category

Category must be: MagicItem

---

## Optional Columns

- Type
- Value
- Charges
- Tags
- Description

---

## Validation Rules

- `DefinitionId`, `Name`, and `Category` are required and must be non-empty.
- `Category` must be `MagicItem`.
- `Value` and `Charges` (when provided) must parse as numeric.
- Duplicate `DefinitionId` handling is policy-driven by import mode (`RejectDuplicate`, `SkipDuplicate`, `UpdateDuplicate`).

---

## Example

```csv
DefinitionId,Name,Category,Type,Value,Charges
WAND001,Wand of Fireballs,MagicItem,Wand,5000,7
RING001,Ring of Protection,MagicItem,Ring,2000,