# Design

## Context

La Fase 1 (`src/Program.cs`, `Persona.cs`, `GestorPersonas.cs`, `Validaciones.cs`) es una
consola en memoria con menú principal planar (opciones 1-6) y validación inmediata por
campo. `Validaciones` es la capa reutilizable exigida por project.md; `convenciones-entrada`
fija las convenciones permanentes de entrada (localización por nombre legible, `cancelar`,
vacío-conserva, confirmaciones, campos obligatorios). Ver proposal.md para el motivo.

Decisiones acordadas con el usuario (también resumidas en Decisions):

1. Búsqueda de empresas por `NombreComercial` (parcial) o `CIF`; `IdEmpresa` visible pero
   nunca tecleado.
2. CIF con validación de formato (letra/dígito + 7 dígitos + control).
3. CIF único en el catálogo.
4. Nombres comerciales duplicados permitidos (homónimos con lista numerada).
5. Listado ordenado por `NombreComercial`; reutilizar `Validaciones.cs` sin tocar lo
   existente; trabajar en `main` y etiquetar `fase-2`.

## Goals / Non-Goals

**Goals:**

- Catálogo de empresas con CRUD completo, mismo estilo de interacción que personas.
- Menú principal → submenus (Personas / Empresas) con vuelta sin salir; solo "Salir" cierra.
- Cero cambios de comportamiento en los flujos de la Fase 1 (verificación por regresión).

**Non-Goals:**

- Relación persona-empresa (1:N), bloqueo de baja por relaciones, listados cruzados.
- Persistencia a fichero/BBDD (NFR-001: solo memoria).
- Proyecto de pruebas automatizadas (no existe en el repo; la verificación es manual).

## Decisions

- **Estructura de clases** (sigue el patrón de la Fase 1, nombres en español):
  - `src/Empresa.cs`: modelo con `IdEmpresa { get; }` y propiedades con setter.
  - `src/GestorEmpresas.cs`: `List<Empresa>` + `int _siguienteId = 1` (mismo esquema de
    autogeneración que `GestorPersonas`; ids incrementales no reutilizados dentro de la
    ejecución). Expone `AltaEmpresa`, `Listar` (orden por NombreComercial con
    `StringComparer.OrdinalIgnoreCase`), `BuscarPorTexto` (subcadena de nombre o CIF exacto,
    ambas vía `Validaciones.Coincide`/normalización), `Modificar` (actualización parcial
    vacío-conserva) y `Eliminar`; `BuscarPorId` es helper interno.
  - `src/MenuEmpresas.cs`: submenu en bucle con las cinco operaciones + "Volver",
    alternativa a volcar todo en `Program.cs` (que ya tiene ~300 líneas). Las operaciones
    de personas se mueven aquí por analogía **solo si** hace falta reutilizar un helper; si
    no, `Program.cs` conserva sus funciones actuales y solo se renumera el menú.
  - `Program.cs`: menú principal (1. Personas, 2. Empresas, 3. Salir); el switch delega en
    el submenu de personas (bucle propio con "Volver") y en `MenuEmpresas`.
- **Diseño de datos `Empresa`**: `IdEmpresa`, `NombreComercial`, `CIF`, `Telefono`,
  `CorreoElectronico`, `Direccion`. Todos los campos son obligatorios: la data model no
  declara `Direccion` opcional y `convenciones-entrada` implica obligatoriedad ante la
  ausencia de declaración (decisión registrada aquí; Teléfono y Correo ya venían marcados
  como no vacíos en el enunciado).
- **Validaciones**: reutilizar tal cual `ValidarTexto`, `ValidarCorreo`, `ValidarTelefono`,
  `LeerCampoObligatorio`, `LeerCampoModificacion`, `Confirmar`, `LeerEntero`,
  `LeerNumeroCancelable` y `Coincide`. Añadidos en `Validaciones.cs` (aditivo, sin tocar
  métodos existentes):
  - `ValidarCif(string)` → obligatorio + regex `^[A-Za-z0-9]\d{7}[A-Za-z0-9]$`.
  - `ConfirmarCancelable(...)` + `enum Confirmacion { Si, No, Cancelado }`: `Confirmar`
    de la Fase 1 colapsa "n" y "cancelar" en `false`, pero los menús de Fase 2 necesitan
    distinguirlos ("n" → permanece en el submenu, `cancelar` → menú principal, según
    `convenciones-entrada`). Los métodos existentes no se modifican.
  La unicidad NO vive en `Validaciones` (es regla de negocio del catálogo):
  `GestorEmpresas.AltaEmpresa`/`Modificar` la comprueban y el menú
  repregunta solo el CIF, igual que el patrón de repregunta por campo de la Fase 1.
- **Búsqueda**: un único prompt "Nombre comercial o CIF". El texto coincide si es
  subcadena del NombreComercial (acento/case-insensitive, `Validaciones.Coincide`) **o** si
  es igual al CIF (case-insensitive). Alternativa descartada: submenú de criterio (1. nombre
  2. CIF) — añade una interacción sin valor demostrable. Modificación y baja reutilizan esta
  localización + desambiguación numerada (idéntico a `LocalizarPersona` de la Fase 1).
- **Resumen/confirmación**: alta muestra resumen + `Confirmar` antes de crear e informa el
  id; modificación muestra resumen de cambios con la empresa afectada; baja muestra el
  registro afectado. Todo con la palabra `cancelar` heredada de `Validaciones`.
- **Git**: se trabaja en `main` (convención del repo; sin ramas en Fase 1) y se etiqueta
  `fase-2` al final; `fase-1` ya existe como punto de regresión.

## Risks / Trade-offs

- [Reordenar el menú principal rompe el flujo memorizado de la Fase 1] → único cambio
  **BREAKING** de la propuesta; se mitiga con la regresión completa de personas antes de
  terminar y con la etiqueta `fase-1` como referencia ejecutable.
- [Dos "src" de menú (`Program.cs` con personas + `MenuEmpresas`) pueden divergir en estilo]
  → se copian patrones literalmente de las funciones de personas (mensajes en español,
  mismas llamadas a `Validaciones`).
- [Unicidad de CIF y formato podrían no cubrir CIFs reales con letra de control especial]
  → regex de 9 caracteres alfanuméricos: suficiente para el alcance de Fase 2; se documenta
  como validación "evidente", no oficial.
- [Modificar el menú principal toca el requisito de `personas`] → delta MODIFIED limitado a
  ese único requisito; el resto de `personas` queda intacto.

## Open Questions

- Ninguno: todas las decisiones con impacto en specs, diseño o tareas están resueltas.
