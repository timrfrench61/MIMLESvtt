# Unit/Counter Content Model

## Purpose

Define the baseline unit/counter content model for tactical and strategic play styles.

This model is intended for reusable content libraries, manual entry workflows, and import pipelines.

---

## Core Unit/Counter Fields

Minimum baseline fields:

- `Id` (stable unique key)
- `Name` (display label)
- `UnitType` (infantry, cavalry, armor, naval, marker, etc.)
- `Side` (team/alignment in scenario context)
- `Faction` (sub-group identity)
- `StrengthOrValue` (generic combat/value indicator)
- `Movement` (base movement allowance/profile)
- `DefenseOrArmor` (generic defensive indicator)
- `RangeOrReach` (optional attack range indicator)
- `Description` (operator-facing notes)
- `Source` (manual/import/pack/module source)
- `Tags` (search/filter grouping)

---

## Tactical and Strategic Suitability

### Tactical suitability

- supports unit-level identity and movement/defense values
- supports side/faction assignment for combat encounters
- supports optional special tags and marker-like usage

### Strategic suitability

- supports abstract strength/value representation
- supports faction-level grouping and scenario-level side assignment
- supports extension metadata for supply/control/objective semantics

---

## Extensibility Strategy

Ruleset- or scenario-specific fields should be attached through extension metadata (for example `Extensions`) such as:

- morale/cohesion
- command rating
- supply status
- terrain affinity
- era/technology tags

This keeps the core model stable while allowing game-style variation.

---

## Validation Baseline

First-pass validation expectations:

- required: `Id`, `Name`, `UnitType`, `Side`
- numeric fields must parse and remain in practical range
- side/faction values should be non-empty when applicable to scenario mode

---

## Related Docs

- `docs/00-overview/53-content-library-framework.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-006)
