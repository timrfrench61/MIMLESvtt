# Rules Plugin Framework

## Purpose

Define how **game rules plug into the Tabletop Engine**.

The engine:
- manages state (board, pieces, session, turn)
- does NOT contain game-specific logic

Rules modules:
- define what actions are allowed
- define what happens when actions occur

---

## Core Idea

All gameplay follows this flow:

Command → Validate → Resolve → Apply → Log

---

## Key Concepts

### Ruleset
A complete set of mechanics.

Examples:
- Checkers
- AD&D 1e
- BRP
- War game system

---

### Rules Module
The implementation of a ruleset.

A rules module provides:
- validation rules
- resolution logic
- turn sequencing behavior

---

### Command
A user action.

Examples:
- MovePiece
- EndTurn
- AttackTarget
- SkillCheck

Commands are **engine-level**, not rules-specific.

---

### Validation
Determines if a command is allowed.

Examples:
- Is it your turn?
- Is the move legal?
- Is the target valid?

Invalid commands are rejected before any state change.

---

### Resolution
Determines the outcome of a valid command.

Examples:
- Move piece
- Capture piece
- Apply damage
- Advance turn

---

### Effect
A change applied to the session.

Examples:
- piece moved
- piece removed
- condition applied
- turn advanced

---

## Engine vs Rules Responsibility

### Engine
- stores session state
- routes commands
- applies effects
- logs events

### Rules Module
- validates commands
- produces effects
- defines turn order behavior

---

## Command Lifecycle

1. User issues command  
2. Engine builds context  
3. Rules module validates  
4. If valid → resolve  
5. Effects returned  
6. Engine applies effects  
7. Event logged  
8. Turn advances if needed  

---

## Turn and Sequence

Turn handling is defined by the rules module.

Examples:

### Checkers
- alternating turns

### RPG (AD&D / BRP)
- initiative-based order

### War Game
- phase-based turns (movement → combat → supply)

---

## Example: Checkers Move

Command:
- MovePiece(A3 → B4)

Validation:
- diagonal move
- correct direction
- destination empty

Resolution:
- move piece
- check capture
- check promotion

Effects:
- piece moved
- piece captured (optional)
- piece promoted (optional)
- turn advanced

---

## Example: RPG Attack

Command:
- AttackTarget(attacker, defender)

Validation:
- attacker can act
- target is valid

Resolution:
- roll attack
- compare vs defense
- roll damage

Effects:
- damage applied
- condition applied (optional)

---

## Example: BRP Skill Check

Command:
- SkillCheck(character, skill)

Validation:
- skill exists

Resolution:
- roll d100
- compare to skill

Effects:
- success/failure recorded

---

## Extensibility

Rules modules may:
- define new command types
- define new effects
- define custom turn structures

Rules modules must NOT:
- directly modify session state
- bypass validation
- depend on UI or persistence

---

## Multi-System Support

Each session uses one rules module.

The engine does not change when switching between:
- checkers
- RPG systems
- war games

Only the rules module changes.

---

## Minimal Implementation Path

Start with:

1. Checkers rules module  
2. Validate + resolve + apply flow  
3. Simple turn switching  

Then expand to:
- RPG rules
- war game rules
- complex sequencing

---

## Summary

The Rules Plugin Framework:

- separates game logic from engine
- supports multiple game systems
- keeps the engine simple and reusable
- enables long-term extensibility