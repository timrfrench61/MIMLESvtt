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

- session title
- game/system label (if available)
- last updated timestamp
- owner/host (optional)
- quick status (local/hosted, if available)

### Actions

- **Open** selected session
- optional filter/search/sort

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

---

## 3) Start New Session (Admin-only)

### Purpose

Create a new session from launch flow.

### Permission rule

- visible/enabled only for users with admin rights

### Behavior

- if admin:
  - create new `VttSession`
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
