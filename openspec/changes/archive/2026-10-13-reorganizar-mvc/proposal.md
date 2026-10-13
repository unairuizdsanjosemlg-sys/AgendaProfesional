# Proposal

## Why

La arquitectura actual mezcla responsabilidades de presentación, lógica de negocio y acceso a datos en los gestores (`GestorPersonas`, `GestorEmpresas`, `GestorRelaciones`) y menús. Esto dificulta el mantenimiento, las pruebas y la evolución. Reorganizar en capas MVC (Model-View-Controller / Service) con separación clara de responsabilidades permite mayor modularidad, testeabilidad y escalabilidad.

## What Changes

- **Modelos de dominio** (`Models/`): `Persona` e `Empresa` como entidades puras de datos. `Persona` mantiene `IdEmpresa` opcional (nullable) para la relación. Se conserva `GestorRelaciones` para la integridad referencial y operaciones de relación.
- **Servicios de negocio** (`Services/`): `PersonaService` y `EmpresaService` encapsulan la lógica de negocio (altas, búsquedas, modificaciones, bajas, consultas, asignaciones). Los gestores actuales se convierten en repositorios internos de los servicios.
- **Capa de presentación/UI** (`UI/`): `Consola`, menús (`MenuPrincipal`, `MenuPersonas`, `MenuEmpresas`), `Presentacion` y helpers de entrada. La UI solo solicita datos, muestra resultados y delega en los servicios; no gestiona colecciones directamente.
- **Punto de entrada** (`Program.cs`): Inicializa dependencias (servicios, UI, consola) y arranca la aplicación.
- **Eliminación de código duplicado**: Helpers compartidos (`LocalizarRegistro`, `ResumenCambios`, `ValorIndicadoONuevo`) se mantienen en `UI/Ayudas` o `Services/Shared`.
- **Sin cambios de comportamiento observable**: Misma interfaz de consola, mismos mensajes, mismos flujos (validado con guiones de demostración).

## Capabilities

### New Capabilities

- `arquitectura-mvc`: Define la estructura en capas (Models, Services, UI), responsabilidades de cada capa, reglas de dependencia (UI → Services → Models), y criterios de organización del código.

### Modified Capabilities

- `personas`: Los requisitos de gestión de personas ahora se cumplen a través de `PersonaService` en lugar de `GestorPersonas` directo; la UI delega en el servicio.
- `empresas`: Los requisitos de gestión de empresas ahora se cumplen a través de `EmpresaService` en lugar de `GestorEmpresas` directo.
- `persona-empresa`: La relación se mantiene vía `GestorRelaciones` (interno a los servicios), pero la asignación/desvinculación se expone a través de los servicios.
- `convenciones-entrada`: Se mantiene sin cambios; la capa UI sigue usando `Consola` y validaciones.

## Impact

- **Código afectado**: Reorganización completa de `src/` en carpetas `Models/`, `Services/`, `UI/`, `Program.cs`. Archivos actuales movidos/refactorizados.
- **Nuevos archivos**: `PersonaService.cs`, `EmpresaService.cs`, interfaces de servicio opcionales, estructura de carpetas.
- **Archivos eliminados/refactorizados**: `GestorPersonas.cs`, `GestorEmpresas.cs` → repositorios internos; menús y consola movidos a `UI/`.
- **Sin dependencias externas nuevas**: Solo reorganización interna.
- **Guiones de demostración**: `docs/demo-fase-2.md` y `docs/demo-fase-3.md` siguen siendo válidos (comportamiento idéntico).