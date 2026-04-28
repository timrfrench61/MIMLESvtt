# Persistence and Infrastructure Backlog

> Status note: All backlog items in this document are currently completed/closed.

## Scope

This backlog covers:

- persistence strategy
- service/repository boundaries
- infrastructure standards
- logging and error handling

---

## Related Master Backlog Items

- MB-046 Select and define persistence strategy
- MB-047 Implement initial repository/service boundaries
- MB-048 Implement logging and error-handling standards

---

## Backlog Items

### PI-001
**Title:** Define persistence strategy  
**Master ID:** MB-046  
**Priority:** High  
**Status:** Closed  

**Description:**  
Choose and document the persistence approach for content data and live session state.

**Dependencies:** None

**Acceptance Criteria:**
- Data store direction is documented
- Content persistence is described
- Session-state persistence direction is described

**Implementation Update:**
- Direction is now file-first JSON snapshots with explicit extensions (`.vttsession.json`, `.vttscenario.json`, `.vttgamebox.json`, `.actionlog.json`).
- Runtime workspace state recovery is persisted separately with safe-write behavior and backup fallback.
- Authentication data is persisted using Identity + SQLite.

---

### PI-002
**Title:** Define content-storage boundaries  
**Master ID:** MB-046  
**Priority:** High  
**Status:** Closed  

**Description:**  
Define how reusable content is stored separately from live session state.

**Dependencies:** PI-001

**Acceptance Criteria:**
- Boundary between catalog data and live state is documented
- Import persistence path is clear
- Reuse model is clear

**Implementation Update:**
- Reusable authored content and runtime state are separated by snapshot type.
- Scenario and Gamebox files represent reusable content artifacts.
- Session snapshots represent live runtime game state and progression.

---

### PI-003
**Title:** Define session-state storage boundaries  
**Master ID:** MB-046  
**Priority:** High  
**Status:** Closed  

**Description:**  
Define how live table/session data is persisted and restored.

**Dependencies:** PI-001

**Acceptance Criteria:**
- Session-state aggregate persistence approach is documented
- Save/load granularity is defined
- Future live session sync needs are considered

**Implementation Update:**
- Session state is persisted as full aggregate snapshots through `VttSession` serialization.
- Save/load is file-based per-session with extension guardrails and format validation.
- Join-code registry and workspace recovery persistence establish a baseline for future hosted/session-sync evolution.

---

### PI-004
**Title:** Define service layer boundaries  
**Master ID:** MB-047  
**Priority:** High  
**Status:** Closed  

**Description:**  
Define the separation among UI, application services, domain logic, and infrastructure concerns.

**Dependencies:** PI-001

**Acceptance Criteria:**
- Layer responsibilities are documented
- Domain is isolated from UI concerns
- Infrastructure is accessed through defined boundaries

**Implementation Update:**
- UI uses workspace services; it does not directly serialize domain aggregates.
- Persistence concerns are handled through dedicated services (`*PersistenceService`, workflow services, import/apply services).
- Domain models remain in domain model namespaces, with infrastructure code using those models without UI coupling.

---

### PI-005
**Title:** Define repository abstraction strategy  
**Master ID:** MB-047  
**Priority:** Medium  
**Status:** Closed  

**Description:**  
Determine where repository abstractions are useful and where direct service abstractions are simpler.

**Dependencies:** PI-004

**Acceptance Criteria:**
- Strategy is documented
- Over-abstraction is avoided
- Testability remains practical

**Implementation Update:**
- Repository abstractions are intentionally minimal for file-based persistence.
- Concrete service classes are used for snapshot persistence and import workflows.
- Constructor injection and focused service responsibilities keep unit/integration testing practical without additional repository layers.

---

### PI-006
**Title:** Define import persistence flow  
**Master ID:** MB-047, MB-048  
**Priority:** Medium  
**Status:** Closed  

**Description:**  
Define how validated import data moves into persisted content storage.

**Dependencies:** PI-002, PI-004

**Acceptance Criteria:**
- Import flow is documented
- Transaction or batch behavior is described
- Partial failure policy is documented

**Implementation Update:**
- Import/apply workflow is implemented with format detection and explicit apply policies.
- Scenario import supports dry-run candidate planning before activation.
- Partial failures are surfaced with clear operation messages and no silent mutation when validation/import fails.

---

### PI-007
**Title:** Define logging strategy  
**Master ID:** MB-048  
**Priority:** Medium  
**Status:** Closed  

**Description:**  
Define structured logging for engine events, import failures, synchronization issues, and unhandled exceptions.

**Dependencies:** PI-004

**Acceptance Criteria:**
- Logging categories are documented
- Error vs informational logging is distinguished
- Operational diagnostics are anticipated

**Implementation Update:**
- Current implementation uses operation-history tracking and structured user-facing operation messages with severity levels.
- Recovery diagnostics are captured for workspace-state restore flows (main/backup source, warnings, errors).
- This provides operational traceability for persistence/import/recovery flows without introducing external logging infrastructure yet.

---

### PI-008
**Title:** Define error-handling and user feedback standards  
**Master ID:** MB-048  
**Priority:** Medium  
**Status:** Closed  

**Description:**  
Define how users see validation errors, failed operations, warnings, and recoverable issues.

**Dependencies:** PI-007

**Acceptance Criteria:**
- Error presentation levels are defined
- Validation errors are user-visible
- Unexpected failures are traceable

**Implementation Update:**
- Errors are surfaced through clear exception messages and workspace operation severity (`Success`, `Info`, `Error`).
- Validation/import/load failures are user-visible in selector/workspace UI alerts.
- Registry and workspace recovery now include fallback handling and warning visibility to aid troubleshooting.

---

## Near-Term Execution Order

1. Closed in current implementation cycle (PI-001 through PI-008 completed)
2. Next focus: expand automated persistence resilience tests and recovery diagnostics coverage
3. Next focus: add optional external/centralized logging integration when hosting model requires it

---

## Progress Snapshot (Auth + Join-Code Persistence Slice)

- Completed implementation work:
  - persistent known-session registry file with startup load and explicit save
  - persisted join-code mapping per known session path
  - admin join-code operations (set/reset/generate) with conflict checks
  - join audit metadata persisted in registry (LastJoinedUtc, LastJoinCodeUpdatedUtc)
  - UI status indicator for registry load state and troubleshooting save action

- Immediate next persistence steps:
  1. Add automated tests for registry startup reload, join-code conflict behavior, and audit timestamp updates.
  2. Harden malformed registry handling with explicit fallback/recovery messages.
  3. Add simple rotation/backup policy for registry file writes consistent with workspace-state safety approach.
