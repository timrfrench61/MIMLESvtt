# Test — Library Management

## Purpose

Define QA checks for content library list/detail/create-edit/import management behavior.

## Scope

Included:
- content-type list navigation and filtering
- detail/read-only review behavior
- create/edit/cancel workflow behavior
- import workflow summary/error review visibility

## Preconditions

- Authenticated user with content-management access.
- Baseline content entries available for at least one category.

## Test Cases

### LIB-001
**Title:** Content home routes to category pages  
**Priority:** High

**Steps:**
1. Open `/content`.
2. Navigate to monsters/treasure/equipment/magic items.

**Expected:**
- Each route renders corresponding page.
- No route-level access/regression errors.

### LIB-002
**Title:** List filtering and sorting update visible rows  
**Priority:** High

**Steps:**
1. Enter search/filter values.
2. Change sort option.

**Expected:**
- Visible rows update according to criteria.

### LIB-003
**Title:** Selecting list row populates detail panel  
**Priority:** High

**Steps:**
1. Select a list row.

**Expected:**
- Detail section reflects selected entry.
- Edit mode can load selected values.

### LIB-004
**Title:** Create/edit save path shows success and reflects persisted values  
**Priority:** High

**Steps:**
1. Create a valid entry.
2. Edit the same entry.

**Expected:**
- Save success feedback is shown.
- List/detail data updates correctly.

### LIB-005
**Title:** Cancel edit path is no-mutation and safe return  
**Priority:** High

**Steps:**
1. Change entry fields.
2. Cancel.

**Expected:**
- No changes persisted.
- Safe return messaging displayed.

### LIB-006
**Title:** Import workflow reports summary and issue diagnostics  
**Priority:** Medium

**Steps:**
1. Run import preview/workflow for at least one content type.

**Expected:**
- Summary counts are visible.
- Issue list includes severity/stage context when errors/warnings exist.

## Cross References

- `../05-backlog/64-backlog-reusable-csv-import-pipeline.md`
- `../05-backlog/65-backlog-import-validation-model.md`
- `96-qa-run-log.md`
- `98-pre-alpha-testing-packet.md`