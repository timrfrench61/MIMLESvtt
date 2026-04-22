# CSV Import Overview

## Purpose

Define how structured game content is imported into the Tabletop Engine using CSV files.

The CSV import system is intended to support:

- RPG content
- board-game content
- tactical unit data
- strategic war-game data
- reusable markers and tokens

This document defines the overall import approach, not the individual CSV schemas.

---

## Goals

The CSV import system should:

- allow bulk loading of reusable content
- support GM-maintained libraries
- support test data creation
- separate imported definitions from live session state
- validate imported data clearly
- be extensible for multiple game systems

---

## Core Rule

CSV import creates or updates **definitions**, not live session instances.

Import targets include:

- PieceDefinitions
- MonsterDefinitions
- UnitDefinitions
- EquipmentDefinitions
- MagicItemDefinitions
- MarkerDefinitions
- Scenario seed data (later, if supported)

CSV import should not directly create active in-play PieceInstances unless a special scenario import workflow is defined.

---

## Import Scope

### In Scope

- bulk import of reusable content
- create/update operations
- validation and error reporting
- support for multiple content categories
- sample/test packet support

### Out of Scope (initially)

- direct import into active live sessions
- spreadsheet-style in-app editing
- complex relational dependency resolution
- user-defined schema designer

---

## Import Workflow

### Standard Flow

1. user selects import type  
2. user uploads CSV file  
3. system parses rows  
4. system validates row structure  
5. system validates field values  
6. system maps rows to domain definitions  
7. system reports errors and warnings  
8. valid rows are saved  
9. import summary is displayed  

---

## Import Targets

Initial likely targets:

- monsters
- equipment
- magic items
- treasure
- units/counters
- markers/tokens
- piece definitions

Not every project phase needs every target immediately.

---

## Import Modes

### Create Only
- insert new records
- fail if key already exists

### Update Only
- update existing records
- fail if key does not exist

### Upsert
- create if missing
- update if existing

Initial recommendation:
- support **Create Only** first
- add **Upsert** later

---

## Key Field Requirement

Each import type should define a stable key.

Examples:

- `DefinitionId`
- `ExternalId`
- `Code`
- `Name` (only if guaranteed unique, usually not preferred)

A reliable key is required for update and upsert scenarios.

---

## Validation Layers

### 1. File Validation
Checks:

- file is present
- file is readable
- CSV format is valid
- headers are present

### 2. Schema Validation
Checks:

- required columns exist
- column names are recognized
- duplicate headers are rejected

### 3. Row Validation
Checks:

- required values are present
- data types are valid
- enum/category values are allowed
- text length limits are respected

### 4. Domain Validation
Checks:

- referenced categories are valid
- ruleset-specific values are allowed
- business/domain rules are satisfied

---

## Import Results

Each import should return a structured result.

Suggested summary:

- total rows read
- valid rows
- invalid rows
- created rows
- updated rows
- warnings
- errors

The user should be able to see which rows failed and why.

---

## Error Reporting

Error reporting should be clear and row-specific.

Each error should include:

- row number
- field name
- error type
- readable message

Example:

- Row 12, `Category`: invalid value `CreatureToken`
- Row 19, `DefinitionId`: required field missing

---

## Warning Reporting

Warnings are non-blocking issues.

Examples:

- optional field missing
- unknown tag ignored
- deprecated column used

Warnings should not stop valid rows from importing unless configured otherwise.

---

## Partial Success

Initial recommendation:

- allow partial success
- import valid rows
- reject invalid rows
- provide full report

This is usually more practical than all-or-nothing imports.

---

## Import Categories

The system should support category-specific schemas.

Examples:

### Piece Definitions
Generic reusable pieces

### Monsters
RPG creature definitions

### Units / Counters
Tactical or war-game unit definitions

### Equipment
Usable gear definitions

### Magic Items
Special item definitions

### Treasure
Loot and reward definitions

### Markers / Tokens
State indicators and lightweight pieces

Each category may have its own CSV spec.

---

## Generic vs Specialized Imports

### Generic Piece Import
Useful when importing broad piece types for board, tactical, or custom systems.

### Specialized Imports
Useful when a domain needs richer structure, such as:
- monsters
- units
- equipment
- magic items

Recommendation:
- support both
- use specialized imports where structure matters

---

## Relationship to Piece Definition Model

CSV import is mainly a content creation pipeline.

It should populate reusable definitions such as:

- PieceDefinition
- MonsterDefinition
- UnitDefinition

It should not manage live state such as:

- board position
- current damage
- temporary conditions
- turn order

That belongs to session state, not import.

---

## Relationship to Rules Modules

The import system stores data.

Rules modules interpret the data.

Examples:

- a checkers rules module interprets piece categories and flags
- an AD&D module interprets monster stats
- a BRP module interprets skill values
- a war-game module interprets movement and combat values

The import system should not embed rules behavior.

---

## Relationship to UI

The UI should provide:

- import type selection
- file upload
- validation result display
- summary display
- failure review

Later enhancements may include:

- downloadable error reports
- dry-run preview
- import templates

---

## Template Strategy

For each supported import type, provide:

- required columns
- optional columns
- sample CSV
- validation notes

This reduces ambiguity and makes testing easier.

---

## Test Data Packet Support

CSV import should work with test data packets.

A test packet may include:

- one or more CSV files
- packet metadata
- expected results
- known invalid examples

This supports:

- QA
- regression testing
- onboarding
- repeatable demos

---

## Initial Implementation Recommendation

Build in this order:

1. generic CSV parser  
2. header validation  
3. row validation  
4. import result reporting  
5. generic piece-definition import  
6. monster import  
7. equipment import  
8. magic item import  
9. unit/counter import  

This allows engine-first growth while still serving RPG needs.

---

## Minimal Initial Features

For first implementation:

- one file upload at a time
- one import type at a time
- required/optional column validation
- row-level error reporting
- create-only mode
- import summary

That is enough to prove the pipeline.

---

## Future Enhancements

Possible future additions:

- batch packet import
- upsert support
- dependency-aware imports
- dry run mode
- rollback support
- in-app import templates
- export-to-CSV support
- scenario setup import
- campaign seed import

---

## Summary

The CSV import system is a content pipeline for reusable tabletop definitions.

It should:

- import definitions, not live state
- validate clearly
- support multiple game styles
- remain extensible
- produce useful error reporting

Start simple, then expand by content category.