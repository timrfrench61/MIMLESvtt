VTT Model Design Document (Blazor MVU)
1. Purpose
This document defines the Model layer for a Blazor MVU-based Virtual Tabletop (VTT). The Model represents the complete in-memory state of a game session.
2. Core Principles
• Model represents current state only
• No UI logic
• No database or persistence logic
• Minimal helper methods allowed
• All state changes occur in the Update layer
3. Top-Level State Object
TabletopState is the root object containing all data needed to render and run the session.
Contains:
• Board
• Tokens
• Players
• Turn state
• Selection state
4. Core Model Components
Board: Defines grid or coordinate system
Token: Represents pieces or units
Player: Represents participants
Position: Coordinates on board
TurnState: Tracks turn and phase
5. Example Model Structure
TabletopState
  - Board
  - Tokens
  - Players
  - SelectedTokenId
  - TurnState
6. Model Layer Rules
Include:
• Data structures
• Simple validation helpers
Exclude:
• Blazor components
• Database or file I/O
• HTTP or API calls
• Business workflows
• UI event handling
7. Relationship to MVU
Model = current state
Update = state transitions
View = UI rendering
8. Design Guidance
• Keep model simple and explicit
• Avoid unnecessary abstraction
• Prefer records for immutability
• Organize around features, not layers
