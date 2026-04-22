# Workspace Use Cases (Detailed)

## Purpose

This page describes detailed use-case flows for working in Workspace.

It is more detailed than Quickstart and intended for repeatable pre-alpha operation.

---

## Use Case 1: Create a New Session and Basic Table Setup

1. Open **Workspace**.
2. Open the right-side controls panel.
3. Ensure **Tabletop Surface** is set to **Default x-y grid**.
4. Click **Create New Session**.
5. Confirm the success message appears.
6. In **Session Details**, confirm title/id/session summary values are present.
7. In **Add Surface**, enter surface id/definition and click **Add Surface**.
8. In **Add Piece**, enter piece id/definition/target surface and click **Add Piece**.
9. Confirm piece appears in board and in Pieces list.

Expected outcome:
- Session, surface, and piece exist in current live session.

---

## Use Case 2: Move and Rotate a Piece Safely

1. In **Pieces**, select a piece.
2. In **Move Piece**, set target surface and x/y.
3. Click **Move**.
4. Confirm board position and piece table coordinates update.
5. In **Rotate Piece**, set degrees.
6. Click **Rotate**.
7. Confirm board rotation and piece rotation value update.

Expected outcome:
- Piece position and rotation mutate through action flow and are visible.

---

## Use Case 3: Add and Remove Marker/State Data

1. Select a piece.
2. In **Add Marker**, enter marker id and click **Add Marker**.
3. Confirm marker count increases.
4. In **Remove Marker**, use marker id and click **Remove Marker**.
5. In **Change Piece State**, enter key/value and click **Update State**.
6. In selected piece data block, verify state appears.
7. Remove state key with **Remove Key**.

Expected outcome:
- Marker and state changes are visible and logged as operations.

---

## Use Case 4: Save and Reload Session

1. Enter path in **Game save file path (.session.json)**.
2. Click **Save Game As**.
3. Confirm success message.
4. Click **Create New Session** to clear context.
5. Click **Load Game**.
6. Confirm session data (surfaces/pieces/participants) restores.

Expected outcome:
- Session round-trip works from saved file.

---

## Use Case 5: Scenario Pending Plan and Activation

1. Enter path in **Scenario file path (.scenario.json)**.
2. Click **Open Scenario For Pending Plan**.
3. Confirm pending scenario is shown in Session Summary.
4. Click **Activate Pending Scenario**.
5. Confirm board/session updates to scenario content.
6. Confirm pending scenario field clears.

Expected outcome:
- Pending-plan workflow completes and activates scenario safely.

---

## Use Case 6: Stamp Mode Repeated Placement

1. Enable **Add Piece At Board Click**.
2. Enable **Stamp Mode (repeat placement)**.
3. Set quick placement fields (id/definition/owner/rotation).
4. Click board multiple times.
5. Confirm multiple pieces are created and id preview advances.
6. Use **Id +1 / Id -1 / Reset Id To Base** as needed.

Expected outcome:
- Repeated placement works with controlled id progression.

---

## Use Case 7: Surface Workflow Strip and Defaults

1. Add at least two surfaces.
2. Use **Activate** in strip to switch active surface.
3. Set default preset for a surface via default preset dropdown.
4. Use **Use Active Preset** to map current preset to selected surface.
5. Confirm switching surfaces updates board context accordingly.

Expected outcome:
- Active surface and default-preset mapping are consistent.

---

## Use Case 8: Queue-Based Multi-Surface Placement

1. In stamp mode, configure surface + preset + next id.
2. Click **Add Current Setup To Queue**.
3. Add multiple queue rows.
4. Use **Apply** on specific row or **Next Queue Item**.
5. Confirm board context updates to selected queue item.
6. Save template with **Save Current Queue As Template**.
7. Apply template and verify rows restore.

Expected outcome:
- Queue and template flows accelerate multi-surface setup.

---

## Use Case 9: Participant and Turn Workflow

1. Add participants (id/name).
2. Initialize turn order with participant ids.
3. Use **Next Turn** and **Previous Turn**.
4. Update phase with **Set Phase**.
5. Reorder participants with row **↑ / ↓** controls.
6. Assign selected piece owner using **Apply Owner**.

Expected outcome:
- Turn/phase and ownership workflows remain coherent.

---

## Use Case 10: Developer Toggle Gating

1. In **Developer Settings**, disable a feature path (for example board interaction).
2. Attempt the related action.
3. Confirm message appears:
   - **Feature not enabled (development toggle)**
4. Re-enable the toggle and retry action.

Expected outcome:
- Disabled paths fail clearly; enabled paths resume normally.

---

## Use Case 11: Tabletop Fallback States

1. Set **Tabletop Surface** to **Missing tabletop**.
2. Confirm missing tabletop status panel is visible.
3. Set to **Page not loading**.
4. Confirm page-loading error status panel is visible.
5. Set to **Default x-y grid**.
6. Confirm grid tabletop is visible behind controls.

Expected outcome:
- Operator can diagnose tabletop state quickly without silent failures.

---

## Recommended Operating Order (Pre-Alpha Session)

1. Set tabletop state to **Default x-y grid**.
2. Create session.
3. Add surfaces.
4. Add participants (if needed).
5. Add and place pieces.
6. Configure turn/phase.
7. Use stamp/presets/queue for setup speed.
8. Save session.
9. Optionally import/activate scenario.
10. Validate operation messages and logs before handoff.
