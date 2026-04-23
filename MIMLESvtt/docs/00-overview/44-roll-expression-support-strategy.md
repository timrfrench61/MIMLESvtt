# Roll Expression Support Strategy

## Purpose

Define the first-pass and future-path strategy for dice roll expressions in the shared randomization layer.

This strategy is intentionally minimal for initial module integration while preserving expansion headroom.

---

## First-Pass Capability (Implemented)

Supported expression form:

- `NdS`
- `NdS+M`
- `NdS-M`

Examples:

- `1d20`
- `2d6+3`
- `3d8-1`

First-pass constraints:

- integer count and sides only
- no whitespace-inside expression parsing
- no advanced operators/grouping
- invalid expressions fail clearly with argument exceptions

---

## Why This Scope

This capability is sufficient for:

- AD&D-style attack/damage baseline usage
- BRP auxiliary rolls where needed
- quick module prototyping without introducing full parser complexity

It keeps the shared service simple and testable while allowing rule modules to move forward.

---

## Future Expansion Path

Potential next increments:

1. Whitespace-tolerant parsing
2. Keep/drop highest/lowest semantics
3. Grouped/compound expressions (for example `(2d6+1)*2`)
4. Named roll profiles/macros
5. Deterministic seed controls for simulation/replay scenarios

These can be layered without breaking existing minimal expression contracts.

---

## Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-004)
