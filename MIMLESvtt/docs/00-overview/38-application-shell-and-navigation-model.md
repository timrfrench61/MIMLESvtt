# Application Shell and Navigation Model

## Purpose

Define the practical application shell layout and navigation model used in the current Blazor app.

This document closes the shell/navigation planning items by mapping the implemented layout, route entry points, and operator workflow expectations.

---

## Implemented Shell Structure

The app shell is implemented with:

- **Left sidebar navigation** (NavMenu)
- **Top bar row** in main content area
- **Main workspace/content region** for page body
- **Optional right-side workspace panel** on Workspace page for control-heavy interaction

Current implementation mapping:

- Shell layout component: `Components/Layout/MainLayout.razor`
- Left navigation component: `Components/Layout/NavMenu.razor`
- Page routing host: `Components/Routes.razor`

---

## Navigation Zones

### 1) Left Sidebar (Primary app navigation)

Current links:

- Home (`/`)
- Workspace (`/workspace`)
- Login (`/login`) / Logout (`/logout`) via auth-state view

### 2) Top Row (Context/utility)

- Top-row utility link placeholder (About) in current layout.

### 3) Main Content Region

- Routed page body (`@Body`) rendered by layout.
- Home hosts launch/game-selector workflow.
- Workspace hosts tabletop and right-slide controls panel.

---

## Route vs Navigation-Link Clarification

- Route is defined on the page component (for example `@page "/workspace"` in `Components/Pages/Workspace.razor`).
- Navigation link is declared in the nav menu (for example `<NavLink href="workspace">` in `Components/Layout/NavMenu.razor`).

This distinction is important when adding or troubleshooting navigation behavior.

---

## Page Hierarchy (Current)

1. **Home/Launch (`/`)**
   - Opens Game Selector interaction flow:
     - My Games
     - Join Existing Game
     - Start New Session (admin-gated)
2. **Workspace (`/workspace`)**
   - Tabletop visual layer
   - Right-slide controls panel for session/surface/piece/play workflows
3. **Authentication**
   - Login (`/login`)
   - Logout (`/logout`)

---

## GM-Heavy Workflow Fit

Navigation is usable for GM/operator-heavy flows by design:

- Launch + selector starts at Home with explicit session actions.
- Workspace centralizes operational controls in the right-side panel while keeping tabletop visible.
- Admin-gated creation is enforced at launch/selector paths.

---

## Acceptance Mapping

### UI-001 (application shell layout)

- Shell structure documented: **Yes**
- Navigation zones defined: **Yes**
- Main workspace supports table/board presentation: **Yes**

### UI-002 (navigation model)

- Primary navigation groups documented: **Yes**
- Page hierarchy documented: **Yes**
- Navigation supports GM-heavy workflows: **Yes**
