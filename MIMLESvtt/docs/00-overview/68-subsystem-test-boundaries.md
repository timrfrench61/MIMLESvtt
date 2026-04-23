# Subsystem Test Boundaries

## Purpose

Define which code layers each subsystem owns for testing, and where shared integration tests belong.

This keeps engine, rules, UI, persistence, import, and networking verification responsibilities explicit.

---

## Ownership by Subsystem

### 1) Tabletop Engine

Owns tests for:

- core session/state models
- action request validation and apply behavior
- board/space/zone/piece state transformations
- turn/phase state behavior

Does not own:

- ruleset-specific legality logic
- UI rendering behavior

### 2) Rules Framework

Owns tests for:

- rules request/result contract behavior
- module validation/rejection logic
- module resolution output payloads
- dice/randomization usage inside module flows

Does not own:

- UI interaction tests
- generic engine state persistence tests

### 3) UI / Presentation

Owns tests for:

- page/component state transitions
- route-level access/flow behavior
- user-visible validation and feedback messaging

Does not own:

- deep domain rule correctness
- persistence serializer invariants

### 4) Persistence / Import

Owns tests for:

- serializer round-trip compatibility
- file workflow safety and recovery behavior
- import parse/map/validate/report behavior
- import packet baseline and edge-case expectations

Does not own:

- final UI interaction workflows (except integration handoff checks)

### 5) Networking (when enabled)

Owns tests for:

- connection/session sync behavior
- server-authoritative apply ordering expectations
- reconnect/rehydration behavior

---

## Cross-Subsystem Integration Test Placement

Shared integration tests should verify interactions between subsystems without replacing subsystem unit tests.

Recommended integration slices:

- engine + persistence (save/load/restore)
- engine + import (import/apply outcome and state mutation)
- engine + rules (module validation/resolution handoff)
- UI + workspace services (operator flow state and feedback)

---

## Separation Rules

- Engine tests remain separate from rules-module tests.
- UI tests remain separate from domain logic tests.
- Integration tests verify contract handoffs between layers and should not duplicate unit-level assertions.

---

## Backlog Mapping

- `docs/05-backlog/60-backlog-testing-qa.md` (TQA-002)
- Depends on TQA-001 strategy baseline
