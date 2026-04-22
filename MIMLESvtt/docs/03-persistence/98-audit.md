# Persistence Audit (Design vs Code vs Backlog)

## Scope

This audit maps persistence design intent to implemented code and persistence backlog expectations.

Requested extraction dimensions:

1. Objects persisted
2. Persistence actions
3. Persistence state update events

It also identifies variances between:
- `03-persistence` design docs
- `src/Domain/Persistence` implementation
- `05-backlog` persistence planning

---

## 1) Objects Persisted

## A. Snapshot objects persisted to disk

Implemented persisted object families:

1. **Session snapshot** (`.session.json`)
- Root shape: `{ Version, VttSession }`
- Types: `VttSessionSnapshot`, `VttSessionSnapshotSerializer`, `VttSessionFilePersistenceService`

2. **Scenario snapshot** (`.scenario.json`)
- Root shape: `{ Version, Scenario }`
- Types: `ScenarioSnapshot`, `ScenarioSnapshotSerializer`, `ScenarioFilePersistenceService`

3. **VTT content pack snapshot** (`.vttcontentpack.json`)
- Root shape: `{ Version, Manifest, Definitions, Assets }`
- Types: `VttContentPackSnapshot`, `VttContentPackSnapshotSerializer`, `VttContentPackFilePersistenceService`

3b. **App-facing VTT content pack model**
- Type: `VttContentPack`
- Mapping: `VttContentPackMapper` (`VttContentPack` <-> `VttContentPackSnapshot`)

4. **Action log snapshot** (`.actionlog.json`)
- Root shape: `{ Version, SessionId, Actions }`
- Types: `ActionLogSnapshot`, `ActionLogSnapshotSerializer`, `ActionLogFilePersistenceService`

5. **Workspace recovery state** (separate workspace-state file)
- Purpose: restore current file/pending scenario context and operation history
- Types: `SessionWorkspaceRecoveryState`, `SessionWorkspaceStatePersistenceService`

## B. Runtime persistence-related state objects (not snapshot payload roots)

- `SessionWorkspaceState`
  - `CurrentVttSession`
  - `CurrentFilePath`
  - `CurrentSnapshotFormat`
  - `IsDirty`
  - `CurrentPendingVttScenarioPlan`
  - `PendingVttScenarioSourcePath`
  - operation history and undo/redo stacks

- `PendingVttScenarioApplicationPlan`
  - non-mutating intermediate for scenario activation flow

---

## 2) Persistence Actions

## A. File save/load actions (direct)

From `SnapshotFileWorkflowService`:
- `SaveVttSession`
- `LoadVttSession`
- `SaveScenario`
- `LoadScenario`
- `SaveVttContentPack` (snapshot + app-model overloads)
- `LoadVttContentPack` (snapshot)
- `LoadVttContentPackModel` (app model)
- `SaveActionLog`
- `LoadActionLog`

All enforce extension guardrails via `SnapshotFileExtensions`.

## B. File import/apply actions (orchestrated)

From `SnapshotFileImportApplyWorkflowService`:
- detect format by extension
- import and apply:
  - VttSession path -> replace runtime session (policy-controlled)
  - Scenario path -> scenario activation workflow (dry-run/activate)
  - ContentPack/ActionLog path -> import-only outcome

## C. Workspace-facing persistence actions

From `SessionWorkspaceService` (`WorkspaceOperationKind`):
- `OpenVttSessionFromFile`
- `SaveCurrentSession`
- `SaveCurrentSessionAs`
- `SaveCurrentLayoutAsScenario`
- `ImportScenarioToPendingPlanFromFile`
- `ActivatePendingScenario`
- `SaveWorkspaceState`
- `RestoreWorkspaceState`

Additional related operation kinds that interact with persisted runtime state include `CreateNewSession`, `ProcessAction`, undo/redo, etc.

---

## 3) Persistence State Update Events (What actually changes state)

No explicit domain event objects were found for persistence state transitions.

Current state updates are operational side-effects in service methods.

Key state update points:

## A. Session lifecycle and file path state

- On open session:
  - `State.CurrentVttSession` replaced
  - `State.CurrentFilePath` set
  - `State.CurrentSnapshotFormat` set
  - `State.IsDirty = false`
  - pending scenario state cleared

- On save/save-as:
  - file written
  - current file path may be set (save-as)
  - `State.IsDirty = false`

## B. Scenario flow state

- Import scenario to pending:
  - `State.CurrentPendingVttScenarioPlan` set
  - `State.PendingVttScenarioSourcePath` set
  - no runtime session mutation in dry-run path

- Activate pending scenario:
  - candidate produced and activated
  - `State.CurrentVttSession` replaced
  - pending scenario state cleared
  - `State.IsDirty = true`

## C. Workspace recovery state

- Save workspace state:
  - recovery snapshot persisted

