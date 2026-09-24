# Design

## Context

Proyecto greenfield: el repositorio no tiene aún código ni proyecto .NET (SDK .NET 10
disponible). La Fase 1 es una consola en español, datos solo en memoria (NFR-001) y entrega
con etiqueta git `fase-1`. Los requisitos objetivo están definidos en
`specs/personas/spec.md` (alta, listado, búsqueda por Id/texto, modificación, baja,
validaciones y menú robusto). Ver `proposal.md` para la motivación.

## Goals / Non-Goals

**Goals:**
- Estructura de 4 clases simple y mantenible alineada con RA2 (colecciones, funciones y clases).
- Diseño que deje preparada la relación 1 Empresa : N Personas (RA3) sin sobre-diseñar la Fase 1.
- Validación centralizada y un bucle de menú que nunca falle ante entrada incorrecta (FR-012).

**Non-Goals:**
- Implementar la entidad `Empresa`, persistencia o autenticación (fuera de alcance de la Fase 1).
- Añadir frameworks de inyección de dependencias, librerías externas o proyectos de test
  (verificación manual mediante demo, según criterio de aceptación §10).
- Validación exhaustiva de correo (regex de RFC): solo los criterios de FR-013/014.

## Decisions

- **D1 — Estructura de archivos**: proyecto de consola único `src/AgendaProfesional.csproj`
  con `Program.cs`, `Persona.cs`, `GestorPersonas.cs` y `Validaciones.cs`.
  *Alternativa descartada*: todo en un único `Program.cs` (choca con RA2).
  *Alternativa descartada*: `.sln` con proyecto de tests (la verificación es manual por demo).

- **D2 — Modelo `Persona`**: clase con propiedades públicas; `IdPersona` con `get` de solo
  lectura, asignado internamente por el gestor (FR-003).
  *Alternativa descartada*: `record`/`struct` inmutable (la modificación necesita re-asignar
  propiedades).

- **D3 — Almacenamiento**: `List<Persona>` + contador `siguienteId` dentro de `GestorPersonas`.
  *Alternativa descartada*: `Dictionary<int, Persona>` (complica el listado ordenado y no
  aporta nada en esta fase).

- **D4 — Preparación para Fase 2**: `EmpresaAsignada` se expone como propiedad pública (no
  campo directo) y su valor se guarda con `Trim()`. En la Fase 2 podrá sustituirse
  internamente por una referencia a `Empresa` sin romper consumidores. `GestorPersonas` no
  depende de una implementación concreta de almacenamiento.

- **D5 — Validaciones**: clase estática `Validaciones` con métodos para correo (no vacío,
  contiene `@`, dominio con al menos un punto), teléfono (no vacío, solo dígitos, longitud 9)
  y lectura numérica de opciones mediante `int.TryParse` en bucle.

- **D6 — Interacción**: bucle `while (opcion != Salir)` con `switch` de opciones; cada opción
  de modificar/eliminar muestra el registro y exige confirmación `s/n`. Entrada no numérica u
  opción inexistente → aviso y repetición del menú.

- **D7 — Orden del listado**: por `Apellidos` y luego `Nombre`, con formato alineado legible
  en consola (FR-004).

## Risks / Trade-offs

- [La validación de correo minimalista (FR-013) puede aceptar formatos poco realistas] →
  se documenta como asunción en `spec.md` §9 y queda como punto abierto para el profesor.
- [Texto libre de empresa mal normalizado] → `Trim()` al guardar; texto libre por decisión FR-015.
- [Datos efímeros por estar en memoria] → decisión de alcance NFR-001; se comunica en la demo.
- [Bucle de menú con entradas reiteradas puede resultar tedioso] → mensajes de aviso claros y
  opción "Salir" siempre visible.

## Migration Plan

- Proyecto nuevo: no hay migración de datos. Implementación secuencial según `tasks.md`,
  verificando con la demo manual del criterio de aceptación (§10 de `spec.md`).
- Entrega: etiqueta git `fase-1` sobre el estado implementado; las fases posteriores
  continúan sobre la misma base sin romper la Fase 1 (NFR-003).

## Open Questions

- Ninguna: las decisiones que podrían cambiar requisitos (reglas exactas de validación,
  convención de entrega) ya están resueltas en los requisitos y en `spec.md` §9.