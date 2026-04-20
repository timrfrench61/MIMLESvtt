# Design Packet Integration Report

## 1. Purpose

This report compares the current repository implementation and documentation to the authoritative design packet and identifies a conservative alignment path.

Scope of this report:

- align existing code and docs to the design packet
- avoid inventing replacement architecture
- preserve working code where possible
- explicitly classify status as:
  - already implemented
  - partially implemented
  - missing
  - conflicting

---

## 2. Design packet files reviewed

- `docs/00-overview/vision-and-scope.md`
- `docs/00-overview/subsystem-map.md`
- `docs/02-domain/domain-glossary.md`
- `docs/02-domain/table-state-model.md`
- `docs/02-domain/action-system.md`
- `docs/03-import-export/persistence-and-import-export.md`
- `docs/04-modules/module-architecture.md`
- `docs/05-backlog/roadmap.md`
- `docs/05-backlog/backlog.md`
- `docs/06-testing/testing-strategy.md`

---

## 3. Current code areas reviewed

- `MIMLESvtt.csproj`
- `Program.cs`
- `Components/App.razor`
- `Components/Routes.razor`
- `Components/Pages/Home.razor`
- `src/Participant.cs`
- `src/SurfaceInstance.cs`
- `src/PieceInstance.cs`
- `src/Location.cs`
- `src/Coordinate.cs`
- `src/Stack.cs`
- `src/Container.cs`
- `src/VisibilityState.cs`
- `src/TableOptions.cs`
- `src/ActionModel.cs`

Build status observed:

- `dotnet build` currently fails due to `MIMLESvtt.csproj` compiling a markdown file (`docs/00-overview/roadmap.md`) as C# source.

---

## 4. Current doc areas reviewed

Beyond the design packet, these current docs were reviewed for overlap/conflict:

- `docs/00-overview/roadmap.md`
- `docs/02-domain/session-state-persistence.md`
- `docs/02-domain/piece-definition-model.md`
- `docs/02-domain/rules-plugin-framework.md`
- `docs/02-domain/turn-sequence-model.md`
- `docs/02-domain/checkers-rules-module.md`
- `docs/05-backlog/master-backlog.md`

---

## 5. Alignment summary

### Already implemented

- Blazor Web App shell exists and runs through standard startup (`Program.cs`, router, app layout).
- Early domain type stubs exist for key vocabulary:
  - `SurfaceInstance`
  - `PieceInstance`
  - `VisibilityState`
  - `TableOptions`
  - `ActionRecord` (in `ActionModel.cs`)
- Definition vs Instance direction is partially reflected with `DefinitionId` on `SurfaceInstance` and `PieceInstance`.

### Partially implemented

- Domain model is present only as skeletal data classes; many referenced types are undefined.
- `ActionRecord` exists, but `ActionRequest` and action flow are not implemented.
- `Stack` and `Container` types exist, but integration with session aggregate is not implemented.

### Missing

- `TableSession` root aggregate (authoritative center of state).
- `ModuleState` on `TableSession`.
- action dispatch/validate/apply/log pipeline.
- persistence/import/export implementation.
- module activation/isolation implementation.
- test project and tests matching testing strategy.

### Conflicting

- Build currently blocked by markdown file included as `<Compile>` item.
- Design packet path conventions (for example `src/Domain/Actions/...`) do not match current file layout (`src/*.cs`).
- Non-packet docs use mixed vocabulary (`Command`, `TurnState`, `Board`) that is not consistently aligned with packet terms (`ActionRequest`, `ActionRecord`, `TableSession`).

---

## 6. Gaps between code and design packet

1. **No `TableSession` aggregate (missing)**  
   Design packet centers all runtime state in `TableSession`; code has no such type.

2. **No `ActionRequest` (missing)**  
   `ActionRecord` exists, but request model and full action lifecycle are absent.

3. **No `ModuleState` root storage (missing)**  
   Module architecture requires `TableSession.ModuleState`; not present.

