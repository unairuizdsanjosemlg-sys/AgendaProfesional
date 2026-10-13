# Design

## Context

Ver `proposal.md` — Why. Estado actual tras refactorización anterior:

- `src/` plano con 18 archivos: `Program.cs` (6 líneas), `MenuPrincipal.cs`, `MenuPersonas.cs`,
  `MenuEmpresas.cs`, `Consola.cs`, `Validaciones.cs`, `ResultadoOperacion.cs`,
  `Presentacion.cs`, `PersonaDatos.cs`, `EmpresaDatos.cs`, `Persona.cs`, `Empresa.cs`,
  `GestorPersonas.cs`, `GestorEmpresas.cs`, `GestorRelaciones.cs`, carpeta `Ayudas/` con
  `LocalizarRegistro`, `ResumenCambios`, `ValorIndicadoONuevo`.
- Arquitectura actual: UI (menús + Consola) llama directamente a gestores (`GestorPersonas`,
  `GestorEmpresas`, `GestorRelaciones`). Los gestores contienen lógica de negocio + repositorio
  en memoria. No hay separación Service/Repository.
- Comportamiento validado con `docs/demo-fase-2.md` y `docs/demo-fase-3.md`.

## Goals / Non-Goals

**Goals:**

- Reorganizar en 4 capas: `Models/` (entidades + GestorRelaciones), `Services/`
  (`PersonaService`, `EmpresaService` con repositorios internos), `UI/` (menús, Consola,
  Presentacion, helpers), `Program.cs` (composition root).
- Inversión de dependencias: UI → Services → Models. Ninguna referencia inversa.
- `Persona` expone `int? IdEmpresa` (nullable) para relación opcional.
- `GestorRelaciones` se mantiene como componente interno coordinado por los servicios.
- Preservar comportamiento byte a byte (guiones de demostración intactos).
- Mantener helpers compartidos (`LocalizarRegistro`, `ResumenCambios`, `ValorIndicadoONuevo`)
  accesibles desde UI.

**Non-Goals:**

- Cambiar validaciones, mensajes, flujos de usuario o formato de salida.
- Introducir persistencia real, DI container, tests automatizados, logging.
- Dividir en múltiples proyectos/assemblies (un solo proyecto, carpetas).
- Cambiar `Validaciones` (reglas puras) ni `Consola` (estado de entrada).

## Decisions

### D1. Estructura de carpetas destino

```
src/
  Models/
    Persona.cs
    Empresa.cs
    GestorRelaciones.cs          (interno, coordinado por servicios)
  Services/
    PersonaService.cs
    EmpresaService.cs
    Repositories/
      PersonaRepository.cs       (extraído de GestorPersonas)
      EmpresaRepository.cs       (extraído de GestorEmpresas)
    Shared/
      ValorIndicadoONuevo.cs     (helper "vacío conserva")
  UI/
    Consola.cs
    Presentacion.cs
    PersonaDatos.cs
    EmpresaDatos.cs
    ResultadoOperacion.cs
    Ayudas/
      LocalizarRegistro.cs
      ResumenCambios.cs
    MenuPrincipal.cs
    MenuPersonas.cs
    MenuEmpresas.cs
  Program.cs                     (composition root)
```

*Alternativa considerada:* mantener gestores como servicios públicos. Rechazada: mezcla
repositorio y lógica de negocio; mejor separar `*Repository` (datos) y `*Service` (negocio).

### D2. PersonaService y EmpresaService como fachadas

- `PersonaService` expone: `AltaPersona`, `ListarPersonas`, `BuscarPersonasPorNombre`,
  `ModificarPersona`, `EliminarPersona`, `AsignarEmpresa`, `DesvincularEmpresa`.
- `EmpresaService` expone: `AltaEmpresa`, `ListarEmpresas`, `BuscarEmpresasPorTexto`,
  `ModificarEmpresa`, `EliminarEmpresa`, `VerPlantilla`.
- Cada service instancia su `*Repository` y comparte `GestorRelaciones` (inyectado en
  constructor). Los repositorios son `private` y no se exponen fuera de `Services/`.

### D3. Repositorios internos (equivalentes a gestores actuales)

