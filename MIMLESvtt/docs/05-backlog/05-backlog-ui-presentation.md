# UI and Presentation Backlog

> Status note: All backlog items in this document (UI-001 through UI-019) are currently completed/closed.

## Scope

This backlog covers:

- application shell
- navigation
- board/table presentation
- content-management screens
- tactical and RPG workflow screens

The UI must prioritize practical use at the table over decorative complexity.

---

## Related Master Backlog Items

- MB-043 Define UI shell layout
- MB-044 Define board presentation patterns
- MB-045 Define content-management screen inventory
- MB-022 Create tactical scenario setup workflow
- MB-026 Create manual GM entry screens

---

## Backlog Items

### UI-001
**Title:** Define application shell layout  
**Master ID:** MB-043  
**Priority:** High  
**Status:** Done  

**Description:**  
Define the primary app shell with left navigation, top bar, main workspace, and optional side panels.

**Dependencies:** None

**Acceptance Criteria:**
- Shell structure is documented
- Navigation zones are defined
- Main workspace supports table/board presentation

---

### UI-002
**Title:** Define navigation model  
**Master ID:** MB-043  
**Priority:** High  
**Status:** Done  

**Description:**  
Define how users reach table views, content libraries, import screens, sessions, and settings.

**Dependencies:** UI-001

**Acceptance Criteria:**
- Primary navigation groups are documented
- Page hierarchy is documented
- Navigation is usable for GM-heavy workflows

---

### UI-003
**Title:** Define board presentation rules  
**Master ID:** MB-044  
**Priority:** High  
**Status:** Done  

**Description:**  
Define how grids, boards, pieces, selection, hover, legal moves, and overlays are shown.

**Dependencies:** UI-001

**Acceptance Criteria:**
- Rendering patterns are documented
- Selection state is defined
- Piece placement and move indicators are defined

---

### UI-004
**Title:** Define piece visual states  
**Master ID:** MB-044  
**Priority:** High  
**Status:** Done  

**Description:**  
Document how pieces visually represent side, status, selection, disabled state, and promotion/elite/leader style markers where applicable.

**Dependencies:** UI-003

**Acceptance Criteria:**
- Visual-state list exists
- States are distinguishable
- Rules system can influence displayed state later

---

### UI-005
**Title:** Define checkers reference UI flow  
**Master ID:** MB-012, MB-013  
**Priority:** High  
**Status:** Done  

**Description:**  
Define the simple interaction flow for the reference game.

**Dependencies:** UI-003

**Acceptance Criteria:**
- Flow for selecting and moving a piece is documented
- Turn indication is visible
- Win-state message area is planned

---

### UI-006
**Title:** Define tactical scenario setup screen  
**Master ID:** MB-022  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Plan a screen for creating a tactical scenario with board, sides, and initial units.

**Dependencies:** UI-001, UI-003

**Acceptance Criteria:**
- Setup screen purpose is documented
- Placement and side assignment workflow is documented
- Save behavior is defined

---

### UI-007
**Title:** Define content management screen inventory  
**Master ID:** MB-045  
**Priority:** High  
**Status:** Done  

**Description:**  
Inventory all required content screens for list, detail, create, edit, import, and validation result review.

**Dependencies:** UI-001

**Acceptance Criteria:**
- Screen inventory exists
- Related workflows are grouped
- Inventory covers monster, treasure, equipment, and magic items

---

### UI-008
**Title:** Define monster management UI flow  
**Master ID:** MB-026, MB-045  
**Priority:** High  
**Status:** Done  

**Description:**  
Plan list/detail/edit screens for monsters.

**Dependencies:** UI-007

**Acceptance Criteria:**
- List screen is defined
- Detail screen is defined
- Entry/edit screen is defined

---

### UI-009
**Title:** Define treasure management UI flow  
**Master ID:** MB-026, MB-045  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Plan list/detail/edit screens for treasure.

**Dependencies:** UI-007

**Acceptance Criteria:**
- List screen is defined
- Detail screen is defined
- Entry/edit screen is defined

---

### UI-010
**Title:** Define equipment management UI flow  
**Master ID:** MB-026, MB-045  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Plan list/detail/edit screens for equipment.

**Dependencies:** UI-007

**Acceptance Criteria:**
- List screen is defined
- Detail screen is defined
- Entry/edit screen is defined

---

### UI-011
**Title:** Define magic item management UI flow  
**Master ID:** MB-026, MB-045  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Plan list/detail/edit screens for magic items.

**Dependencies:** UI-007

**Acceptance Criteria:**
- List screen is defined
- Detail screen is defined
- Entry/edit screen is defined

