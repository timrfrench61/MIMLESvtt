# Turn Sequence Model

## Purpose

Define how play advances in the Tabletop Engine.

This model must support:

- simple alternating turns
- initiative-based play
- phase-based war games
- activation-based tactical systems

The engine must support many game styles without assuming one fixed turn structure.

---

## Core Rule

The engine owns turn state.

Rules modules define how turn state is created, interpreted, and advanced.

---

## Main Concepts

### Turn
A bounded opportunity for a side, seat, actor, or unit to act.

Examples:
- one move in checkers
- one character’s action in RPG combat
- one side’s turn in a war game

---

### Round
A full cycle in which all intended participants have had a turn or activation.

Examples:
- both players move once in checkers
- every combatant acts once in an RPG round

Not every game needs a round concept.

---

### Phase
A named subdivision of a turn or round.

Examples:
- Movement
- Combat
- Reinforcement
- End Phase

Common in war games and some tactical systems.

---

### Step
A smaller procedural unit inside a phase or turn.

Examples:
- declare attack
- resolve hit
- apply damage

Use only when needed.

---

### Activation
A temporary permission for a specific piece, unit, or actor to act.

Common in skirmish and tactical games.

Activation is more specific than a turn.

---

### Sequence
The full structure that defines how play advances.

A sequence may be:

- alternating turn
- side-based turn
- initiative order
- activation order
- phase-based structure

Sequence is the broad engine term.

---

## Design Goals

The model must:

- support different game types
- remain rules-agnostic
- be easy to save and restore
- be easy to synchronize over the network
- allow simple implementations first

---

## TurnState

TurnState is the runtime record of where play is in the sequence.

### Suggested Fields

- `SequenceType`
- `TurnNumber`
- `RoundNumber`
- `CurrentSideId`
- `CurrentSeatId`
- `CurrentActorId`
- `CurrentPhase`
- `CurrentStep`
- `ActivationOrder`
- `HasEnded`

Not every field is required for every game.

---

## Sequence Types

### Alternating Turn

Used for simple two-sided games such as checkers.

Example:

- Red moves
- Black moves
- Red moves
- Black moves

Suggested active fields:

- `SequenceType`
- `TurnNumber`
- `CurrentSideId`

---

### Side-Based Turn

Used for games where an entire faction or side acts before another side acts.

Example:

- Blue side turn
- Red side turn

Suggested active fields:

- `SequenceType`
- `TurnNumber`
- `CurrentSideId`
- `CurrentPhase` (optional)

---

### Initiative-Based Sequence

Used for RPG combat and some tactical systems.

Example:

- Actor A
- Actor B
- Actor C
- new round

Suggested active fields:

- `SequenceType`
- `RoundNumber`
- `CurrentActorId`
- `ActivationOrder`

---

### Phase-Based Sequence

Used for war games and larger procedural games.

Example:

- Movement Phase
- Combat Phase
- Supply Phase
- End Phase

Suggested active fields:

- `SequenceType`
- `TurnNumber`
- `CurrentSideId`
- `CurrentPhase`
- `CurrentStep` (optional)

---

### Activation-Based Sequence

Used for systems where units activate one at a time.

Example:

- Unit 1 activates
- Unit 7 activates
- Unit 3 activates
- End round

Suggested active fields:

- `SequenceType`
- `RoundNumber`
- `CurrentActorId`
- `ActivationOrder`

---

## Recommended Initial Model

Start with a generic model that allows unused fields to remain null.

### Minimum Starting Fields

- `SequenceType`
- `TurnNumber`
- `CurrentSideId`
- `CurrentActorId`
- `CurrentPhase`
- `HasEnded`

This is enough for early development.

---

## Checkers Example

### Structure
- alternating turns
- no phases
- no initiative
- no activations beyond current side

### Suggested State
- `SequenceType = AlternatingTurn`
- `TurnNumber = 5`
- `CurrentSideId = Black`

### Advance Rule
After a legal move:
- switch current side
- increment turn number

---

## RPG Combat Example

### Structure
- initiative-based
- optional round tracking
- active combatant changes one by one

### Suggested State
- `SequenceType = InitiativeBased`
- `RoundNumber = 2`
- `CurrentActorId = Goblin-03`
- `ActivationOrder = [Fighter, Goblin-03, Cleric, Goblin-04]`

### Advance Rule
After action ends:
- move to next actor
- if list ends, increment round

---

## War Game Example

### Structure
- side-based turns
- multiple phases per turn

### Suggested State
- `SequenceType = PhaseBased`
- `TurnNumber = 3`
- `CurrentSideId = Blue`
- `CurrentPhase = Combat`

### Advance Rule
After phase completion:
- move to next phase
- after final phase, switch side or increment turn

---

## Activation Example

### Structure
- units activate individually
- activation list may be predetermined or dynamic

### Suggested State
- `SequenceType = ActivationBased`
- `RoundNumber = 1`
- `CurrentActorId = Unit-12`
- `ActivationOrder = [Unit-12, Unit-05, Unit-09]`

### Advance Rule
After each activation:
- move to next eligible unit
- when all are done, end round

---

## Responsibilities

### Engine Responsibilities

The engine should:

- store TurnState
- expose current active side/actor
- persist TurnState
- synchronize TurnState
- apply updates produced by rules modules

### Rules Module Responsibilities

The rules module should:

- define initial turn state
- define valid actor/side for current point in play
- define how turn state advances
- define special sequencing rules

---

## Turn Advancement

Turn advancement should happen through rules logic, not engine assumptions.

Examples:

- checkers switches sides
- RPG combat advances actor order
- war games advance phase then side
- activation systems remove actors from eligible pool

The engine should only store the result.

---

## End of Turn / End of Round / End of Game

The model should distinguish:

### End of Turn
Current acting side or actor is finished.

### End of Round
A complete cycle of actors or sides is finished.

### End of Game
The session has reached a victory, defeat, or stop state.

Suggested field:

- `HasEnded`

Optional future fields:

- `WinnerSideId`
- `EndReason`

---

## Sequence and Commands

Turn state controls which commands are allowed.

Examples:

- only current side may move in checkers
- only current actor may attack in RPG combat
- only active side may enter Combat Phase in a war game

Validation should use TurnState as one of its main inputs.

---

## Persistence

TurnState must be saved with session state.

At minimum, save:

- sequence type
- current side or actor
- current phase if any
- turn and round counters
- ended state

This ensures save/load works correctly.

---

## Networking

TurnState must be synchronized as part of shared session state.

Clients need current turn information for:

- rendering active side
- enabling/disabling actions
- displaying phase or round
- avoiding illegal commands

---

## UI Considerations

The UI should be able to display:

- current side
- current actor
- turn number
- round number
- current phase
- current step if used

The exact display depends on the game type.

---

## Initial Recommendation

Implement in this order:

1. Alternating turn support
2. Initiative-based support
3. Phase-based support
4. Activation-based refinements

This matches the likely project order:

- checkers first
- RPG support next
- larger tactical/war-game systems later

---

## Summary

The Turn Sequence Model defines how play advances without forcing a single style of play.

It supports:

- checkers-style alternating turns
- RPG initiative
- war-game phases
- tactical activations

The engine stores turn state.
Rules modules define how it changes.
Rules modules define how it changes.