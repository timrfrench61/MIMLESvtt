# Monster CSV Specification

## Purpose

Define the CSV format for importing monster definitions.

Each row creates one monster definition.

---

## Required Columns

- DefinitionId
- Name
- Category

Category must be: Monster

---

## Optional Columns

- HitPoints
- ArmorClass
- Movement
- Tags
- Description

---

## Validation Rules

- `DefinitionId`, `Name`, and `Category` are required and must be non-empty.
- `Category` must be `Monster`.
- `HitPoints` and `ArmorClass` (when provided) must parse as integers.
- `Movement` (when provided) must parse as numeric.
- Duplicate `DefinitionId` handling is policy-driven by import mode (`RejectDuplicate`, `SkipDuplicate`, `UpdateDuplicate`).

---

## Example

```csv
DefinitionId,Name,Category,HitPoints,ArmorClass,Movement
GOB001,Goblin,Monster,5,6,9
ORC001,Orc,Monster,8,5,9