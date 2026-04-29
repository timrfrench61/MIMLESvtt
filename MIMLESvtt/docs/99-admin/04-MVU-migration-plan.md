# MVU Migration Plan (Execution Draft)

## Scope and Intent
This plan uses the guidance from:
- `docs/99-admin/01-MVU-migration-models.md`
- `docs/99-admin/02-MVU-migration-views.md`
- `docs/99-admin/03-MVU-mogration-folders.md`

Goal:
- Move to a feature-first MVU approach.
- Treat existing `src/*` code as migration source/reference.
- Stop compiling new runtime behavior against `src/*` once equivalent MVU slices are in place.
- Build forward in `Models`, `Features`, `Services`, `GameModules`, and `Shared`.

Non-goal (for this migration):
- Do not redesign into layered clean-architecture terms.
- Do not do a big-bang rewrite.

---

## Target Folder Strategy
Adopt and enforce this working structure:

- `Models/` shared nouns and root state records
- `Features/` per-screen MVU files (`State`, `Actions`, `Update`, `Page`, `Components`)
- `Services/` persistence and integration services only
- `GameModules/` module/rules setup implementations
- `Shared/` common shell/layout/navigation components

Rule:
- New behavior goes in `Features/*` + `Models/*` + `Services/*`.
- `src/*` is read-only migration source until retired.

---

## Migration Phases

## Phase 0 - Baseline and Guardrails
1. Create a migration tracker section in this doc (status by feature).
2. Freeze `src/*` for net-new features (bug fixes only if blocking migration).
3. Add coding rule: every new UI behavior must dispatch an Action and flow through an Update function.
4. Add compile/inclusion strategy in project file as a staged switch (see Phase 6).

Exit criteria:
- Team follows MVU-only policy for new work.
- Migration checklist accepted.

## Phase 1 - Core Model Seed
Build first root model set in `Models/` aligned with doc 01:
- `TabletopState`
- `Board`
- `Token`
- `Player`
- `Position`
- `TurnState`
- `GameSession`

Rules:
- Records preferred.
- No UI logic, no persistence, no HTTP/file calls.

Exit criteria:
- Root model compiles and is used by at least one feature state.

## Phase 2 - Workspace Shell and Navigation MVU
Implement MVU shell entry in `Features/` aligned with doc 02:
- Workspace shell state
- Navigation actions
- Navigation update reducer
- Shell page rendering from state

Initial screens:
- Game System Picker
- Scenario/Save Picker
- Tabletop screen placeholder

Exit criteria:
- User can run the launch flow via MVU state transitions.

## Phase 3 - GameSetup Feature Slice
Create `Features/GameSetup`:
- `GameSetupState.cs`
- `GameSetupActions.cs`
- `GameSetupUpdate.cs`
- `GameSetupPage.razor`

Responsibilities:
- Select game system
- Select new/load mode
- Select session source
- Dispatch action to open Tabletop

Exit criteria:
- Setup flow no longer depends on `src/*` orchestration logic.

## Phase 4 - Tabletop Feature Slice
Create `Features/Tabletop`:
- `TabletopState.cs`
- `TabletopActions.cs`
- `TabletopUpdate.cs`
- `TabletopPage.razor`
- `Components/*` (`BoardView`, `TokenView`, `ToolbarView`, `DicePanel`)

Initial supported actions:
- Select token
- Move token
- Rotate token
- End turn/advance phase

Exit criteria:
- Tabletop interaction loop runs from MVU only.

## Phase 5 - Settings Feature Slice
Create `Features/Settings`:
- `SettingsState.cs`
- `SettingsActions.cs`
- `SettingsUpdate.cs`
- `SettingsPage.razor`

Include feature toggles (from views doc):
- Multiplayer
- Chat
- Fog of War
- Rules Engine
- Persistence

Exit criteria:
- Settings UI is fully state-driven via MVU.

## Phase 6 - Services and Build Cutover
1. Move persistence concerns behind `Services/*` (`SaveGameService`, `LoadGameService`, `ModuleImportService`).
2. Update MVU update handlers to call services through explicit commands/effects.
3. Staged project cutover:
   - Stage A: keep `src/*` compiled while MVU coverage is partial.
   - Stage B: exclude migrated `src/*` files/folders from compile.
   - Stage C: remove remaining `src/*` compile includes once parity is reached.

Exit criteria:
- Main app behavior compiles and runs without runtime dependency on retired `src/*` feature paths.