4. **No explicit aggregate invariants (missing)**  
   Required validations (id uniqueness, reference validity) are not implemented.

5. **Domain types are non-public field stubs (partially implemented)**  
   Current classes are package-internal with private fields; not yet usable as stable domain contracts.

6. **Undefined referenced types (conflicting/incomplete)**  
   Examples: `ParticipantRole`, `SurfaceType`, `CoordinateSystem`, `Layer`, `Zone`, `SurfaceTransform`, `Rotation`.

7. **Persistence and import/export behavior (missing)**  
   No implementation for session save/load, scenario export, content pack, action log export.

8. **Testing strategy execution (missing)**  
   No test project or test suites found for domain/action/persistence/module boundaries.

---

## 7. Gaps between docs and design packet

1. **Duplicate roadmap tracks (conflicting)**  
   - `docs/00-overview/roadmap.md` (legacy/alternate)
   - `docs/05-backlog/roadmap.md` (design packet authoritative)

2. **Legacy terminology drift (partially conflicting)**  
   Several non-packet docs describe `Command` pipelines and `TurnState`/`Board` centric modeling that can diverge from packet action vocabulary.

3. **Path references not aligned with repository (partially conflicting)**  
   `docs/02-domain/action-system.md` references `src/Domain/Actions/...` while code is currently in `src/` root.

4. **Module architecture markdown formatting issue (conflicting)**  
   `docs/04-modules/module-architecture.md` contains an unbalanced fence around `TableSession.ModuleState`.

5. **Backlog duplication (partially conflicting)**  
   `docs/05-backlog/master-backlog.md` and `docs/05-backlog/backlog.md` overlap with different granularity/status conventions.

---

## 8. Conflicts or ambiguities that need resolution

1. **Authoritative roadmap location**  
   Confirm `docs/05-backlog/roadmap.md` as canonical and mark `docs/00-overview/roadmap.md` as legacy/archive.

2. **Authoritative action vocabulary**  
   Confirm `ActionRequest` / `ActionRecord` terminology as standard; treat `Command` wording in legacy docs as historical.

3. **Domain source layout expectation**  
   Confirm whether to adopt packet-referenced `src/Domain/...` structure now, or temporarily keep `src/` flat and update docs.

4. **Turn/phase model placement**  
   Clarify whether turn sequencing is part of core `TableSession` state now or module-owned until Phase 4/5.

5. **Build policy for docs**  
   Confirm docs must never be included as compile items.

---

## 9. Recommended target project structure

Conservative target (aligns with packet without introducing new subsystems):

- `src/Domain/`
  - `Sessions/` (`TableSession`)
  - `Participants/`
  - `Surfaces/`
  - `Pieces/`
  - `Placement/` (`Location`, `Coordinate`, `Stack`, `Container`)
  - `Visibility/`
  - `Actions/` (`ActionRequest`, `ActionRecord`)
  - `Options/` (`TableOptions`)
- `src/Application/`
  - `Actions/` (dispatch/validation/apply orchestration)
- `src/Infrastructure/`
  - `Persistence/` (JSON/database adapters)
  - `ImportExport/`
- `Components/` (Blazor UI)
- `tests/` (domain-first test projects)

This keeps the existing Blazor app and grows the architecture in-place.

---

## 10. Recommended domain model locations in src

Proposed current-to-target mapping:

- `src/Participant.cs` → `src/Domain/Participants/Participant.cs`
- `src/SurfaceInstance.cs` → `src/Domain/Surfaces/SurfaceInstance.cs`
- `src/PieceInstance.cs` → `src/Domain/Pieces/PieceInstance.cs`
- `src/Location.cs` → `src/Domain/Placement/Location.cs`
- `src/Coordinate.cs` → `src/Domain/Placement/Coordinate.cs`
- `src/Stack.cs` → `src/Domain/Placement/Stack.cs`
- `src/Container.cs` → `src/Domain/Placement/Container.cs`
- `src/VisibilityState.cs` → `src/Domain/Visibility/VisibilityState.cs`
- `src/TableOptions.cs` → `src/Domain/Options/TableOptions.cs`
- `src/ActionModel.cs` (contains `ActionRecord`) → `src/Domain/Actions/ActionRecord.cs`

