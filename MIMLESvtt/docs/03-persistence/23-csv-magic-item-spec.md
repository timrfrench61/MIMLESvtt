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

## Example

```csv
DefinitionId,Name,Category,Type,Value,Charges
WAND001,Wand of Fireballs,MagicItem,Wand,5000,7
RING001,Ring of Protection,MagicItem,Ring,2000,