---

## Vertical Slice Order (Recommended)
Implement in this order to keep visible progress:
1. Workspace shell navigation slice
2. Game setup slice
3. Tabletop basic token interaction slice
4. Save/load service hookup slice
5. Settings/toggles slice

---

## Definition of Done per Slice
A slice is complete when:
1. State is represented in `State.cs`.
2. User intents are represented in `Actions.cs`.
3. All transitions are in `Update.cs`.
4. Razor page/components render only from state and dispatch actions.
5. No direct UI mutation bypassing update flow.
6. Tests cover update transitions for the slice.

---

## Risks and Controls
- Risk: mixed architecture drift.
  - Control: reject new non-MVU feature code paths.
- Risk: duplicated logic during transition.
  - Control: migrate feature-by-feature and delete old path after parity.
- Risk: hidden `src/*` coupling.
  - Control: compile exclusion in phases and fix remaining references.

---

## Immediate Next Tasks
1. Add initial `Models/TabletopState.cs` and supporting model records.
2. Add `Features/GameSetup/*` skeleton files with one end-to-end action.
3. Add `Features/Tabletop/*` skeleton with select-token action.
4. Add migration status table in this file and mark first slice In Progress.

---

## Migration Status (Initial)
- Phase 0: In Progress
- Phase 1: Planned
- Phase 2: In Progress
- Phase 3: Planned
- Phase 4: Planned
- Phase 5: Planned
- Phase 6: Planned

## Slice Progress Log

### Slice 1 - Workspace shell navigation slice (Completed)
Implemented:
- `Features/WorkspaceLaunch/WorkspaceLaunchActions.cs`
- `Features/WorkspaceLaunch/WorkspaceLaunchUpdate.cs`
- `Components/Pages/WorkspaceLaunchPage.razor`
- `Components/Layout/NavMenu.razor` link: `workspace-launch`

Current behavior:
- State-driven flow: ChooseGameSystem -> ChooseScenarioMode -> ChooseSavedGame/ChooseNewScenario -> ConfirmLaunch
- All user intents dispatch typed actions and reduce through `WorkspaceLaunchUpdate.Reduce`
- Launch confirmation provides navigation to `/workspace`

Completed notes:
- Added update tests for action transitions.
- Connected picker data to service-backed game systems and saved-session sources in `WorkspaceLaunchPage.razor`.

### Slice 2 - Game setup slice (Completed)
Implemented:
- `Features/GameSetup/GameSetupState.cs`
- `Features/GameSetup/GameSetupActions.cs`
- `Features/GameSetup/GameSetupUpdate.cs`
- `Components/Pages/GameSetupPage.razor`
- `Components/Layout/NavMenu.razor` link: `game-setup`
- `MIMLESvtt.Tests/GameSetupUpdateTests.cs`

Current behavior:
- State-driven flow: ChooseGameSystem -> ChooseSessionMode -> ChooseSessionSource -> ConfirmLaunch
- Typed actions dispatch through `GameSetupUpdate.Reduce`
- Launch path is generated from state and mode (`/tabletop?mode=edit|play&source=...`)

Next in this slice:
- Replace seeded options with service-backed session/game system sources.
- Wire selected setup output into workspace startup state.

Completed notes:
- Launch execution now normalizes missing launch path inside reducer effect flow before launch validation/effect emission, removing nullable launch-path warning risk in StageB builds.

### Slice 3 - Tabletop basic token interaction slice (In Progress)
Implemented:
- `Features/Tabletop/TabletopState.cs`
- `Features/Tabletop/TabletopActions.cs`
- `Features/Tabletop/TabletopUpdate.cs`
- `Components/Pages/TabletopPage.razor`
- `Components/Pages/BoardView.razor`
- `Components/Pages/ToolbarView.razor`
- `Components/Pages/DicePanel.razor`
- `Components/Layout/NavMenu.razor` link: `tabletop`
- `MIMLESvtt.Tests/TabletopUpdateTests.cs`

Current behavior:
- State-driven actions for select token, move token, rotate token, end turn, set active tool, and roll d6.
- Board, toolbar, and dice UI dispatch typed actions to reducer.
- Turn/status and selected token render from state only.

Completed notes:
- Tabletop components were moved into `Features/Tabletop/Components` and `TabletopPage.razor` now imports that namespace for feature-local component resolution.

Next in this slice:
- Connect tabletop state to workspace/game setup selected context.

