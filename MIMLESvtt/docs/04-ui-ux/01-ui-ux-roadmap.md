# 01 - UI/UX Roadmap

## Purpose
This roadmap defines how UI/UX design and analysis will be developed across the full application.

It provides:
- a practical delivery sequence,
- a consistent terminology baseline,
- a merge plan for existing `00-overview` UI docs,
- a working outline that can be refined into implementation-ready UI slices.

---

## Scope
In scope:
- all user-facing screens and flows in the Blazor app,
- navigation and information architecture,
- page-level interaction patterns,
- state handling in loading/empty/error/success scenarios,
- accessibility and usability validation.

Out of scope for this roadmap version:
- deep visual branding/theming systems,
- unrelated domain model refactors,
- non-UI infrastructure changes unless required by a UI slice.

---

## Product Terminology (UI copy baseline)
Use these terms consistently in UI and docs:
- **Game**: user-facing item in selector/list views.
- **Session**: runtime/table state container.
- **Campaign**: single campaign attached to a session (`VttSession.Campaign`).
- **Scenario**: playable scenario state/content within campaign flow.
- **GameBox**: content package identity.

Copy guidance:
- Prefer user-facing wording in UI labels.
- Use domain terms where precision is required in admin/advanced flows.

---

## Delivery Phases

## Roadmap Status Tracker
Use this table as the live execution view for UI/UX planning and delivery.

| Phase | Focus | Status | Owner | Target Window | Notes |
|---|---|---|---|---|---|
| Phase 0 | Foundation and doc merge setup | In Progress | Product Owner + UX Lead | Sprint 0 (current) | Canonical file moved to `docs/04-ui-ux/01-ui-ux-roadmap.md`. |
| Phase 1 | Current-state UX audit | In Progress | UX Lead + Blazor UI Lead | Sprint 1 | Workspace Design/Play split matrix and first implementation pass completed. |
| Phase 2 | Information architecture and navigation | Done | UX Lead + App Shell Owner | Sprint 1-2 | Route/link matrix finalized, placement decisions recorded, and navigation test checklist added. |
| Phase 3 | Design system baseline | In Progress | Blazor UI Lead | Sprint 2 | Baseline drafted; Home action hierarchy and Workspace advanced-control tiering slices implemented. |
| Phase 4 | Vertical slice UX specs | In Progress | Feature Owners (Home / Workspace / Content / Import) | Sprint 2-4 | Workspace split slice Step 1-5 implemented. |
| Phase 5 | Validation and iteration | Not Started | Product Owner + QA Lead | Sprint 3+ (rolling) | KPI tracking and per-slice UX deltas. |

Status legend:
- **Not Started**
- **In Progress**
- **Blocked**
- **Done**

---

## Readiness Checkpoint: Workspace Top-Level Usage Split (Design vs Play)
Status: **Ready to start now**

Purpose:
- define top-level usage split between **Design** workflows and **Play** workflows in Workspace,
- prevent control overlap and reduce mode confusion.

Entry criteria (met):
- canonical roadmap file established,
- Workspace baseline and UX focus points documented,
- status tracker in place for phase execution.

Planned output:
- one-page split matrix for Workspace controls:
  - **Design-only**,
  - **Play-only**,
  - **Shared (mode-aware)**.

Next action:
- execute this as the first task under Phase 1 and feed results into Phase 2 navigation and Phase 4 Workspace slice.

---

## Workspace Usage Split Matrix (Design vs Play)
This is the initial top-level usage split for Workspace controls.

### Mode Definitions
- **Design mode**: session setup, layout authoring, scenario preparation, and data-entry operations.
- **Play mode**: turn execution, active piece interaction, and in-session adjudication.

