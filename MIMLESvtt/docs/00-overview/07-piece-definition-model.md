# Piece Definition Model

## Purpose

Define how reusable tabletop content is represented in the system.

This model separates:

- reusable piece definitions
- live piece instances in a session

This supports:

- checkers pieces
- board-game counters
- tactical units
- RPG monsters and characters
- markers and tokens

---

## Core Rule

Always separate:

- **PieceDefinition** = reusable template
- **PieceInstance** = live session object

A definition is content.
An instance is play state.

---

## PieceDefinition

A PieceDefinition describes a type of piece that can be used in one or more games or sessions.

Examples:

- Red Checker
- Black Checker
- Goblin Spearman
- Infantry Counter
- Torch Marker

A PieceDefinition is not placed on the board directly.

### Required Fields

- `DefinitionId`
- `Name`
- `Category`

### Common Fields

- `DefinitionId`
- `Name`
- `Category`
- `Tags`
- `VisualKey`
- `BaseAttributes`
- `RulesMetadata`
- `Description`
- `IsActive`

---

## PieceInstance

A PieceInstance is a live piece placed into a session.

Examples:

- Red Checker at B6
- Goblin #3 in Room 12
- Infantry Unit in Hex A4

A PieceInstance changes during play.

### Common Fields

- `InstanceId`
- `DefinitionId`
- `SessionId`
- `OwnerSeatId`
- `SideId`
- `Position`
- `CurrentAttributes`
- `StatusFlags`
- `Facing`
- `Hidden`
- `RemovedFromPlay`

---

## Definition vs Instance

### PieceDefinition
Use for:

- imported CSV content
- GM library data
- reusable templates
- display defaults
- default rules values

### PieceInstance
Use for:

- location on board
- damage
- temporary conditions
- status markers
- turn-by-turn changes

---

## Category

Category identifies the general kind of piece.

Suggested starting values:

- `Piece`
- `Token`
- `Marker`
- `Counter`
- `Unit`
- `Character`
- `Monster`

These categories are for filtering, rendering defaults, and rules interpretation.

They should not hard-code behavior in the engine.

---

## Tags

Tags are lightweight labels used for grouping and filtering.

Examples:

- Flying
- Infantry
- Undead
- Elite
- Fire
- Vehicle

Tags are optional and may be system-specific.

---

## Visual Data

A PieceDefinition may include presentation data used by the UI.

Suggested fields:

- `VisualKey`
- `IconPath`
- `SpritePath`
- `ColorHint`
- `SizeClass`

This data supports rendering but should not contain live state.

---

## Base Attributes

BaseAttributes are the default values for a piece.

Examples:

- Movement
- Strength
- ArmorClass
- HitPoints
- Attack
- Defense
- Range

The engine stores them, but game rules interpret them.

---

## Rules Metadata

RulesMetadata stores ruleset-specific fields that do not belong in the generic engine.

Examples:

### Checkers
- `StartsKing = false`

### AD&D 1e
- `THAC0 = 19`
- `SaveVsDeath = 14`

### BRP
- `SwordSkill = 55`
- `Dodge = 32`

This data should be available to the rules module but not interpreted by the base engine.

---

## Position

Position belongs to the PieceInstance, not the PieceDefinition.

A piece may be:

- on a board coordinate
- in a zone
- off-board
- removed from play

Suggested position forms:

- square coordinate
- hex coordinate
- zone reference
- null

---

## Attribute Strategy

### Initial Recommendation

Use **copy on create**.

When a PieceInstance is created from a PieceDefinition:

- copy BaseAttributes into CurrentAttributes
- allow CurrentAttributes to change during play
- do not modify the original definition

This is simpler and safer for early development.

---

## Minimal Checkers Example

### PieceDefinitions

- Red Checker
- Black Checker

### PieceInstances

Each checker on the board is a PieceInstance.

Instance state includes:

- board position
- side
- promoted/king status

---

## Tactical / Wargame Example

### PieceDefinition

Infantry Counter

BaseAttributes:
- Movement = 4
- Strength = 3
- Range = 1

### PieceInstance

Infantry Counter #7 at Hex C5

Current state:
- Disrupted
- HasMoved
- Current position

---

## RPG Example

### PieceDefinition

Goblin Spearman

BaseAttributes:
- HitPoints = 5
- ArmorClass = 6
- Movement = 9

RulesMetadata:
- THAC0 = 20
- Morale = 7

### PieceInstance

Goblin Spearman #2 in current encounter

Current state:
- Position = Room12-A
- DamageTaken = 3
- Status = Frightened

---

## Tokens and Markers

Not every piece is a character or unit.

Some pieces are simple markers.

Examples:

- Stunned Marker
- Fire Marker
- Objective Token
- Control Marker

These should still use PieceDefinition and PieceInstance unless a later optimization is needed.

---

## CSV Import Role

CSV import should normally create or update PieceDefinitions, not PieceInstances.

Examples of imported content:

- checkers piece types
- monsters
- counters
- gear tokens
- magic-effect markers

Live session placement should create PieceInstances separately.

---

## Persistence

Persist separately:

### Content Store
- PieceDefinitions

### Session Store
- PieceInstances

Do not mix reusable content with live play state.

---

## Naming Guidance

Preferred terms:

- `PieceDefinition`
- `PieceInstance`

Avoid vague terms like:

- object
- thing
- entity

Use more specific names only when needed in specialized modules:

- MonsterDefinition
- UnitDefinition
- TokenDefinition

But keep the core model unified.

---

## Initial Required Fields Recommendation

### PieceDefinition
- `DefinitionId`
- `Name`
- `Category`

### PieceInstance
- `InstanceId`
- `DefinitionId`
- `SessionId`
- `Position`

These are enough to begin early engine work.

---

## Summary

The Piece Definition Model exists to keep:

- reusable content
- live game state

cleanly separated.

This makes the system easier to build, test, import, render, and extend across many game types.
