# Backlog Additions from 00-overview/31

Source: `docs/00-overview/31-tactical-scenario-setup-ui.md`
Target backlog: `docs/05-backlog/05-backlog-ui-presentation.md`

## Code Project Additions

- [x] Add a dedicated tactical scenario setup page/component with explicit route.
- [x] Add view-model/state object for setup mode (surface, side assignment, placement context).
- [x] Add UI actions for side/faction assignment wired to `PlayerSeat` and participant ownership.
- [x] Add integration flow to save setup directly as scenario snapshot with validation messages.
- [x] Add tests for setup create/save path and invalid-path rejection behavior.

## Slice Plan

1. Route + page shell (done)
2. Setup state/view-model (surface, side assignment, placement context)
3. Side/faction assignment actions wired to PlayerSeat + piece ownership
4. Direct scenario-save integration and validation feedback
5. Automated tests for setup save success and invalid-path rejection