Completed notes:
- Launch routing from setup flows now targets MVU tabletop (`/tabletop?mode=...&source=...`) instead of legacy workspace route.
- `Components/Pages/TabletopPage.razor` now hydrates launch mode/source from query and applies state initialization/status from that selected setup context.

### Slice 4 - Save/load service hookup slice (In Progress)
Implemented:
- `Services/LoadGameService.cs`
- `Services/SaveGameService.cs`
- `Services/ModuleImportService.cs`
- `Program.cs` DI registration for the new services
- `Components/Pages/GameSetupPage.razor` now hydrates game systems and session sources from services

Current behavior:
- New Game setup uses module/gamebox list from `ModuleImportService`.
- Load Saved Game setup uses known sessions from `LoadGameService`.
- Launch action invokes `SaveGameService.CreateNewSession` or `LoadGameService.OpenSession` before navigation.
- Launch command execution is now routed through explicit MVU effect handling (`ExecuteLaunchAction` -> `ExecuteLaunchEffect` -> page effect runner).

Next in this slice:
- Expand effect handler coverage for additional service commands.

### Slice 5 - Settings/toggles slice (In Progress)
Implemented:
- `Features/Settings/SettingsState.cs`
- `Features/Settings/SettingsActions.cs`
- `Features/Settings/SettingsUpdate.cs`
- `Components/Pages/SettingsPage.razor`
- `Components/Layout/NavMenu.razor` link: `settings`
- `MIMLESvtt.Tests/SettingsUpdateTests.cs`

Current behavior:
- State-driven settings screen for Display and Feature Toggles.
- Toggles implemented from design doc list: Multiplayer, Chat, Fog of War, Rules Engine, Persistence.
- All updates dispatch typed actions and reduce through `SettingsUpdate.Reduce`.
- Settings now load/save through service-backed storage and explicit MVU effect handling (`PersistSettingsAction` -> `PersistSettingsEffect` -> effect runner).

Next in this slice:
- Apply settings output to workspace shell behavior/theme.

### Slice 6 - Build cutover and migration hardening (In Progress)
Implemented:
- Added explicit MVU effect patterns in both GameSetup and Settings slices for service command execution.
- Added service tests for settings persistence storage path (`SettingsPreferenceServiceTests`).
- Added service adapter tests for load/save flow (`SaveLoadServicesTests`) covering session listing, open, create, and save paths.
- Added staged compile-inclusion switch in `MIMLESvtt.csproj` (`LegacyCompileStage` / `UseLegacySrcCompile`) to prepare controlled `src/*` exclusion after StageA.
- Ran StageB validation build: `dotnet build MIMLESvtt.csproj -p:LegacyCompileStage=StageB`.
- Added temporary StageB compatibility bridge include group in `MIMLESvtt.csproj` for required `src/*` paths while MVU replacements are completed.
- Re-ran StageB validation build and reached successful compile (`Build succeeded with warnings`).

Current behavior:
- Multiple slices now run action -> reducer -> effect -> service execution.
- Settings persistence is validated with reducer effect tests + service round-trip tests.
- Load/save service adapters now have direct test coverage to support staged cutover confidence.
- StageA default keeps current compile behavior; non-StageA mode conditionally removes `src/**/*.cs` except legacy compatibility files.
- StageB now compiles successfully using the compatibility bridge include set (trimmed to `src/Application/Authentication`, `src/Application/Workspace`, `src/Domain/Models`, `src/Domain/Persistence`, `src/Domain/Rules`, and `src/Domain/Actions`).

