# Design

## Context

Fase 1 is implemented and working (`src/Program.cs`, `Persona.cs`, `GestorPersonas.cs`,
`Validaciones.cs`, single `AgendaProfesional.csproj`, net10.0). Today's input handling is
uniformly "read the whole form, then validate, and if anything fails `continue` and re-ask
everything": see the `while (true)` blocks in `Program.cs:87` (Alta) and `Program.cs:200`
(Modificar). `GestorPersonas.BuscarPorTexto` does one free-text `Contains` with an OR between
Nombre and Apellidos (`GestorPersonas.cs:36-48`), and `Program.cs:143-177` gates search behind
a "1. Buscar por Id / 2. Buscar por texto" sub-menu. `Validaciones` holds only three things:
`EsCorreoValido`, `EsTelefonoValido` (length exactly 9, `Validaciones.cs:24-37`) and
`LeerEntero`. Storage is in-memory only (NFR-001), so nothing survives a restart and there is no
migration surface.

Constraints that shape the approach: verification is manual (`dotnet build` + running the
console, per the acceptance evidence in `spec.md` §10) — no test project is introduced, matching
the Phase 1 decision; console text stays Spanish (NFR-004) while these planning artifacts are
English; no identifier is renamed (`Persona`, `GestorPersonas`, `Validaciones`, `IdPersona`,
`EmpresaAsignada`, …); no new NuGet dependency; the Phase 2 hooks of `spec.md` §8.5 must survive
(`EmpresaAsignada` as a settable property, no dependency on concrete storage).

## Goals / Non-Goals

**Goals:**
- One reusable input-reading primitive that validates a single field and re-asks only that
  field, so Phase 2's `Empresas` module inherits the behaviour instead of re-implementing it.
- Name-based lookup with tolerant matching and homonym disambiguation, shared by search,
  modify and delete.
- A single place that decides what `cancelar` means and where cancellation propagates.
- Keep the whole update inside the existing 4 files; no new project, no new fields, no renames.

**Non-Goals:**
- Persistence, `Empresa` as a real entity, or any 1:N wiring (still Phase 2 / out of scope).
- Normalizing stored names (stripping accents at write time). Search folds accents, storage does
  not — the demo shows real accented names.
- A test project, DI container or input abstraction library.
- Changing the Listing (option 2) output or the main menu's option numbers.

## Decisions

- **D1 — Accent-insensitive comparison by normalization at comparison time, not at storage
  time.** A helper folds a string once (upper-invariant + `FormD` + strip
  `UnicodeCategory.NonSpacingMark`) and both sides of the comparison are folded, then
  `IndexOf(..., OrdinalIgnoreCase)` does the substring test on the folded forms.
  *Alternatives rejected*: (a) `string.Normalize` on the *stored* value would mean rewriting
  `García` to `Garcia` in memory, losing data the user typed; (b) mapping accents to a
  dictionary or using a custom comparison key (ICU collation) is heavier than a course project
  needs; (c) `ToLowerInvariant()` alone does not fold accents, which is exactly the case R1
  requires. Folding both sides also keeps the rule symmetric for future modules.

- **D2 — Name lookup lives in `GestorPersonas`, keyed by two fields with AND semantics.**
  `GestorPersonas` gains a lookup that takes Nombre and Apellidos and filters on both, using the
  D1 folding; `BuscarPorTexto`'s OR behaviour disappears. The single free-text method is
  replaced rather than kept alongside, so there is no dead code and no second definition of
  "search".
  *Alternatives rejected*: keeping `BuscarPorTexto` for a future "buscar en cualquier campo"
  feature — no requirement asks for it, and YAGNI at this size; putting the predicate in
  `Program.cs` would leak business rules into the console layer.

- **D3 — `BuscarPorId` survives as an internal helper, not as user-facing behaviour.** R1
  removes Id *input*, not the Id concept: `IdPersona` stays auto-incremental and displayed. The
  public `BuscarPorId(int)` is no longer called from any prompt; `Modificar`/`Eliminar` keep
  taking an id internally (they are still id-keyed operations) and can resolve it through the
  existing private lookup. This keeps the diff small and leaves a clean seam for Phase 2.
  *Alternatives rejected*: deleting the method outright would force `Modificar`/`Eliminar` to
  re-scan the list themselves; re-keying `Modificar`/`Eliminar` on a `Persona` reference would
  be a larger refactor with no behavioural gain here.

