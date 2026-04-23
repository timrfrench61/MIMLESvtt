# Unit / Counter CSV Specification

## Purpose

Define the CSV format for importing unit or counter definitions for tactical and war-game use.

Each row creates one unit or counter definition.

---

## Required Columns

- DefinitionId
- Name
- Category

Category must be: Unit or Counter

---

## Optional Columns

- Movement
- Strength
- Range
- Defense
- Tags
- Description

---

## Validation Rules

- `DefinitionId`, `Name`, and `Category` are required and must be non-empty.
- `Category` must be `Unit` or `Counter`.
- `Movement`, `Strength`, `Range`, and `Defense` (when provided) must parse as numeric.
- Duplicate `DefinitionId` handling is policy-driven by import mode (`RejectDuplicate`, `SkipDuplicate`, `UpdateDuplicate`).

---

## Example

```csv
DefinitionId,Name,Category,Movement,Strength,Range,Defense
INF001,Infantry,Unit,4,3,1,2
TNK001,Tank,Unit,6,8,3,5
ART001,Artillery,Counter,2,5,5,1
```
