# Proyecto: Agenda de Consultoría

## Overview

Aplicación de consola en C# para una consultora, que funciona como agenda interna de
personas y empresas colaboradoras. Se desarrolla por fases con Spec Driven Development
(SDD). La **Fase 1** se centra en la gestión de Personas (alta, listado, búsqueda,
modificación y baja); el diseño queda preparado para la Fase 2 (Empresas y relación 1:N).
La **Fase 1.1** refina la Fase 1: las personas se localizan por nombre y apellidos (nunca
por `IdPersona`), la validación es inmediata por campo y la palabra `cancelar` permite
abandonar cualquier operación.

## Aims

- Ofrecer una consola usable con menús en bucle, validaciones de entrada y mensajes
  claros de éxito/error (RA1).
- Gestionar Personas: alta, listado, búsqueda por nombre y apellidos, modificación y baja,
  aplicando colecciones, funciones y clases para un programa mantenible (RA2).
- Prever el modelado de la relación 1 empresa : N personas sin bloquear la Fase 2 (RA3).
- Garantizar robustez: ninguna entrada no numérica ni opción inexistente cierra el
  programa ni lanza excepciones no controladas.

## Non-aims

- Gestionar la entidad `Empresa` como clase en la Fase 1 (alta/baja/listado).
- Validar que la empresa asignada exista contra un catálogo real de empresas.
- Persistir datos en fichero o base de datos (Fase 1 = solo memoria).
- Autenticación, roles o control de acceso.

## Technology

- Lenguaje: **C#** en **.NET**, aplicación de consola.
- Colecciones estándar (`List<T>`), sin librerías externas.
- Interfaz de consola en español.
- Persistencia solo en memoria durante la ejecución (NFR-001).

## Conventions

- Clases y responsabilidades de dominio en español (`Persona`, `GestorPersonas`, `Validaciones`).
- Nombres significativos, sangrado coherente y comentarios solo donde aporten contexto (NFR-002).
- Mensajes de éxito/error en español y claros (NFR-004).
- Escenarios de comportamiento en formato Given/When/Then.
- Entrega por fases: la Fase 1 debe seguir funcionando en las posteriores (NFR-003).
- **Convenciones permanentes de entrada y localización** (definidas con escenarios en la
  capacidad `convenciones-entrada`, obligatorias para todo módulo actual o futuro, por
  ejemplo `Empresas` en la Fase 2):
  - Localizar por nombre legible por personas (Nombre y Apellidos), nunca por identificador
    interno crudo; el `IdPersona` se muestra siempre pero el usuario nunca lo escribe.
  - Coincidencia parcial, sin distinguir acentos ni mayúsculas/minúsculas, con lista
    numerada para desambiguar homónimos.
  - Validación inmediata de cada dato: se repregunta solo el campo que falla, nunca el
    formulario completo.
  - La palabra `cancelar` (sin distinguir mayúsculas) aborta cualquier operación sin
    guardar nada y vuelve al menú principal.
  - Al modificar, un campo en blanco conserva su valor actual; antes de guardar se muestra
    un resumen de cambios y se pide confirmación.
  - Todos los campos son obligatorios salvo que una especificación declare lo contrario.

## Architecture

Estructura de archivos base:

```
/src
  Program.cs        -> punto de entrada, bucle del menú principal
  Persona.cs        -> modelo de datos
  GestorPersonas.cs -> lógica de negocio (alta, listado, búsqueda, modificar, baja)
  Validaciones.cs   -> reglas de dato y lectura de campos con repregunta del campo fallido
```

- `Persona` es el modelo de datos con `IdPersona` autogenerado e incremental (no editable).
- `GestorPersonas` mantiene una `List<Persona>` y un contador `siguienteId`; expone
  `AltaPersona`, `Listar`, `BuscarPorNombre`, `Modificar` y `Eliminar`. `BuscarPorId` queda
  como helper interno: el `IdPersona` se muestra pero nunca se pide al usuario.
- `Modificar` es una actualización parcial: solo se aplican los campos con valor; un campo
  vacío conserva el anterior.
- `EmpresaAsignada` se expone como propiedad (no campo público) para poder sustituirse
  internamente por una referencia a `Empresa` (`IdEmpresa`) en la Fase 2 sin romper el resto.
- `GestorPersonas` no depende de una implementación concreta de almacenamiento, para
  relacionarse después con un futuro `GestorEmpresas`.
- `Validaciones` es la capa reutilizable de entrada: predicado por campo + mensaje de error
  en español, lectura con repregunta de un único campo, palabra reservada `cancelar` y
  confirmaciones s/n. Los módulos futuros deben reutilizarla en lugar de reimplementarla.

## Domain

- **Persona**: colaboradora registrada en la agenda. Campos (todos obligatorios):
  `IdPersona` (autogenerado, no editable, solo se muestra), `Nombre`, `Apellidos`, `Telefono`
  (validado: `+` prefijo de 1-3 dígitos, espacio opcional y 6-12 dígitos), `Correo`
  (validado: no vacío, con `@` y dominio con punto) y `EmpresaAsignada` (texto libre
  obligatorio en Fase 1).
- **Empresa**: entidad prevista para la Fase 2; relación 1 empresa : N personas.
- **GestorPersonas**: lógica de negocio sobre el conjunto de personas.