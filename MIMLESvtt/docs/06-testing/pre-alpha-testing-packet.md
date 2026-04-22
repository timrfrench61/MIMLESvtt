# Pre-Alpha Testing Packet

## Purpose

This packet defines the practical QA artifacts needed to run pre-alpha testing consistently.

It complements `testing-strategy.md` by adding execution checklists, triage rules, and repeatable workflow coverage.

---

## Scope

This packet covers:
- manual smoke testing for core workflows
- workflow-level regression checks
- visible feedback verification
- defect triage severity model
- test data catalog for repeatable runs

Out of scope:
- load/perf testing
- multiplayer sync validation
- production release gates

---

## QA Execution Model (Pre-Alpha)

## Cadence
- Per implementation block: run targeted workflow checks for changed areas.
- Daily (active dev): run smoke checklist once against latest main branch build.
- Pre-merge for major UI/domain changes: run full pre-alpha packet.

## Test Layers Used
- Unit/integration tests (`dotnet test`)
- Manual workflow smoke checks (this packet)
- Regression workflow checks for previously fixed issues

---

## Entry / Exit Criteria

## Entry Criteria
- Build succeeds.
- Automated tests pass or known failures are documented.
- Feature toggle states for test run are explicitly recorded.

## Exit Criteria
- All P0/P1 smoke items pass, or blocking defects are logged.
- Any failed workflow has a repro and severity.
- Results are recorded in a QA run note.

---

## Smoke Checklist (Pre-Alpha)

Use this as a minimum run before declaring a block “usable”.

### Core Workspace Lifecycle
- [ ] Open Workspace page and verify no dead UI controls in the first viewport.
- [ ] Create New Session shows visible success message.
- [ ] Session Summary updates session id/title/session number as expected.
- [ ] Update session title shows visible success and reflects in summary.

### Session Setup
- [ ] Add Surface succeeds and appears in summary/list controls.
- [ ] Add Piece succeeds and appears on board and in list.
- [ ] Move Piece updates board and piece coordinates.
- [ ] Rotate Piece updates board and piece rotation state.

### Piece State and Markers
- [ ] Add Marker succeeds and marker summary updates.
- [ ] Remove Marker succeeds and marker summary updates.
- [ ] Change Piece State succeeds and state value is visible.
- [ ] Remove Piece State key succeeds.

### Persistence and Scenario Flow
- [ ] Save Session path flow gives visible success/failure.
- [ ] Open Session path flow gives visible success/failure.
- [ ] Import Scenario to pending plan shows pending scenario context.
- [ ] Activate Pending Scenario updates session and clears pending plan.

### Board Interaction / Stamp / Queue / Presets
- [ ] Board click interactions produce visible success/error.
- [ ] Stamp mode placement creates multiple pieces when enabled.
- [ ] Surface strip switching updates active surface and board rendering.
- [ ] Queue/preset actions produce visible operation feedback.

### Developer Toggles and Disabled Features
- [ ] Disabling board interaction shows “Feature not enabled (development toggle)”.
- [ ] Disabling stamp-mode path blocks related flow with clear message.
- [ ] Multiplayer placeholder toggle does not silently imply implemented behavior.

---

## Workflow Regression Set

Run this set whenever workspace interaction behavior changes.

1. Basic setup flow
- create session → add surface → add piece → move piece

2. Scenario flow
- import scenario → pending plan created → activate → board/session state updated

3. Stamp flow
- enable add-at-click + stamp mode → place multiple pieces → id preview remains coherent

4. Invalid action flow
- trigger invalid move/state change → no mutation + visible error

5. Toggle-gated flow
- disable a feature toggle → attempt feature action → clear not-enabled feedback

---

## Visible Feedback Expectations

Every user-triggered operation should produce one of:
- success feedback (green)
- info/warning feedback (yellow)
- error feedback (red)

Failures must include:
- human-readable message
- no silent mutation
- no silent no-op

---

## Defect Triage Severity Policy

## Severity Levels
- **P0 – Blocker**: app cannot perform core workflow (create/add/move/save/import) or data corruption risk.
- **P1 – Critical**: workflow works inconsistently, misleading state, or high-impact UI dead interaction.
- **P2 – Major**: non-blocking but significant usability or correctness issue.
- **P3 – Minor**: polish issue, unclear text, low-risk inconsistency.

## Triage Fields (minimum)
- Title
- Area (Workspace, Persistence, Scenario, Testing, Docs)
- Repro steps
- Expected vs actual
- Severity (P0–P3)
- Feature toggle state at repro
- Build/branch reference

---

## Test Data Catalog (Pre-Alpha)

Maintain a small deterministic set:
- Empty session baseline
- Session with 1 surface + 1 piece
- Session with multiple surfaces and pieces
- Scenario file with pending-plan activation path
- Invalid input samples (duplicate ids, bad paths, bad references)

Preferred storage location:
- `docs/06-testing/test-data/` (if docs-only references)
- `MIMLESvtt.Tests` fixtures for automated execution

---

## Reporting Template

For each packet run, capture:
- Date/time
- Commit/branch
- Tester
- Toggle configuration
- Automated test result summary
- Smoke checklist pass/fail summary
- Defects logged (with severities)
- Go/No-Go recommendation for pre-alpha handoff

---

## Immediate Follow-Up Actions

1. Add lightweight `docs/06-testing/qa-run-log.md` template for run history.
2. Add explicit mapping from smoke checklist items to automated tests where available.
3. Add component-level UI tests for critical Workspace controls over time.
