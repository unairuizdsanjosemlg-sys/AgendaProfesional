# Proposal

## Why

La Fase 1 solo gestiona Personas: la agenda no puede mantener el catálogo de empresas
colaboradoras, que es el otro bloque previsto desde el project.md (Fase 2) y el paso
necesario para preparar la relación 1:N futura. Sin este cambio, cada empresa se sigue
anotando como texto libre dentro de "Empresa asignada" de la persona, sin identidad
propia ni posibilidad de buscarla, modificarla o darla de baja.

## What Changes

- Nueva entidad `Empresa` con `IdEmpresa` autogenerado e incremental, `NombreComercial`,
  `CIF`, `Telefono`, `CorreoElectronico` y `Direccion`.
- Nuevo menú de Empresas (submenu propio en bucle) con alta, listado, búsqueda,
  modificación y baja, accesible desde el menú principal y con opción
  "Volver al menú principal" que no cierra la aplicación.
- El menú principal pasa a ofrecer "Personas", "Empresas" y "Salir"; las operaciones
  sobre personas se agrupan en su propio submenu con "Volver". Solo "Salir" termina
  el programa. **BREAKING** respecto al menú planar de la Fase 1 (las opciones 1-6
  dejan de estar en un único nivel), sin cambio en el comportamiento de cada
  operación de personas.
- Validaciones para empresas: `NombreComercial` y `CIF` obligatorios; `CIF` con
  formato (letra o dígito inicial, 7 dígitos, dígito/letra de control) y único en el
  catálogo; `Telefono` y `CorreoElectronico` obligatorios y con formato, reutilizando
  los validadores existentes de `Validaciones` sin alterar su comportamiento; solo se
  añade un nuevo predicado `ValidarCif`.
- Búsqueda de empresas por subcadena de `NombreComercial` (sin acentos ni
  mayúsculas/minúsculas) y por `CIF` exacto; el `IdEmpresa` se muestra siempre pero
  nunca se teclea, conforme a `convenciones-entrada`.
- Sin persistencia: las empresas, igual que las personas, se mantienen solo en memoria
  durante la ejecución (NFR-001).

## Capabilities

### New Capabilities

- `empresas`: gestión del catálogo de empresas desde consola — alta con resumen y
  confirmación, listado ordenado por `NombreComercial`, búsqueda por nombre o CIF con
  desambiguación de homónimos, modificación con vacío-conserva y resumen de cambios,
  baja con confirmación, validaciones de CIF (formato y unicidad) y tolerancia a
  entradas inválidas en su menú.

### Modified Capabilities

- `personas`: requisito "Menú principal en bucle" — el menú principal deja de ser el
  menú de operaciones de personas y pasa a ser un selector (Personas / Empresas /
  Salir); las operaciones de personas se mueven a un submenu con opción de volver al
  menú principal sin salir de la aplicación, y solo "Salir" finaliza el programa.
  Ningún otro requisito de `personas` cambia.

## Impact

- **Código**: `src/Program.cs` (reestructuración del menú principal y enganche del
  menú de empresas), nuevos `src/Empresa.cs`, `src/GestorEmpresas.cs` y clase de menú
  de empresas; `src/Validaciones.cs` solo recibe el nuevo predicado `ValidarCif`
  (métodos existentes intactos). `Persona.cs`, `GestorPersonas.cs` y las operaciones
  de personas no cambian de comportamiento.
- **Specs**: nueva capacidad `empresas`; delta sobre `personas` (menú). La capacidad
  permanente `convenciones-entrada` se respeta sin modificación: localización por
  nombre legible, validación inmediata, `cancelar`, vacío-conserva y confirmaciones
  se aplican igual en empresas.
- **Dependencias**: ninguna nueva; sigue siendo consola .NET con `List<T>`.
- **Verificación**: regresión manual de los flujos de personas (etiqueta `fase-1`)
  más el recorrido completo de empresas; el proyecto no tiene proyecto de pruebas.
