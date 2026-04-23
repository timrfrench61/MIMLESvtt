# Refactor Plan - Make VttGamebox

## Purpose

Define a code-first refactor plan to introduce `VttGamebox` as the core content model name and move snapshot naming fully into persistence code.

Given current project state, we are **not contract-locked yet**. The goal is to get clean structure now.

---

## Problem Statement

Current naming mixes two concerns in one type surface:

- content model identity (`VttGamebox` concept)
- persistence envelope (`VttGameboxSnapshot`)

Using `VttGameboxSnapshot` outside persistence makes persistence wrappers look like core runtime/content models.

Desired direction:
- core content model named `VttGamebox`
- snapshot wrappers strictly persistence-only

---

## Current State (Observed)

- Core docs previously listed `VttGameboxSnapshot` as root content class.
- Code currently has `VttGameboxManifest`, `VttGameboxDefinition`, `VttGameboxAsset` and snapshot serializers/services.
- Snapshot envelope currently represented as:
  - `Version`
  - `Manifest`
  - `Definitions`
  - `Assets`

---

## Refactor Goal

Introduce and adopt:
- `VttGamebox` as the primary content package model used by core/application flows

Keep persistence-specific naming:
- `VttGameboxSnapshot` remains the persistence envelope type

---

## Design Split (Target)

## A) Core content model (non-snapshot, app-facing)

- `VttGamebox`
  - `Manifest`
  - `Definitions`
  - `Assets`

Use in:
- core class docs
- content library and feature-level contracts
- non-persistence service APIs (where appropriate)

## B) Persistence envelope (snapshot, infra-facing)

- `VttGameboxSnapshot`
  - `Version`
  - `Manifest`
  - `Definitions`
  - `Assets`

Use in:
- serializers
- file persistence services
- import/export workflows

---

## Proposed Code Refactor Plan

## Phase 1 - Add core model type

1. Create `VttGamebox` class.
2. Move app-facing usage toward `VttGamebox` (core docs, class lists, feature-facing contracts).
3. Keep `VttGameboxSnapshot` unchanged for now.

Definition target:

```csharp
public class VttGamebox
{
    public VttGameboxManifest Manifest { get; set; } = new();
    public List<VttGameboxDefinition> Definitions { get; set; } = [];
    public List<VttGameboxAsset> Assets { get; set; } = [];
}
```

## Phase 2 - Add explicit mapping code

Add a small mapper class:

- `ToSnapshot(VttGamebox model, int version)`
- `FromSnapshot(VttGameboxSnapshot snapshot)`

This creates a clear code-layer split:

- app/core uses `VttGamebox`
- persistence uses `VttGameboxSnapshot`

## Phase 3 - Refactor persistence service surfaces

Refactor service signatures so app-facing entry points accept/return `VttGamebox`, while serialization remains snapshot-based internally.

Target pattern:

- external/public app call: `SaveVttGamebox(VttGamebox pack, path)`
- internal persistence call: serialize `VttGameboxSnapshot`

## Phase 4 - Remove snapshot bleed into non-persistence code

Replace `VttGameboxSnapshot` usage in:

- overview/core class docs
- non-persistence feature/service APIs
- any workflow method that is not directly serializer/file I/O

Keep `VttGameboxSnapshot` in:

- serializer classes
- file persistence classes
- import/export dispatch classes where envelope inspection is required

## Phase 5 - Clean-up and stabilization

1. Remove compatibility shims if no longer needed.
2. Ensure `03-persistence` docs clearly state snapshot envelope is infra-only.
3. Ensure `00-overview` docs list `VttGamebox` as root content model.

---

## Non-Goals

- Do not refactor unrelated session/action/workspace areas.
- Do not bundle multiplayer or module-system changes into this pass.

---

## Risks and Mitigations

1. Risk: Naming churn introduces confusion during transition.
- Mitigation: keep explicit “core model vs snapshot envelope” sections in docs.

2. Risk: service/API break during rename.
- Mitigation: do phased changes with mapper and thin compatibility pass.

3. Risk: partial rename leaves snapshot types in app/core code.
- Mitigation: run code search sweep and finish all non-persistence replacements in same pass.

---

## Variance Tracking Notes

Track these as refactor checkpoints:

- Any core/root docs still naming `VttGameboxSnapshot` as core model.
- Any non-persistence service API exposing snapshot types directly.
- Any persistence service exposing snapshot type in app-facing signatures.

---

## Success Criteria

- Core docs consistently use `VttGamebox` for content model naming.
- Non-persistence code surfaces no longer expose `VttGameboxSnapshot`.
- Persistence code owns snapshot envelope naming.
- Build passes and content-pack tests (or equivalent regression checks) pass.