### Control Classification Matrix
| Workspace Area / Control | Mode Classification | UX Handling Rule |
|---|---|---|
| Session title and session metadata | Design-only | Visible in Design; hidden or read-only summary in Play. |
| Surfaces Entry/Edit (add/edit surfaces) | Design-only | Keep under Design workspace sections only. |
| Pieces Entry/Edit (add/move/rotate/state from form panels) | Design-only | Full forms in Design; remove authoring forms from Play. |
| Marker Entry/Edit admin controls | Design-only | Keep as Design tooling; avoid in Play quick HUD unless needed by rules. |
| Developer settings and pre-alpha toggles | Design-only | Move to advanced Design foldout. |
| Turn / Phase controls (advance, previous, phase set) | Play-only | Primary in Play; read-only summary in Design. |
| Initiative / roster ordering actions | Play-only | Center in Play command area. |
| Board selection and movement interactions | Shared (mode-aware) | Enabled in both; behavior profile changes by mode. |
| Board visibility toggles and overlays | Shared (mode-aware) | Visible in both; simplify defaults in Play. |
| Stamp presets and queue tooling | Design-only | Authoring workflow; hide in Play unless explicitly enabled. |
| Undo / Redo | Shared (mode-aware) | Available in both; label scope by mode context. |
| Save/Open/Create session actions | Design-only | Keep in Design command group. |
| Play log / action feed | Play-only | Always visible in Play; optional collapsed section in Design. |

### Immediate UI Layout Direction
1. Keep the existing session mode switch (Edit/Play) as the primary mode selector.
2. In **Design** mode, show setup/editor tabs and creation tooling.
3. In **Play** mode, collapse authoring tabs and prioritize turn, board, and action-log controls.
4. For shared controls, keep one location but alter defaults and visibility by mode.

### Open Decisions to Resolve in Phase 1
- Whether marker quick actions require a minimal Play shortcut bar.
- Whether limited piece state edits (HP/status) should be Play-visible for GM-only contexts.
- Whether stamp mode should ever be exposed in Play for specific game profiles.

### Deliverable Linkage
- Feeds **Phase 2** IA/navigation updates for Workspace tabs.
- Feeds **Phase 4** Workspace vertical slice acceptance criteria.

---

## Phase 1 Workspace Audit (Current vs Target)
This section translates the split matrix into implementation-ready audit notes.

### Current vs Target Control Mapping
| Control Group | Current Placement (observed) | Target Placement | Mode Rule |
|---|---|---|---|
| Session Summary + metadata | Session Summary tab in right panel | Keep in Session Summary | Design: full; Play: summary-only |
| Workspace mode switch (Edit/Play) | Session Summary section | Keep high in Session Summary header area | Always visible |
| Surfaces Entry/Edit forms | Surfaces Entry/Edit tab | Keep in Surfaces Entry/Edit | Design-only |
| Pieces Entry/Edit forms | Pieces Entry/Edit tab | Keep in Pieces Entry/Edit | Design-only |
| Marker Entry/Edit forms | Marker Entry/Edit tab | Keep in Marker Entry/Edit | Design-only |
| Developer settings toggles | Session Summary section | Move into advanced Design foldout | Design-only |
| Turn/phase controls | Session Summary section | Promote into Play-first command strip | Play-only primary |
| Initiative/roster controls | Session Summary section | Keep near turn/phase in Play cluster | Play-only primary |
| Board interaction panel | Surfaces/board-heavy section | Keep central board region | Shared (mode-aware) |
| Board visibility filters | Surfaces tab sections | Keep as board options with Play-safe defaults | Shared (mode-aware) |
| Stamp presets + queue tooling | Surfaces tab advanced sections | Keep under Design board tools | Design-only |
| Undo/Redo app controls | Top workspace controls panel | Keep top app controls | Shared (mode-aware) |
| Save/Open/Create actions | Top workspace controls panel | Keep top app controls | Design-only |
| Play log/action feed | Below workspace panel content | Promote in Play and collapse in Design | Play-only primary |

### Gap Notes (Current Friction)
- Play-critical controls are mixed inside Session Summary with setup tooling.
- Design-heavy sections (stamp/queue/advanced board authoring) are too visible during Play workflow.
- Shared controls lack explicit mode-tuned defaults.

### Target Interaction Rules (Phase 1 Baseline)
1. **Play mode**
   - shows turn/phase, initiative/roster, board interactions, and play log prominently,
   - hides setup/editor data-entry tabs by default.
