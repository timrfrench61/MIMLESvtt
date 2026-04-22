# Persistence Subsystem (Implementation Guide)

## Purpose

This document explains the current persistence subsystem implementation and how session/scenario import/apply flows are structured in code.

It complements:
- `01-session-state-persistence.md` (domain persistence concepts)

This document now includes both:

- persistence policy/contracts (what and why)
- persistence subsystem implementation map (how and where in code)

---

## Policy and Contracts

## Core Principles

### Separation of Runtime State and Definitions

The system distinguishes between:

- runtime state (`TableSession`/`VttSession`)
- reusable definitions (content)

Runtime state represents current live or saved session state.

Reusable definitions represent authored data such as:

- piece definitions
- surface definitions
- marker definitions
- item definitions
- creature definitions
- scenario definitions

Runtime state references definitions by identifiers (for example `DefinitionId`).

### Domain-Only Persistence

Persist domain-relevant state, not UI transients.

Persisted data includes:

- session identity and metadata
- surfaces and pieces
- placement/coordinates
- ownership/visibility
- module state
- action history

Excluded data includes:

- UI-only state
- temporary interaction state
- client-only rendering state

### Versioning

All persisted structures must include a `Version` field.

### Validation Before Acceptance

All imports must validate:

- structure
- references
- domain invariants

Invalid import input is rejected as a whole (no partial apply).

---

## Persistence Types

The subsystem supports:

- Session Save (`.session.json`)
- Scenario Snapshot (`.scenario.json`)
- Content Pack (`.contentpack.json`)
- Action Log Snapshot (`.actionlog.json`)

Conceptual shapes:

```json
{ "Version": 1, "TableSession": {} }
```

```json
{ "Version": 1, "Scenario": {} }
```

```json
{ "Version": 1, "Manifest": {}, "Definitions": [], "Assets": [] }
```

```json
{ "Version": 1, "SessionId": "id", "Actions": [] }
```

---

## Import and Export Rules

### Import Rules

Validate:

- required root shape + version
- reference integrity (`DefinitionId`, `SurfaceId`, `ParticipantId`, etc.)
- invariant integrity (no duplicate ids, coherent placement/visibility links)

On failure:

- reject import
- do not mutate runtime session

### Export Rules

- export domain state only
- keep references valid
- always include `Version`
- keep output predictable where practical (debugging/diffing/testing)

---

## Storage Strategy

- structured metadata: database-ready model (future-friendly)
- runtime/session snapshots: JSON
- assets: file/blob storage references

---

## Snapshot Types

The subsystem currently supports four persisted snapshot families:

- Table Session (`.session.json`)
- Scenario (`.scenario.json`)
- Content Pack (`.contentpack.json`)
- Action Log (`.actionlog.json`)

Extension constants are defined in:
- `src/Domain/Persistence/SnapshotFileExtensions.cs`

---

## Scenario Model (Current)

Scenario persistence now uses a unified **Scenario** model:

- `src/Domain/Persistence/Scenario.cs`
- `src/Domain/Persistence/ScenarioSnapshot.cs`
- `src/Domain/Persistence/ScenarioSnapshotSerializer.cs`

Snapshot shape remains:

```json
{
  "Version": 1,
  "Scenario": { }
}
```

This is intentionally separate from live `TableSession` runtime state.

---

## Core Services by Responsibility

## File persistence services (single format)

- `TableSessionFilePersistenceService`
- `ScenarioFilePersistenceService`
- `ContentPackFilePersistenceService`
- `ActionLogFilePersistenceService`

Responsibility:
- read/write one snapshot type to disk
- serialize/deserialize and throw clear errors

## Snapshot workflow facade (extension-guarded)

- `SnapshotFileWorkflowService`

Responsibility:
- save/load by snapshot family
- enforce file extension guardrails per format

## Import/dispatch/application pipeline

- `SnapshotImportService`
- `SnapshotImportApplicationService`
- `SnapshotImportIntentService`
- `SnapshotImportApplyExecutor`
- `SnapshotImportApplyWorkflowService`
- `SnapshotFileImportApplyWorkflowService`

Responsibility:
- import raw snapshot
- map to application outcome
- produce explicit apply intent
- execute allowed operations against runtime context
- return structured success/error response

## Scenario-specific activation pipeline

- `ScenarioActivationWorkflowService`
- `ScenarioPlanApplyService`
- `ScenarioCandidateActivationService`
- `PendingScenarioApplicationPlan`

Responsibility:
- convert imported scenario into pending plan
- build isolated table-session candidate
- activate candidate (dry-run or live activation modes)

## Workspace orchestration

- `SessionWorkspaceService`

Responsibility:
- user-facing create/open/save/import/activate operations
- operation history + feedback + dirty state updates

---

## Why there are multiple Scenario* classes

The Scenario pipeline separates concerns deliberately:

- snapshot payload model (`Scenario`, `ScenarioSnapshot`)
- serializer/persistence I/O (`ScenarioSnapshotSerializer`, `ScenarioFilePersistenceService`)
- import/apply orchestration (`ScenarioActivationWorkflowService` + apply services)
- pending activation state (`PendingScenarioApplicationPlan`)

This avoids overloading one class with all responsibilities and preserves explicit phase boundaries.

---

## Runtime Relationship: Scenario vs Session

- **Scenario**: prepared starting layout/template
- **TableSession**: live runtime game state

Activation flow:
1. import `.scenario.json`
2. build `PendingScenarioApplicationPlan`
3. create isolated `TableSession` candidate
4. activate candidate into current workspace context

---

## Current Constraints

- Single-user architecture remains authoritative.
- No multiplayer/network persistence semantics in this pass.
- Serializer contracts remain conservative and versioned.

---

## Maintenance Rule of Thumb

When changing persistence behavior:
1. preserve snapshot contract shape unless versioning is planned
2. keep import/apply pipeline phase boundaries explicit
3. update this doc and related persistence docs in `03-persistence/`
4. add/adjust tests for serializer + workflow + workspace behavior
