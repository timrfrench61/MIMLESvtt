# Content and Import Backlog

> Status note: All backlog items in this document (CI-001 through CI-021) are currently completed/closed.

## Scope

This backlog covers:

- reusable content libraries
- GM manual entry screens
- CSV import framework
- CSV specifications
- validation rules
- test data packets

This subsystem must support both RPG and non-RPG content over time.

---

## Related Master Backlog Items

- MB-023 Define content library framework
- MB-024 Define RPG content models
- MB-025 Define unit and counter content models
- MB-026 Create manual GM entry screens
- MB-027 Build CSV import framework
- MB-028 Define CSV specifications for RPG content
- MB-029 Define CSV specifications for unit/counter content
- MB-030 Design test data packet specification

---

## Backlog Items

### CI-001
**Title:** Define content library framework  
**Master ID:** MB-023  
**Priority:** High  
**Status:** Done  

**Description:**  
Define the overall structure for reusable content types and shared metadata.

**Dependencies:** None

**Acceptance Criteria:**
- Content categories are documented
- Shared metadata concepts are identified
- Framework supports future growth beyond RPG content

---

### CI-002
**Title:** Define RPG content model: Monster  
**Master ID:** MB-024  
**Priority:** High  
**Status:** Done  

**Description:**  
Create the initial monster model for imported and manually entered RPG creatures.

**Dependencies:** CI-001

**Acceptance Criteria:**
- Core monster fields are documented
- Optional ruleset-specific extensions are allowed
- Model supports AD&D1 and BRP use cases

---

### CI-003
**Title:** Define RPG content model: Treasure  
**Master ID:** MB-024  
**Priority:** High  
**Status:** Done  

**Description:**  
Create the treasure model for coin, loot bundles, and treasure entries.

**Dependencies:** CI-001

**Acceptance Criteria:**
- Treasure structure is documented
- Value and composition can be represented
- Can be linked to encounter or scenario later

---

### CI-004
**Title:** Define RPG content model: Equipment  
**Master ID:** MB-024  
**Priority:** High  
**Status:** Done  

**Description:**  
Create the equipment model for mundane gear and weapons/armor.

**Dependencies:** CI-001

**Acceptance Criteria:**
- Equipment structure is documented
- Category/type is supported
- Ruleset-specific details can be extended

---

### CI-005
**Title:** Define RPG content model: Magic Item  
**Master ID:** MB-024  
**Priority:** High  
**Status:** Done  

**Description:**  
Create the magic item model.

**Dependencies:** CI-001

**Acceptance Criteria:**
- Magic item structure is documented
- Effect metadata can be attached
- Rules-specific interpretation is deferred to modules where needed

---

### CI-006
**Title:** Define unit/counter content model  
**Master ID:** MB-025  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Create a content model for tactical or strategic counters and units.

**Dependencies:** CI-001

**Acceptance Criteria:**
- Unit/counter model exists
- Side/faction fields are supported
- Suitable for tactical and war-game scenarios

---

### CI-007
**Title:** Define manual GM entry workflow  
**Master ID:** MB-026  
**Priority:** High  
**Status:** Done  

**Description:**  
Document how a GM adds and edits content manually.

**Dependencies:** CI-002 through CI-005

**Acceptance Criteria:**
- Workflow exists for create/edit/list/review
- Manual entry is separated by content type
- Validation expectations are clear

---

### CI-008
**Title:** Build GM monster entry screen backlog slice  
**Master ID:** MB-026  
**Priority:** High  
**Status:** Done  

**Description:**  
Plan the monster entry screen as the first manual content screen.

**Dependencies:** CI-007

**Acceptance Criteria:**
- Field list is defined
- Validation rules are listed
- Save/edit behavior is documented

---

### CI-009
**Title:** Build GM treasure entry screen backlog slice  
**Master ID:** MB-026  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Plan the treasure entry workflow.

**Dependencies:** CI-007

**Acceptance Criteria:**
- Treasure field list exists
- Entry and edit workflow exists
- Validation is documented

---

### CI-010
**Title:** Build GM equipment entry screen backlog slice  
**Master ID:** MB-026  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Plan the equipment entry workflow.

**Dependencies:** CI-007

**Acceptance Criteria:**
- Equipment field list exists
- Entry and edit workflow exists
- Validation is documented

---

