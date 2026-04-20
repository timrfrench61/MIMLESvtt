# Persistence and Import/Export

## File Location

docs/03-import-export/persistence-and-import-export.md

---

## Purpose

This document defines how all domain data in the Blazor VTT is stored, restored, imported, and exported.

It establishes a single, consistent model for:

- persisting live TableSession state
- exchanging reusable content
- packaging assets with definitions
- validating incoming data
- maintaining compatibility across versions

This document applies to all persisted representations derived from the domain model, including TableSession, definitions, assets, and action history.

---

## Core Principles

### Separation of Runtime State and Definitions

The system distinguishes between:

- runtime state (TableSession)
- reusable definitions (content)

Runtime state represents the current state of a live or saved session.

Reusable definitions represent authored data such as:

- piece definitions
- surface definitions
- marker definitions
- item definitions
- creature definitions
- scenario definitions

Runtime state references definitions using identifiers such as DefinitionId.

Definitions are stored separately from session state unless explicitly embedded for portability.

---

### Domain-Only Persistence

Only domain-relevant data is persisted.

Persisted data includes:

- session identity and metadata
- surfaces and pieces
- placement and coordinates
- ownership and visibility
- module state
- action history

Persisted data excludes:

- UI-only state
- temporary interaction state
- client-only rendering state

---

### Explicit State Requirement

All important state must be represented explicitly.

State must not depend on:

- UI reconstruction
- implicit ordering
- transient calculations

Persisted state must be sufficient to:

- reconstruct a session
- validate integrity
- support replay or migration later

---

### Versioning

All persisted data structures must include a version field.

Versioning is required for:

- schema evolution
- backward compatibility
- migration logic

No implicit version detection is allowed.

---

### Validation Before Acceptance

All imported data must be validated before use.

Validation must include:

- structural validation
- reference validation
- domain invariant validation

Invalid data must be rejected.

Partial import is not allowed.

---

## Persistence Types

The system supports four persistence types:

- Session Save
- Scenario Export
- Content Pack
- Action Log Export

Each has a distinct purpose and structure.

---

## Session Save

A Session Save is a serialized snapshot of a live TableSession.

It represents the complete runtime state at a point in time.

A Session Save includes:

- TableSession.Id
- TableSession.Title
- TableSession.Participants
- TableSession.Surfaces
- TableSession.Pieces
- TableSession.Options
- TableSession.Visibility
- TableSession.ActionLog
- TableSession.ModuleState

A Session Save does not include:

- full definition records
- full asset binaries
- unrelated content library data

Instead, it references definitions and assets using identifiers.

A Session Save is used to:

- save game progress
- reload game state
- duplicate sessions
- archive sessions

The conceptual structure is:

```json
{
  "Version": 1,
  "TableSession": {}
}
```

## Scenario Export

A Scenario Export represents a prepared starting configuration.

It is not a snapshot of ongoing play.

It includes:

- prepared surfaces
- initial piece placement
- initial piece state
- initial visibility configuration
- initial table options
- module configuration

A Scenario Export does not include:

- action history
- runtime drift
- UI state

A Scenario Export is used for:

- reusable encounters
- wargame setups
- templates
- demonstrations

The conceptual structure is:

```json
{
  "Version": 1,
  "Scenario": {}
}
```

## Content Pack

A Content Pack represents reusable definitions and assets.

It is independent of runtime sessions.

It includes:

- definitions (piece, surface, marker, item, scenario)
- asset references
- metadata

It may include:

- multiple definitions grouped logically
- optional scenario definitions
- optional module-specific data

A Content Pack does not include:

- live session state
- participant data
- runtime-only values

It is used for:

- importing ruleset content
- sharing authored data
- moving content between systems

The conceptual structure is:

```json
{
  "Version": 1,
  "Manifest": {},
  "Definitions": [],
  "Assets": []
}
```

## Action Log Export

An Action Log Export represents accepted actions from a session.

It includes:

- session identifier
- ordered action records
- timestamps
- actor identifiers
- action payloads

It does not include:

- full session reconstruction data
- definitions
- UI state

It is used for:

- debugging
- auditing
- replay support (future)

The conceptual structure is:

```json
{
  "Version": 1,
  "SessionId": "id",
  "Actions": []
}
```

## Import Rules

All imports must follow strict validation rules.

### Validate Structure

Check:

- Version exists
- required root fields exist
- structure matches expected format

### Validate References

Check:

- DefinitionId exists
- SurfaceId exists
- ParticipantId exists
- Stack and Container ids are valid if present
- Asset references are valid

### Validate Domain Invariants

Check:

- no duplicate ids
- valid placements
- valid visibility state
- consistent relationships
- no broken references

### Reject Invalid Data

If validation fails:

- reject entire import
- do not mutate session
- do not partially accept data

## Export Rules

### Export Only Domain Data

Exclude:

- UI-only state
- temporary interaction data

### Ensure Consistency

All references must be valid.

No broken identifiers.

### Include Version

Every export must include a Version field.

### Maintain Stable Structure

Where possible:

- maintain consistent ordering
- produce predictable output

This supports:

- debugging
- diffing
- testing

## Storage Strategy

### Structured Data

Use a database for:

- session metadata
- content metadata
- indexing

### Session State

Use JSON for:

- TableSession snapshots
- scenario exports

### Assets

Use file or blob storage for:

- images
- maps
- icons

## Summary

TableSession is persisted as JSON.

Definitions are stored separately and referenced.

All data is versioned.

Imports are validated before acceptance.

Exports include only domain state.