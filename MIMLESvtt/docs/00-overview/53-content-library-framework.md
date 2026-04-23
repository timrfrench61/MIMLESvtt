# Content Library Framework

## Purpose

Define the reusable content-library framework for manual entry, import, validation, and runtime consumption.

This framework is content-type agnostic and supports RPG and non-RPG growth over time.

---

## Content Categories

Current baseline categories:

- Monster
- Treasure
- Equipment
- Magic Item
- Unit/Counter (tactical/strategic extension track)

Future categories can be added without changing the core framework shape.

---

## Shared Metadata Concepts

All content categories should support common metadata concepts:

- `Id` (stable content key)
- `Name` (operator-facing label)
- `Category/Type` (content-specific classification)
- `Source` (manual/imported/pack reference)
- `Tags` (search/filter grouping)
- `Version`/`Revision` marker (optional, for update tracking)
- `Extensions` dictionary/bag for ruleset-specific fields

This enables consistent list/detail/edit/import behavior across content types.

---

## Framework Layers

1. **Content Model Layer**
   - typed content structures per category
   - shared metadata conventions

2. **Manual Entry Workflow Layer**
   - list/detail/create/edit/review flows by content type
   - field validation and save feedback

3. **CSV Import Layer**
   - upload -> parse -> map -> validate -> persist flow
   - warning/error reporting model

4. **Packet/Test Data Layer**
   - baseline valid and edge-case import packets
   - expected outcome documentation for validation testing

---

## Growth and Compatibility Principles

- Keep core content contracts broad enough for RPG and non-RPG systems.
- Keep rules-specific interpretation in rules modules, not base content contracts.
- Use extension metadata for system-specific fields to avoid schema churn.
- Prefer additive evolution so existing imports/manual entries remain usable.

---

## Related Docs

- `docs/00-overview/32-content-management-screen-inventory.md`
- `docs/00-overview/33-monster-management-ui-flow.md`
- `docs/00-overview/34-treasure-management-ui-flow.md`
- `docs/00-overview/35-equipment-management-ui-flow.md`
- `docs/00-overview/36-magic-item-management-ui-flow.md`
- `docs/00-overview/37-import-workflow-ui.md`
- `docs/05-backlog/09-backlog-content-import.md` (CI-001)
