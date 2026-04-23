# Testing Strategy

## File Location

docs/06-testing/testing-strategy.md

---

## Purpose

This document defines how the Blazor VTT will be tested.

It establishes:

- what kinds of tests are required
- what parts of the system are tested at each level
- how testing aligns with the domain model
- how regressions are prevented as the engine grows

This document applies to:

- domain model behavior
- action system behavior
- persistence and import/export
- multiplayer synchronization
- modules
- UI behavior where appropriate

---

## Core Principle

The most important testing target is the domain model.

If the domain model is stable, the UI and infrastructure are easier to maintain.

If the domain model is unstable, UI and multiplayer behavior will drift.

---

## Testing Goals

Testing must verify that:

- VttSession state remains coherent
- actions validate and apply correctly
- persisted data can be saved and restored
- imports reject invalid data
- multiplayer state remains synchronized
- modules do not violate core boundaries
- regressions are detected early

---

## Testing Layers

The system should be tested at multiple layers:

- domain tests
- action system tests
- persistence tests
- import/export tests
- multiplayer tests
- module tests
- UI/component tests

Each layer has a different purpose.

---

## Domain Tests

### Purpose

Verify that the core runtime model behaves correctly.

### Covers

- VttSession creation
- participant management
- surface creation
- piece creation
- piece placement
- coordinate handling
- visibility state behavior
- table options behavior
- module state behavior
- invariant preservation

### Example Assertions

- a piece references a valid surface
- duplicate ids are rejected
- location data remains coherent
- visibility state can be stored and restored
- module state does not overwrite core state

### Rule

Domain tests should not depend on Blazor UI.

They should test the model directly.

---

## Action System Tests

### Purpose

Verify that all state changes happen correctly through actions.

### Covers

- ActionRequest creation
- action validation
- action execution
- action logging
- rejection of invalid actions
- preservation of invariants during mutation

### Example Assertions

- MovePiece updates only the intended piece location
- AddMarker updates marker state correctly
- invalid actions do not mutate VttSession
- accepted actions are added to ActionLog
- rejected actions are not logged

### Rule

Every core action must have tests for:

- valid case
- invalid case
- invariant preservation

---

## Persistence Tests

### Purpose

Verify that session state can be saved and loaded without corruption.

### Covers

- VttSession serialization
- VttSession deserialization
- version field presence
- preservation of references
- module state persistence
- action log persistence

### Example Assertions

- a saved session restores with the same ids
- piece locations survive round-trip serialization
- module state survives round-trip serialization
- visibility survives round-trip serialization
- missing version is rejected

### Rule

Persistence tests must verify round-trip integrity.

---

## Import/Export Tests

### Purpose

Verify that imported and exported data follows the defined contract.

### Covers

- session save format
- scenario export format
- gamebox format
- action log export format
- structural validation
- reference validation
- domain invariant validation

### Example Assertions

- malformed session import is rejected
- broken DefinitionId reference is rejected
- duplicate ids are rejected
- exported content includes version field
- exported session excludes UI-only data

### Rule

Every import path must have both acceptance and rejection tests.

---

## Multiplayer Tests

### Purpose

Verify that session state remains consistent across multiple clients.

### Covers

- session join
- session leave
- action dispatch from client to server
- server-side validation
- server-side execution
- state broadcast to clients
- reconnect behavior

### Example Assertions

- valid MovePiece action updates all connected clients
- invalid action is rejected by server
- client cannot bypass validation
- reconnect receives current session state
- action order is applied consistently

### Rule

Multiplayer tests must treat the server as authoritative.

---

## Module Tests

### Purpose

Verify that modules extend behavior without breaking the core engine.

### Covers

- module registration
- module activation
- module state persistence
- module-defined actions
- module validation behavior
- module isolation
- module dependency checks if implemented

### Example Assertions

- module state is stored under correct key
- module actions follow standard action flow
- module cannot overwrite core state
- multiple modules can coexist without state collision
- module state survives persistence round-trip

### Rule

Module tests must confirm boundaries as well as behavior.

---

## UI / Component Tests

### Purpose

Verify that UI components correctly use the domain and action systems.

### Covers

- rendering of surfaces and pieces
- piece selection
- UI action dispatch
- table controls
- side panels
- module UI integration

### Example Assertions

- moving a piece in the UI dispatches MovePiece action
- component reflects updated session state
- invalid action results are handled correctly
- module controls render only when module is active

### Rule

UI tests should verify behavior and integration, not duplicate domain logic tests.

---

## Regression Testing

### Purpose

Prevent previously working behavior from breaking during refactor or feature growth.

### Covers

- previously fixed bugs
- persistence compatibility
- action behavior
- module behavior
- multiplayer synchronization behavior

### Rule

Every bug that exposes a domain or persistence defect should result in a regression test.

### Regression Promotion Checklist

Promote a test path into regression when:

1. The behavior is part of an active gameplay/import workflow.
2. A defect fix or high-risk branch recently touched the behavior.
3. The fixture is deterministic and practical for local + CI execution.
4. Assertion output is diagnosis-ready without extra repro steps.

---

## Automated Execution Guidance

For engine-affecting feature slices:

1. Run targeted unit tests for modified validators/mappers/models.
2. Run integration tests for changed service handoff and summary propagation.
3. Run applicable regression cases when fixing defects or stabilizing a slice.

For UI-heavy slices:

- run page/component tests first,
- run integration tests when UI behavior now invokes new service-stage behavior.

---

## Test Data and Fixtures

The project should maintain reusable test fixtures for:

- empty session
- session with one surface
- session with multiple pieces
- session with visibility state
- session with module state
- valid gamebox
- invalid gamebox
- valid scenario export
- invalid scenario export

Fixtures should be:

- simple
- readable
- deterministic

---

## Test Priorities

Testing effort should prioritize:

1. domain behavior
2. action system behavior
3. persistence and import/export
4. multiplayer synchronization
5. module boundaries
6. UI behavior

This order reflects architectural importance.

---

## Rules for Test Design

- test the domain before the UI
- test actions, not UI mutation shortcuts
- test invalid cases as well as valid cases
- keep fixtures small and explicit
- avoid hidden dependencies between tests
- keep tests deterministic
- prefer clear failures over clever test code

---

## Minimum Required Test Coverage

At minimum, the project should have tests for:

- VttSession creation
- piece placement
- MovePiece validation and execution
- AddMarker validation and execution
- session save/load round trip
- invalid import rejection
- multiplayer action validation on server
- module state persistence

These are the minimum tests required to protect the core architecture.

---

## Summary

The testing strategy centers on protecting the domain model.

The system is tested in layers:

- domain
- action system
- persistence
- import/export
- multiplayer
- modules
- UI

The highest priority is always:

- coherent VttSession state
- correct action behavior
- reliable persistence
- stable module boundaries

See implementation companion:

- `docs/06-testing/67-engine-test-strategy-implementation-notes.md`
