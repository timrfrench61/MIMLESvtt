# VTT Workspace Launch

## Purpose

Define the **required startup flow** for opening the VTT Workspace.

The Workspace must not open until a **fully-resolved session** exists.

---

## Core Rule

> The Workspace is a host for a session — it does not decide what the session is.

---

## Launch Contract

### Inputs (must be resolved before launch)

- GameSystem (Checkers, Chess, Go, AD&D, etc.)
- SessionMode (New | Load)
- ScenarioOrSave (starting setup or saved state)
- Optional:
  - Variant / Ruleset
  - SessionOptions (player count, toggles, etc.)

---

### Process

```text
Resolve GameSystem
    → Resolve SessionMode
        → Resolve ScenarioOrSave
            → Build or Load Session