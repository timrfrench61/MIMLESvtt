# Checkers Extension Class List

## Game Root
- CheckersGame — Represents one live game of checkers running on top of the VTT core.

## Rules and Variant
- CheckersRules — Validates moves, captures, kinging, turn flow, and endgame conditions.
- CheckersVariant — Defines which ruleset is in use, such as American checkers or another variant.

## Board and Setup
- CheckersBoardDefinition — Defines the size, playable squares, and layout rules of a checkers board.
- StartingPosition — Defines the initial arrangement of pieces at the beginning of the game.

## Pieces
- CheckersPiece — Represents a checker piece with game-specific properties.
- PieceColor — Identifies which side a piece belongs to.
- PieceRank — Identifies whether a piece is a man or a king.

## Players and Sides
- PlayerSeat — Represents one player slot in the game.
- PlayerSide — Identifies which side of the board a player controls.

## Turn and Match State
- TurnState — Tracks whose turn it is and whether a move sequence is still in progress.
- MoveSequence — Represents a complete move, including single-step movement or multi-jump captures.
- GameStatus — Tracks whether the game is active, won, drawn, or resigned.
- MatchResult — Stores the final outcome of the game.

## Checkers Actions
- SelectPieceAction — Represents selecting a checker for a possible move.
- SubmitMoveAction — Represents an attempted legal checker move.
- CrownPieceAction — Promotes a checker to king when it reaches the far side.
- EndTurnAction — Advances play to the next turn after the move is complete.
- ResignGameAction — Ends the game because one player resigns.
- OfferDrawAction — Represents a player offering a draw.
- AcceptDrawAction — Represents a player accepting a draw offer.

## Online Play
- MatchLobby — Manages player joining, seating, and readiness before the game starts.
- PlayerConnection — Tracks the connected user or client for each player seat.
- GameSynchronizationState — Tracks the authoritative synchronization state for online play.
- ActionLog — Records moves and actions for replay, auditing, and synchronization.