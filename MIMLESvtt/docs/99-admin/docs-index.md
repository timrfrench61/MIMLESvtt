# Documentation Index

## Canonical Purpose

This index maps the current docs set and identifies canonical planning documents.

Primary canonical planning docs:
- Platform roadmap: `docs/05-backlog/roadmap.md`
- Working backlog: `docs/05-backlog/backlog.md`
- Vision/scope: `docs/00-overview/vision-and-scope.md`
- Testing strategy: `docs/06-testing/testing-strategy.md`

---

## How to Use This Documentation

### If you are new to the project

Start here:

1. `/00-overview/vision-and-scope.md`
2. `/00-overview/subsystem-map.md`
3. `/05-backlog/roadmap.md`
4. `/05-backlog/backlog.md`

Then review subsystem backlogs relevant to your work.

---

### If you are implementing features

1. Find the relevant subsystem backlog in `/05-backlog/`
2. Locate backlog items by ID
3. Review dependencies and acceptance criteria
4. Check related domain or architecture docs if needed

---

### If you are designing or refactoring

1. Review subsystem map
2. Review domain and architecture docs
3. Update backlog items if scope changes
4. Record major decisions in architecture notes (ADR if used)

---

## Folder Structure

---

### `/00-overview/`

High-level project definition and direction.

- **vision-and-scope.md**  
  Defines product goals, scope, and guiding principles

- **subsystem-map.md**  
  Defines system decomposition and responsibilities

- **design-packet-integration-report.md**  
  Tracks integration status against authoritative packet docs

- **roadmap.md**  
  Historical/alternate roadmap narrative (use `05-backlog/roadmap.md` as canonical implementation roadmap)

---

### `/00-overview/`

High-level project direction and consolidated core-model documentation.

Includes:

- vision/scope and subsystem framing
- glossary, table-state model, and global action system
- turn/persistence/rules supporting model docs

---

### `/03-import-export/`

Data ingestion and validation.

- CSV specifications
- import workflow
- validation rules
- test data packet format

---

### `/05-backlog/`

All planning and execution tracking.

- **backlog.md**  
- **00-backlog.md**  
  Canonical working backlog

- **01-roadmap.md**  
  Canonical platform implementation roadmap

- **21-checkers-roadmap.md**  
  Scoped reference implementation roadmap (prototype track)

- **90-master-backlog-reference.md**  
  Roll-up/reference backlog

- **backlog-table-engine.md**  
  Core engine (board, pieces, turns)

- **05-backlog-ui-presentation.md**  
  Layout, rendering, and UI behavior

- **09-backlog-persistence-infrastructure.md**  
  Database, storage, and services

- **20-backlog-checkers-reference.md**  
  Reference implementation for engine validation

- **backlog-rules-framework.md**  
  Rules plugin system and shared mechanics

- **09-backlog-content-import.md**  
  Content models, CSV import, validation

- **08-backlog-networking.md**  
  Multiplayer session and synchronization

- **60-backlog-testing-qa.md**  
  Testing strategy, regression, and validation

- **90-master-backlog-reference.md**  
  Reference-only historical roll-up and documentation-planning traceability catalog

---

### `/06-testing/`

Testing artifacts and data.

- test strategy
- test cases
- test data packets
- regression definitions

---

### `/09-user-documentation/`

User-facing guidance.

- **quickstart.md**  
  Step-by-step pre-alpha usage flow

---

### `/99-admin/`

Administrative/process docs.

- this index
- lifecycle assessment report
- prompt and planning notes

---

## Backlog Conventions

Each backlog item includes:

- ID (e.g., MB-###, TQA-###, DOC-###)
- Title
- Subsystem
- Description
- Priority
- Status
- Dependencies
- Acceptance Criteria

Subsystem backlogs reference **Master Backlog IDs** for traceability.

---

## Key Design Principles

- **Engine-first design**  
  The platform is not RPG-specific

- **Data-driven content**  
  CSV import and structured models are core

- **Modular rules systems**  
  AD&D1 and BRP are plugins, not foundations

- **Separation of concerns**  
  UI, domain, rules, and persistence remain isolated

- **Reference implementation strategy**  
  Checkers is used to validate the engine early

---

## Current Development Focus

Refer to:

- `/05-backlog/roadmap.md`
- `/05-backlog/backlog.md`

for current priorities and execution order.

---

## Notes

- Keep documentation updated as design evolves
- Do not allow design decisions to live only in chat or code
- Use backlog items to track all meaningful work
- Prefer updating existing docs over creating duplicates