- **D4 — Homonym selection is a console-layer concern; the manager never picks for the user.**
  `Program.cs` gets one shared "locate a person" routine used by modify and delete: search →
  0 results → explicit message and return; 1 result → use it; >1 → print a temporary numbered
  list (1..N, each row including `IdPersona`) and ask for a number, re-asking on out-of-range.
  Search (option 3) prints *all* matches and does not ask for a number.
  *Alternatives rejected*: (a) letting the manager resolve "the first match" silently would
  delete or modify the wrong homonym — unacceptable for a destructive operation; (b) adding a
  permanent "seleccionar persona" mode to the menu increases the surface without a requirement;
  (c) filtering candidates by extra fields (empresa, teléfono) to break ties automatically
  would guess on the user's behalf; the spec asks the user.

- **D5 — A single reusable per-field reader in `Validaciones` is the mechanism for R4.**
  `Validaciones` gains a small helper family with the same shape everywhere:
  - read a line, expose whether the raw line was the reserved word `cancelar`;
  - validate that line against a predicate plus a Spanish error message;
  - loop until valid, re-printing *only* that field's prompt;
  - return a `cancelar` signal to the caller instead of throwing across layers.
  The caller decides what to do on cancellation (return to the menu), because only the caller
  knows the operation. Two variants are needed: a mandatory one (Alta: empty rejected) and an
  "empty keeps current value" one (Modify). Numeric prompts (menu option, homonym number) and the
  s/n confirmation reuse the same loop shape, extended to `int.TryParse` and to `s`/`n`.
  *Alternatives rejected*: (a) keeping the current whole-form `while (true)` and just moving the
  error messages — that is exactly the behaviour R4 replaces; (b) exceptions for cancellation —
  control flow through exceptions is harder to follow here and would pollute `Validaciones`'s
  public shape; (c) a `ConsoleInput`/`IConsole` abstraction with a testable surface — no test
  project exists, so the abstraction would pay for itself only in Phase 2+, not now.

- **D6 — `cancelar` is recognised in one place, before any field validation.** The raw line is
  compared to `cancelar` with `Trim()` + case-insensitive comparison *before* the value reaches
  a field validator, so cancellation wins over invalid input (no validation error is shown for
  `cancelar`) and over "empty" semantics.
  *Edge cases accepted and documented*: (i) `cancelar` becomes a reserved word, so no
  Nombre/Apellidos/Correo/EmpresaAsignada value can ever be literally `cancelar` — accepted,
  because the alternative (a special escape key or Ctrl+C) is less discoverable in a console; the
  prompts state the reserved word so it is not surprising; (ii) the word is matched after `Trim()`,
  so `"  cancelar  "` also cancels; (iii) it is matched case-insensitively, so `Cancelar` works;
  (iv) cancelling during the modify summary confirmation must not save anything, which is the same
  path as answering "no", so the guarantee is "nothing is saved"; (v) `cancelar` typed at the menu
  option prompt is simply not a valid option (non-numeric), so it re-asks — consistent.

- **D7 — Modify is a partial update computed in the console layer, applied in one call.** The
  console asks the five fields in order, keeps only non-empty answers in a small local set of
  pending changes, and:
  - if the pending set is empty → print "no changes" and return to the menu, with no confirmation;
  - otherwise → print a `campo: valor anterior → valor nuevo` summary and ask s/n; on "n" or
    `cancelar`, nothing is saved.
  Only after confirmation does the manager apply the pending values, so a cancelled or declined
  edit can never leave a half-updated `Persona`.
  *Alternatives rejected*: mutating the `Persona` in place as the user types and relying on
  `cancelar` to undo — no reliable undo, and a crash would leave corrupted data; asking for a
  final full-form echo — noisier and it would repeat validations.

- **D8 — Phone format is one regex plus one decomposed rule set for messages.** The regex
  `^\+\d{1,3} ?\d{6,12}$` encodes "+", 1-3 prefix digits, an optional single space, and 6-12
  number digits. The rule is validated as a whole (no letters or special characters are possible
  by construction), and the *error message* is chosen by checking the specific failure
  (missing prefix / non-digit character / bad lengths) so the user learns which rule failed, as
  R4 requires. No new `Persona` field: `Telefono` stays one `string`, which also keeps Phase 2's
  model hooks intact.
  *Alternatives rejected*: a non-regex character-class walk — same result, harder to read and
  harder to reuse for other formats; validating prefix and number separately with two regexes —
  two failure messages for one field, more code for the same behaviour. The exact digit ranges
  are an assumption (see proposal.md) and live in one place so they are trivial to adjust.

