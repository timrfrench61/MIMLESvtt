# Game Selector UI

## Purpose

Define the launch-stage Game Selection dialog for session entry.

This UI is the entry point for:

- opening a saved/subscribed game session
- joining an existing hosted game
- creating a new session (admin-only)

---

## Dialog Scope

The Game Selection dialog is shown from the Launch/Home screen.

Primary sections:

1. **My Games (Subscribed / Saved Sessions)**
2. **Join Existing Game**
3. **Start New Session** (permission-gated)

---

## 1) My Games (Subscribed / Saved Sessions)

### Purpose

Show sessions the user can open directly.

### Data shown per row

- join code
- session title
- game/system label (if available)
- last updated timestamp
- owner/host (optional)
- quick status (local/hosted, if available)

### Actions

- **Open** selected session
- filter/search/sort for known sessions
- add/remove known session paths for subscription-like session list management

### Behavior

- selecting a row enables Open action
- Open loads selected `VttSession` and routes to Workspace

---

## 2) Join Existing Game

### Purpose

Allow user to join a running/hosted session they are not opening from local saved list.

### Inputs

- join code and/or hosted game selector

### Actions

- **Join**

### Behavior

- validate join token/session target
- validate user access
- if valid: connect and route to Workspace
- if invalid: show user-visible error

Current single-user implementation note:

- join code resolves to known local session entries from My Games.
- if no known-session match is found, launcher also checks docs sample snapshots (`docs/03-persistence/*.vttsession.json`).

### Friendly join code (current)

- Join code is derived from the session file name.
- Example:
  - `sample-session.vttsession.json` -> `SAMPLE-SESSION`
- The Join action accepts:
  - friendly join code (preferred)
  - session file name
  - full session path (fallback)

---

## 3) Start New Session (Admin-only)

### Purpose

Create a new session from launch flow.

### Permission rule

- visible/enabled only for users with admin rights

### Behavior

- if admin:
  - create new `VttSession`
  - optional title assignment
  - optional create+save in one step (when save path provided)
  - optional scenario/setup selection
  - route to Workspace
- if not admin:
  - control hidden or disabled
  - if invoked indirectly, return access-denied message

---

## Launch Decision Flow (Concise)

1. Open Game Selection dialog
2. Load subscribed/saved session list
3. User chooses one:
   - Open saved session
   - Join existing game
   - Start new session (admin-only)
4. Resolve permission/validation
5. Initialize Workspace

---

## Minimal State Requirements

Game selector UI state should include:

- loaded sessions list
- selected session id
- join code/target
- current user permissions (`CanCreateSession`)
- operation feedback message/error

---

## Related Docs

- `04-workspace-use-case-flows.md` (launch and workspace flow diagrams)
- `03-workspace-launch-contract.md` (launch behavior contract)
- `09-tabletop-state-model.md` (runtime session model)
