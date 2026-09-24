# Proyecto: Agenda de Consultoría

## Overview

Aplicación de consola en C# para una consultora, que funciona como agenda interna de
personas y empresas colaboradoras. Se desarrolla por fases con Spec Driven Development
(SDD). La **Fase 1** se centra en la gestión de Personas (alta, listado, búsqueda,
modificación y baja); el diseño queda preparado para la Fase 2 (Empresas y relación 1:N).

## Aims

- Ofrecer una consola usable con menús en bucle, validaciones de entrada y mensajes
  claros de éxito/error (RA1).
- Gestionar Personas: alta, listado, búsqueda (por Id y por texto), modificación y baja,
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

## Architecture

Estructura de archivos base:

```
/src
  Program.cs        -> punto de entrada, bucle del menú principal
  Persona.cs        -> modelo de datos
  GestorPersonas.cs -> lógica de negocio (alta, listado, búsqueda, modificar, baja)
  Validaciones.cs   -> validación de correo, teléfono y entrada numérica
```

- `Persona` es el modelo de datos con `IdPersona` autogenerado e incremental (no editable).
- `GestorPersonas` mantiene una `List<Persona>` y un contador `siguienteId`; expone
  `AltaPersona`, `Listar`, `BuscarPorId`, `BuscarPorTexto`, `Modificar` y `Eliminar`.
- `EmpresaAsignada` se expone como propiedad (no campo público) para poder sustituirse
  internamente por una referencia a `Empresa` (`IdEmpresa`) en la Fase 2 sin romper el resto.
- `GestorPersonas` no depende de una implementación concreta de almacenamiento, para
  relacionarse después con un futuro `GestorEmpresas`.

## Domain

- **Persona**: colaboradora registrada en la agenda. Campos: `IdPersona` (autogenerado),
  `Nombre`, `Apellidos`, `Telefono` (validado: solo dígitos, longitud 9), `Correo`
  (validado: no vacío, con `@` y dominio con punto) y `EmpresaAsignada` (texto libre en Fase 1).
- **Empresa**: entidad prevista para la Fase 2; relación 1 empresa : N personas.
- **GestorPersonas**: lógica de negocio sobre el conjunto de personas.