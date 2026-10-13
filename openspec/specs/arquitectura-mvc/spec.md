# arquitectura-mvc Specification

## Purpose

Define la arquitectura en capas MVC (Model-View-Controller/Service) para la agenda: separación
clara entre modelos de dominio, servicios de negocio, capa de presentación/UI y punto de
entrada, con reglas de dependencia unidireccional y responsabilidades bien definidas por capa.

## Requirements

### Requirement: Capa de Modelos (Models)

El sistema SHALL definir una capa de modelos que contenga únicamente las entidades de dominio
(`Persona`, `Empresa`) y sus relaciones (`GestorRelaciones`). Los modelos SHALL ser clases puras
de datos sin lógica de negocio, sin acceso a consola y sin dependencias hacia capas superiores.
`Persona` SHALL tener `IdEmpresa` opcional (nullable) para representar la relación.

#### Scenario: Modelos sin dependencias externas
- **WHEN** se revisan las clases `Persona` y `Empresa`
- **THEN** no tienen referencias a `Services`, `UI`, `Consola` ni gestores

#### Scenario: Relación por identificador en Persona
- **WHEN** se examina la clase `Persona`
- **THEN** expone `IdEmpresa` nullable como propiedad para la relación opcional

### Requirement: Capa de Servicios (Services)

El sistema SHALL definir una capa de servicios que encapsule toda la lógica de negocio.
`PersonaService` SHALL exponer operaciones de personas (alta, listado, búsqueda, modificación,
baja, asignar/desvincular empresa). `EmpresaService` SHALL exponer operaciones de empresas
(alta, listado, búsqueda, modificación, baja, ver plantilla). Los servicios SHALL usar
repositorios internos (equivalentes a los gestores actuales) para persistencia en memoria y
SHALL coordinar `GestorRelaciones` para integridad referencial. Los servicios SHALL NOT
interactuar con la consola ni contener lógica de presentación.

#### Scenario: PersonaService expone operaciones de personas
- **WHEN** la UI necesita gestionar personas
- **THEN** llama a `PersonaService.Alta`, `Listar`, `BuscarPorNombre`, `Modificar`, `Eliminar`,
  `AsignarEmpresa`, `DesvincularEmpresa`

#### Scenario: EmpresaService expone operaciones de empresas
- **WHEN** la UI necesita gestionar empresas
- **THEN** llama a `EmpresaService.Alta`, `Listar`, `BuscarPorTexto`, `Modificar`, `Eliminar`,
  `VerPlantilla`

#### Scenario: Servicios sin I/O de consola
- **WHEN** se revisan `PersonaService` y `EmpresaService`
- **THEN** no contienen `Console.WriteLine`, `Console.ReadLine` ni referencias a `Consola`

#### Scenario: Servicios coordinan GestorRelaciones
- **WHEN** `PersonaService` asigna o desvincula empresa
- **THEN** usa `GestorRelaciones` internamente para mantener integridad referencial

### Requirement: Capa de Presentación/UI (UI)

El sistema SHALL definir una capa UI que contenga toda la interacción con el usuario:
`Consola` (lectura/validación), menús (`MenuPrincipal`, `MenuPersonas`, `MenuEmpresas`),
`Presentacion` (formato de salida) y helpers (`LocalizarRegistro`, `ResumenCambios`,
`ValorIndicadoONuevo`). La UI SHALL delegar toda lógica de negocio en los servicios y SHALL
NOT gestionar colecciones ni repositorios directamente.

#### Scenario: UI delega en servicios
- **WHEN** el usuario elige una operación en el menú
- **THEN** la UI recopila datos, llama al servicio correspondiente y muestra el resultado

#### Scenario: UI sin gestión de colecciones
- **WHEN** se revisan menús y `Consola`
- **THEN** no acceden a `List<Persona>`, `List<Empresa>` ni a gestores directamente

### Requirement: Punto de entrada (Program)

El sistema SHALL tener un único punto de entrada `Program.cs` que inicialice las dependencias
(servicios, UI, consola, repositorios, `GestorRelaciones`) y arranque el bucle principal
delegando en `MenuPrincipal`. `Program.cs` SHALL ser la única clase que conoce la composición
completa del objeto grafo.

#### Scenario: Program inicializa y arranca
- **WHEN** se ejecuta la aplicación
- **THEN** `Program.cs` crea servicios, UI, consola y llama a `MenuPrincipal.Ejecutar`

#### Scenario: Program es el único composition root
- **WHEN** se busca instanciación de servicios y repositorios
- **THN** solo ocurre en `Program.cs`

### Requirement: Reglas de dependencia entre capas

Las dependencias SHALL fluir únicamente en una dirección: `UI` → `Services` → `Models`.
`Models` no depende de nada. `Services` depende solo de `Models`. `UI` depende de `Services`
y `Models` (para DTOs/entidades). `Program` depende de todas para composición. Queda
prohibido cualquier referencia inversa o cíclica.

#### Scenario: Sin dependencias inversas
- **WHEN** se analiza el grafo de dependencias
- **THEN** no hay referencias de `Services` a `UI`, ni de `Models` a `Services` o `UI`

### Requirement: Preservación del comportamiento observable

La reorganización arquitectónica SHALL NOT alterar ningún comportamiento observable: menús,
prompts, validaciones, mensajes, formato de salida y flujos de usuario SHALL permanecer
idénticos. Los guiones de demostración `docs/demo-fase-2.md` y `docs/demo-fase-3.md`
SHALL seguir ejecutándose sin cambios.

#### Scenario: Comportamiento idéntico tras reorganización
- **WHEN** se ejecuta cualquier guion de demostración
- **THEN** la secuencia de interacción coincide exactamente con la versión anterior