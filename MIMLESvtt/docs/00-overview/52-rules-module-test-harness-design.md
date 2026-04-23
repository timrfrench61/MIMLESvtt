# Rules Module Test Harness Design

## Purpose

Define how rules modules are tested in isolation from UI concerns.

The harness design focuses on deterministic inputs, explicit expected outputs, and repeatable rule validation/resolution checks.

---

## Harness Goals

- exercise rules modules without Razor/UI dependency
- provide deterministic session context fixtures
- validate both rejection and success paths
- keep assertions focused on module outputs and engine-apply expectations

---

## Harness Structure

### 1) Fixture builder layer

Build synthetic test context:

- session aggregate fixture
- participant/seat setup
- turn/phase setup
- objective metadata setup (when needed)

### 2) Request driver layer

Submit action/check requests to module logic:

- validation requests
- resolution requests
- objective/victory evaluation requests

### 3) Assertion layer

Verify:

- validity/invalidity outcomes
- structured result payload fields
- expected effect outputs
- expected user-facing message summaries

---

## Determinism Strategy

- Inject deterministic random source into shared dice service when asserting roll-dependent behavior.
- Use fixed seed or stub random provider for reproducible tests.
- Keep non-random scenarios separate from random-resolution scenarios.

---

## Test Categories

1. **Validation rejection tests**
   - invalid actor/target/phase
   - missing/invalid payload fields

2. **Resolution success tests**
   - expected roll/result interpretation
   - expected effect payload shape

3. **Turn/phase integration tests**
   - expected advancement hints/outputs

4. **Objective/victory tests**
   - simple condition completion
   - layered condition aggregation

5. **Regression fixtures**
   - preserved historical edge cases

---

## Input/Output Contract Guidance

Each harness case should define:

- Input context fixture
- Input request payload
- Expected validation or resolution result
- Expected effect payload fragments
- Expected diagnostics/user summary strings (where applicable)

---

## Backlog Mapping

- `docs/05-backlog/06-backlog-rules-framework.md` (RF-012)
- Depends on RF-002 and RF-003