2. **Design mode**
   - exposes session setup, Surfaces/Pieces/Marker entry-edit tabs, and authoring utilities,
   - keeps play execution controls available as read-only summary where needed.
3. **Shared controls**
   - remain in fixed locations,
   - switch defaults and helper text by mode.

### Acceptance Criteria for First Workspace Split Slice
- In Play mode, users do not see add/edit authoring forms by default.
- In Design mode, all authoring forms remain available and unchanged in capability.
- Mode switch remains visible and updates UI sections without page navigation.
- Board interactions remain functional in both modes with mode-appropriate defaults.
- No loss of current save/open/session workflow capabilities.

### Implementation Hand-off Notes
- This audit should be used to drive a Workspace UI task list in Phase 4.
- Keep changes conservative and focused on visibility/layout behavior first, then control relocation.

---

## Workspace Split Implementation Checklist (Ordered)
Use this as the immediate execution plan for the first Workspace Design/Play split slice.

### Step 1 - Add Mode-Aware Visibility Flags
- Introduce clear computed flags for:
  - Design-only sections,
  - Play-only sections,
  - Shared sections.
- Keep this logic centralized in Workspace page code-behind.

Done when:
- visibility is controlled from one place,
- no behavior change yet beyond wiring.

Status: **Done**

### Step 2 - Hide Design Authoring Forms in Play Mode
- Hide Surfaces/Pieces/Marker entry-edit forms when mode is Play.
- Keep all forms unchanged and fully functional in Design mode.

Done when:
- Play mode removes authoring form clutter,
- Design mode remains feature-complete.

Status: **Done**

### Step 3 - Promote Play Command Cluster
- Group turn/phase and initiative controls into a Play-first section.
- Keep this section visible and prominent in Play mode.

Done when:
- play execution controls are easy to find and use,
- no regression in turn actions.

Status: **Done**

### Step 4 - Apply Shared Control Defaults by Mode
- Keep board controls accessible in both modes.
- Apply safer/simpler defaults for Play.
- Keep full tuning and advanced options in Design.

Done when:
- board remains usable in both modes,
- default behavior matches mode intent.

Status: **Done**

### Step 5 - Tune Session Summary for Mode Context
- Keep session metadata always available.
- In Play mode, prefer compact summary and key runtime context.
- In Design mode, keep broader setup context.

Done when:
- Session Summary is readable and mode-appropriate.

Status: **Done**

### Step 6 - Regression Pass + UX Validation Notes
- Validate save/open/session actions are unaffected.
- Validate mode switch updates UI without navigation.
- Capture before/after notes for this slice.

Done when:
- no functional regressions,
- acceptance criteria from Phase 1 section are met.

Status: **Done**

### Regression + UX Validation Notes (Workspace Split Slice)
- Build status: successful after Session Summary mode tuning.
- Tests status: `MIMLESvtt.Tests` project run completed with all tests passing.
- Functional checks completed in implementation:
  - mode switch updates visibility without route navigation,
  - Play mode hides design authoring tabs/forms,
  - Design mode retains full authoring capability,
  - shared board controls remain available with mode-specific defaults,
  - save/open/session control wiring unchanged.

### Suggested Execution Order by File
1. `Components/Pages/Workspace.razor.cs` (visibility flags / mode logic)
2. `Components/Pages/Workspace.razor` (section visibility and grouping)
3. `Components/Pages/WorkspaceSessionSettingsPanel.razor` (summary tuning)
4. Relevant tests (workspace/page behavior coverage)

---

### Phase 0 - Foundation and Doc Merge Setup
Goals:
- establish one source of truth for UI/UX planning,
- capture merge candidates from existing `00-overview` docs,
- define ownership and update cadence.

Outputs:
- this roadmap file,
- merge backlog (see below),
- section owners per major area.

### Phase 1 - Current-State UX Audit
Goals:
- map all current screens and primary user tasks,
- identify high-friction interactions,
- classify issues by severity and user impact.

