# Copilot Instructions

## Project Guidelines
- Use conservative, pass-by-pass implementation for this repo: make only scoped changes requested, preserve existing serializer contracts, keep TableSession/action-system naming from design packet, and avoid unrelated UI, storage, multiplayer, or module changes unless explicitly requested.
- Prefer larger, practical implementation blocks over thin wrapper/micro-polish passes, while keeping conservative scoped changes and maintaining existing code layers intact.
- Shift strategy to larger UI-connected vertical slices delivering visible working capability, avoiding micro-infrastructure passes unless directly required by the UI slice.
- Avoid architecture jargon like 'boundaries'; use direct programming terms like 'code layers' and keep explanations concrete.
- Maintain a clean separation where root domain classes are data models located in a root/domain models directory, not placed under persistence.

## Component Refactoring
- When refactoring a Razor component to use code-behind, move the `@code` block into a `.razor.cs` partial class instead of making unrelated fixes.

## Navigation Guidelines
- When discussing route locations, distinguish between where a page is defined and where its navigation link is declared (e.g., NavMenu link vs page component).

## Documentation Structure
- Consolidate documentation in `00-overview`, merging former `02-domain` content for clarity and coherence.
- Rename `action-system.md` to `06-global-action-system.md` to reflect its updated context and purpose.