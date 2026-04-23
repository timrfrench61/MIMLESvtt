# Networking Backlog

## Scope

This backlog covers:

- session hosting
- client connection model
- state synchronization
- permissions/visibility
- reconnect and recovery behavior

Initial direction is Blazor Server with real-time synchronized state.

---

## Related Master Backlog Items

- MB-039 Define networking/session hosting model
- MB-040 Implement real-time session synchronization
- MB-041 Define permissions and visibility model
- MB-042 Implement reconnect and session recovery behavior
- MB-053 Create networking synchronization test cases

---

## Backlog Items

### NW-001
**Title:** Define live-session hosting model  
**Master ID:** MB-039  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define how tables are created, hosted, identified, and joined.

**Dependencies:** None

**Acceptance Criteria:**
- Session host concept is documented
- Join flow is documented
- Session identity and lifetime are defined

---

### NW-002
**Title:** Define client connection roles  
**Master ID:** MB-039, MB-041  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define GM, player, observer, and future assistant roles from a connection perspective.

**Dependencies:** NW-001

**Acceptance Criteria:**
- Role list is documented
- Permissions assumptions are documented
- Role-to-seat relationship is defined

---

### NW-003
**Title:** Define synchronization model  
**Master ID:** MB-040  
**Priority:** High  
**Status:** Pending  

**Description:**  
Determine whether sync is event-driven, state-diff-driven, snapshot-based, or hybrid.

**Dependencies:** NW-001

**Acceptance Criteria:**
- Sync model is documented
- Tradeoffs are noted
- Chosen approach is consistent with engine state design

---

### NW-004
**Title:** Define authoritative-state strategy  
**Master ID:** MB-040  
**Priority:** High  
**Status:** Pending  

**Description:**  
Determine whether the server is authoritative for state transitions and rule outcomes.

**Dependencies:** NW-003

**Acceptance Criteria:**
- Authority model is explicit
- Conflict resolution assumptions are documented
- Client responsibilities are limited appropriately

---

### NW-005
**Title:** Define session event channel model  
**Master ID:** MB-040  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define how moves, updates, chat/log events, and state changes are transmitted.

**Dependencies:** NW-003, NW-004

**Acceptance Criteria:**
- Event categories are documented
- Ordering assumptions are documented
- Channel responsibilities are clear

---

### NW-006
**Title:** Define permissions and visibility rules  
**Master ID:** MB-041  
**Priority:** High  
**Status:** Pending  

**Description:**  
Document who can see, move, edit, or inspect which pieces or information.

**Dependencies:** NW-002

**Acceptance Criteria:**
- GM permissions are documented
- Player permissions are documented
- Observer permissions are documented
- Future hidden/private info is anticipated

---

### NW-007
**Title:** Define reconnect behavior  
**Master ID:** MB-042  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Document how disconnected clients restore session state.

**Dependencies:** NW-003, NW-004

**Acceptance Criteria:**
- Reconnect workflow exists
- State rehydration method is documented
- Timeout assumptions are documented

---

### NW-008
**Title:** Define session recovery and reload strategy  
**Master ID:** MB-042  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Plan restoration after page reload, dropped connection, or temporary service interruption.

**Dependencies:** NW-007

**Acceptance Criteria:**
- Recovery scenarios are listed
- Reload flow is documented
- Recovery expectations are realistic

---

### NW-009
**Title:** Define minimal networking MVP  
**Master ID:** MB-039 to MB-042  
**Priority:** High  
**Status:** Pending  

**Description:**  
Constrain the first multiplayer slice so it is practical.

**Dependencies:** NW-001 through NW-008

**Acceptance Criteria:**
- MVP scope is documented
- Deferred networking features are listed
- Testable first-release behaviors are identified

---

### NW-010
**Title:** Create networking sync test plan  
**Master ID:** MB-053  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define tests for multi-client consistency, reconnect, authority, and ordering.

**Dependencies:** NW-003 through NW-009

**Acceptance Criteria:**
- Test categories exist
- High-risk scenarios are listed
- Initial sync regression pack is outlined

---

## Near-Term Execution Order

1. NW-001 Define live-session hosting model  
2. NW-002 Define client connection roles  
3. NW-003 Define synchronization model  
4. NW-004 Define authoritative-state strategy  
5. NW-006 Define permissions and visibility rules  
6. NW-007 Define reconnect behavior  
7. NW-009 Define minimal networking MVP

## Progress Snapshot (Pre-networking Foundation)

- Completed foundation relevant to networking join flow:
  - authenticated entry points with default admin role gating
  - persisted known-session join-code registry across restarts
  - admin-managed join-code lifecycle (set/reset/generate)
  - join audit timestamps for operational visibility

- Next networking-aligned steps:
  1. Define hosted-session identity model independent of local file path.
  2. Define join-code claim and ownership rules for host and reconnect scenarios.
  3. Integrate join-code registry policy with future server-authoritative session directory.
