# Import Workflow UI

## Purpose

Define the user-facing import workflow for content ingestion, validation feedback, and result review.

This workflow supports GM/operator content operations and aligns with the content-management inventory.

---

## Workflow Stages

### 1) Upload Step

Primary responsibilities:

- select source file(s)
- choose import type/profile
- confirm import target context

Expected operator actions:

- pick file
- select import mode
- continue to validation preview

### 2) Validation Result Step

Primary responsibilities:

- show validation summary (valid/warning/error counts)
- display row/item-level issues before commit
- allow operator to stop or continue depending on policy

Expected operator actions:

- review validation output
- decide whether to proceed or cancel

### 3) Error Review Step

Primary responsibilities:

- provide actionable error detail list
- include enough context to fix source data
- support repeatable correction/retry workflow

Expected operator actions:

- inspect errors
- correct source file
- rerun import

### 4) Success Summary Step

Primary responsibilities:

- present post-import summary
- include counts such as created/updated/skipped/failed
- confirm completion state clearly

Expected operator actions:

- verify successful import
- return to relevant content list/detail views

---

## Feedback Expectations

- Validation and processing failures are explicit and user-visible.
- Stage transitions are clear and do not silently hide errors.
- Successful completion always provides a summary message.

---

## Navigation Flow (Concise)

1. Upload file
2. Validate preview
3. Review errors (if any)
4. Complete import and show summary
5. Return to content-management screens

---

## Related Docs

- `docs/00-overview/32-content-management-screen-inventory.md`
- `docs/05-backlog/05-backlog-ui-presentation.md` (UI-012)
