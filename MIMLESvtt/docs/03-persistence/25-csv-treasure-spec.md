# Treasure CSV Specification

## Purpose

Define the CSV format for importing treasure definitions.

Each row creates one treasure definition.

---

## Required Columns

- DefinitionId
- Name
- Category

Category must be: Treasure

---

## Optional Columns

- TreasureType
- Value
- Currency
- Quantity
- Tags
- Description

---

## Validation Rules

- `DefinitionId`, `Name`, and `Category` are required and must be non-empty.
- `Category` must be `Treasure`.
- `Value` and `Quantity` (when provided) must parse as numeric.
- Duplicate `DefinitionId` handling is policy-driven by import mode (`RejectDuplicate`, `SkipDuplicate`, `UpdateDuplicate`).

---

## Example

```csv
DefinitionId,Name,Category,TreasureType,Value,Currency,Quantity
TRS001,Goblin Coin Pouch,Treasure,Coin,27,gp,1
TRS002,Small Gem Cache,Treasure,Bundle,150,gp,3
```
