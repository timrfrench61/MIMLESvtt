# 04 - App Theming (Google Material Design)

## Purpose
This document summarizes Google Material Design theming standards for practical use in this project.

---

## Core Theming Model (Material 3)
Material Design uses a token-based system:

- **Color tokens** (roles, not hardcoded per-component colors)
- **Typography tokens** (display/headline/title/body/label scales)
- **Shape tokens** (corner radii and geometry)
- **Elevation tokens** (depth/surfaces/shadows)
- **State tokens** (hover/focus/pressed/disabled)
- **Motion tokens** (durations/easing for interaction)

Theme decisions should be made at token level first, component level second.

---

## Material Defaults (Colors and Fonts)

### Default color behavior
- Material 3 uses **dynamic color roles** derived from a seed/source color.
- In practical implementation, color defaults are not fixed hardcoded hex values per component; they are role-driven (`primary`, `surface`, `error`, etc.).
- If dynamic color is not used, teams typically define a static light/dark scheme mapped to Material color roles.

### Default typography behavior
- Material baseline type family is typically **Roboto** for app UI text.
- Material symbols/icons commonly use **Material Symbols** font set.
- Typography defaults are applied through tokenized scales (Display/Headline/Title/Body/Label), not arbitrary per-component font sizing.

---

## Color System (Role-Based)
Material 3 color roles typically include:

- `primary`, `on-primary`
- `secondary`, `on-secondary`
- `tertiary`, `on-tertiary`
- `error`, `on-error`
- `background`, `on-background`
- `surface`, `on-surface`
- `surface-variant`, `on-surface-variant`
- `outline`

Guideline:
- Use **semantic roles** (e.g., error for destructive states), not arbitrary colors per button.

---

## Typography System
Material typography is hierarchical and tokenized:

- **Display**: hero/large marketing headers
- **Headline**: major section headers
- **Title**: medium-weight section or card headings
- **Body**: default reading text
- **Label**: buttons, chips, compact UI labels

Guideline:
- Define base text scale once; avoid ad hoc font size per page.

---

## Buttons (Material Semantics)
Common Material button types:

- **Filled button**: primary action
- **Filled tonal button**: medium emphasis action
- **Outlined button**: secondary action
- **Text button**: low emphasis action

Use emphasis hierarchy:
- One primary action per section
- Secondary actions visually subordinate
- Destructive actions use error semantics and explicit labels

---

## Interaction States
Each interactive control should support:

- Rest
- Hover
- Focus-visible
- Pressed
- Disabled

Focus guidance:
- Focus state must be visible and not color-only.

---

## Layout and Spacing
Material favors 8dp-style spacing rhythm and consistent layout density.

Practical rule:
- Use a fixed spacing scale (small/medium/large) and apply consistently.

---

## Accessibility Expectations
- Color contrast compliant for text and controls
- Focus indicators always visible
- Meaning not conveyed by color alone
- Touch/click targets sized for reliable interaction

---

## Strategic Use in This Blazor Project
1. Keep app-wide tokens in global CSS variables.
2. Map tokens to Bootstrap/custom classes consistently.
3. Keep component `.razor.css` for structure and local exceptions only.
4. Document semantic button usage in theme guide and enforce in PR review.

---

## Reference
- Material Design 3 overview: https://m3.material.io/
- Material buttons: https://m3.material.io/components/buttons/overview
