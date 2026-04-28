# Copilot Instructions

## Project Guidelines
- Use conservative, pass-by-pass implementation for this repo: make only scoped changes requested, preserve existing serializer contracts, keep TableSession/action-system naming from design packet, and avoid unrelated UI, storage, multiplayer, or module changes unless explicitly requested.
- Prefer larger, practical implementation blocks over thin wrapper/micro-polish passes, while keeping conservative scoped changes and maintaining existing code layers intact.
- Shift strategy to larger UI-connected vertical slices delivering visible working capability, avoiding micro-infrastructure passes unless directly required by the UI slice.
- Avoid architecture jargon like 'boundaries'; use direct programming terms like 'code layers' and keep explanations concrete.
- Maintain a clean separation where root domain classes are data models located in a root/domain models directory, not placed under persistence.
- Consider using a more user-facing name like 'Gamebox' instead of 'VttGamebox' for content package naming.
- Structure the domain model so that `VttSession` has a single `VttCampaign` and a `CurrentVttScenario` (scenario static once published), rather than multiple campaigns per session.

## Component Refactoring
- When refactoring a Razor component to use code-behind, move the `@code` block into a `.razor.cs` partial class instead of making unrelated fixes.

## Navigation Guidelines
- When discussing route locations, distinguish between where a page is defined and where its navigation link is declared (e.g., NavMenu link vs page component).

## Documentation Structure
- Consolidate documentation in `00-overview`, merging former `02-domain` content for clarity and coherence.
- Rename `action-system.md` to `06-global-action-system.md` to reflect its updated context and purpose.
- Place UI/UX roadmap documentation under `docs/04-ui-ux` and use `01-ui-ux-roadmap.md` as the canonical file.

## UI and Session Management
- Ensure that the Join by Code feature adds the code/session into the campaigns list.
- The Campaigns UI should support both detail-list and card views while remembering the user's last selected view option.

## UX Improvement Guidelines
- When discussing UX work, focus on concrete improvements, clearly state what was improved, and if prioritization is uncertain, ask the user for guidance before proceeding.
- When the user says 'selected' in this context, clarify whether they mean the selected/primary action button (e.g., btn-success Next button) rather than a selected menu/tab item before styling.
- Treat requests about color/layout/UX as site-wide theming unless scoped otherwise, and clarify the target scope before implementing.
- When documenting theming, include explicit button color semantics (what each color means and when to use it).