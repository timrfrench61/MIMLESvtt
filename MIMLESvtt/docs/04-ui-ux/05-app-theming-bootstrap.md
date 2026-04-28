# 05 - App Theming (Bootstrap)

## Purpose
This document summarizes Bootstrap theming standards and how to apply them consistently in this project.

---

## Bootstrap Theming Model
Bootstrap supports theme customization through:

1. **CSS variables** (runtime overrides)
2. **Sass variables/maps** (build-time customization)
3. **Utility classes** (layout/spacing/visual consistency)
4. **Component variants** (`.btn-primary`, `.alert-danger`, etc.)

For this project’s current setup, runtime CSS variable and class overrides in `wwwroot/app.css` are the primary theming path.

---

## Bootstrap Defaults (Colors and Fonts)

### Bootstrap default semantic colors
| Semantic color | Hex | Swatch |
|---|---|---|
| `primary` | `#0d6efd` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#0d6efd;"></span> |
| `secondary` | `#6c757d` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#6c757d;"></span> |
| `success` | `#198754` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#198754;"></span> |
| `danger` | `#dc3545` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#dc3545;"></span> |
| `warning` | `#ffc107` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#ffc107;"></span> |
| `info` | `#0dcaf0` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#0dcaf0;"></span> |
| `light` | `#f8f9fa` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#f8f9fa;"></span> |
| `dark` | `#212529` | <span style="display:inline-block;width:1.5rem;height:1rem;border:1px solid #666;background:#212529;"></span> |

These are Bootstrap 5 default semantic colors before project-level overrides.

### Bootstrap default font stack
- Base sans-serif stack is the Bootstrap variable `--bs-font-sans-serif`, typically:
  - `system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", "Liberation Sans", sans-serif`

In this project, `wwwroot/app.css` currently overrides runtime base text family to:
- `'Helvetica Neue', Helvetica, Arial, sans-serif`

---

## Global Theme Strategy
Order in this project:

1. `bootstrap.min.css`
2. `app.css`
3. `MIMLESvtt.styles.css` (component CSS isolation output)

Guideline:
- Put app-wide semantic theming in `app.css`.
- Use component `.razor.css` for local layout or intentional exceptions.

---

## Color Semantics with Bootstrap Variants
Recommended semantic mapping:

- `.btn-primary`: primary action in context
- `.btn-outline-secondary`: secondary/support action
- `.btn-danger` / `.btn-outline-danger`: destructive action
- `.btn-success`: positive/advance action (if globally retained)
- `.btn-warning`: cautionary action
- `.btn-info`: informational/system utility

Guideline:
- Keep one primary action per action row.
- Avoid using color as the only meaning signal.

---

## Component State Theming
Common state targets:

- `.list-group-item.active`
- `.nav-link.active`
- `.table-primary` (selected row patterns)
- `.alert-*` state semantics

When overriding active/selected visuals:
- Prefer Bootstrap variables at container scope where available.
- Use direct selector overrides only when variable approach is insufficient.

---

## Typography and Spacing with Bootstrap
Use Bootstrap scales consistently:

- Typography classes (where needed): `h1`-`h6`, `.text-muted`, etc.
- Spacing utilities: `mb-2`, `mb-3`, `p-2`, `gap-2`, etc.

Guideline:
- Avoid custom pixel spacing unless no utility class fits.

---

## Accessibility with Bootstrap Components
- Ensure contrast for custom overrides
- Preserve visible focus states
- Keep explicit labels for buttons and controls
- Do not encode status only via color

---

## Strategic Use in This Blazor Project
1. Define app color semantics in `app.css`.
2. Keep button meaning documented and stable.
3. Avoid fragmenting variant overrides across many `.razor.css` files.
4. Reserve component-scoped overrides for true local needs.
5. Re-check specificity/load-order whenever expected color does not apply.

---

## Reference
- Bootstrap theming: https://getbootstrap.com/docs/5.3/customize/overview/
- Bootstrap CSS variables: https://getbootstrap.com/docs/5.3/customize/css-variables/
- Bootstrap buttons: https://getbootstrap.com/docs/5.3/components/buttons/
