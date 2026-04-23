# Persistence Subsystem

## Purpose

This document describes save/load for the four persisted records:

- Session
- Scenario
- Gamebox
- Action Log

It focuses on what each record stores and which services save/load it.

---

## Shared Rules

- All records are JSON snapshots.
- All snapshots include `Version`.
- File extension must match snapshot type.
- Invalid input is rejected (no partial apply).

Extension constants:
- `src/Domain/Persistence/SnapshotFileExtensions.cs`

Primary save/load facade:
- `src/Domain/Persistence/SnapshotFileWorkflowService.cs`

---

## 1) Session Save/Load

File type:
- `.vttsession.json`

Snapshot root:
- `VttSession` (`{ Version, VttSession }`)

Main components persisted:
- session id/title
- participants
- surfaces
- pieces
- turn/phase state
- tabletop options
- visibility state
- action log
- module state

Code:
- `TableSessionSnapshot`
- `TableSessionSnapshotSerializer`
- `TableSessionFilePersistenceService`

---

## 2) Scenario Save/Load

File type:
- `.vttscenario.json`

Snapshot root:
- `Scenario` (`{ Version, Scenario }`)

Main components persisted:
- scenario title
- scenario surfaces
- scenario pieces
- scenario tabletop options

Code:
- model: `src/Domain/Models/VttScenario.cs`
- `ScenarioSnapshot`
- `ScenarioSnapshotSerializer`
- `ScenarioFilePersistenceService`

Notes:
- Scenario is a prepared starting layout.
- It is separate from live session progress.

---

## 3) Gamebox Save/Load

File type:
- `.vttgamebox.json`

Snapshot root:
- `VttGameboxSnapshot` (`{ Version, Manifest, Definitions, Assets }`)

Main components persisted:
- manifest metadata
- reusable definitions
- asset references

Code:
- app model: `src/Domain/Models/VttGamebox.cs`
- envelope: `VttGameboxSnapshot`
- `VttGameboxSnapshotSerializer`
- `VttGameboxFilePersistenceService`
- mapper: `VttGameboxMapper` (`VttGamebox` <-> `VttGameboxSnapshot`)

---

## 4) Action Log Save/Load

File type:
- `.actionlog.json`

Snapshot root:
- `ActionLogSnapshot` (`{ Version, SessionId, Actions }`)

Main components persisted:
- session id
- ordered action records

Code:
- `ActionLogSnapshot`
- `ActionLogSnapshotSerializer`
- `ActionLogFilePersistenceService`

---

## Import/Application (Current)

Import format detection and apply orchestration:
- `SnapshotImportService`
- `SnapshotFileImportApplyWorkflowService`

Current apply behavior:
- Session: supported for runtime replace
- Scenario: supported through pending-plan/activation flow
- Gamebox: import recognized; runtime apply not enabled in this pass
- Action Log: import recognized; runtime apply not enabled in this pass

---

## Workspace State (Related)

Workspace recovery save/load is separate from the four snapshot families:

- `SessionWorkspaceRecoveryState`
- `SessionWorkspaceStatePersistenceService`

This stores workspace context (current file references, pending scenario references, operation history), not the core four content/session snapshot records above.
