# UI and Presentation Backlog

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

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
**Status:** Pending  

**Description:**  
Plan file selection, validation preview, import result summary, and error review screens.

**Dependencies:** UI-007

**Acceptance Criteria:**
- Upload step is defined
- Validation result step is defined
- Error review step is defined
- Success summary step is defined

---

## Near-Term Execution Order

1. UI-001 Define application shell layout  
2. UI-002 Define navigation model  
3. UI-003 Define board presentation rules  
4. UI-004 Define piece visual states  
5. UI-005 Define checkers reference UI flow  
6. UI-007 Define content management screen inventory  
7. UI-012 Define import workflow UI