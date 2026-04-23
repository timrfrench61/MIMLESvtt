# RPG Content Model: Treasure

## Purpose

Define the baseline treasure content model used by manual entry and import workflows.

The model supports coin/value entries and loot bundles while remaining adaptable for future scenario/encounter linking.

---

## Core Treasure Fields

Minimum baseline fields:

- `Id` (stable unique key)
- `Name` (display label)
- `TreasureType` (coin, bundle, hoard, reward, etc.)
- `BaseValue` (normalized value indicator)
- `CurrencyOrValueUnit` (for example gp, sp, abstract points)
- `Quantity` (count/stack quantity where applicable)
- `Description` (operator-facing notes)
- `Source` (manual/import/pack/module source)
- `Tags` (search/filter grouping)

---

## Composition Support

Treasure composition can be represented with a nested collection concept (for example `Components`) where each row describes:

- component id/name
- component type
- quantity
- value contribution

This enables both simple single-value treasure and multi-part bundles.

---

## Future Linking Readiness

Treasure model should allow optional future linkage fields for:

- encounter references
- scenario/reward table references
- placement/container context references

These are optional in first-pass model and should not block import/manual entry.

---

## Validation Baseline

First-pass validation expectations:

- required: `Id`, `Name`, `TreasureType`
- numeric fields (`BaseValue`, `Quantity`) must parse and remain in practical ranges
- composition rows validate independently and report row-level issues

---

## Related Docs

- `docs/00-overview/53-content-library-framework.md`
- `docs/00-overview/34-treasure-management-ui-flow.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-003)
