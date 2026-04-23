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

## Example

```csv
DefinitionId,Name,Category,Type,Value,Weight
SWD001,Longsword,Equipment,Weapon,15,3
SHD001,Shield,Equipment,Armor,10,6