Focus screens:
- Home / Game selector,
- Workspace and right-side slide controls,
- Content management pages,
- Import workflow shell.

Outputs:
- annotated flow notes,
- prioritized UX issue list (P0/P1/P2),
- baseline screenshots for before/after comparison.

### Phase 2 - Information Architecture and Navigation
Goals:
- confirm top-level navigation model,
- clarify route definitions vs navigation-link placement,
- reduce ambiguity in where tasks begin and end.

Outputs:
- IA map,
- navigation matrix (page, route, nav entry point),
- naming and placement rules.

### Phase 2 Working Draft - IA Map (Current)
Top-level structure:
- **Home/Game Launcher**: `/`
- **Workspace**: `/workspace`
- **Tactical Setup**: `/workspace/tactical-setup`
- **Content**: `/content`
  - `/content/import`
  - `/content/monsters`
  - `/content/treasure`
  - `/content/equipment`
  - `/content/magic-items`
- **Auth**:
  - `/login`
  - `/logout`

### Phase 2 Navigation Matrix (Route Definition vs Link Declaration)
| Page Purpose | Route (where defined) | Primary Navigation Link Declaration | Secondary Entry Points |
|---|---|---|---|
| Home / Game Launcher | `Components/Pages/Home.razor` (`@page "/"`) | `Components/Layout/NavMenu.razor` (home NavLink) | Login page "Go to Home", Workspace "Back to Selector" links |
| Workspace | `Components/Pages/Workspace.razor` (`@page "/workspace"`) | `Components/Layout/NavMenu.razor` (`href="workspace"`) | Home flow (`Navigation.NavigateTo("/workspace")`), Tactical Setup code-behind |
| Tactical Scenario Setup | `Components/Pages/TacticalScenarioSetup.razor` (`@page "/workspace/tactical-setup"`) | `Components/Layout/NavMenu.razor` (`href="workspace/tactical-setup"`) | In-page continue/back navigation to workspace/home |
| Content Home | `Components/Pages/ContentHome.razor` (`@page "/content"`) | `Components/Layout/NavMenu.razor` (`href="content"`) | Home code-behind (`NavigateTo("/content")`) |
| Content Import | `Components/Pages/ContentImport.razor` (`@page "/content/import"`) | `Components/Layout/NavMenu.razor` (`href="content/import"`) | Content Home links |
| Content Monsters | `Components/Pages/ContentMonsters.razor` (`@page "/content/monsters"`) | no direct top-level nav item | Content Home link list |
| Content Treasure | `Components/Pages/ContentTreasure.razor` (`@page "/content/treasure"`) | no direct top-level nav item | Content Home link list |
| Content Equipment | `Components/Pages/ContentEquipment.razor` (`@page "/content/equipment"`) | no direct top-level nav item | Content Home link list |
| Content Magic Items | `Components/Pages/ContentMagicItems.razor` (`@page "/content/magic-items"`) | no direct top-level nav item | Content Home link list |
| Login | `Components/Pages/Login.razor` (`@page "/login"`) | `Components/Layout/NavMenu.razor` (`href="login"`) | Logout page sign-in link |
| Logout | `Components/Pages/Logout.razor` (`@page "/logout"`) | `Components/Layout/NavMenu.razor` (`href="logout"`) | Auth/session expiration flow |

### Phase 2 Placement Rules (Draft)
1. Keep route definition and nav-link declaration explicitly documented as separate concerns.
2. Keep top-level navigation focused on app sections (Home, Workspace, Tactical Setup, Content, Auth).
3. Keep content subtype pages (`monsters`, `treasure`, `equipment`, `magic-items`) accessible from Content Home rather than top-level nav.
4. For action-driven transitions (`Home` -> `Workspace`), keep code-behind navigation documented in this matrix.

### Phase 2 Next Actions
- Confirm whether Tactical Setup remains top-level nav or becomes Workspace sub-navigation.
- Decide if Content Import should remain top-level Content sub-link in the left nav.
- Add an explicit navigation test checklist for route/link consistency.

