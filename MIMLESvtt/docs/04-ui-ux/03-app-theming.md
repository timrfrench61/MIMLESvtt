## 03 - App Theming (Current State)

## Purpose
This document records the current theming stack, the CSS files that affect runtime UI, and the strategy for site-wide vs component-local styling in this Blazor app.

---

## CSS Load Order (Source of Truth)
Defined in `Components/App.razor`:

1. `lib/bootstrap/dist/css/bootstrap.min.css`
2. `app.css`
3. `MIMLESvtt.styles.css` (generated Blazor CSS isolation bundle)

Why this matters:
- Later files win on equal specificity.
- Component `.razor.css` rules are compiled into `MIMLESvtt.styles.css`, so they typically apply after `app.css`.

---

## All CSS Currently Affecting UI

### Framework CSS
- `wwwroot/lib/bootstrap/dist/css/bootstrap.min.css`
  - Base Bootstrap tokens, components, utilities, and default variant colors.

### Global App CSS
- `wwwroot/app.css`
  - Global font/link/button/focus/error defaults.
  - Global list-group active token overrides.
  - Preferred place for site-wide theme decisions.

### Generated Blazor Isolation Bundle
- `wwwroot/MIMLESvtt.styles.css` (referenced by asset pipeline)
  - Generated from all `*.razor.css` files.
  - Contains component-scoped selectors.

### Permanent Blazor Component CSS Files (`*.razor.css`)
- `Components/Layout/MainLayout.razor.css`
  - Shell/page frame, sidebar gradient, top-row behavior, responsive layout.
- `Components/Layout/NavMenu.razor.css`
  - Left nav visuals, nav toggler icon, active/hover link styling.
- `Components/Layout/ReconnectModal.razor.css`
  - Reconnect modal animation, backdrop, reconnect button visuals.
- `Components/Pages/Workspace.razor.css`
  - Workspace canvas/board visuals, overlays, slide panel styling, workspace header controls, workspace-local active/selected overrides.

---

## Current Theme Decisions (As Implemented)

### Global (from `app.css`)
- Font stack: Helvetica Neue/Helvetica/Arial/sans-serif.
- Link/btn-link color: `#006bb7`.
- Primary button (`.btn-primary`):
  - bg `#1b6ec2`, border `#1861ac`, text white.
- Focus ring:
  - `0 0 0 0.1rem white, 0 0 0 0.25rem #258cfb`.
- Validation/error:
  - invalid outline `#e50000`, message `#e50000`.
- List-group active tokens:
  - bg silver, border silver, text white.

### Workspace-local (from `Workspace.razor.css`)
- Workspace header hamburger:
  - silver background, white icon.
- Workspace slide panel active list-group item:
  - forced silver background/border, white text (`!important`).
- Workspace `btn-success` override:
  - silver background/border, white text (`!important`).

---

## Button Color Semantics (Meaning and Intended Use)

This table defines what button colors mean in UX terms. Use these meanings consistently across pages.

| Button Style | Typical Color | Meaning | Use For | Avoid For |
|---|---|---|---|---|
| `.btn-primary` | Blue | Primary action in current context | Main action the user should take next (open, continue, confirm current task) | Destructive actions, passive navigation-only links |
| `.btn-success` | Green (currently silver in Workspace override) | Positive completion/advance action | Confirmed forward progression (e.g., next-turn/play-turn style actions) | Routine secondary actions |
| `.btn-danger` / `.btn-outline-danger` | Red | Destructive or risky action | Delete/remove/reset with impact | Safe actions, mode toggles |
| `.btn-outline-secondary` | Gray/Silver | Secondary/supporting action | Cancel, back, refresh, optional utilities, non-primary navigation | Primary CTA |
| `.btn-warning` / `.btn-outline-warning` | Yellow/Amber | Cautionary or attention-needed action | Actions that need user awareness but are not fully destructive | Normal primary flows |
| `.btn-info` / `.btn-outline-info` | Cyan/Blue-teal | Informational/system utility action | Info/diagnostic/system-oriented actions where emphasis is lower than primary | Main completion actions |
| Custom accent (e.g., purple if introduced) | Purple | Domain-specific special mode/action | Reserved, explicitly documented special workflow only | General CRUD and common navigation |

Current implementation note:
- In `Workspace.razor.css`, `.btn-success` is intentionally overridden to silver for the current workspace theming direction.
- If silver should be global for "success" semantics, move that override to `wwwroot/app.css` as a site-wide decision.

Selection/active state note:
- Selected list-group state is currently themed silver (`.list-group-item.active` in workspace context and list-group token overrides in `app.css`).
- Keep selected-state color semantics distinct from destructive-state red.

---

## Text Styles (Current)

This section documents current text styling as implemented now.

### Base Text (Normal)
- Body/default text uses font stack:
  - `'Helvetica Neue', Helvetica, Arial, sans-serif`.[^S2]
