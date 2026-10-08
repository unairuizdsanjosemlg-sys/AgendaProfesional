# Design

## Context

La agenda es una consola .NET en C# sin persistencia: `GestorPersonas` y `GestorEmpresas`
mantienen `List<T>` en memoria, `Program.cs` contiene el menú principal y el submenu de
personas como funciones locales, y `MenuEmpresas.cs` es una clase estática con su submenu.
`Persona` guarda hoy un campo de texto libre `EmpresaAsignada` y no existe ninguna estructura
que relacione `IdPersona` con `IdEmpresa`. Las convenciones de entrada
(`openspec/specs/convenciones-entrada`) obligan a localizar por nombre legible, desambiguar
homónimos con lista numerada, validar campo a campo, tratar `cancelar` y pedir confirmación
antes de cualquier modificación. Ver proposal.md - Why y los deltas de esta change para el
comportamiento exigido.

## Goals / Non-Goals

**Goals:**
- Almacenar la relación persona-empresa por IDs en un array gestionado por una única clase.
- Reutilizar los flujos de localización, desambiguación, confirmación y cancelación ya
  existentes para las nuevas operaciones, sin reinventar convenciones.
- Mantener el programa en memoria y sin dependencias nuevas.

**Non-Goals:**
- Persistir datos a disco o base de datos.
- Permitir varias empresas por persona o historial de empresas.
- Refactorizar el submenu de personas a una clase propia (se mantiene en `Program.cs` por
  la estructura actual de funciones locales).
- Renombrar o migrar el campo de texto `EmpresaAsignada` (se conserva, como se decidió).

## Decisions

### D1. Clase `GestorRelaciones` con array de pares de IDs
La relación vive en una clase nueva `GestorRelaciones` con un array `RelacionAsignacion[]`
(elementos `record struct (int IdPersona, int IdEmpresa)`) y capacidad ampliable, encapsulando
alta/baja/búsqueda de pares. El usuario pidió literalmente un array; se descarta `List<T>`
(más idiomática pero no respeta el enunciado) y `Dictionary<int,int>` (oculta el par y no
expresa "una persona, una empresa" tan claro). Operaciones expuestas:
- `Asignar(idPersona, idEmpresa)` → crea o sustituye la entrada de la persona.
- `Desvincular(idPersona)` → bool (false si no tenía relación).
- `IdEmpresaDe(idPersona)` → `int?`.
- `TienePersonas(idEmpresa)` → bool; `PersonasDe(idEmpresa)` → `List<int>`.
- `DesvincularTodas(idEmpresa)` → int (número de personas desvinculadas).
- `EliminarRelacionesDe(idPersona)` → limpieza al dar de baja una persona.

### D2. Reparto de responsabilidades entre gestores
`GestorRelaciones` no conoce objetos `Persona` ni `Empresa`: solo IDs. La resolución de
nombres se hace en la capa de presentación consultando `GestorEmpresas` y `GestorPersonas`,
a los que se les hará público `BuscarPorId` (hoy `private`). Alternativa descartada: que el
relaciones devuelva objetos directamente (acoplaría los tres gestores y duplicaría el
concepto de "listado").

### D3. Presentación con nombre resuelto
Todas las presentaciones de personas (`PersonaTexto` en `Program.cs` y el formato del
listado) pasarán a recibir también `GestorRelaciones` y `GestorEmpresas` para mostrar
`<NombreComercial> (Id: <IdEmpresa>)` o `Sin empresa`. El texto libre `EmpresaAsignada` no
se muestra como empresa de la persona cuando existe relación. Alternativa descartada:
guardar el nombre resuelto en la persona al momento de asignar (quedaría obsoleto al
renombrar la empresa, que es exactamente el problema que se quiere eliminar).

