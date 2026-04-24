# Test — Login and Create Path

## Purpose

Define QA checks for authentication entry and first-session creation flow.

## Scope

Included:
- login page load and submit behavior
- authenticated navigation to workspace/content routes
- new session creation and visible feedback

Not included:
- multiplayer synchronization behavior
- deep scenario import/edit behavior (covered in `02-test-scenario-edit.md`)

## Preconditions

- App builds and starts without startup errors.
- Test user account exists (or local auth stub is enabled).

## Test Cases

### AUTH-001
**Title:** Login page renders required controls  
**Priority:** High

**Steps:**
1. Open app unauthenticated.
2. Navigate to login route.

**Expected:**
- Login form renders with required inputs and submit control.
- No broken component errors.

### AUTH-002
**Title:** Invalid credentials show clear feedback  
**Priority:** High

**Steps:**
1. Submit invalid credentials.

**Expected:**
- User remains unauthenticated.
- Visible error feedback appears.

### AUTH-003
**Title:** Valid credentials allow protected route access  
**Priority:** High

**Steps:**
1. Submit valid credentials.
2. Navigate to `/content` and `/workspace`.

**Expected:**
- Protected pages render without redirect loop.

### AUTH-004
**Title:** Create session from authenticated flow  
**Priority:** High

**Steps:**
1. Open workspace while authenticated.
2. Trigger create-session action.

**Expected:**
- Session is created.
- Success feedback is visible.

### AUTH-005
**Title:** Create-session failure path is non-destructive  
**Priority:** Medium

**Steps:**
1. Trigger create-session with forced failure condition.

**Expected:**
- Clear error feedback appears.
- No partial/corrupt session state is committed.

## Cross References

- `96-qa-run-log.md`
- `97-testing-strategy.md`
- `98-pre-alpha-testing-packet.md`