- **D9 — "Non-essential data" review outcome: there is none, so no clearing mechanism.**
  All five `Persona` fields are mandatory (`EmpresaAsignada` included, overriding `spec.md` §6's
  "no" column). Because nothing is optional, the previously proposed `-`-to-clear convention is
  dropped: on modify an empty answer already means "keep", so a sentinel for clearing would be a
  second way to say the same thing. `Persona.cs` therefore needs no change, and the console's
  current `"-"` placeholder for an empty company disappears from the displayed rows.

- **D10 — Conventions become a capability, `project.md` becomes the pointer.** The permanent
  rules (readable-name lookup, tolerant matching with homonyms, per-field re-ask, `cancelar`,
  empty-to-keep + confirmation, mandatory-by-default, Id shown-not-typed) are written as an
  ADDED capability `convenciones-entrada` with scenarios, and `openspec/project.md` gains a
  "Conventions" pointer plus an updated `Domain` section (phone format with prefix, mandatory
  company). Each module's own spec stays about *that* module; the shared rules are stated once.
  *Alternatives rejected*: duplicating the rules into every future module's spec (drift is then
  inevitable); putting them only in `project.md` (unenforceable — no scenarios, nothing to
  validate against).

- **D11 — Archive ordering is part of the plan.** `openspec/specs/` is empty: the `personas`
  capability currently lives only as an unarchived delta in `fase-1-gestion-personas`. This
  change deliberately uses MODIFIED/REMOVED against `personas`, so `fase-1-gestion-personas` must
  be archived (or its specs synced) *before* this change is archived; otherwise archive would
  report that the target spec does not exist. `openspec validate` already surfaces this as an
  informational note, not an error.

## Risks / Trade-offs

- [Folded-string comparison means `"García"` and `"Garcia"` are indistinguishable in *matching*,
  so two stored people differing only by accents could both match] → acceptable for a name
  search; disambiguation is by explicit user choice (D4), never by automatic narrowing.
- [`cancelar` is a reserved word, so a person literally named/called something containing it
  cannot be entered] → accepted and documented (D6-v); prompts announce the reserved word; this
  is the trade-off of a text-only console.
- [Nested loops (field loop × operation loop) can read as complex control flow] → keep the
  per-field loops inside small named helpers (D5) so each operation body stays a straight
  sequence of statements; cancellation returns a value, it is not thrown.
- [Phone format assumption may not match the professor's expected rule] → isolated in one regex
  and one message set (D8) plus a named constant for the ranges; changing it is a one-line edit
  and the spec delta documents it as an assumption.
- [Removing the Id search removes a fast path for large datasets] → data is in-memory and small
  (NFR-001); correctness for the human user outranks lookup speed here.
- [Making `EmpresaAsignada` mandatory breaks data entered under Phase 1] → no migration is
  possible or needed (in-memory); restarting gives an empty agenda, and no existing record can
  hold an empty company because Phase 1 could not create one either in practice after this rule.
- [`GestorPersonas` gains a two-field lookup whose signature is specific to Personas] → Phase 2
  modules will need their own lookup; that is expected, and D5's reader helpers are the part that
  should genuinely be shared (per `convenciones-entrada`).

## Migration Plan

- No data migration: storage is in-memory (NFR-001) and the app restarts empty.
- Rollback is a source revert of the four files; no external state is touched.
- Implementation order (see `tasks.md`): `Validaciones` helpers → `GestorPersonas` lookup and
  partial update → `Program` flows → manual verification checklist → `dotnet build`.
- Archive order: archive `fase-1-gestion-personas` (or sync its specs) before archiving this
  change, so the `personas` capability exists for the MODIFIED/REMOVED delta (D11).

## Open Questions

- None blocking. The remaining unknowns are the phone digit ranges (a single regex constant,
  adjustable without touching the specs' behaviour) and whether the professor wants
  `cancelar` announced in every prompt or only in the operation headers; both are wording/
  constant choices, not spec changes.