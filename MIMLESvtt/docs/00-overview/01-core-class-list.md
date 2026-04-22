# Core Class List (Merged with Subsystem Map)

## Purpose

This document merges:

- the subsystem-level architecture view
- the concrete core class inventory view

Use this as the primary “what exists in code” map.

---

## Root Runtime and Content Classes

- `VttSession` (compat: `TableSession`) — live runtime game/session object.
- `VttScenario` (compat: `Scenario`) — prepared starting layout/template.
- `VttContentPack` (target) — reusable content package model (non-snapshot runtime/content contract).

---

## Subsystem-to-Class Map

## 1) Table Session Core

Primary classes:

- `VttSession`
- `Participant`
- `TabletopOptions`
- `VisibilityState`
- `ActionRecord`

Responsibility:

- authoritative runtime state
- participants/turn/phase/options
- action log and module state

---

## 2) Surface / Map / Board

Primary classes:

- `SurfaceInstance`
- `SurfaceType`
- `CoordinateSystem`
- `Layer`
- `Zone`
- `SurfaceTransform`

Responsibility:

- playable spaces
- spatial model and layout metadata

---

## 3) Piece / Token / Counter

Primary classes:

- `PieceInstance`
- `Location`
- `Coordinate`
- `Rotation`
- `Stack`
- `Container`

Responsibility:

- placed objects and mutable state during play

---

## 4) Content Library and Definitions

Primary classes (target content shape + current persisted shape):

- `VttContentPack` (target)
- `VttContentPackManifest`
- `VttContentPackDefinition`
- `VttContentPackAsset`
- `VttScenario`

Responsibility:

- reusable authored content and packaged definitions/assets
- snapshot wrappers belong to persistence mapping, not root content-model naming

---

## 5) Interaction and Tooling

Primary classes/services:

- `WorkspaceBoardState`
- `SessionWorkspaceService`
- `ITableSessionCommandService`

Responsibility:

- workspace interaction flows and command dispatch from UI

---

## 6) Turn / Phase / Flow

Primary fields (on `VttSession`):

- `TurnOrder`
- `CurrentTurnIndex`
- `TurnNumber`
- `CurrentPhase`

Responsibility:

- ordered turn progression and current phase tracking

---

## 7) Notes / Chat / Dice / Logs

Current implemented core:

- `ActionRecord`
- `ActionLogSnapshot`

Planned/partial:

- dice and chat-specific models are not fully established yet.

---

## 8) Visibility and Knowledge Model

Primary classes:

- `VisibilityState`
- piece-level visibility fields on `PieceInstance`

Responsibility:

- hidden/visible modeling and role-aware visibility basis.

---

## 9) Persistence and Serialization

Primary classes:

- `TableSessionSnapshot`, `TableSessionSnapshotSerializer`
- `ScenarioSnapshot`, `ScenarioSnapshotSerializer`
- `VttContentPackSnapshot`, `VttContentPackSnapshotSerializer`
- `SnapshotFileWorkflowService`
- `SnapshotFileImportApplyWorkflowService`

Responsibility:

- save/load/import/export and versioned snapshot handling

---

## 10) Networking and Synchronization

Current status:

- designed in docs, not implemented as active multiplayer runtime in this pass.

---

## 11) Rules and Automation Extensions

Primary classes (extension foundation):

- `ModuleState` storage on `VttSession` (`Dictionary<string, object>`)
- rules/plugin framework docs and planned services

Responsibility:

- optional rules-aware behavior layered over engine state

---

## 12) Asset Pipeline

Primary classes:

- `VttContentPackAsset`
- content-pack persistence services

Responsibility:

- asset references and package-level media metadata

---

## 13) Administration and Configuration

Primary classes/services:

- `WorkspaceSettings`
- workspace/app settings state within `SessionWorkspaceState`

Responsibility:

- developer toggles and runtime workspace configuration

---

## Core Rule

`VttSession` is the runtime center. Other classes/services should operate through it or through explicit workflow services that update it.
