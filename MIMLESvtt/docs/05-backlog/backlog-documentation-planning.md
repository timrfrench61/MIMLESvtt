# Backlog — Documentation and Planning

## Purpose

This backlog covers the documentation and planning work required to keep the Blazor Tabletop Engine / VTT project understandable, traceable, and buildable over time.

The project is broader than a single RPG assistant. It is a general tabletop engine supporting:

- board games
- tactical tabletop play
- strategic war games
- RPG modules

Documentation must therefore support both architecture and controlled growth.

---

## Scope

Included in this subsystem:

- product vision docs
- subsystem maps
- roadmap docs
- backlog governance
- repo conventions
- onboarding guidance
- architecture records
- milestone planning
- planning traceability

Not included:

- feature implementation
- runtime user help systems
- external marketing material

---

## Dependencies

Primary dependencies:

- `master-backlog.md`
- all subsystem backlog files
- architecture documents
- domain model documents
- roadmap documents

This subsystem depends on core design decisions remaining visible and current.

---

## Traceability to Master Backlog

This subsystem backlog primarily expands:

- MB-002 Define product vision and scope
- MB-003 Define subsystem map
- MB-054 Create subsystem backlog set
- MB-055 Create roadmap and milestone tracking doc
- MB-056 Create developer onboarding and repo conventions doc

It also supports the long-term usability of all other subsystems.

---

## Backlog Items

---

### DOC-001
**Master ID:** MB-002  
**Title:** Maintain vision and scope document  
**Priority:** High  
**Status:** Pending  

**Description:**  
Keep the core vision document aligned with the actual product direction as a general tabletop engine.

**Dependencies:** None

**Acceptance Criteria:**
- Vision document exists
- Scope and non-goals are defined
- RPG, tactical, strategic, and board-game support are represented
- Major product shifts are reflected in the doc

---

### DOC-002
**Master ID:** MB-003  
**Title:** Maintain subsystem map  
**Priority:** High  
**Status:** Pending  

**Description:**  
Keep the subsystem map current as architecture and module boundaries evolve.

**Dependencies:** DOC-001

**Acceptance Criteria:**
- Subsystems are listed clearly
- Responsibilities are defined
- High-level dependencies are identified
- New major subsystems are added when introduced

---

### DOC-003
**Master ID:** MB-055  
**Title:** Maintain roadmap and milestone tracking  
**Priority:** High  
**Status:** Pending  

**Description:**  
Track project phases, milestone goals, and phase transitions.

**Dependencies:** DOC-001, DOC-002

**Acceptance Criteria:**
- Roadmap exists
- Milestones are named and described
- Near-term execution band is visible
- Phase changes are updated when priorities shift

---

### DOC-004
**Master ID:** MB-054  
**Title:** Maintain master backlog  
**Priority:** High  
**Status:** Pending  

**Description:**  
Keep the master backlog aligned with actual project priorities and subsystem splits.

**Dependencies:** DOC-001, DOC-002

**Acceptance Criteria:**
- Master backlog exists
- Items remain traceable to subsystem backlogs
- Priority/status values are maintained
- Duplicate/conflicting items are cleaned up

---

### DOC-005
**Master ID:** MB-054  
**Title:** Maintain subsystem backlog set  
**Priority:** High  
**Status:** Pending  

**Description:**  
Keep the subsystem backlog files current and synchronized with master backlog intent.

**Dependencies:** DOC-004

**Acceptance Criteria:**
- Subsystem backlog files exist
- Each subsystem backlog references master IDs where applicable
- New work is assigned to a subsystem backlog
- Cross-subsystem dependencies are visible

---

### DOC-006
**Master ID:** MB-056  
**Title:** Create developer onboarding guide  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Document how a developer should understand, open, and navigate the project.

**Dependencies:** DOC-002

**Acceptance Criteria:**
- Onboarding doc exists
- Project structure is explained
- Key docs to read first are listed
- Build/run entry points are identified

---

### DOC-007
**Master ID:** MB-056  
**Title:** Create repo conventions document  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Document naming, folder layout, file organization, and documentation conventions.

**Dependencies:** DOC-006

**Acceptance Criteria:**
- Repo conventions doc exists
- Folder and naming conventions are described
- Backlog/doc placement is defined
- Convention examples are included

---

### DOC-008
**Master ID:** MB-056  
**Title:** Define module-addition workflow  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Document how new engine modules, rules modules, or content modules should be introduced.

**Dependencies:** DOC-007

**Acceptance Criteria:**
- Workflow for adding a subsystem or module is written
- Required docs/backlog updates are listed
- Dependency review expectations are stated
- Test/doc update expectations are stated

---

### DOC-009
**Master ID:** MB-055  
**Title:** Create milestone review checklist  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Create a simple checklist for evaluating whether a milestone is actually complete.