Completed notes:
- Added route compatibility redirect page `Components/Pages/WorkspaceRedirectPage.razor` so `/workspace` now forwards to MVU tabletop (`/tabletop`) while preserving `mode` and `source` query values.
- Moved legacy workspace shell route from `/workspace` to `/workspace-legacy` to keep the old path available during pass-by-pass cutover.
- Updated `Components/Layout/NavMenu.razor` so primary "Workspace" navigation now points at MVU tabletop while exposing an explicit "Workspace (Legacy)" entry for transition access.
- Removed unused `sessionPath` dependency from legacy workspace page open-session path and now reuse `WorkspaceService.State.CurrentFilePath` with an info severity message when no file path is available.
- Cleared remaining StageB compiler warnings by normalizing nullable CSV fields before trim usage in `EquipmentContentMapper`, `TreasureContentMapper`, `MonsterContentMapper`, and `MagicItemContentMapper`.
- Added `Services/CheckersMoveValidationService.cs` and switched `ActionValidationService` to consume the service-owned checkers validator namespace instead of `src/Domain/Rules` for move validation.
- Removed direct runtime registration of legacy rule randomization services from `Program.cs` and trimmed StageB compatibility bridge by removing `src/Domain/Rules/**/*.cs` include.
- Moved authentication runtime types (`AuthUser`, `AuthDbContext`, `AuthSeedDefaults`, `AuthDataSeeder`) from `src/Application/Authentication` to `Services/Authentication` while preserving namespace and serializer/DI contracts.
- Trimmed StageB compatibility bridge by removing `src/Application/Authentication/**/*.cs` include after authentication type move.
- Moved `WorkspaceBoardState` from `src/Application/Workspace` to `Services/Workspace` (`MIMLESvtt.Services.Workspace` namespace) and updated `Workspace.razor.cs` imports to consume the service-layer board state.
- Trimmed StageB compatibility bridge by removing `src/Application/Workspace/**/*.cs` include after workspace board-state relocation.
- Moved action processor/contracts and payload types from `src/Domain/Actions` to `Services/Actions` while preserving `MIMLESvtt.src` namespace contracts for compatibility.
- Trimmed StageB compatibility bridge by removing `src/Domain/Actions/**/*.cs` include after action-type relocation.
- Moved `VttGameboxMapper`, `VttGameboxSnapshot`, and `VttGameboxSnapshotSerializer` out of `src/Domain/Persistence/VttGameboxNSPC` into `Services/Persistence/VttGameboxNSPC` while preserving original namespace contracts.
- Excluded `src/Domain/Persistence/VttGameboxNSPC/**/*.cs` from StageB compile via explicit remove in `MIMLESvtt.csproj`.
- Moved `src/Domain/Persistence/Workspace/*.cs` contract files to `Services/Persistence/Workspace` while preserving original namespace `MIMLESvtt.src.Domain.Persistence.Workspace` for compatibility.
- Excluded `src/Domain/Persistence/Workspace/**/*.cs` from StageB compile via explicit remove in `MIMLESvtt.csproj`.
- Moved `src/Domain/Persistence/VttScenario/*.cs` contract files to `Services/Persistence/VttScenario` while preserving original namespace `MIMLESvtt.src.Domain.Persistence.VttScenario` for compatibility.
- Excluded `src/Domain/Persistence/VttScenario/**/*.cs` from StageB compile via explicit remove in `MIMLESvtt.csproj`.
- Moved `src/Domain/Persistence/ActionLog/*.cs` contract files to `Services/Persistence/ActionLog` while preserving original namespace `MIMLESvtt.src.Domain.Persistence.ActionLog` for compatibility.
- Excluded `src/Domain/Persistence/ActionLog/**/*.cs` from StageB compile via explicit remove in `MIMLESvtt.csproj`.
- Moved `src/Domain/Persistence/Models/*.cs` contract files to `Services/Persistence/Models` while preserving original namespace `MIMLESvtt.src.Domain.Persistence.Models` for compatibility.
- Excluded `src/Domain/Persistence/Models/**/*.cs` from StageB compile via explicit remove in `MIMLESvtt.csproj`.
- Moved remaining `src/Domain/Persistence/Services/File`, `src/Domain/Persistence/Services/Scenario`, and `src/Domain/Persistence/Services/VttContentPackFilePersistenceService.cs` files to `Services/Persistence/Services/*` with namespace preservation.
- Moved remaining `src/Domain/Persistence/Services/Import`, `src/Domain/Persistence/Snapshot`, and `src/Domain/Persistence/VttSessionNSPC` files to `Services/Persistence/*` with namespace preservation.
- Excluded `src/Domain/Persistence/Services/File`, `src/Domain/Persistence/Services/Scenario`, `src/Domain/Persistence/Services/VttContentPackFilePersistenceService.cs`, `src/Domain/Persistence/Services/Import`, `src/Domain/Persistence/Snapshot`, and `src/Domain/Persistence/VttSessionNSPC` from StageB compile via explicit remove entries in `MIMLESvtt.csproj`.
- Cleanup pass: removed now-empty `src/Domain/Persistence` directory tree after relocation and pruned obsolete StageB include/remove entries and stale folder declarations from `MIMLESvtt.csproj`.
- Follow-up model migration pass: moved `src/Domain/Models/Testing/SubsystemTestBoundaryCatalog.cs` to `Models/Testing/SubsystemTestBoundaryCatalog.cs` and excluded `src/Domain/Models/Testing/**/*.cs` from StageB compile via explicit remove.
- Follow-up model migration pass: moved `src/Domain/Models/Visibility/VisibilityState.cs` to `Models/Visibility/VisibilityState.cs`, updated imports in `VttSession`, `PieceInstance`, scenario plan apply service, and test global usings, and excluded `src/Domain/Models/Visibility/**/*.cs` from StageB compile.
- Bulk model merge pass: moved all remaining `src/Domain/Models/**` files into `Models/**` (using `Models/DomainLegacy/*` for overlapping folders to avoid overwrite collisions during fast merge) and validated StageB + test pass.
- Consolidation follow-up: moved non-conflicting `Models/DomainLegacy` files into final `Models/*` folders (root, `Placement`, `Surfaces`, `VttGamebox`, and partial `Pieces`) and removed now-empty `src/Domain/Models` directory tree.
- Kept overlapping legacy `PieceDefinition`/`PieceInstance` files under `Models/DomainLegacy` (renamed to `LegacyPieceDefinition.cs` and `LegacyPieceInstance.cs`) to preserve both legacy and MVU piece-model sets until targeted reconciliation.
- Reconciliation follow-up: moved `LegacyPieceDefinition.cs` and `LegacyPieceInstance.cs` into `Models/Pieces` and removed now-empty `Models/DomainLegacy` staging folder; both legacy and MVU piece models now co-locate under `Models/Pieces` for subsequent naming/contract cleanup.
- Rules relocation pass: moved remaining `src/Domain/Rules/**` files into `Services/Rules/**` as a fast file-location migration while keeping namespaces/behavior unchanged.
- For StageB stability, temporarily re-enabled `src/Domain/Rules/**/*.cs` include in `MIMLESvtt.csproj` until rules namespace contract cleanup is completed in a follow-up pass.

