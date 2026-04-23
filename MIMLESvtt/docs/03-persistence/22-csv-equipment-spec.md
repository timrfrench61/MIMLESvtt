# Equipment CSV Specification

## Purpose

Define the CSV format for importing equipment definitions.

Each row creates one equipment definition.

---

## Required Columns

- DefinitionId
- Name
- Category

Category must be: Equipment

---

## Optional Columns

- Type
- Value
- Weight
- Tags
- Description

---

## Validation Rules

- `DefinitionId`, `Name`, and `Category` are required and must be non-empty.
- `Category` must be `Equipment`.
- `Value` and `Weight` (when provided) must parse as numeric.
- Duplicate `DefinitionId` handling is policy-driven by import mode (`RejectDuplicate`, `SkipDuplicate`, `UpdateDuplicate`).

---

## Example

```csv
DefinitionId,Name,Category,Type,Value,Weight
SWD001,Longsword,Equipment,Weapon,15,3
SHD001,Shield,Equipment,Armor,10,6