---

### UI-012
**Title:** Define import workflow UI  
**Master ID:** MB-026, MB-045  
**Priority:** High  
**Status:** Done  

**Description:**  
Plan file selection, validation preview, import result summary, and error review screens.

**Dependencies:** UI-007

**Acceptance Criteria:**
- Upload step is defined
- Validation result step is defined
- Error review step is defined
- Success summary step is defined

---

### UI-013
**Title:** Implement Workspace right-slide controls panel over tabletop  
**Master ID:** MB-043, MB-044  
**Priority:** High  
**Status:** Done  

**Description:**  
Consolidate Workspace control-heavy UI into a right-side slide-in panel (Android-style interaction pattern) while keeping tabletop visible behind the panel.

**Dependencies:** UI-001, UI-003

**Acceptance Criteria:**
- Workspace controls and text-entry workflows are reachable in a right slide panel.
- Panel can open/close without losing table state.
- Tabletop remains visible behind panel.
- Tabletop supports explicit fallback states:
  - default x-y grid
  - missing tabletop
  - page not loading

---

### UI-014
**Title:** Document Workspace slide-panel interaction model and fallback tabletop states  
**Master ID:** MB-044  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Add explicit UI design documentation for the slide-in panel behavior, expected responsiveness, and fallback tabletop states.

**Dependencies:** UI-013

**Acceptance Criteria:**
- UI behavior spec includes panel open/close states and layering.
- Fallback tabletop states are documented with operator intent.
- Documentation references Workspace controls grouping and expected user flow.

---

### UI-015
**Title:** Implement Launch Game Selector dialog shell  
**Master ID:** MB-043  
**Priority:** High  
**Status:** Done  

**Description:**  
Add a launch-stage Game Selector dialog that is opened from Home/Launch and acts as the entry point into session flows.

**Dependencies:** UI-001, UI-002

**Acceptance Criteria:**
- Game Selector dialog is reachable from launch screen.
- Dialog includes sections for:
  - My Games (saved/subscribed sessions)
  - Join Existing Game
  - Start New Session
- Cancel/close behavior returns user to launch screen safely.

---

### UI-016
**Title:** Implement My Games (saved/subscribed sessions) list in Game Selector  
**Master ID:** MB-043  
**Priority:** High  
**Status:** Done  

**Description:**  
Implement session list rendering and selection behavior for opening existing sessions from the Game Selector dialog.

**Dependencies:** UI-015

**Acceptance Criteria:**
- Saved/subscribed sessions list is rendered.
- Selecting a session enables Open action.
- Open action loads selected session and routes to Workspace.
- Empty/error/loading list states are shown clearly.
- Known session list supports add/remove and refresh operations.
- List supports filter/search/sort behaviors for practical session lookup.

---

### UI-017
**Title:** Implement Join Existing Game flow in Game Selector  
**Master ID:** MB-043  
**Priority:** High  
**Status:** Done  

**Description:**  
Implement join flow using join code or existing hosted-game selector from the Game Selector dialog.

**Dependencies:** UI-015

**Acceptance Criteria:**
- Join input is available in dialog.
- Join action validates input and reports user-visible errors.
- Successful join routes user into Workspace.
- Join flow supports friendly join-code matching (not only full file path input).

---

### UI-018
**Title:** Implement admin-gated Start New Session action in Game Selector  
**Master ID:** MB-043  
**Priority:** High  
**Status:** Done  

**Description:**  
Add permission-gated new-session creation from the Game Selector dialog so only admin users can initiate new sessions.

**Dependencies:** UI-015

**Acceptance Criteria:**
- Start New Session action is visible/enabled only for admin users.
- Non-admin invocation attempts return explicit access-denied feedback.
- Admin path creates a new session and routes to Workspace.
- Admin flow supports create+save session from selector so saved game appears in My Games list.

---

### UI-019
**Title:** Document and test Game Selector interaction states  
**Master ID:** MB-043  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Add UI documentation and test coverage for Game Selector behavior and permission/state handling.

**Dependencies:** UI-016, UI-017, UI-018

**Acceptance Criteria:**
- Documentation covers selection/open/join/admin gating behavior.
- Tests cover:
  - saved session open path
  - join validation + success path
  - admin-only new session gating
  - loading/empty/error UI states

---

## Near-Term Execution Order

1. Closed in current implementation cycle (UI-001 through UI-019 completed)
2. Next focus: promote key documented flows into end-to-end UI automation coverage
3. Next focus: begin networking-era UI extensions once hosted session model is active
