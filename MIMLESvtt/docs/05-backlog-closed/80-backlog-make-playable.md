# Fix structure of Campaign and Scenario
1. Add Campaign to Root Class list and define basic properties (name, description, etc.)
2. Has 1..* Scenarios
3. Has 1 Gamebox
4. A Child of a Session.
5. selectable Scenario/Campaign section screen (renamed)
6. Can be hidden in the screen
# Add Scenarios for Admin to make them playable
1. Add Empty Scenario (A Campaign with one Scenario that has no pieces or board) to use as a test case for playability and iteration.
2. Add VTTCheckers Game (a Campaign with one Scenario) to use as a test case for playability and iteration.

## Implementation Slices (80)

- [x] Slice 80-01: Add `VttCampaign` root model with basic campaign properties, session linkage, gamebox linkage, scenario ids, read-only flag, and hidden flag.
- [x] Slice 80-02: Add readonly scenario catalog support with seeded snapshots:
  - `EMPTY-SCENARIO` (readonly)
  - `CHECKERS-SCENARIO` (readonly)
- [x] Slice 80-03: Add readonly campaign catalog support with hidden toggle:
  - `EMPTY-CAMPAIGN`
  - `CHECKERS-CAMPAIGN`
- [x] Slice 80-04: Add admin-only readonly scenario activation workflow in Game Selection (not in Checkers Reference).
- [x] Slice 80-05: Add minimal checkers movement rule validation for admin move testing (basic diagonal move + forward-only men + king exception).
- [x] Slice 80-06: Add tests for readonly catalog visibility/hide behavior, readonly scenario activation, and minimal checkers move validation.



## Checkers Scenario Setup Notes
For **Checkers**, your `VttGamebox` should include more than the visible board and pieces.

## VttGamebox: Checkers Elements

### 1. Game identity

* Game name: `Checkers`
* Game family/type: `Abstract Board Game`
* Ruleset/version: American Checkers / English Draughts, or other variant

### 2. Board definition

* 8×8 checkerboard
* Playable squares: dark squares only
* Coordinate system: row/column, algebraic-style, or numbered 1–32
* Starting orientation: red side / black side

### 3. Piece definitions

You already have:

* Red checker
* Black checker

You also need:

* Red king
* Black king

These may be separate piece types or the same piece with a `Kinged` state.

### 4. Starting setup

* Initial red piece positions
* Initial black piece positions
* Empty playable squares
* Which player moves first

### 5. Player/side definitions

* Red side
* Black side
* Turn order
* Player ownership of pieces

### 6. Rules metadata

Not the full rules engine necessarily, but enough to declare:

* Diagonal movement
* Forward-only movement for men
* Kings move both directions
* Capture rules
* Mandatory capture rule, if used
* Multiple jump rule
* Promotion rule

### 7. Game actions

Common actions:

* Select piece
* Move piece
* Capture piece
* Multi-jump
* Crown/king piece
* End turn
* Resign
* Reset/new game

### 8. Game state fields

A saved checkers session needs:

* Current board state
* Captured pieces
* Current player turn
* Legal move/capture status
* Winner / draw / active game status
* Move history

### 9. Assets

* Board image or generated board style
* Red piece image/style
* Black piece image/style
* King marker or alternate king image
* Optional sounds: move, capture, king, win

### 10. UI helpers

Useful but not core:

* Highlight legal moves
* Highlight forced captures
* Show selected piece
* Show last move
* Show captured pieces
* Undo/redo controls
* Save/load controls

## Minimal Checkers Gamebox

At minimum:

```text
VttGamebox
- GameDefinition: Checkers
- BoardDefinition: 8x8 checkerboard
- CoordinateSystem: 32 playable dark squares
- PieceDefinitions:
  - Red Man
  - Red King
  - Black Man
  - Black King
- StartingSetup
- PlayerSides:
  - Red
  - Black
- RulesProfile
- ActionDefinitions
- AssetManifest
```

The big things you are missing are **king pieces**, **starting setup**, **side/player definitions**, **rules profile**, and **action definitions**.

## Delivered in this pass

- Readonly snapshots are seeded to `App_Data/readonly-scenarios/`:
  - `empty-scenario.vttscenario.json`
  - `checkers-scenario.vttscenario.json`
- Checkers scenario includes:
  - 8x8 board surface metadata
  - red and black starting piece setup
  - side state and king-capable state field
  - rules profile marker (`Checkers`) for validation hook
- Game Selection now includes admin controls for readonly seeds (instead of Checkers Reference) to:
  - hide/unhide readonly campaigns
  - activate readonly scenarios into workspace for movement testing

## Placement Note

- `EMPTY-SCENARIO` and `CHECKERS-SCENARIO` belong in **Game Selection** flow.
- Checkers Reference should remain focused on launching/observing checkers workspace behavior, not owning seed selection.