- Default paragraph/body color currently follows Bootstrap defaults (no global override in `app.css`).[^S1][^S2]

### Links and Link-like Controls
- `a` and `.btn-link` are set to `#006bb7` globally.[^S2]
- Top-row nav/link behavior is further scoped in `MainLayout.razor.css` (spacing/underline hover behavior).[^S4]

### Headings
- App pages commonly use semantic heading tags (`h1`, `h3`, `h5`, `h6`) in Razor markup.[^S6]
- No global heading size/color override is currently defined in `app.css`; Bootstrap heading scale is in effect.[^S1][^S2]
- Existing accessibility helper: `h1:focus { outline: none; }` in `app.css`.[^S2]

### Form/Validation Text
- Validation message color: `#e50000` (`.validation-message`).[^S2]
- Invalid field outline: `#e50000`; valid modified outline: `#26b050`.[^S2]
- Placeholder alignment rules are defined for floating labels in `app.css`.[^S2]

### Status/Feedback Text Context
- Error boundary text appears as white on dark red background in `.blazor-error-boundary`.[^S2]
- Reconnect modal text styles are scoped in `ReconnectModal.razor.css`.[^S5]

---

## Normalized Button Rules (Current Standard)

Use these rules in all new UI work unless explicitly scoped otherwise.

1. **One primary action per action row**
   - Exactly one prominent CTA (`.btn-primary` or approved equivalent) in a row/group.[^S7][^S8]

2. **Secondary actions are visually subordinate**
   - Use `.btn-outline-secondary` for cancel/back/refresh/navigation support actions.[^S7]

3. **Destructive actions are always danger variants**
   - Use `.btn-danger` or `.btn-outline-danger` for delete/reset/remove actions only.[^S7]

4. **Success variant is reserved for positive advance/confirm semantics**
   - Current Workspace implementation maps `.btn-success` to silver locally; treat that as a scoped exception until global decision is made.[^S3]

5. **Do not theme single buttons ad hoc**
   - Theme by variant class globally (`app.css`) or by documented component scope (`*.razor.css`), not by one-off inline/button-specific hacks.[^S2][^S3]

6. **Selected-state color semantics must remain distinct from destructive state**
   - Selected/active is silver; destructive remains red.[^S2][^S3]

7. **Accessibility requirement**
   - Color alone must not be the only signal; keep explicit text labels (e.g., “Delete”, “Play”, “Cancel”).[^S6]

---

## Strategic Theming Guidance

### 1) Site-wide theming
Use `wwwroot/app.css` and Bootstrap variables/classes for global color semantics and component variants.

### 2) Component-local styling
Use `*.razor.css` only for:
- layout specific to that component,
- visual behavior tightly coupled to that component,
- temporary exceptions where global theme is not yet established.

### 3) Avoid fragmented variant overrides
Do not scatter variant overrides (e.g., `.btn-success`) across many component files unless intentionally local.
Prefer a single global definition in `app.css` for predictable app-wide semantics.

### 4) If an active/selected color appears incorrect
Debug in this order:
1. Confirm element/class (`.btn-success`, `.list-group-item.active`, etc.).
2. Check whether style comes from Bootstrap class defaults.
3. Check global override in `app.css`.
4. Check component-level override in matching `.razor.css`.
5. Remember CSS isolation bundle (`MIMLESvtt.styles.css`) loads after `app.css`.

---

## Suggested Next Consolidation
- Move all variant-color decisions (primary/success/danger/accent/selected) to `app.css`.
- Keep `Workspace.razor.css` focused on workspace structure/layout.
- Keep a single button semantics table in UX docs (what each variant means by action intent).

---

## Footnotes / Decision Sources

[^S1]: `Components/App.razor` - CSS load order (`bootstrap.min.css` -> `app.css` -> `MIMLESvtt.styles.css`).
[^S2]: `wwwroot/app.css` - global typography, link color, button/focus, validation, and list-group token overrides.
[^S3]: `Components/Pages/Workspace.razor.css` - workspace-local active/selected and `.btn-success` scoped overrides.
[^S4]: `Components/Layout/MainLayout.razor.css` - shell/top-row link behavior and layout styling context.
[^S5]: `Components/Layout/ReconnectModal.razor.css` - reconnect modal text/button styling.
[^S6]: `Components/Pages/*.razor` (notably `Workspace.razor`, `Home.razor`, content pages) - live heading and button usage patterns.
[^S7]: `docs/04-ui-ux/01-ui-ux-roadmap.md` - UI checklist/action hierarchy conventions and UX consistency guidance.
[^S8]: **CTA** = **Call To Action**: the main button/control asking the user to take the next important step (e.g., Play, Save, Create, Next Turn). Use plain wording "primary action" when audience may not use UX jargon. External references: Bootstrap button semantics (`https://getbootstrap.com/docs/5.3/components/buttons/`) and Google Material Design action hierarchy (`https://m3.material.io/components/buttons/overview`).
