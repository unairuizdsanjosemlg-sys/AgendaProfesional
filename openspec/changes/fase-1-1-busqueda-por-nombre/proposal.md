# Proposal

## Why

Phase 1 (`fase-1-gestion-personas`) is implemented and working, but it forces the user to
remember and type the internal `IdPersona` to find somebody in search, modify and delete.
For a human-facing agenda that is the wrong lookup model: the user knows a person's
Nombre and Apellidos, not a database identifier. Phase 1.1 (small update of Phase 1)
replaces that lookup with a name-based one and fixes the input experience that Phase 1
left rough (whole-form re-validation instead of per-field, a Spain-only 9-digit phone, and
a supposedly optional `EmpresaAsignada`).

The rules settled here — lookup by readable name, immediate per-field validation,
`cancelar` as a universal escape, empty-to-keep on edit — are meant to be permanent, so
they are also written down as project-wide conventions that Phase 2 (`Empresas`) must
follow instead of reinventing them.

## What Changes

### Removed

- **BREAKING** Search by `IdPersona` is removed entirely: the sub-menu "Buscar por Id" of
  menu option 3 disappears, and options 4 and 5 no longer ask for an Id. Requirements
  FR-005 and FR-008, the Id part of US-3, and the "by Id" wording of US-4/US-5 are dropped
  or rewritten.
- The internal `BuscarPorId` is no longer part of user-facing behaviour; it survives only as
  a private/internal helper if the implementation still needs it.
- The `-` convention for clearing an optional value on modify is **not** adopted: the review
  of "non-essential data" concluded that there is no such data, so no clearing mechanism is
  needed (see design.md, D9).

### Modified

- **Search (option 3), Modify (4) and Delete (5)** locate a person by asking for Nombre and
  Apellidos in two separate prompts. Matching is partial (substring), case-insensitive and
  accent-insensitive ("garcia" matches "García"), and both fields must match (AND).
- **Search** shows every match. **Modify/Delete** resolve the match as follows: exactly one
  match is used directly; several matches (homonyms) produce a temporary numbered list
  (1, 2, 3...) for the user to pick from; no matches is reported explicitly and returns to
  the menu without an error.
- **`IdPersona` stays internal** (auto-incremental, never editable, never typed to locate a
  person) but is still **displayed** everywhere a person is shown: listing, search results,
  homonym selection list and delete confirmation. The Listing (option 2) is unchanged.
- **Modify** shows the current data and then asks each field in turn (Nombre, Apellidos,
  Telefono, Correo, EmpresaAsignada). An empty answer keeps the current value; only non-empty
  fields are changed. If every field is left empty the system says there are no changes and
  returns to the menu. Before saving, it shows a summary of the fields that will change and
  asks for explicit s/n confirmation. The s/n confirmation before delete is kept.
- **Phone validation** (`Validaciones.EsTelefonoValido`) replaces FR-014 (exactly 9 digits,
  Spain only): Telefono stays a single text field, now required to carry the international
  prefix — `+` followed by 1 to 3 digits, an optional single space, then 6 to 12 digits; only
  digits and that single `+` are allowed. Email validation (FR-013) is unchanged.
- **All Persona fields are mandatory**, including `EmpresaAsignada`, which `spec.md` §6 listed
  as "no" (not mandatory). Empty input is never accepted on Alta; on Modify, empty means
  "keep".
- **Immediate per-field validation** applies transversally: each datum is validated right
  after it is typed, the error names the rule that failed, and only the failing field is
  re-asked — never the whole form again. This covers Nombre, Apellidos, Telefono, Correo,
  EmpresaAsignada, the menu option, the homonym number and the s/n confirmations. In Modify,
  an empty answer is valid (keep) and skips validation; a non-empty answer is validated at once.
- **The word `cancelar` (case-insensitive) typed at ANY prompt** aborts the current operation
  without saving and returns to the main menu. It is a reserved word; design.md records the
  edge cases this creates.

### Added

