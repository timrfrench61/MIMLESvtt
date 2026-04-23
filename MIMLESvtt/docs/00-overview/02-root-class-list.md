# Root Classes API Documentation

This page documents the project root gameplay/content classes used by the current implementation.

## Naming Note

In code, the current names are:

- `VttSession` 
- `VttScenario`
- `VttGamebox`

---

## 1) `VttSession`

**Namespace:** `MIMLESvtt.src`  
**Source:** `src/Domain/Models/VttSession.cs

Represents one live game/session runtime state.

### Route and Creation Context

- Workspace > workspace state > current session
- On first Workspace load, if no current session exists, `WorkspaceService.CreateNewSession()` is called in `Workspace.OnInitialized()`, creating the live session object used by WorkspaceBoardState.

### Properties

- `string Id`  -   Unique session identifier.
- `string Title`  -   User-facing session title.
- `List<Participant> Participants`  -   Current session participants (GM/players/observers).
- `List<SurfaceInstance> Surfaces`  -   Active playable surfaces/boards/maps.
- `List<PieceInstance> Pieces`  -   Live placed pieces/tokens/counters.
- `List<string> TurnOrder`  -   Ordered participant ids for turn sequence.
- `int CurrentTurnIndex`  -   Current index in `TurnOrder`.
- `int TurnNumber`  -   Current turn number (default: `1`).
- `string CurrentPhase`  -   Current phase label for gameplay flow.
- `TabletopOptions Options`  -   Table-level options and toggles.
- `VisibilityState Visibility`  -   Session visibility state model.
- `List<ActionRecord> ActionLog`  -   Accepted action history for current session.
- `Dictionary<string, object> ModuleState`  -   Module-specific state storage.

---

## 2) `VttScenario`

**Namespace:** `MIMLESvtt.src`  
**Source:** `src/Domain/Models/VttScenario.cs` (contains `VttScenario` and compatibility `Scenario`)

Represents a prepared starting layout/template that can be imported and activated into a live `VttSession`.

### Properties

- `string Title`  -   VttScenario title.
- `List<SurfaceInstance> Surfaces`  -   VttScenario starting surfaces.
- `List<PieceInstance> Pieces`  -   VttScenario starting piece placements.
- `TabletopOptions Options`  -   VttScenario starting tabletop options.

### Related Snapshot Container

`ScenarioSnapshot` (`src/Domain/Persistence/ScenarioSnapshot.cs`) wraps a persisted VttScenario with:

- `int Version`
- `VttScenario? Scenario`

---

## 3) `VttGamebox` (Content Pack Root)

**Namespace:** `MIMLESvtt.src`  
**Source:** `src/Domain/Models/VttGamebox.cs`

Represents a persisted content-pack envelope containing definitions and assets.

### Properties

- `int Version`  -  Content pack schema version.
- `VttGameboxManifest? Manifest`  -  High-level metadata.
- `List<VttGameboxDefinition>? Definitions`  -  Included reusable definitions.
- `List<VttGameboxAsset>? Assets`  -  Included asset references.

### Related Types

- `VttGameboxManifest` (`Name`, `Description`)
- `VttGameboxDefinition` (`Id`, `Type`)
- `VttGameboxAsset` (`Id`, `AssetPath`)

---

## Quick Relationship Summary

- `VttSession` = live gameplay/runtime state.
- `VttScenario` = prepared starting state template.
- `VttGamebox` = package of reusable definitions/assets.