- `PersonaRepository`: `List<Persona>`, `siguienteId`, métodos `Add`, `GetAll`, `GetById`,
  `FindByNombre`, `Update`, `Delete`. Sin lógica de negocio (validaciones, relaciones).
- `EmpresaRepository`: análogo para `Empresa`. Incluye `ExisteCif` para unicidad.
- `GestorRelaciones`: sin cambios funcionales; usado por ambos services para integridad
  referencial (asignar, desvincular, consultar plantilla, proteger borrado).

### D4. UI sin cambios funcionales, solo reubicación

- `MenuPersonas` y `MenuEmpresas` reciben `PersonaService` y `EmpresaService` en lugar de
  gestores. Lógica de menús, prompts, validaciones, helpers se mantiene idéntica.
- `Consola`, `Validaciones`, `Presentacion`, `PersonaDatos`, `EmpresaDatos`,
  `ResultadoOperacion`, `Ayudas/*` se mueven a `UI/` sin cambios de comportamiento.

### D5. Inyección de dependencias manual en Program.cs

```csharp
var consola = new Consola();
var gestorRelaciones = new GestorRelaciones();
var personaRepo = new PersonaRepository();
var empresaRepo = new EmpresaRepository();
var personaService = new PersonaService(personaRepo, empresaRepo, gestorRelaciones);
var empresaService = new EmpresaService(empresaRepo, personaRepo, gestorRelaciones);
var menuPrincipal = new MenuPrincipal(consola, personaService, empresaService);
menuPrincipal.Ejecutar();
```

Sin contenedor DI externo (overhead innecesario para consola simple).

### D6. Preservación literal de salida

Todos los `Console.WriteLine`, prompts, mensajes, orden de menús se mantienen igual.
Validación: ejecutar guiones `demo-fase-2.md` y `demo-fase-3.md` y comparar salida.

## Risks / Trade-offs

- **[Riesgo] Rotura accidental de flujo al mover código** → Mitigación: mover archivo a
  archivo, compilar y testear con guiones tras cada paso. Commits atómicos.
- **[Riesgo] Dependencias circulares Services ↔ Repositories** → Mitigación: Services
  referencian Repositories; Repositories no conocen Services. `GestorRelaciones` en Models
  conocido por ambos Services.
- **[Riesgo] `GestorRelaciones` en Models pero usado por Services** → Mitigación: es un
  componente de dominio puro (estructura de datos + invariantes), no de presentación.
  Aceptable en Models.
- **[Trade-off] Más archivos y capas** → Aceptado a cambio de separación de responsabilidades,
  testeabilidad futura y mantenibilidad.
- **[Trade-off] Inyección manual vs contenedor DI** → Manual aceptado por simplicidad y
  ausencia de dependencias externas.

## Migration Plan

1. Crear carpetas `Models/`, `Services/Repositories/`, `Services/Shared/`, `UI/Ayudas/`.
2. Mover `Persona.cs`, `Empresa.cs`, `GestorRelaciones.cs` a `Models/`.
3. Extraer `PersonaRepository` de `GestorPersonas` y `EmpresaRepository` de
   `GestorEmpresas` a `Services/Repositories/`.
4. Crear `ValorIndicadoONuevo` en `Services/Shared/`.
5. Crear `PersonaService` y `EmpresaService` en `Services/` inyectando repositorios y
   `GestorRelaciones`.
6. Mover `Consola.cs`, `Validaciones.cs`, `ResultadoOperacion.cs`, `Presentacion.cs`,
   `PersonaDatos.cs`, `EmpresaDatos.cs`, `Ayudas/*` a `UI/`.
7. Mover `MenuPrincipal.cs`, `MenuPersonas.cs`, `MenuEmpresas.cs` a `UI/` y adaptar
   constructores para recibir `PersonaService`/`EmpresaService`.
8. Actualizar `Program.cs` como composition root.
9. Ajustar namespaces (`AgendaProfesional.Models`, `AgendaProfesional.Services`,
   `AgendaProfesional.UI`).
10. Compilar y ejecutar guiones de demostración completos.
11. Rollback: `git revert` del commit de reorganización.

## Open Questions

- Ninguna que afecte a specs, enfoque o desglose de tareas.