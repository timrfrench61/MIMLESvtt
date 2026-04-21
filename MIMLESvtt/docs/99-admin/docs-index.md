# Documentation Index

## Overview

This folder contains all planning, architecture, and development documentation for the **Blazor Tabletop Engine / VTT** project.

The project is designed as a **general tabletop platform**, supporting:

- board games
- tactical tabletop games
- strategic war games
- roleplaying games (AD&D1, BRP, and future systems)

This documentation is structured to support:
- long-term maintainability
- subsystem isolation
- iterative development
- clear onboarding for new contributors

---

## How to Use This Documentation

### If you are new to the project

Start here:

1. `/00-overview/vision-and-scope.md`
2. `/00-overview/subsystem-map.md`
3. `/00-overview/roadmap.md`
4. `/05-backlog/master-backlog.md`

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

- **roadmap.md**  
  Defines phased development plan and milestones

---

### `/01-architecture/` *(planned/optional)*

System-level technical decisions.

- system architecture overview
- networking model
- persistence strategy
- architecture decision records (ADR)

---

### `/02-domain/` *(planned/optional)*

Core data and engine concepts.

- table/board model
- piece/token model
- turn/phase model
- rules framework abstractions
- content models (RPG + wargame)

---

### `/03-import-export/` *(planned/optional)*

Data ingestion and validation.

- CSV specifications
- import workflow
- validation rules
- test data packet format

---

### `/04-ui/` *(planned/optional)*

UI and interaction design.

- UI shell layout
- board presentation patterns
- screen inventory
- GM workflows
- player workflows (future)

---

### `/05-backlog/`

All planning and execution tracking.

- **master-backlog.md**  
  Top-level backlog across all subsystems

- **backlog-table-engine.md**  
  Core engine (board, pieces, turns)

- **backlog-ui-presentation.md**  
  Layout, rendering, and UI behavior

- **backlog-persistence-infrastructure.md**  
  Database, storage, and services

- **backlog-checkers-reference.md**  
  Reference implementation for engine validation

- **backlog-rules-framework.md**  
  Rules plugin system and shared mechanics

- **backlog-content-import.md**  
  Content models, CSV import, validation

- **backlog-networking.md**  
  Multiplayer session and synchronization

- **backlog-testing-qa.md**  
  Testing strategy, regression, and validation

- **backlog-documentation-planning.md**  
  Documentation maintenance and planning control

---

### `/06-testing/` *(planned/optional)*

Testing artifacts and data.

- test strategy
- test cases
- test data packets
- regression definitions

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

- `/00-overview/roadmap.md`
- `/05-backlog/master-backlog.md`

for current priorities and execution order.

---

## Notes

- Keep documentation updated as design evolves
- Do not allow design decisions to live only in chat or code
- Use backlog items to track all meaningful work
- Prefer updating existing docs over creating duplicates