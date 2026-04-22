# Root Classes API Documentation

This page documents the project root gameplay/content classes used by the current implementation.

## Naming Note

In code, the current names are:

- `VttSession` 
- `VttScenario`
- `ContentPackSnapshot` (serialized Content Pack root)

---

## 1) `VttSession`

**Namespace:** `MIMLESvtt.src`  
**Source:** `src/Domain/Sessions/TableSession.cs` (contains `VttSession` and compatibility `TableSession`)

Represents one live game/session runtime state.

### Route and Creation Context

- Workspace is a routed page via `@page "/workspace"` in `Components/Pages/Workspace.razor`.
- Routing is handled by `Components/Routes.razor` (`<Router AppAssembly="typeof(Program).Assembly" ... />`).
- On first Workspace load, if no current session exists, `WorkspaceService.CreateNewSession()` is called in `Workspace.OnInitialized()`.
- That call creates the live session object used by Workspace state.

### Properties

- `string Id`  
  Unique session identifier.

- `string Title`  
  User-facing session title.

- `List<Participant> Participants`  
  Current session participants (GM/players/observers).

- `List<SurfaceInstance> Surfaces`  
  Active playable surfaces/boards/maps.

- `List<PieceInstance> Pieces`  
  Live placed pieces/tokens/counters.

- `List<string> TurnOrder`  
  Ordered participant ids for turn sequence.

- `int CurrentTurnIndex`  
  Current index in `TurnOrder`.

- `int TurnNumber`  
  Current turn number (default: `1`).

- `string CurrentPhase`  
  Current phase label for gameplay flow.

- `TableOptions Options`  
  Table-level options and toggles.

- `VisibilityState Visibility`  
  Session visibility state model.

- `List<ActionRecord> ActionLog`  
  Accepted action history for current session.

- `Dictionary<string, object> ModuleState`  
  Module-specific state storage.

---

## 2) `VttScenario`

**Namespace:** `MIMLESvtt.src`  
**Source:** `src/Domain/Persistence/Scenario.cs` (contains `VttScenario` and compatibility `Scenario`)

Represents a prepared starting layout/template that can be imported and activated into a live `VttSession`.

### Properties

- `string Title`  
  VttScenario title.

- `List<SurfaceInstance> Surfaces`  
  VttScenario starting surfaces.

- `List<PieceInstance> Pieces`  
  VttScenario starting piece placements.

- `TableOptions Options`  
  VttScenario starting table options.

### Related Snapshot Container

`ScenarioSnapshot` (`src/Domain/Persistence/ScenarioSnapshot.cs`) wraps a persisted VttScenario with:

- `int Version`
- `VttScenario? Scenario`

---

## 3) `ContentPackSnapshot` (Content Pack Root)

**Namespace:** `MIMLESvtt.src`  
**Source:** `src/Domain/Persistence/ContentPackSnapshot.cs`

Represents a persisted content-pack envelope containing definitions and assets.

### Properties

- `int Version`  
  Content pack schema version.

- `ContentPackManifest? Manifest`  
  High-level metadata.

- `List<ContentPackDefinition>? Definitions`  
  Included reusable definitions.

- `List<ContentPackAsset>? Assets`  
  Included asset references.

### Related Types

- `ContentPackManifest` (`Name`, `Description`)
- `ContentPackDefinition` (`Id`, `Type`)
- `ContentPackAsset` (`Id`, `AssetPath`)

---

## Quick Relationship Summary

- `VttSession` = live gameplay/runtime state.
- `VttScenario` = prepared starting state template.
- `ContentPackSnapshot` = package of reusable definitions/assets.