- Restore workspace state:
  - may restore current session from file
  - may rebuild pending scenario plan
  - resets dirty and undo/redo state
  - preserves/rehydrates operation history

## D. Operation telemetry state (pseudo-event trail)

Instead of formal persistence events, system records:
- `WorkspaceOperationEntry` records in `State.OperationHistory`
- `State.LastOperationMessage`
- `State.LastOperationSeverity`

This is currently the closest implemented “state update event” trail.

---

## Variances (Design vs Code vs Backlog)

## Variance 1 — Event model expectation vs implementation

Design/backlog language implies explicit action/event progression. Persistence implementation mostly uses service-level operation logging (`WorkspaceOperationEntry`) rather than explicit persistence event contracts.

Impact:
- lower formal traceability for persistence transitions
- operation history works, but typed event stream for persistence is absent

## Variance 2 — Naming drift in docs vs current numbered docs

Some docs/backlog references still use older unnumbered names in narrative contexts, while actual files are sequence-prefixed.

Impact:
- medium documentation navigation confusion
- low runtime risk

## Variance 3 — Backlog status mismatch in persistence-infrastructure backlog

`09-backlog-persistence-infrastructure.md` still shows many PI items as pending, while `00-backlog.md` persistence section marks broad implementation completed.

Impact:
- planning inconsistency
- hard to tell whether PI backlog is design debt, implementation debt, or stale status

## Variance 4 — Consolidated persistence doc includes duplicated conceptual sections

`00-persistence-subsystem.md` now includes merged policy + implementation content and has some overlap (snapshot-type descriptions appear in multiple sections).

Impact:
- readability cost
- not a functional code issue

## Variance 5 — Runtime object naming compatibility layering

Code currently carries compatibility naming (e.g., `VttSession`, `VttSession`, `TabletopState` aliases in other areas), while design docs increasingly use tabletop-centric terminology.

Impact:
- terminology friction during refactor planning
- serializer contracts remain stable (good)

---

## Backlog Alignment Snapshot

From `00-backlog.md` persistence section, major implementation items are marked complete:
- save/load, import/export, version handling, validation
- import/apply pipeline layers
- scenario activation workflow and pending-plan handling
- workspace recovery persistence and diagnostics

From `09-backlog-persistence-infrastructure.md`, PI items remain mostly pending and read as design/planning tasks.

Interpretation:
- `00-backlog.md` is tracking implementation reality.
- `09-backlog-persistence-infrastructure.md` appears to be lagging and should be reclassified as:
  - design-governance follow-up,
  - or updated to reflect implemented status.

---

## Refactor Progress Update (Phase 1 + Phase 2)

Completed in code:

1. Added app-facing model: `VttContentPack`.
2. Added explicit mapper: `VttContentPackMapper`.
3. Added app-facing save/load surfaces:
   - `VttContentPackFilePersistenceService.SaveToFile(VttContentPack, path)`
   - `VttContentPackFilePersistenceService.LoadModelFromFile(path)`
   - `SnapshotFileWorkflowService.SaveVttContentPack(VttContentPack, path)`
   - `SnapshotFileWorkflowService.LoadVttContentPack(path)`
4. Preserved snapshot APIs for compatibility.

## Refactor Progress Update (Phase 3)

Completed in code:

1. Promoted app-facing content-pack workflow methods as primary API names:
   - `SaveVttContentPack(VttContentPack, path)`
   - `LoadVttContentPack(path)` returns `VttContentPack`
2. Kept explicit snapshot methods for infra-focused use:
   - `SaveVttContentPackSnapshot(VttContentPackSnapshot, path)`
   - `LoadVttContentPackSnapshot(path)`

Phase 3 variance reduction result:
- Non-persistence-facing naming is now model-first at workflow surface.
- Snapshot usage remains available but explicitly named as snapshot operations.

Open variance after Phase 1+2:
- Some messages and contracts still use “snapshot-first” wording.
- Import pipeline remains envelope-first (expected for current phase).

---

## Recommended Next Refactor Preparation (After Phase 1 + 2)

1. Decide whether persistence state transitions need explicit event contracts or if `WorkspaceOperationEntry` remains sufficient.
2. Reconcile `09-backlog-persistence-infrastructure.md` statuses against implemented code.
3. Split `00-persistence-subsystem.md` into clearly labeled sections:
   - policy/contracts
   - code implementation map
   - operational state transitions
4. Produce a concise persistence sequence diagram per major flow:
   - session save/load
   - scenario import/activate
   - workspace restore

---

## Conclusion

You were correct that the code is effectively implementing persistence state transitions; however, it does so through service operations + operation-history entries, not a formal persistence event model. Design intent, implementation reality, and backlog status are mostly aligned on capabilities, but not yet aligned on naming consistency and planning/status clarity.
