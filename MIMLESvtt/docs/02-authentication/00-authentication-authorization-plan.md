# Authentication and Authorization Plan

## Goal

Add basic authentication/authorization with a default simple admin account, and make join-code behavior persist across application restarts.

## Scope

- In scope:
  - App sign-in/sign-out flow.
  - Default admin identity and role assignment.
  - Authorization checks for admin-only actions.
  - Persisted join-code registry (durable across restarts).
- Out of scope for this pass:
  - OAuth/social login.
  - Multi-tenant identity providers.
  - Full production hardening (MFA, external secrets rotation, etc.).

## Current State Notes

- Workspace already has an admin gate concept (`CanCreateSession`) in `VttSessionWorkspaceService`.
- Join code is currently derived from file name (`BuildJoinCode`) and matched against known session entries.
- Snapshot file library paths can already be persisted via `SnapshotFileLibraryPersistenceService`.

## Proposed Design

### 1) Authentication Foundation (Blazor + ASP.NET Core Identity)

- Add ASP.NET Core Identity with cookie auth.
- Use EF Core store for users/roles.
- Introduce seeded defaults on startup:
  - Role: `Admin`
  - Default admin user (simple initial credentials, force-change path can be added later).

### 2) Authorization Model

- Add role checks for admin-only operations:
  - Create new session from selector.
  - Potentially other management actions as needed.
- Replace or back `SetCanCreateSession` from authenticated role state so UI and service checks align.

### 3) Join-Code Persistence Across Restarts

- Add a persistent join-code registry file/document:
  - Suggested fields: `JoinCode`, `SessionPath`, `CreatedByUserId`, `CreatedUtc`, `LastUsedUtc`, `IsActive`.
- Resolve join flow by registry first, then fallback to existing file-name strategy for backward compatibility.
- Keep registry and snapshot library in sync when sessions are added/removed/renamed.

### 4) UI Changes

- Add auth pages/components:
  - Login
  - Logout
  - Access denied (or inline message)
- On selector/workspace, show current user and role.
- For admin-only actions, hide/disable controls for non-admin users and enforce server-side/service-side checks.

## Implementation Phases

### Phase A - Identity Bootstrapping

1. Configure Identity + cookie auth in app startup.
2. Add user/role data store and migrations.
3. Add default admin seeding.
4. Add login/logout UI and basic authorization pipeline.

### Phase B - Role-Based Workspace Integration

1. Wire authenticated role into workspace state.
2. Enforce admin requirement for session creation and other protected actions.
3. Update selector/workspace UI to reflect authorization state.

### Phase C - Join-Code Registry Persistence

1. Add join-code registry model + persistence service.
2. Write entries when sessions are saved/registered.
3. Read entries at startup.
4. Update join flow to use persisted registry and maintain backward-compatible fallback.

### Phase D - Validation and Hardening

1. Add tests for:
   - default admin seeding,
   - auth success/failure,
   - admin-only action enforcement,
   - join-code persistence across restart simulation.
2. Add clear user-facing error messages for invalid/expired join codes.

## Acceptance Criteria

1. On clean startup, a default admin account exists and can sign in.
2. Non-admin user cannot execute admin-only actions.
3. Join codes continue to work after app restart without requiring re-subscription.
4. Existing join-by-file-name behavior still works for legacy/session sample paths.
5. Build passes and targeted auth/join-code tests pass.

## Risks and Mitigations

- Risk: breaking existing local workflows with strict auth.
  - Mitigation: keep fallback behavior and start with minimal required protected actions.
- Risk: stale join-code mappings after file moves.
  - Mitigation: validate path existence on join and provide recovery/update flow.
- Risk: weak default admin credentials.
  - Mitigation: document dev-only default, require change for non-dev environments in follow-up.