### CI-011
**Title:** Build GM magic item entry screen backlog slice  
**Master ID:** MB-026  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Plan the magic item entry workflow.

**Dependencies:** CI-007

**Acceptance Criteria:**
- Magic item field list exists
- Entry and edit workflow exists
- Validation is documented

---

### CI-012
**Title:** Design reusable CSV import pipeline  
**Master ID:** MB-027  
**Priority:** High  
**Status:** Done  

**Description:**  
Design the upload, parse, map, validate, and persist pipeline.

**Dependencies:** CI-001

**Acceptance Criteria:**
- Pipeline stages are documented
- Import result object is defined
- Error collection/reporting approach is defined

---

### CI-013
**Title:** Define import validation model  
**Master ID:** MB-027  
**Priority:** High  
**Status:** Done  

**Description:**  
Define row-level and file-level validation, warnings vs errors, and duplicate handling.

**Dependencies:** CI-012

**Acceptance Criteria:**
- Validation severity model exists
- Required vs optional fields are supported
- Duplicate handling policy is documented

---

### CI-014
**Title:** Write monster CSV specification  
**Master ID:** MB-028  
**Priority:** High  
**Status:** Done  

**Description:**  
Define columns, rules, and sample rows for monster imports.

**Dependencies:** CI-002, CI-012, CI-013

**Acceptance Criteria:**
- Required fields are defined
- Optional fields are defined
- Example file is included
- Validation rules are listed

---

### CI-015
**Title:** Write treasure CSV specification  
**Master ID:** MB-028  
**Priority:** High  
**Status:** Done  

**Description:**  
Define columns, rules, and sample rows for treasure imports.

**Dependencies:** CI-003, CI-012, CI-013

**Acceptance Criteria:**
- Required fields are defined
- Optional fields are defined
- Example file is included
- Validation rules are listed

---

### CI-016
**Title:** Write equipment CSV specification  
**Master ID:** MB-028  
**Priority:** High  
**Status:** Done  

**Description:**  
Define columns, rules, and sample rows for equipment imports.

**Dependencies:** CI-004, CI-012, CI-013

**Acceptance Criteria:**
- Required fields are defined
- Optional fields are defined
- Example file is included
- Validation rules are listed

---

### CI-017
**Title:** Write magic item CSV specification  
**Master ID:** MB-028  
**Priority:** High  
**Status:** Done  

**Description:**  
Define columns, rules, and sample rows for magic item imports.

**Dependencies:** CI-005, CI-012, CI-013

**Acceptance Criteria:**
- Required fields are defined
- Optional fields are defined
- Example file is included
- Validation rules are listed

---

### CI-018
**Title:** Write unit/counter CSV specification  
**Master ID:** MB-029  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Define a CSV format for tactical or war-game units if adopted in the first implementation band.

**Dependencies:** CI-006, CI-012, CI-013

**Acceptance Criteria:**
- Columns are documented
- Validation rules are documented
- Example file is included

---

### CI-019
**Title:** Design test data packet format  
**Master ID:** MB-030  
**Priority:** High  
**Status:** Done  

**Description:**  
Define how test content packets are packaged, named, described, and validated.

**Dependencies:** CI-012, CI-013, CI-014 through CI-017

**Acceptance Criteria:**
- Packet structure is documented
- Manifest approach is defined if needed
- Expected outcomes can be stored with packet

---

### CI-020
**Title:** Create baseline valid import packet  
**Master ID:** MB-030  
**Priority:** High  
**Status:** Done  

**Description:**  
Create a sample packet containing valid monsters, treasure, equipment, and magic items.

**Dependencies:** CI-019

**Acceptance Criteria:**
- Packet contains valid sample files
- Expected import counts are documented
- Packet is usable for smoke testing

---

### CI-021
**Title:** Create edge-case/error import packet  
**Master ID:** MB-030  
**Priority:** Medium  
**Status:** Done  

**Description:**  
Create a sample packet with missing fields, duplicate keys, bad types, and warnings.

**Dependencies:** CI-019

**Acceptance Criteria:**
- Packet includes common failure cases
- Expected errors/warnings are documented
- Packet is usable for validation testing

---

## Near-Term Execution Order

1. Closed in current implementation cycle: CI-001 through CI-021 completed
2. Next focus: implement packet runner utility to compare import results against expected-outcome manifests
3. Next focus: add automated regression execution of baseline + edge-case packets in testing backlog
