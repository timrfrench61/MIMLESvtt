VTT View Design Document
1. Purpose
This document defines the View Layer (UI/UX Design) for the Blazor Virtual Tabletop (VTT).

It specifies:
- user-facing screens
- navigation flow
- layout structure
- interaction responsibilities

This document is design-only and contains no implementation details.
2. Design Goals
1. Game-Agnostic
- Must support board games, war games, and RPGs

2. State-Driven UI
- UI reflects the Table State

3. Modular Screens
- Each screen has a single responsibility

4. Pre-Alpha Friendly
- Incomplete features must be visible but toggleable
3. Workspace Shell
The Workspace is the root container of the VTT UI.

Responsibilities:
- Host all major views
- Control navigation between screens
- Maintain session context
- Provide global controls
4. Launch Flow
Start → Open Workspace → Select Game System → Select Session Type → Load Tabletop Screen

Session Types:
- New Game / Scenario
- Load Saved Game / Scenario
5. Core Views
Game System Picker:
- Select rule/content system

Scenario / Save Picker:
- Start new or load existing

Tabletop Screen:
- Main gameplay interface
6. Tabletop Layout
Top Bar: Game, Scenario, Save, Settings
Left Panel: Tools (Select, Move, Measure, Dice)
Main Surface: Board / Map / Tokens
Bottom Panel: Action Log
Right Panel: Properties / Chat (optional)
7. User Interaction Model
Player Actions:
- Select, move, rotate
- Roll dice, draw cards
- Save/load

GM Actions:
- Modify board
- Manage players
- Override rules
8. Development Views
Feature Toggles:
- Multiplayer
- Chat
- Fog of War
- Rules Engine
- Persistence
9. Navigation Model
Workspace → Game System → Scenario → Tabletop → Actions → State Change → UI Refresh
10. View vs Engine
View:
- Render state
- Capture input

Engine:
- Validate actions
- Update state
11. Key Rule
The UI is not the source of truth. The Table State Model is authoritative.
12. Summary
Structured, scalable UI supporting multiple game systems without redesign.
