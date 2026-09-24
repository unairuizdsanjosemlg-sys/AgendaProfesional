# Proposal

## Why

La aplicación Agenda de Consultoría (consola en C#) estrena su desarrollo con la Fase 1
según `spec.md`: una agenda interna de personas colaboradoras. Este cambio materializa esa
fase mediante Spec Driven Development, cubriendo los RA1 (consola usable), RA2
(colecciones, funciones y clases) y RA3 (diseño preparado para la relación 1 empresa : N
personas). Parte de un repositorio greenfield, por lo que define además la arquitectura
base y las convenciones que seguirán las fases posteriores.

## What Changes

- Nueva capacidad `personas` con alta, listado, búsqueda (por Id y por texto), modificación
  y baja de personas de la agenda.
- Validación de correo y teléfono en el alta y en la modificación de datos de contacto.
- Menú principal en bucle y robusto: ninguna entrada no numérica ni opción inexistente
  cierra el programa ni lanza una excepción no controlada.
- Confirmación explícita (s/n) antes de modificar o eliminar un registro, mostrando el
  registro afectado.
- Arquitectura base: `Program.cs` (entrada/menú), `Persona.cs` (modelo), `GestorPersonas.cs`
  (lógica de negocio) y `Validaciones.cs` (validaciones), en `src/AgendaProfesional.csproj`.
- Datos solo en memoria durante la ejecución (se pierden al cerrar el programa).
- Sin cambios **BREAKING**: proyecto nuevo, no hay comportamiento previo.

## Capabilities

### New Capabilities

- `personas`: gestión de personas colaboradoras en la agenda (alta, listado, búsqueda por Id
  y por texto, modificación y baja), con validaciones de correo/teléfono, confirmaciones
  explícitas y un menú de consola en español que nunca falla ante entrada inválida.

### Modified Capabilities

- Ninguna.

## Impact

- Código: nuevo proyecto de consola bajo `src/` (`Program.cs`, `Persona.cs`,
  `GestorPersonas.cs`, `Validaciones.cs`).
- Dependencias: ninguna externa; colecciones estándar de .NET (`List<T>`).
- Sistemas: sin persistencia en esta fase; `EmpresaAsignada` se modela como propiedad (no
  campo público) para poder sustituirse por una referencia a `Empresa` en la Fase 2 sin
  romper el resto del código.