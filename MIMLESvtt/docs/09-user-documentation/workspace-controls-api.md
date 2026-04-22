# Workspace Controls API (User-Facing)

## Purpose

This page documents the controls in the Workspace slide panel and what each field/button does.

This is a user-facing control reference for pre-alpha.

---

## 1. Top-Level Workspace Controls

### Show Workspace Controls / Hide Workspace Controls
- Opens or closes the right-side Workspace controls panel.

### Close (inside panel header)
- Closes the Workspace controls panel.

### Tabletop Surface
- **Default x-y grid**: shows default tabletop grid behind controls.
- **Missing tabletop**: shows tabletop unavailable message.
- **Page not loading**: shows page-loading error message.

---

## 2. Session and File Controls

### Undo
- Reverts the last supported workspace operation.

### Redo
- Re-applies the last undone supported workspace operation.

### Create New Session
- Creates a new table session with default title and reset state.

### Load Game
- Opens a session from the file path in **Game save file path**.

### Save Game
- Saves current session to current file path.

### Save Game As
- Saves current session using the path in **Game save file path**.

### Save Current Layout As Scenario
- Saves current board layout as scenario using:
  - **Scenario file path**
  - **Scenario title for export**

### Open Scenario For Pending Plan
- Imports scenario from **Scenario file path** into pending plan state.

### Activate Pending Scenario
- Activates imported pending scenario into current live session.

### Game save file path (.session.json)
- Path used by Load Game / Save Game As.

### Scenario file path (.scenario.json)
- Path used by scenario import/export actions.

### Scenario title for export
- Title written when saving current layout as scenario.

---

## 3. Session Summary and Title Controls

### Session Details > Id
- Displays current session id.

### Session title input
- Editable session title text.

### Update Title
- Applies title input to current session title.

### Session Summary fields
- Session #
- Title
- Mode
- Current File
- Dirty
- Participants
- Surfaces
- Pieces
- Pending Scenario
- Last Operation

---

## 4. Workspace Mode Controls

### Edit Mode
- Switches workspace mode to Edit.

### Play Mode
- Switches workspace mode to Play.

---

## 5. Developer Settings Controls

### Enable Multiplayer
- Placeholder toggle for staged multiplayer UI behavior.

### Enable Rules Validation
- Enables/disables rules validation behavior in applicable paths.

### Enable Stamp Mode
- Enables/disables stamp mode usage paths.

### Enable Board Interaction
- Enables/disables board click/keyboard interaction paths.

When a disabled feature path is used, expected message:
- **Feature not enabled (development toggle)**

---

## 6. Turn / Phase Controls

### Previous Turn
- Moves turn index backward in turn order.

### Next Turn
- Advances to next participant in turn order.

### Turn order input
- Comma-separated participant ids for initialization.

### Initialize Turn Order
- Sets turn order from input.

### Current phase input
- Editable phase string.

### Set Phase
- Applies current phase input.

---

## 7. Initiative / Roster Controls

### ↑ / ↓ (per participant row)
- Reorders participant in current turn order.

---

## 8. Participant Controls

### Id / Name
- Inputs used by Add Participant.

### Add Participant
- Adds participant to session.

### Remove Id
- Input used by Remove Participant.

### Remove Participant
- Removes participant by id.

### Rename Id / New Name
- Inputs used by Rename Participant.

### Rename Participant
- Renames participant by id.

---

## 9. Setup Controls

### Add Surface section
- **Surface Id**
- **Definition Id**
- **Surface Type**
- **Coordinate System**
- **Add Surface**: adds surface to current session.

### Add Piece section
- **Piece Id**
- **Definition Id**
- **Target Surface Id**
- **X / Y**
- **Rotation**
- **Owner (optional)**
- **Add Piece**: adds piece to current session.

---

## 10. Surface Workflow Strip Controls

### Activate
- Sets row surface as active surface.

### Default Preset dropdown
- Sets/clears default preset mapping for that surface.

### Use Active Preset
- Applies active preset as default for row surface.

### Quick Add Surface fields + Add
- Adds new surface quickly from strip.

### Duplicate Active Surface field + Duplicate
- Duplicates active surface configuration to new surface id.

### Active surface for board dropdown
- Selects active surface for board rendering/interactions.

---

## 11. Board Visibility Controls

### Show Surfaces / Show Pieces / Show Markers / Active Surface Only
- Toggles board rendering filters.

### Filter DefinitionId / Filter OwnerParticipantId
- Applies visibility filtering by definition and owner.

---

## 12. Piece Palette and Selection Controls

### Arm Placement (per palette row)
- Arms selected preset for add-at-click placement.

### Clear Selection
- Clears selected piece(s).

### Select All On Active Surface
- Selects all pieces on active surface.

---

## 13. Group Controls

### Group Move ΔX / ΔY + Move Selected Group
- Moves selected pieces as a group by delta.

### Group label + Create From Selection
- Creates piece group from current multi-selection.

### Move Group By Δ
- Moves selected group id by current delta values.

### Group row actions
- **Select Group**
- **Move**
- **Disband**

---

## 14. Board Interaction and Stamp Session Controls

### Add Piece At Board Click
- Enables click-to-place/click-to-move board behavior.

### Stamp Mode (repeat placement)
- Keeps add-at-click placement active across repeated clicks.

### Quick Piece Id / Quick Definition Id / Owner / Rotation
- Placement parameters used by click placement.

### Nudge Step
- Keyboard nudge increment for selected piece movement.

### Clamp Movement To Board
- Clamps move operations inside board bounds.

### Move Selected To Clicked Position
- Moves selected piece to last board click position.

### Center Selected
- Centers selected piece in board bounds.

### Rotate Left / Rotate Right
- Rotates selected piece by quick increment.

### Stamp Session controls
- **Id -1 / Id +1**
- **Reset Id To Base**
- **Clear Id Preview**
- **Remember DefinitionId Per Surface**

### Stamp Preset controls
- **Save Preset**
- Preset selector
- **Use Preset**
- **Clear Selection**
- Rename input + **Rename**
- **Duplicate**
- **Set Default For Surface**
- **Clear Surface Default**
- **Delete Preset**

---

## 15. Cross-Surface Stamp Queue Controls

### Add Current Setup To Queue
- Appends current surface/preset/id context to queue.

### Next Queue Item
- Applies next queued setup to board context.

### Queue row actions
- **Apply**
- **↑ / ↓** reorder
- **Remove**

### Queue item Next Id input
- Sets editable next-id per queue row.

### Queue template controls
- Template name + **Save Current Queue As Template**
- Per-template actions: **Apply / Rename / Duplicate / Delete**
- Template rename input

---

## 16. Selected Piece Data and Piece Actions

### Selected Piece Data state controls
- **State Key / State Value + Add/Update State**
- **Key to remove + Remove Key**
- **HP Quick Update + Set HP**

### Assign Owner controls
- Owner dropdown + **Apply Owner**

### Pieces section controls
- Piece selector dropdown
- Move Piece: surface/x/y + **Move**
- Rotate Piece: degrees + **Rotate**
- Add Marker: marker id + **Add Marker**
- Remove Marker: marker id + **Remove Marker**
- Change Piece State: key/value + **Update State**

---

## 17. Operation and Log Displays

### Recent Operations
- Shows last workspace operations with timestamp/result/message.

### Play Log
- Shows latest action records with type/actor/payload summary.