StageB validation findings (current tracked legacy dependency areas):
- `Components/Pages/Workspace.razor` and `.razor.cs` still depend on legacy `src/*` workspace/session model and command types.
- `Components/Pages/ContentImportWorkflowShell.razor.cs` still depends on legacy `src/*` CSV import model types.

Completed notes:
- Added `Services/ContentImportWorkflowService.cs` and moved import workflow shell preview execution behind this service path.
- `ContentImportWorkflowShell.razor(.cs)` now uses service-side workflow models for stage, duplicate policy, preview result, and issue severity instead of direct legacy CSV contract types in the UI layer.
- Added non-DI fallback service activation in `ContentImportWorkflowShell` validation path to preserve existing test-hook execution behavior while service injection is the default runtime path.
- Added `Services/ContentManualEntryWorkflowService.cs` and refactored `ContentManualEntryWorkflowShell.razor` to consume service-owned workflow option models instead of direct `src/Application/Content/ManualEntry` registry/adapter types.
- Trimmed StageB compatibility bridge by removing `src/Application/Content/**/*.cs` include after service-side replacement of shell workflow dependencies.

Next in this slice:
- Prioritize replacement of `Workspace` page path with MVU tabletop shell usage to remove the largest legacy dependency cluster.
- Trim compatibility bridge include paths pass-by-pass as MVU replacements reach parity.

### Slice 7 - Workspace launch service parity pass (In Progress)
Implemented:
- `Components/Pages/WorkspaceLaunchPage.razor` now hydrates game systems via `ModuleImportService` and saved session options via `LoadGameService`.
- Launch target path now reflects mode + selected scenario source (`/workspace?mode=...&source=...`).
- Added `MIMLESvtt.Tests/WorkspaceLaunchServiceHydrationTests.cs` to validate service hydration and saved-game step behavior.
- Added explicit launch action/effect flow for WorkspaceLaunch (`ExecuteLaunchAction` -> `NavigateToWorkspaceEffect` -> page effect runner).
- Added reducer effect coverage in `MIMLESvtt.Tests/WorkspaceLaunchUpdateTests.cs` for launch success and validation paths.

Current behavior:
- Workspace launch selection UI now uses live service-backed options instead of static seed lists.
- Scenario list updates when session mode changes.
- Workspace open action now uses reducer-emitted navigation effect for MVU parity with GameSetup.

Next in this slice:
- Reuse a shared setup source model between WorkspaceLaunch and GameSetup to remove duplicated setup logic.