### Phase 2 Decision Outcomes (Executed)
1. **Tactical Setup placement**
   - Decision: keep Tactical Setup as a top-level left-nav entry for this release cycle.
   - Rationale: it is currently an active, distinct flow and benefits from direct access during iteration.
   - Follow-up trigger: if setup usage drops or merges into Workspace workflow, move it under Workspace sub-navigation.

2. **Content Import placement**
   - Decision: keep Content Import as a visible Content sub-link in left nav.
   - Rationale: import is a frequent onboarding and bulk-entry workflow and should stay one-click discoverable.
   - Follow-up trigger: if content IA is reworked into sub-navigation tabs, re-evaluate nav prominence.

### Phase 2 Navigation Test Checklist (Route/Link Consistency)
Run this checklist when changing routes, nav links, or code-behind navigation.

- [ ] Every `@page` route has an intentional entry point (top nav, in-page link, or code-behind transition).
- [ ] Every top-level nav link resolves to an existing `@page` route.
- [ ] Route definition location and nav-link declaration location are both documented in the matrix.
- [ ] `Navigation.NavigateTo(...)` transitions are listed under secondary entry points.
- [ ] Content subtype pages remain reachable from Content Home links.
- [ ] Auth links (`/login`, `/logout`) remain reachable from nav and flow transitions.
- [ ] Manual smoke run verifies: `/`, `/workspace`, `/workspace/tactical-setup`, `/content`, `/content/import`, auth pages.

### Phase 2 Completion Notes
- IA map and route/link matrix are now established as the baseline for future nav changes.
- Placement decisions for Tactical Setup and Content Import are recorded.
- Route/link consistency checklist is in place for regression prevention.

### Phase 3 - Design System Baseline
Goals:
- standardize reusable interaction patterns for Blazor pages/components,
- define states for data and actions.

Outputs:
- baseline guidance for:
  - layout density and spacing,
  - button hierarchy and destructive actions,
  - forms and validation behavior,
  - tables/cards/detail-list presentation,
  - alerts and feedback messages,
  - loading/empty/error/success states.

### Phase 3 Baseline Standards (Draft v1)

#### 1) Layout and Spacing
- Use existing Bootstrap spacing scale consistently (`mb-2`, `mb-3`, `p-2`, `gap-2`).
- Prefer section wrappers already used in app (`border rounded bg-light`) for grouped controls.
- Keep primary task controls above supporting/advanced controls.

#### 2) Button Hierarchy
- Primary action in a region: `btn-primary` or `btn-success` (single dominant action).
- Secondary actions: `btn-outline-secondary`.
- Destructive actions: `btn-outline-danger` plus confirmation for irreversible operations.
- Avoid multiple competing primary buttons in the same row.

#### 3) Form and Validation Behavior
- Keep labels/placeholders explicit and task-oriented.
- Validation and operation feedback should flow through existing status alert patterns.
- For mode-restricted actions (Design vs Play), show guidance text rather than silently disabling context.

#### 4) Tables, Cards, and Lists
- Use table view for dense operational data (turn order, pieces, surfaces).
- Use cards for selector/discovery flows (Games list card mode).
- Highlight active rows with existing `table-primary` pattern.
- Keep column labels concrete and avoid internal jargon in user-facing tables.

#### 5) Alerts and Feedback
- Keep current semantic classes:
  - info: `alert-info`
  - warning: `alert-warning`
  - error: mapped from workspace error severity
  - success: mapped from workspace success severity
- Prefer concise action-result messages: “what changed” + optional object id/name.

#### 6) State Patterns
- Required states for each interactive section:
  - Loading (where async loads exist),
  - Empty (`No ... available`),
  - Error (clear next step),
  - Success (operation confirmation).
- Keep empty-state copy consistent with current terminology (“games”, “session”, “campaign”).

#### 7) Accessibility Baseline
- Preserve visible text labels for form controls and toggles.
- Keep keyboard reachability for interactive board and panel controls.
- Keep contrast-safe status badges/alerts and avoid status conveyed by color alone.

