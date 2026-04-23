# Backlog Additions from 00-overview/58

Source: `docs/00-overview/58-unit-counter-content-model.md`
Target backlog: `docs/05-backlog/09-backlog-content-import.md`

## Code Project Additions

- [x] Add concrete Unit/Counter content model class with core fields (`Id`, `Name`, `UnitType`, `Side`, `Faction`, `StrengthOrValue`, `Movement`, `DefenseOrArmor`, `RangeOrReach`).
- [x] Add extension metadata support for tactical/strategic fields (morale, command rating, supply status, terrain affinity, era/technology tags).
- [x] Add import/manual-entry validators for required and numeric range constraints, including side/faction requirements where scenario mode applies.
- [x] Add CSV/import row mapper and list/detail/edit mapping contracts for unit/counter workflows.
- [x] Add tests for tactical/strategic compatibility, extension payload pass-through, and validation edge cases.
