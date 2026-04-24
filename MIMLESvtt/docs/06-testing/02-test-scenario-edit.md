# Test — Scenario Edit

## Purpose

Define QA checks for scenario edit workflows and state safety.

This page covers scenario-candidate edit/review paths used before final activation.

## Scope

Included:
- loading scenario-like state into editable context
- editing scenario metadata and content fields
- save/cancel behavior with visible feedback
- return flow integrity after save/cancel
- activation-readiness checks after edit

Not included:
- packet-level import validation internals (covered by import pipeline docs)

## Preconditions

- Authenticated user with scenario-edit access.
- At least one scenario or scenario-candidate context exists.

---

## Mode Expectations

### Edit mode
- Existing values load into editor controls.
- Save updates existing scenario/candidate record.

### Cancel flow
- Cancel exits without persistence.
- User returns to previous safe context (detail/list/workflow shell).

---

## Validation Expectations

- Required fields block save when empty.
- Typed/structured fields block save when malformed.
- Save failures show visible summary feedback.
- Save successes show visible success confirmation.

## Test Cases

### SCN-001
**Title:** Scenario edit screen loads existing values  
**Priority:** High

**Steps:**
1. Open scenario edit for an existing scenario.

**Expected:**
- Existing values are pre-populated.
- Editor mode indicates edit context.

### SCN-002
**Title:** Required-field validation blocks invalid save  
**Priority:** High

**Steps:**
1. Clear required fields.
2. Attempt save.

**Expected:**
- Save is blocked.
- Inline/form-level validation feedback is visible.

### SCN-003
**Title:** Save persists valid edits  
**Priority:** High

**Steps:**
1. Modify valid scenario fields.
2. Save.

**Expected:**
- Changes persist.
- Success message appears.
- Detail/list views reflect updates.

### SCN-004
**Title:** Cancel edit does not persist changes  
**Priority:** High

**Steps:**
1. Modify fields.
2. Cancel.

**Expected:**
- No mutation is persisted.
- User returns safely to prior context.

### SCN-005
**Title:** Invalid structured field input reports actionable errors  
**Priority:** Medium

**Steps:**
1. Enter malformed structured values where applicable.
2. Save.

**Expected:**
- Save is blocked.
- Error identifies failing field/category.

### SCN-006
**Title:** Edit-save keeps scenario activation path coherent  
**Priority:** Medium

**Steps:**
1. Open a scenario candidate.
2. Edit valid metadata/content fields.
3. Save and return to scenario workflow context.

**Expected:**
- Candidate remains accessible for activation flow.
- No stale pre-edit values are shown in detail/review panel.

### SCN-007
**Title:** Cancel after edits preserves pre-edit scenario data  
**Priority:** High

**Steps:**
1. Open existing scenario in edit mode.
2. Modify multiple fields.
3. Cancel.
4. Re-open scenario details.

**Expected:**
- Original values are preserved.
- No partial edit fragments are persisted.

---

## Run Logging Notes

- Record pass/fail per case ID in `96-qa-run-log.md`.
- Log any save/cancel inconsistency as P1 or higher when data integrity is affected.

## Cross References

- `96-qa-run-log.md`
- `97-testing-strategy.md`
- `98-pre-alpha-testing-packet.md`
- `03-test-library-management.md`