- New capability `convenciones-entrada`: the permanent input/lookup conventions that any
  current or future module (e.g. `Empresas` in Phase 2) must follow — lookup by readable name,
  partial/case/accent-insensitive search with homonym disambiguation, immediate per-field
  validation, `cancelar` everywhere, empty-to-keep with confirmation, mandatory fields unless
  a spec says otherwise, and "Id shown but never typed to locate".
- `openspec/project.md` references these conventions as project-wide rules.

### Unchanged

- Acceptance criteria and evidence (`spec.md` §10) stay as they are: a complete demo with an
  alta, a search, a modification and a delete, showing their confirmations and validations.
- Listing (option 2), main menu loop, exit option, in-memory-only storage (NFR-001), Spanish
  console output (NFR-004) and all existing Spanish identifiers (`Persona`, `GestorPersonas`,
  `Validaciones`, `IdPersona`, …) — nothing is renamed.
- The Phase 2 design hooks from `spec.md` §8.5 stay intact: `EmpresaAsignada` remains a
  property (not a public field) and `GestorPersonas` keeps no dependency on concrete storage.

## Capabilities

### New Capabilities

- `convenciones-entrada`: project-wide conventions for console data entry and record lookup
  (readable-name lookup, tolerant matching with homonym disambiguation, immediate per-field
  validation, universal `cancelar`, empty-to-keep with confirmation, mandatory fields, and
  Id shown but never typed).

### Modified Capabilities

- `personas`: name-based lookup for search/modify/delete replaces Id-based lookup, `IdPersona`
  becomes display-only, modify becomes empty-to-keep with a change summary and confirmation,
  phone validation gains a mandatory international prefix, all fields become mandatory, and
  per-field immediate validation plus `cancelar` are introduced.

## Impact

- **Code** (all under `src/`, single `AgendaProfesional.csproj`, no new project):
  - `Program.cs` — remove the Id search sub-menu; new locate-by-name flow for options 3/4/5,
    homonym numbered selection, modify empty-to-keep + change summary + confirmation,
    `cancelar` handling at every prompt, per-field re-ask.
  - `GestorPersonas.cs` — replace `BuscarPorTexto` with a name-based lookup
    (partial + case- + accent-insensitive, Nombre AND Apellidos); modify becomes a partial
    update; `BuscarPorId` no longer user-facing.
  - `Validaciones.cs` — new phone rule with prefix, accent-insensitive comparison helper,
    "optional in edit" input helper, homonym number reader, s/n reader with re-ask.
  - `Persona.cs` — unchanged: same properties, same Spanish names (R3 adds no field).
- **Data**: none. Storage is in-memory only (NFR-001), so the phone-format change needs no
  migration and restarting the app yields an empty agenda.
- **Dependencies**: none added; standard .NET collections only.
- **Verification**: manual, via `dotnet build` plus a scripted console run. No test framework is
  introduced (consistent with the Phase 1 decision in `fase-1-gestion-personas/design.md`).
- **Specs**: delta on `personas` (MODIFIED/REMOVED/ADDED) plus the new `convenciones-entrada`
  capability. `spec.md` itself is not modified.

## Assumptions to confirm

1. **Phone format** — `+` + 1-3 prefix digits + optional single space + 6-12 digits. Chosen
   because it covers Spain (`+34`) and other countries without new fields. Confirm the digit
   ranges and whether the space is acceptable.
2. **The Id display stays** in listing, search results, selection list and confirmations —
   useful for the demo, invisible as an input.
3. **Archive order** — the `personas` capability currently exists only as an unarchived delta
   in `fase-1-gestion-personas`; `openspec/specs/` is empty. This change's `personas` delta
   uses MODIFIED/REMOVED, so `fase-1-gestion-personas` must be archived (or its specs synced)
   before archiving this one.
4. **"Non-essential data" review outcome** — no field qualifies, therefore no `-`-to-clear
   mechanism; `EmpresaAsignada` becomes mandatory.
5. **`cancelar` is reserved** — it can never be a legitimate value of Nombre, Apellidos,
   Correo or EmpresaAsignada. Recorded as an accepted edge case.
6. **Accent-insensitive matching** covers Spanish diacritics (á é í ó ú ü ñ Ñ) for both typed
   input and stored values.