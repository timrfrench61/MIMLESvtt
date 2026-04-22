# Quickstart

This application supports three main usage modes:

- **Game template setup**: create and prepare scenario/session layouts.
- **Solo play/testing**: run and validate gameplay workflows by yourself.
- **Multiplayer-ready staging**: prepare sessions and workflows for future multiplayer usage.

This guide walks you through a basic flow in Workspace.

## 1. Create a session

1. Open the app.
2. Go to **Workspace**.
3. Click **Create New Session**.
4. Confirm you see session details in **Session Summary**.

## 2. Add a surface

1. In **Setup > Add Surface**, enter a **Surface Id** (example: `surface-1`).
2. Enter a **Definition Id** (example: `def-surface-1`).
3. Leave defaults for type and coordinate system, or choose your own.
4. Click **Add Surface**.
5. Confirm the new surface appears in **Surfaces**.

## 3. Add a piece

1. In **Setup > Add Piece**, enter:
   - **Piece Id** (example: `piece-1`)
   - **Definition Id** (example: `def-piece-1`)
   - **Target Surface Id** (`surface-1`)
2. Set X/Y if needed (or leave at default).
3. Click **Add Piece**.
4. Confirm the piece appears in **Pieces** and on the board.

## 4. Move a piece

1. In **Pieces**, select the piece from the dropdown.
2. In **Move Piece**, set the target surface/X/Y.
3. Click **Move**.
4. Confirm the piece location updates in the list and board.

## 5. Use stamp mode

1. In board controls, enable **Add Piece At Board Click**.
2. Enable **Stamp Mode (repeat placement)**.
3. Set:
   - **Quick Piece Id**
   - **Quick Definition Id**
4. Click on the board to place pieces.
5. Confirm each click adds a new piece and advances the next id preview.

## 6. Import a scenario

1. Enter a scenario file path in **Scenario file path (.scenario.json)**.
2. Click **Open Scenario For Pending Plan**.
3. Confirm **Pending Scenario** in Session Summary now shows the scenario title.

## 7. Activate a scenario

1. Click **Activate Pending Scenario**.
2. Confirm the board/session updates to the imported scenario layout.
3. Confirm **Pending Scenario** is cleared in Session Summary.