### Slice 8 - Shared setup source consolidation (In Progress)
Implemented:
- Added `Services/WorkspaceSetupOptionsService.cs` as shared setup source provider returning `GameSystemSummary` and `ScenarioSummary` models.
- Updated `WorkspaceLaunchPage.razor` to use `WorkspaceSetupOptionsService` for game system and scenario hydration.
- Updated `GameSetupPage.razor` to use `WorkspaceSetupOptionsService` for game systems and saved session sources.
- Added `MIMLESvtt.Tests/WorkspaceSetupOptionsServiceTests.cs` coverage for shared provider mappings.
- Moved GameSetup new-scenario option generation to `WorkspaceSetupOptionsService.ListNewScenarios(...)` for full setup source parity.

Current behavior:
- WorkspaceLaunch and GameSetup now consume the same setup source provider and model mapping path.
- Duplicated setup hydration logic across the two pages is reduced, including new-scenario option generation.

Next in this slice:
- Introduce small shared UI component(s) for setup option lists if further duplication remains.

### Slice 9 - Shared setup UI component consolidation (In Progress)
Implemented:
- Added `Components/Shared/SetupOptionButtonList.razor` to centralize setup option button list rendering.
- Updated `GameSetupPage.razor` to use `SetupOptionButtonList` for game system, session mode, and session source selection.
- Updated `WorkspaceLaunchPage.razor` to use `SetupOptionButtonList` for game system, scenario mode, and scenario selection.
- Added focused source-level assertions for shared setup UI wiring and empty-state messages in `HomeGameSelectorTests.SharedSetupOptionList_Wiring_IsPresentInSetupPages`.
- Added explicit empty-state message parameters on shared option list usage in both setup pages.
- Added `Components/Shared/SetupOptionButtonTone.cs` and tone-based defaults in `SetupOptionButtonList` to centralize selected/unselected style semantics.
- Updated setup pages to use `Tone` instead of repeating button class mappings.
- Added `Components/Shared/SetupStepNavigationRow.razor` and moved setup Back/Continue row usage in both setup pages to the shared component.
- Added shared disable-state bindings (`DisableBack`, `DisableContinue`) in both setup pages for `SetupStepNavigationRow`.
- Added feature-level navigation rule helpers (`GameSetupNavigationRules`, `WorkspaceLaunchNavigationRules`) for step-based Back/Continue enablement logic.
- Added reducer-level test coverage for navigation rule conditions in `GameSetupUpdateTests` and `WorkspaceLaunchUpdateTests`.
- Added shared readiness model `Models/Workspace/SetupNavigationReadiness.cs` and centralized rule evaluator `Services/SetupNavigationRulesService.cs`.
- Refactored `GameSetupNavigationRules` and `WorkspaceLaunchNavigationRules` to map feature state into shared readiness and reuse central rule evaluation.
- Added shared rule tests in `SetupNavigationRulesServiceTests`.
- Added shared launch-readiness model `Models/Workspace/SetupLaunchReadiness.cs` and centralized launch evaluator `Services/SetupLaunchRulesService.cs`.
- Refactored execute-launch guards in `GameSetupUpdate` and `WorkspaceLaunchUpdate` to use feature navigation launch-rule helpers backed by shared launch readiness evaluation.
- Added launch readiness tests in `SetupLaunchRulesServiceTests` plus feature rule tests in `GameSetupUpdateTests` and `WorkspaceLaunchUpdateTests`.
- Added shared readiness builder `Services/SetupReadinessBuilder.cs` to centralize setup readiness object construction.
- Refactored feature navigation rule classes to use `SetupReadinessBuilder` for both navigation and launch readiness mapping.
- Added builder mapping tests in `SetupReadinessBuilderTests`.
- Added shared selection mapping service `Services/SetupSelectionMappingService.cs` for setup view-model conversion (`GameSystemSummary`/`ScenarioSummary` -> feature option/card models).
- Refactored `GameSetupPage.razor` and `WorkspaceLaunchPage.razor` to use `SetupSelectionMappingService` for setup option mapping paths.
- Added mapping tests in `SetupSelectionMappingServiceTests`.
- Added shared selection fallback policy service `Services/SetupSelectionDefaultsService.cs`.
- Refactored setup pages to use shared selected-id fallback resolution for game systems and scenario/session source lists.
- Added fallback policy tests in `SetupSelectionDefaultsServiceTests`.
- Added shared setup refresh orchestration helper `Services/SetupRefreshWorkflowService.cs` to centralize setup sequencing (`source -> map -> fallback -> selected-id`).
- Refactored `GameSetupPage.razor` and `WorkspaceLaunchPage.razor` refresh paths to use the shared workflow helper.
- Added workflow helper tests in `SetupRefreshWorkflowServiceTests`.