### D4. Selector de empresa reutilizable
Un único helper (en la parte de personas de `Program.cs`) encapsula "pedir nombre →
`BuscarPorTexto` → 0 coincidencias: aviso; 1: usarla; N: lista numerada con `IdEmpresa` y
`LeerNumeroCancelable`". Se usa desde:
- la operación "Asignar/cambiar empresa" (con confirmación final s/n);
- el campo "Empresa asignada" de alta (vacío aceptado = sin empresa) y de modificación
  (vacío = conserva), como bucle propio en lugar del validador genérico de `CamposPersona()`,
  porque el validador no puede mostrar la lista de desambiguación de forma limpia.

### D5. Nuevos menús
Submenu de personas (renumerado conservando 1-5): `6. Asignar/cambiar empresa`,
`7. Desvincular empresa`, `8. Volver al menú principal`. Submenu de empresas: `6. Ver
personas de la empresa`, `7. Volver al menú principal`. Las dos operaciones de personas
reutilizan `LocalizarPersona` (nombre y apellidos) tal como modificación y baja.

### D6. Baja de empresa protegida
Flujo: localizar → mostrar registro → `ConfirmarCancelable`; si responde "S" y
`TienePersonas` → mostrar aviso con el número de personas, ofrecer dos opciones con
`LeerNumeroCancelable(1, 2)` (1 = cancelar el borrado, 2 = desvincular todas y borrar); la
opción 2 pide además `ConfirmarCancelable` y, al confirmar, `DesvincularTodas` (vaciando el
texto de cada persona vía `GestorPersonas.Modificar` o un método equivalente) + `Eliminar`
de la empresa en el mismo acto. Si no tiene personas, la baja sigue el flujo actual.
`cancelar` en cualquier punto vuelve al menú principal sin cambios.

### D7. Alta y modificación de persona sincronizan texto y relación
El alta resuelve primero la empresa (D4), da de alta la persona y después `Asignar` si
hubo empresa. La modificación, tras la confirmación del resumen, aplica los campos con
`GestorPersonas.Modificar` y, si el usuario indicó empresa, `Asignar` con el ID elegido
(sustituye la relación anterior). La baja de persona llama a `EliminarRelacionesDe`.
Asignar/desvincular actualizan también el texto `EmpresaAsignada` para que texto y array
nunca diverjan.

## Risks / Trade-offs

- [Texto libre y array pueden divergir si algún flujo olvida uno de los dos] → Toda
  operación que toca la relación (alta con empresa, modificar empresa, desvincular,
  desvinculación total) pasa por un único punto que actualiza ambos; la presentación lee
  solo la relación.
- [Quedar referencias huérfanas en el array] → Baja de persona elimina su entrada; borrado
  de empresa exige desvincular todas antes; `PersonasDe` filtra contra personas que existen
  como red de seguridad.
- [La interpretación de `convenciones-entrada` "no ofrecer mecanismos especiales para
  vaciar un valor" podría chocar con "Desvincular empresa"] → La regla está redactada "en
  la modificación" (desvincular es una operación propia, no el formulario de modificación) y
  la opcionalidad del campo se declara explícitamente en la spec de `personas`, como esa
  misma convención exige; no se modifica `convenciones-entrada`.
- [Interacción con la consola dentro de la validación del campo empresa (lista de
  desambiguación)] → El campo empresa deja de usar el validador genérico y tiene su propio
  bucle de lectura (D4), manteniendo el patrón de repreguntar solo ese campo.
- [Program.cs crece con las dos operaciones nuevas] → Aceptado por coherencia con la
  estructura actual (las funciones de personas ya viven ahí); un `MenuPersonas` en clase
  propia queda fuera de esta change.

## Migration Plan

No aplica: los datos viven solo en memoria y no hay formatos ni esquemas que migrar. El
cambio se desplegaría reconstruyendo con `dotnet build`; la reversión es volver al commit
anterior.

## Open Questions

Ninguna. Todas las decisiones con impacto observable (campo opcional, selección por
lista, desambiguación en alta, comportamiento de la baja de empresa, desvinculación de la
baja de persona) se resolvieron con el usuario antes de generar esta propuesta.