**Dependencies:** DOC-003

**Acceptance Criteria:**
- Checklist exists
- Functional completion is covered
- Documentation completion is covered
- Test readiness is covered

---

### DOC-010
**Master ID:** MB-055  
**Title:** Create architecture decision record index  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Create and maintain an index of architecture decisions and where each applies.

**Dependencies:** DOC-002

**Acceptance Criteria:**
- ADR index exists
- Decisions are linked or referenced
- Status of each ADR is visible
- Superseded decisions can be tracked

---

### DOC-011
**Master ID:** MB-055  
**Title:** Maintain domain glossary  
**Priority:** High  
**Status:** Pending  

**Description:**  
Keep the shared glossary current so terms such as board, map, zone, piece, token, counter, scenario, and session remain stable.

**Dependencies:** DOC-002

**Acceptance Criteria:**
- Glossary document exists
- Core terms are defined consistently
- Ambiguous terms are clarified
- New platform concepts are added as needed

---

### DOC-012
**Master ID:** MB-054  
**Title:** Create dependency tracking notes across subsystems  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Record significant subsystem dependencies and sequencing constraints.

**Dependencies:** DOC-002, DOC-005

**Acceptance Criteria:**
- Cross-subsystem dependency notes exist
- High-risk sequences are identified
- Engine-first dependencies are made clear
- Planning notes reflect actual subsystem order

---

### DOC-013
**Master ID:** MB-055  
**Title:** Create release packaging and milestone naming notes  
**Priority:** Low  
**Status:** Pending  

**Description:**  
Document how milestones, internal builds, and reference releases should be named.

**Dependencies:** DOC-003

**Acceptance Criteria:**
- Naming pattern exists
- Milestone naming is consistent
- Reference build naming is defined
- Internal release notes can follow the pattern

---

### DOC-014
**Master ID:** MB-056  
**Title:** Create documentation folder map  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Document the `/docs` structure and where each type of artifact belongs.

**Dependencies:** DOC-007

**Acceptance Criteria:**
- `/docs` folder map exists
- Overview, architecture, domain, backlog, and testing locations are defined
- New docs can be placed consistently
- Folder purposes are briefly described

---

### DOC-015
**Master ID:** MB-054  
**Title:** Define backlog item writing standard  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Create a standard format for backlog items so they remain consistent across files.

**Dependencies:** DOC-005

**Acceptance Criteria:**
- Backlog template exists
- Required fields are defined
- Optional fields are defined
- Priority/status usage is explained

---

### DOC-016
**Master ID:** MB-056  
**Title:** Create contributor quick-start checklist  
**Priority:** Low  
**Status:** Pending  

**Description:**  
Provide a short checklist for starting work on a subsystem or feature.

**Dependencies:** DOC-006, DOC-007

**Acceptance Criteria:**
- Quick-start checklist exists
- References docs to read first
- Includes branch/task prep expectations if used
- Includes test/doc update reminder

---

### DOC-017
**Master ID:** MB-055  
**Title:** Maintain implementation notes and open questions log  
**Priority:** Medium  
**Status:** Pending  

**Description:**  
Track unresolved design questions and interim implementation notes so decisions do not disappear into chat history.

**Dependencies:** DOC-003, DOC-010

**Acceptance Criteria:**
- Open questions log exists
- Entries are dated or versioned
- Resolved questions are marked
- Notes are concise and searchable

---

### DOC-018
**Master ID:** MB-054  
**Title:** Create subsystem ownership matrix  
**Priority:** Low  
**Status:** Pending  

**Description:**  
Summarize which subsystem owns which classes of concern, documents, and backlog areas.

**Dependencies:** DOC-002, DOC-005

**Acceptance Criteria:**
- Ownership matrix exists
- Responsibilities are not heavily duplicated
- Shared responsibilities are identified
- Matrix can be updated as subsystem boundaries evolve

---

## Near-Term Execution Order

Recommended first execution band:

1. DOC-001 Maintain vision and scope document  
2. DOC-002 Maintain subsystem map  
3. DOC-003 Maintain roadmap and milestone tracking  
4. DOC-004 Maintain master backlog  
5. DOC-005 Maintain subsystem backlog set  
6. DOC-011 Maintain domain glossary  
7. DOC-006 Create developer onboarding guide  
8. DOC-007 Create repo conventions document  
9. DOC-014 Create documentation folder map  
10. DOC-015 Define backlog item writing standard  

After that, continue with:

1. ADR index  
2. milestone review checklist  
3. dependency tracking notes  
4. module-addition workflow  
5. open questions log

---

## Notes

- Documentation must support the broader engine-first design, not just the first RPG use case.
- Planning artifacts should be updated whenever subsystem boundaries or phase priorities change.
- Open questions should live in docs, not only in chat history, so the project remains durable over time.