New required files:

- `src/Domain/Sessions/TableSession.cs`
- `src/Domain/Actions/ActionRequest.cs`
- enums/value objects currently referenced but undefined (role/surface/rotation/coordinate-system/layer/zone).

---

## 11. Recommended next implementation sequence

1. **Unblock build (infrastructure hygiene)**
   - remove markdown `<Compile>` include from `.csproj`.

2. **Establish minimal coherent domain root**
   - add `TableSession` with required packet fields:
     - participants, surfaces, pieces, options, visibility, action log, module state.

3. **Normalize existing domain stubs**
   - move files into `src/Domain/*` folders and align namespaces.
   - define missing referenced enums/types.

4. **Implement action contracts first**
   - add `ActionRequest`.
   - retain and refine `ActionRecord`.

5. **Add minimal action pipeline service**
   - validate → apply → append `ActionRecord`.

6. **Add persistence skeleton**
   - versioned session snapshot serialization for `TableSession`.

7. **Add baseline domain and action tests**
   - start with creation, `MovePiece`, invalid action rejection, action logging.

8. **Defer modules/networking until core state + persistence are stable**

---

## 12. Safe refactors now

1. **Remove markdown compile include** (`MIMLESvtt.csproj`)  
   Why: currently breaks build; no runtime value.

2. **Rename/split `ActionModel.cs` to `ActionRecord.cs`**  
   Why: aligns file/class naming and packet terminology.

3. **Move domain files into foldered `src/Domain/*` layout without behavior changes**  
   Why: aligns docs and code organization early with low risk.

4. **Mark legacy roadmap as non-authoritative in-doc**  
   Why: reduces planning ambiguity without deleting history.

5. **Fix markdown fence in module architecture doc**  
   Why: avoids rendering confusion in authoritative packet docs.

---

## 13. Refactors to postpone

1. **Replacing all legacy terminology in one sweep**  
   Postpone until canonical glossary mapping table is added.

2. **Large-scale module framework coding**  
   Postpone until `TableSession` + action pipeline are stable.

3. **Multiplayer SignalR integration**  
   Postpone until single-user action semantics and persistence are validated.

4. **Major UI rewrites**  
   Postpone until domain contracts stabilize; avoid UI-driven domain drift.

5. **Deleting legacy docs outright**  
   Postpone; first mark legacy and cross-reference authoritative packet docs.

---

## 14. Proposed doc updates

1. Add this report reference from overview docs.
2. Add “authoritative vs legacy” note to:
   - `docs/00-overview/roadmap.md`
   - `docs/05-backlog/master-backlog.md`
3. Update `docs/02-domain/action-system.md` file path references after code move decision.
4. Fix code fence issue in `docs/04-modules/module-architecture.md`.
5. Add a short glossary mapping appendix for legacy terms:
   - `Command` → `ActionRequest`
   - move/event log wording → `ActionRecord`/`ActionLog`

---

## 15. Proposed backlog updates

Add/adjust near-term backlog items (priority order):

1. **Build unblock:** remove markdown compile include from `.csproj`.
2. **Create `TableSession` aggregate** with packet-required fields.
3. **Create `ActionRequest` model** and normalize `ActionRecord` location/naming.
4. **Implement minimal action processor** (validate/apply/log).
5. **Define missing domain enums/value objects** referenced by existing stubs.
6. **Reorganize `src` into `src/Domain/*`** with namespace updates.
7. **Add first domain/action tests** aligned to testing strategy minimums.
8. **Implement versioned session snapshot save/load** for `TableSession`.
9. **Mark/align legacy docs** to packet authority and vocabulary.

Status framing for current backlog:

- Domain: partially implemented
- Action system: partially implemented
- Persistence: missing
- Multiplayer: missing
- Modules: missing
- Testing: missing
- UI table workflows: missing (current UI is starter shell)
