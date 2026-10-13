# Tasks

## 1. Estructura de carpetas y Models

- [x] 1.1 Crear carpetas `src/Models`, `src/Services`, `src/Services/Repositories`, `src/Services/Shared`, `src/UI`, `src/UI/Ayudas`; verificar que existen.
- [x] 1.2 Mover `Persona.cs`, `Empresa.cs`, `GestorRelaciones.cs` a `src/Models/`; actualizar namespace a `AgendaProfesional.Models`; verificar `dotnet build`.
- [x] 1.3 Añadir propiedad `public int? IdEmpresa { get; set; }` a `Persona.cs` (nullable) para relación opcional; verificar que compila.

## 2. Repositorios (extraídos de gestores)

- [x] 2.1 Crear `src/Services/Repositories/PersonaRepository.cs` extrayendo almacenamiento (`List<Persona>`, `siguienteId`) y métodos de datos (`Add`, `GetAll`, `GetById`, `FindByNombre`, `Update`, `Delete`) de `GestorPersonas`; sin lógica de negocio ni `GestorRelaciones`; namespace `AgendaProfesional.Services.Repositories`; verificar `dotnet build`.
- [x] 2.2 Crear `src/Services/Repositories/EmpresaRepository.cs` análogo para `Empresa` (incluye `ExisteCif`); namespace `AgendaProfesional.Services.Repositories`; verificar `dotnet build`.
- [x] 2.3 Verificar que `GestorRelaciones.cs` en `Models/` no tiene dependencias hacia `Services` ni `UI`; verificar por inspección.

## 3. Helpers compartidos en Services

- [x] 3.1 Mover `ValorIndicadoONuevo` a `src/Services/Shared/ValorIndicadoONuevo.cs`; actualizar `PersonaRepository` y `EmpresaRepository` para usarlo; verificar `dotnet build`.

## 4. Servicios de negocio

- [x] 4.1 Crear `src/Services/PersonaService.cs` con constructor `(PersonaRepository, EmpresaRepository, GestorRelaciones)` y métodos: `AltaPersona`, `ListarPersonas`, `BuscarPersonasPorNombre`, `ModificarPersona`, `EliminarPersona`, `AsignarEmpresa`, `DesvincularEmpresa`; delega en repositorio y coordina `GestorRelaciones`; namespace `AgendaProfesional.Services`; verificar `dotnet build`.
- [x] 4.2 Crear `src/Services/EmpresaService.cs` con constructor `(EmpresaRepository, PersonaRepository, GestorRelaciones)` y métodos: `AltaEmpresa`, `ListarEmpresas`, `BuscarEmpresasPorTexto`, `ModificarEmpresa`, `EliminarEmpresa`, `VerPlantilla`; delega en repositorio y coordina `GestorRelaciones`/`PersonaRepository` para plantilla y borrado con desvinculación; namespace `AgendaProfesional.Services`; verificar `dotnet build`.
- [x] 4.3 Verificar que ningún servicio usa `Console`, `Consola`, menús ni `Presentacion`; solo reciben parámetros y devuelven resultados; verificar por inspección.

## 5. Capa UI (menús, consola, presentación, helpers)

- [x] 5.1 Mover `Consola.cs`, `Validaciones.cs`, `ResultadoOperacion.cs`, `Presentacion.cs`, `PersonaDatos.cs`, `EmpresaDatos.cs` a `src/UI/`; actualizar namespaces a `AgendaProfesional.UI`; verificar `dotnet build`.
- [x] 5.2 Mover `Ayudas/LocalizarRegistro.cs`, `Ayudas/ResumenCambios.cs` a `src/UI/Ayudas/`; actualizar namespaces; verificar `dotnet build`.
- [x] 5.3 Mover `MenuPrincipal.cs`, `MenuPersonas.cs`, `MenuEmpresas.cs` a `src/UI/`; actualizar constructores para recibir `PersonaService` y `EmpresaService` (en lugar de gestores); adaptar llamadas a métodos de servicio; namespace `AgendaProfesional.UI`; verificar `dotnet build`.
- [x] 5.4 Verificar que la UI no accede a repositorios, `List<T>` ni `GestorRelaciones` directamente; solo llama a servicios; verificar por inspección.

## 6. Composition root (Program.cs)

- [x] 6.1 Reescribir `src/Program.cs` como composition root: instanciar `Consola`, `GestorRelaciones`, repositorios, servicios, menús; llamar a `MenuPrincipal.Ejecutar`; namespace raíz; verificar `dotnet build`.
- [x] 6.2 Verificar que `Program.cs` es el único lugar donde se instancian servicios y repositorios; verificar por inspección.

## 7. Verificación de integración

- [x] 7.1 Ejecutar `dotnet build src/AgendaProfesional.csproj`; verificar 0 errores.
- [x] 7.2 Ejecutar flujo completo de `docs/demo-fase-2.md` comparando prompts, mensajes y formato con versión anterior; verificar salida idéntica.
- [x] 7.3 Ejecutar flujo completo de `docs/demo-fase-3.md` comparando prompts, mensajes y formato; verificar salida idéntica.
- [x] 7.4 Verificar reglas de dependencia: `Models` no referencia `Services`/`UI`; `Services` referencia solo `Models`; `UI` referencia `Services` y `Models` (DTOs); `Program` referencia todas; buscar `using AgendaProfesional.` en cada capa.
- [x] 7.5 Verificar que `GestorRelaciones` se usa solo desde `PersonaService` y `EmpresaService` (no desde UI); buscar referencias.