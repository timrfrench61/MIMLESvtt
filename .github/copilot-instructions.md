# Copilot Instructions

## Project Guidelines
- Use conservative, pass-by-pass implementation for this repo: make only scoped changes requested, preserve existing serializer contracts, keep TableSession/action-system naming from design packet, and avoid unrelated UI, storage, multiplayer, or module changes unless explicitly requested.
- Prefer larger, practical implementation blocks over thin wrapper/micro-polish passes, while keeping conservative scoped changes and maintaining existing architecture boundaries intact.
- Shift strategy to larger UI-connected vertical slices delivering visible working capability, avoiding micro-infrastructure passes unless directly required by the UI slice.