Current behavior:
- Setup selection UIs now use one shared component and handler pattern.
- Repeated button list markup across setup pages is removed.
- Empty-list messaging for setup options is now explicit and consistent per page context.
- Setup button tone semantics (primary/success/secondary) are now shared defaults rather than per-page class duplication.
- Setup step navigation row markup is now shared between GameSetup and WorkspaceLaunch pages.
- Setup step navigation availability now follows shared state-driven disable rules in both setup pages.
- Navigation disable rules are now reusable and test-anchored at feature logic level rather than page-local inline expressions.
- Cross-feature navigation readiness behavior now flows through a shared rule model for consistent enable/disable semantics.
- Launch execution guards now also flow through shared readiness rule evaluation for consistent pre-launch validation behavior.
- Readiness mapping boilerplate is now centralized and tested, reducing duplicated per-feature object construction code.
- Setup selection transformation mapping boilerplate is now centralized and tested, reducing duplicated page-level conversion code.
- Setup selected-id defaulting behavior is now centralized and tested, reducing duplicated page-level fallback branching.
- Setup refresh sequencing is now centralized and tested, reducing duplicated page-level orchestration code across setup pages.

Next in this slice:
- Evaluate extracting the selected-game-system summary projection used by both setup pages into a shared helper to reduce remaining inline projection duplication.

---

## Completed slices

- Content manual-entry workflow cleanup pass:
  - Removed legacy `src/Application/Content/ManualEntry/*` adapter/registry/interface files.
  - Kept the Blazor shell on the service-backed workflow model (`ContentManualEntryWorkflowService`).
  - Updated `MIMLESvtt.Tests/ContentManualEntryWorkflowTests.cs` to validate service options instead of legacy adapters.
  - Validation: workspace build succeeded, StageB app build succeeded, and tests passed (88/88).
- StageB bridge trim pass (rules include cleanup):
  - Removed StageB compile include for `src/Domain/Rules/**/*.cs` from `MIMLESvtt.csproj` now that rules files were relocated.
  - Removed stale project folder declaration `src/Application/Content/ManualEntry/` from `MIMLESvtt.csproj`.
  - Validation: StageB app build succeeded and tests passed (88/88).
- StageB bridge trim pass (domain models include cleanup):
  - Removed the remaining StageB compile bridge block for `src/Domain/Models/**/*.cs` and related Testing/Visibility removes from `MIMLESvtt.csproj`.
  - StageB now compiles without any `src/Domain/Models` bridge include.
  - Validation: StageB app build succeeded and tests passed (88/88).
- Legacy source cleanup pass (domain rules/models directories):
  - Removed now-redundant legacy source directories `src/Domain/Rules` and `src/Domain/Models` after prior relocations to `Services/Rules` and `Models`.
  - Validation: StageB app build succeeded and tests passed (88/88).
- Legacy source cleanup pass (domain persistence directory):
  - Removed now-redundant legacy source directory `src/Domain/Persistence` after prior relocation to `Services/Persistence`.
  - Validation: StageB app build succeeded and tests passed (88/88).
- Legacy source cleanup pass (remaining src/Domain tree):
  - Removed remaining legacy source tree `src/Domain` after prior migration of Models, Rules, and Persistence to `Models` and `Services`.
  - Validation: StageB app build succeeded and tests passed (88/88).
- Legacy source retirement pass (src root removal):
  - Relocated `src/Legacy/LegacyCompatibilityTypes.cs` and `src/Legacy/InternalsVisibleTo.Tests.cs` to `Legacy/*` (same content/namespace contracts).
  - Removed obsolete `src`-based staged compile toggle/include blocks from `MIMLESvtt.csproj`.
  - Removed the now-empty `src` root directory.
  - Validation: StageB app build succeeded and tests passed (88/88).
- Project metadata cleanup pass (post-src retirement):
  - Removed stale `src/Legacy/` folder declaration from `MIMLESvtt.csproj` after compatibility file relocation to `Legacy/`.
  - Validation: build succeeded and tests passed (88/88).