### Phase 3 Component Mapping (Current App)
- **Navigation shell**: `Components/Layout/NavMenu.razor`, `MainLayout.razor`
- **Home/Game selector patterns**: `Components/Pages/Home.razor`
- **Workspace panel patterns**: `Components/Pages/Workspace.razor`
- **Session summary component**: `Components/Pages/WorkspaceSessionSettingsPanel.razor`
- **Settings/action strip component**: `Components/Pages/WorkspaceApplicationSettingsPanel.razor`

### Phase 3 Next Actions
- Apply baseline to Home/Game selector action rows (primary/secondary/destructive consistency).
- Apply baseline to Workspace advanced sections (foldout/tiering for dense controls).
- Add a lightweight UI review checklist to PR workflow for new/changed pages.

### Phase 3 Slice Progress
- **Slice A (Done):** Home/Game selector action hierarchy normalized.
  - Open action promoted as primary in game rows.
  - Delete kept destructive.
  - Refresh kept secondary.
- **Slice B (Done):** Workspace Surfaces dense authoring controls moved under an Edit-mode advanced foldout.
  - Play mode now shows concise guidance instead of dense authoring controls.
- **Slice C (Current):** Add lightweight UI review checklist for PR workflow.

### UI Review Checklist for PRs (Lightweight)
Use this checklist for any PR that adds or changes UI in Blazor pages/components.

- [ ] **Action hierarchy:** each section has one clear primary action; secondary/destructive actions are visually subordinate.
- [ ] **Mode awareness:** Design vs Play visibility rules are respected where applicable.
- [ ] **State coverage:** loading/empty/error/success states are handled for the changed section.
- [ ] **Terminology consistency:** copy uses current baseline terms (Game, Session, Campaign, Scenario, GameBox).
- [ ] **Feedback quality:** status/alert messages clearly describe what changed.
- [ ] **Navigation integrity:** route and entry-point links still match Phase 2 matrix.
- [ ] **Accessibility basics:** controls are keyboard reachable, labelled, and not color-only for status.
- [ ] **Regression check:** changed page builds cleanly and relevant tests are run.

Definition of done for this checklist process:
- checklist is referenced in PR description,
- any unchecked item includes a short follow-up note.

### Sample Checklist Audit - Workspace (Current Slice)
Scope audited:
- `Components/Pages/Workspace.razor`
- `Components/Pages/Workspace.razor.cs`
- `Components/Pages/WorkspaceSessionSettingsPanel.razor`

Checklist results:
- [x] **Action hierarchy:** Play commands promoted; destructive/admin actions remain secondary or danger-styled.
- [x] **Mode awareness:** Design-only forms and advanced authoring controls are hidden in Play mode.
- [x] **State coverage:** guidance/empty/info states present for mode-restricted and empty data scenarios.
- [x] **Terminology consistency:** Session/Campaign/Game wording aligned with current baseline.
- [x] **Feedback quality:** operation feedback remains routed through workspace status messaging.
- [x] **Navigation integrity:** mode changes occur in-place without route changes.
- [x] **Accessibility basics:** labels preserved; keyboard board focus support retained.
- [x] **Regression check:** build + tests passed in latest validation cycle.

Follow-up notes:
- Consider adding explicit loading placeholders for future async-heavy workspace sections.
- Consider extracting advanced authoring foldout into a dedicated component if complexity grows.

### Sample Checklist Audit - Home/Game Selector (Current Slice)
Scope audited:
- `Components/Pages/Home.razor`
- `Components/Pages/Home.razor.cs`

Checklist results:
- [x] **Action hierarchy:** "Open Campaign" is primary; refresh is secondary; delete is destructive.
- [x] **Mode awareness:** not applicable for Home page mode split; no conflicting Design/Play controls present.
- [x] **State coverage:** empty-state and status alert messaging present for known sessions and join/create workflows.
- [x] **Terminology consistency:** user-facing labels use "Games" and align with current baseline terms.
- [x] **Feedback quality:** join/create/delete operations surface explicit status messages.
- [x] **Navigation integrity:** Home flows correctly route to Workspace and Content entry points.
- [x] **Accessibility basics:** button labels and field placeholders are explicit for launcher tasks.
- [x] **Regression check:** build and project test run passed after action-hierarchy updates.

