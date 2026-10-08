# Proposal

## Why

Hoy la relación persona-empresa es solo texto libre escrito por el usuario: no se guarda ningún identificador, no se puede consultar la plantilla de una empresa, una empresa se puede borrar aunque tenga personas asignadas y los datos se desincronizan al renombrar una empresa. Esta change introduce la relación real entre Persona y Empresa para que el programa la gestione internamente por IDs.

## What Changes

- Nueva relación interna persona-empresa almacenada como pares de IDs (`IdPersona`, `IdEmpresa`) en un array; para el usuario siempre se opera con nombres.
- **Asignar / cambiar empresa**: desde el menú de Personas, se localiza a la persona por nombre y apellidos y se elige la empresa escribiendo su nombre (búsqueda con lista de coincidencias y selección por número).
- **Desvincular persona**: la persona queda sin empresa; su campo de texto `Empresa asignada` también se vacía.
- **Ver empresa de una persona**: listados y búsquedas de personas muestran el nombre de la empresa resuelto por ID y, a continuación, su `IdEmpresa`.
- **Ver personas de una empresa**: nueva opción en el menú de Empresas que lista la plantilla de la empresa elegida.
- **Borrado protegido**: no se puede borrar una empresa con personas asociadas; se informa del motivo y se ofrecen dos opciones: cancelar o desvincular a todas las personas para proceder al borrado (tras confirmación).
- El campo `Empresa asignada` del alta y la modificación de personas pasa a ser **opcional** y, cuando se rellena, debe corresponder a una empresa existente (si no, se avisa y se vuelve a pedir hasta validar o dejar en blanco).
- Al eliminar una persona se elimina también su entrada en el array de relaciones.

## Capabilities

### New Capabilities
- `persona-empresa`: relación interna persona-empresa por IDs en array: asignar/cambiar empresa, desvincular, resolución del nombre de empresa para mostrar y protección al borrar empresa con plantilla.

### Modified Capabilities
- `personas`: el campo `Empresa asignada` en alta y modificación pasa de texto libre obligatorio a opcional validado contra el catálogo de empresas; los listados/búsquedas muestran el nombre de la empresa con su ID; el submenu gana las opciones "Asignar/cambiar empresa" y "Desvincular empresa"; la baja elimina la relación.
- `empresas`: el submenu gana la opción "Ver personas de la empresa"; la baja deja de ser libre y queda bloqueada mientras la empresa tenga personas asociadas, con el flujo de cancelar o desvincular-todas-y-borrar.

## Impact

- Código: `src/Persona.cs`, `src/Empresa.cs` (posibles ajustes de representación), `src/GestorPersonas.cs`, `src/GestorEmpresas.cs`, nuevo gestor/clase de relaciones, `src/Program.cs` (menú y operaciones de personas), `src/MenuEmpresas.cs` (nueva opción y borrado protegido), posiblemente `src/Validaciones.cs` (reutilización de ayudas existentes).
- Sin persistencia: los datos siguen solo en memoria (arrays/lists en los gestores), igual que en las fases anteriores.
- Sin cambios de tecnología: consola .NET con C#, sin dependencias nuevas.
- Specs: deltas en `personas` y `empresas`; nueva capacidad `persona-empresa`.
