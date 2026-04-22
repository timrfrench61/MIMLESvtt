# Rules Framework Backlog

## Scope

This backlog covers:

- generic rules plugin architecture
- shared dice/randomization services
- RPG rules modules
- turn/combat workflow integration
- victory/objective hooks

The rules subsystem must attach to the engine rather than define the engine.

---

## Related Master Backlog Items

- MB-031 Define rules plugin framework
- MB-032 Build dice and randomization services
- MB-033 Implement AD&D 1e rules module
- MB-034 Implement BRP rules module
- MB-035 Integrate initiative and combat workflow options
- MB-038 Define scenario objectives and victory condition framework

---

## Backlog Items

### RF-001
**Title:** Define rules plugin boundary  
**Master ID:** MB-031  
**Priority:** High  
**Status:** Pending  

**Description:**  
Document the service boundaries between engine state and game-specific mechanics.

**Dependencies:** None

**Acceptance Criteria:**
- Rules plugin boundary is documented
- Engine remains rules-agnostic
- Plugin responsibilities are explicit

---

### RF-002
**Title:** Define rules context and action model  
**Master ID:** MB-031  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define how rules modules receive state and evaluate actions such as move, attack, skill check, or objective check.

**Dependencies:** RF-001

**Acceptance Criteria:**
- Rules context object is defined
- Action request/result model is defined
- Modules can return validation or outcomes

---

### RF-003
**Title:** Design shared dice/randomization service  
**Master ID:** MB-032  
**Priority:** High  
**Status:** Pending  

**Description:**  
Create a reusable dice and randomization layer.

**Dependencies:** RF-001

**Acceptance Criteria:**
- d4, d6, d8, d10, d12, d20, d100 supported
- Structured result type exists
- Dice service can be consumed by multiple rules modules

---

### RF-004
**Title:** Define roll expression support strategy  
**Master ID:** MB-032  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Determine whether the engine needs simple roll expressions, parsed expressions, or both.

**Dependencies:** RF-003

**Acceptance Criteria:**
- Expression scope is documented
- Minimal first-pass capability is defined
- Future expansion path is documented

---

### RF-005
**Title:** Define RPG combat workflow hook points  
**Master ID:** MB-035  
**Priority:** High  
**Status:** Pending  

**Description:**  
Define where initiative, attack, save, damage, and status workflows connect to generic turn state.

**Dependencies:** RF-001, RF-002

**Acceptance Criteria:**
- Hook points are documented
- Turn engine integration is defined
- Workflow is not hard-coded to one ruleset

---

### RF-006
**Title:** Define AD&D1 module scope for first release  
**Master ID:** MB-033  
**Priority:** High  
**Status:** Pending  

**Description:**  
Constrain the first AD&D1 rules slice so implementation is practical.

**Dependencies:** RF-001, RF-003

**Acceptance Criteria:**
- Supported AD&D1 mechanics are listed
- Deferred mechanics are listed
- Scope is realistic for initial implementation

---

### RF-007
**Title:** Design AD&D1 attack and save resolution  
**Master ID:** MB-033  
**Priority:** High  
**Status:** Pending  

**Description:**  
Design the first-pass handling of attack resolution and saving throws.

**Dependencies:** RF-006

**Acceptance Criteria:**
- Attack resolution flow is documented
- Saving throw flow is documented
- Inputs/outputs are defined

---

### RF-008
**Title:** Design AD&D1 initiative integration  
**Master ID:** MB-033, MB-035  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Define how AD&D1 initiative plugs into the generic turn/sequence model.

**Dependencies:** RF-005, RF-006

**Acceptance Criteria:**
- Initiative data requirements are documented
- Round handling is documented
- Integration points are clear

---

### RF-009
**Title:** Define BRP module scope for first release  
**Master ID:** MB-034  
**Priority:** High  
**Status:** Pending  

**Description:**  
Constrain the first BRP implementation slice.

**Dependencies:** RF-001, RF-003

**Acceptance Criteria:**
- Supported BRP mechanics are listed
- Deferred mechanics are listed
- Scope is practical

---

### RF-010
**Title:** Design BRP percentile and skill resolution  
**Master ID:** MB-034  
**Priority:** High  
**Status:** Pending  

**Description:**  
Design first-pass BRP checks and skill resolution.

**Dependencies:** RF-009

**Acceptance Criteria:**
- Percentile check flow is documented
- Skill resolution flow is documented
- Inputs/outputs are defined

---

### RF-011
**Title:** Define objective and victory evaluation hooks  
**Master ID:** MB-038  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Create a general evaluation hook for scenario objectives and victory conditions across game styles.

**Dependencies:** RF-001, RF-002

**Acceptance Criteria:**
- Objective model consumption is defined
- Victory checks can be triggered by state or turn progression
- Works for both simple and layered win conditions

---

### RF-012
**Title:** Create rules module test harness design  
**Master ID:** MB-033, MB-034, MB-052  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Design how rules modules are tested in isolation from the UI.

**Dependencies:** RF-002, RF-003

**Acceptance Criteria:**
- Test harness approach is documented
- Rules modules can be exercised independently
- Inputs and expected outputs are testable

---

## Near-Term Execution Order

1. RF-001 Define rules plugin boundary  
2. RF-002 Define rules context and action model  
3. RF-003 Design shared dice/randomization service  
4. RF-005 Define RPG combat workflow hook points  
5. RF-006 Define AD&D1 module scope for first release  
6. RF-007 Design AD&D1 attack and save resolution  
7. RF-009 Define BRP module scope for first release  
8. RF-010 Design BRP percentile and skill resolution