Follow-up notes:
- Consider adding grouped section subtitles for "Open", "Join", and "Create" flows to improve first-scan clarity.
- Consider adding helper text for GameBox selection when creating campaigns.

### Phase 4 - Vertical Slice UX Specs (Implementation-Ready)
Goals:
- convert audits and patterns into page-level specs,
- deliver in user-visible slices.

Initial slice order:
1. Home / Game selector UX polish,
2. Workspace panel and tab usability,
3. Content management flow cohesion,
4. Import flow clarity and feedback.

Outputs per slice:
- user goals,
- key tasks,
- interaction rules,
- required states,
- acceptance criteria.

### Phase 5 - Validation and Iteration
Goals:
- validate improvements with measurable outcomes,
- iterate by release cycle.

Core metrics:
- task completion rate,
- time to complete common tasks,
- user error frequency,
- clicks/steps to complete critical flows.

Outputs:
- per-slice validation notes,
- UX delta log (before/after),
- backlog updates.

---

## Initial Screen Baselines

### Home / Game Selector (baseline)
User goals:
- discover available games quickly,
- join/open/delete with confidence,
- create new game/session with clear constraints.

Current UX focus points:
- list vs card view parity,
- destructive action confirmation clarity,
- empty-state messaging consistency,
- terminology consistency (Game/Session/Campaign).

Next actions:
- finalize action grouping hierarchy,
- ensure game-level metadata is scannable in both views,
- verify all destructive actions communicate final impact.

### Workspace (right slide panel and session summary)
User goals:
- understand current session/campaign status at a glance,
- perform setup/edit actions without context switching,
- avoid confusion between board controls and data entry controls.

Current UX focus points:
- tab naming clarity and consistency,
- Session Summary readability,
- grouping of setup vs edit actions,
- reduction of duplicated controls across tabs.

Next actions:
- evaluate progressive disclosure for advanced controls,
- tighten section labels for board operations vs metadata operations,
- define compact vs expanded panel patterns.

---

## Merge Backlog from `00-overview`
These docs should be merged into this roadmap as planning inputs (not duplicated verbatim).

| Source Doc | Merge Target Section | Why |
|---|---|---|
| `30-game-selector-ui.md` | Home / Game Selector baseline + Phase 4 slice spec | Core launcher UX behavior |
| `31-tactical-scenario-setup-ui.md` | Workspace and scenario setup planning | Setup/edit flow alignment |
| `32-content-management-screen-inventory.md` | IA and slice sequencing | Complete screen inventory |
| `33-36` content UI flow docs | Content management slice specs | CRUD flow consistency |
| `37-import-workflow-ui.md` | Import flow slice | Validation and feedback UX |
| `38-application-shell-and-navigation-model.md` | IA and navigation matrix | Route/link structure clarity |
| `39-board-presentation-rules.md` + `40-piece-visual-states.md` | Workspace visual behavior guidance | Board readability and cues |
| `41-checkers-reference-ui-flow.md` | Rules-profile-specific UX notes | Scenario/profile variance |
| `59-65` manual entry/import docs | Content + import slice backlog | End-to-end GM workflows |

Merge method:
1. Extract decisions and constraints,
2. Link to source doc for detail,
3. Record resolved choices in this roadmap,
4. Mark source rows as integrated.

---

## Working Outline for Development
Use this structure when converting roadmap items into implementation work:

1. **Problem statement**
2. **User goal**
3. **Current behavior**
4. **Target behavior**
5. **UI changes (components/pages)**
6. **States and validation**
7. **Accessibility checks**
8. **Acceptance criteria**
9. **Follow-up metrics**

---

## Governance
- Keep this roadmap updated at the end of each UI slice.
- Prefer conservative, scoped updates tied to visible user outcomes.
- Keep terminology and messaging changes synchronized across docs and UI labels.

Owner update cadence:
- quick update every slice,
- deeper pass at milestone/release boundaries.
