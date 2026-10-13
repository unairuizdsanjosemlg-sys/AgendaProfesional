# Spec Delta

## MODIFIED Requirements

### Requirement: Alta de empresa

El sistema SHALL permitir dar de alta una empresa solicitando NombreComercial, CIF,
Teléfono, CorreoElectronico y Dirección; SHALL validar cada dato inmediatamente después
de ser escrito, repreguntando únicamente el campo que falla; SHALL rechazar cualquier
campo vacío; SHALL validar el formato del CIF y exigir que ninguna otra empresa del
catálogo tenga ya ese CIF; SHALL asignar un `IdEmpresa` autogenerado e incremental no
editable por el usuario; SHALL mostrar un resumen de los datos introducidos y pedir
confirmación antes de guardar; y al confirmar SHALL crear la empresa informando del
`IdEmpresa` asignado. **La operación se ejecuta a través de `EmpresaService.Alta()` que
usa el repositorio interno de empresas.**

#### Scenario: Alta válida
- **WHEN** el usuario introduce todos los datos en formato válido, sin duplicar ningún CIF,
  y confirma el resumen
- **THEN** el sistema crea la empresa con un `IdEmpresa` autogenerado y confirma el alta
  informando del id asignado

#### Scenario: NombreComercial vacío
- **WHEN** el usuario deja el NombreComercial vacío o en blanco
- **THEN** el sistema no avanza, indica que el campo es obligatorio y vuelve a preguntar
  únicamente el NombreComercial

#### Scenario: CIF vacío
- **WHEN** el usuario deja el CIF vacío o en blanco
- **THEN** el sistema no avanza, indica que el campo es obligatorio y vuelve a preguntar
  únicamente el CIF

#### Scenario: CIF con formato no válido
- **WHEN** el CIF no cumple el formato (letra o dígito inicial, 7 dígitos y dígito o letra
  de control)
- **THEN** el sistema indica la regla incumplida y vuelve a preguntar únicamente el CIF

#### Scenario: CIF repetido
- **WHEN** el usuario introduce un CIF que ya pertenece a otra empresa del catálogo
- **THEN** el sistema indica que ese CIF ya está registrado y vuelve a preguntar únicamente el CIF

#### Scenario: Teléfono o correo no válidos
- **WHEN** el teléfono o el correo tienen un formato no válido o están vacíos
- **THEN** el sistema no guarda nada, indica qué regla ha fallado en ese campo y vuelve a
  preguntar solo ese campo, sin repetir el resto del formulario

#### Scenario: Dirección vacía
- **WHEN** el usuario deja la Dirección vacía o en blanco
- **THEN** el sistema no avanza, indica que el campo es obligatorio y vuelve a preguntar
  únicamente la Dirección

#### Scenario: Alta cancelada en la confirmación
- **WHEN** el usuario responde "n" en el resumen del alta
- **THEN** el sistema no crea ninguna empresa y vuelve al submenu de empresas

#### Scenario: Alta interrumpida con cancelar
- **WHEN** el usuario escribe `cancelar` en cualquier campo del alta o en su confirmación
- **THEN** el sistema no crea ninguna empresa y vuelve al menú principal

### Requirement: Listado de empresas

El sistema SHALL mostrar el listado de empresas ordenado por `NombreComercial` sin
distinguir mayúsculas/minúsculas, con un formato alineado y legible en consola que incluya
el `IdEmpresa` de cada registro, e SHALL indicar explícitamente cuando no haya empresas
registradas en lugar de mostrar una lista vacía. **La UI obtiene la lista mediante
`EmpresaService.Listar()`.**

#### Scenario: Listado con empresas registradas
- **WHEN** hay empresas registradas
- **THEN** el sistema las muestra ordenadas por NombreComercial, alineadas y con su IdEmpresa visible

#### Scenario: Listado sin empresas
- **WHEN** no hay empresas registradas
- **THEN** el sistema muestra un mensaje claro indicando que no hay empresas

### Requirement: Búsqueda de empresas

El sistema SHALL buscar empresas con un único texto que coincide con una empresa cuando es
una subcadena del `NombreComercial` (sin distinguir acentos ni mayúsculas/minúsculas, tanto
lo tecleado como lo almacenado) o cuando coincide exactamente con su `CIF` (sin distinguir
mayúsculas/minúsculas). La búsqueda SHALL mostrar todas las coincidencias con sus datos y
`IdEmpresa`, y SHALL informar explícitamente cuando no haya ninguna. El `IdEmpresa` no
SHALL pedirse nunca como dato de búsqueda. **La búsqueda se realiza mediante
`EmpresaService.BuscarPorTexto()`.**

#### Scenario: Coincidencia por subcadena del nombre
- **WHEN** el usuario teclea un fragmento del NombreComercial, sin acentos y en minúsculas
  si el almacenado los lleva
- **THEN** el sistema incluye entre las coincidencias a todas las empresas cuyo
  NombreComercial contiene ese fragmento

#### Scenario: Coincidencia por CIF
- **WHEN** el usuario teclea el CIF completo de una empresa
- **THEN** el sistema incluye esa empresa entre las coincidencias

#### Scenario: Sin coincidencias
- **WHEN** el texto buscado no coincide con ninguna empresa por nombre ni por CIF
- **THEN** el sistema lo indica explícitamente y vuelve al menú sin error

#### Scenario: La búsqueda no pide el identificador
- **WHEN** el usuario realiza una búsqueda de empresas
- **THEN** el sistema pide el texto de búsqueda y no solicita en ningún caso el IdEmpresa

### Requirement: Modificación de empresa

El sistema SHALL permitir modificar una empresa localizada previamente; SHALL mostrar los
datos actuales antes de preguntar por los cambios; SHALL pedir cada campo por turnos
(NombreComercial, CIF, Teléfono, CorreoElectronico, Dirección) conservando el valor actual
cuando la respuesta es vacía y revalidando el valor nuevo con las mismas reglas del alta,
incluida la unicidad del CIF; SHALL informar que no hay cambios cuando todos los campos se
dejen vacíos; y SHALL mostrar un resumen de los campos que van a cambiar junto a la empresa
afectada y exigir confirmación explícita (s/n) antes de guardar. **La operación se ejecuta
a través de `EmpresaService.Modificar()`.**

#### Scenario: Modificar empresa existente
- **WHEN** el usuario localiza una empresa existente, escribe al menos un campo nuevo y
  confirma el resumen de cambios
- **THEN** el sistema guarda solo los campos indicados y el resto conserva su valor anterior

#### Scenario: Campo vacío conserva el valor
- **WHEN** el usuario deja vacío uno de los campos al modificar
- **THEN** el sistema conserva el valor actual de ese campo y no lo sustituye por una cadena vacía

#### Scenario: Todos los campos vacíos
- **WHEN** el usuario deja vacíos todos los campos al modificar
- **THEN** el sistema informa que no hay ningún cambio y vuelve al menú sin guardar ni pedir confirmación

#### Scenario: CIF inválido o repetido al modificar
- **WHEN** el usuario escribe un CIF con formato no válido o perteneciente a otra empresa
- **THEN** el sistema indica la regla incumplida y vuelve a preguntar únicamente el CIF

#### Scenario: Modificación rechazada en la confirmación
- **WHEN** el usuario responde "n" en la confirmación del resumen
- **THEN** el sistema no guarda ningún cambio y vuelve al submenu de empresas

#### Scenario: Modificación interrumpida con cancelar
- **WHEN** el usuario escribe `cancelar` en la confirmación del resumen
- **THEN** el sistema no guarda ningún cambio y vuelve al menú principal

### Requirement: Baja de empresa

El sistema SHALL permitir eliminar una empresa localizándola previamente; SHALL mostrar el
registro afectado, incluido su `IdEmpresa`; y SHALL exigir confirmación explícita (s/n)
antes de eliminar. Cuando la empresa no tenga personas asociadas, al confirmar SHALL
eliminarse de forma definitiva del catálogo y, sin confirmación, el registro SHALL
mantenerse. Cuando la empresa sí tenga personas asociadas, el sistema NO la SHALL eliminar:
SHALL informar de que tiene personas relacionadas y que por eso no se puede borrar, SHALL
indicar que la forma de borrarla es desvincular primero a todas las personas, SHALL mostrar
el número de personas afectadas y SHALL ofrecer dos opciones: cancelar el borrado o
desvincular a todas las personas para proceder al borrado. Si elige desvincular, SHALL exigir
confirmación explícita (s/n) y, al confirmar, SHALL desvincular a todas esas personas
(vaciando también su texto `Empresa asignada`) y borrar la empresa en el mismo acto. La
palabra `cancelar` SHALL abortar la operación sin cambios y volver al menú principal. **La
operación se ejecuta a través de `EmpresaService.Eliminar()` que coordina con
`GestorRelaciones` y `PersonaService` para desvincular personas.**

#### Scenario: Baja confirmada
- **WHEN** el usuario localiza una empresa sin personas asociadas y confirma la eliminación
- **THEN** el sistema elimina el registro del catálogo y lo confirma

#### Scenario: Baja cancelada
- **WHEN** el usuario no confirma la eliminación
- **THEN** el sistema mantiene el registro y vuelve al submenu de empresas

#### Scenario: Baja interrumpida con cancelar
- **WHEN** el usuario escribe `cancelar` en la confirmación de la baja
- **THEN** el sistema mantiene el registro y vuelve al menú principal

#### Scenario: Baja bloqueada por personas asociadas
- **WHEN** el usuario confirma la baja de una empresa que tiene personas asociadas
- **THEN** el sistema no la borra, informa del bloqueo, muestra el número de personas
  afectadas y ofrece cancelar el borrado o desvincular a todas para poder borrarla

#### Scenario: Borrado cancelado por plantilla
- **WHEN** el usuario elige cancelar el borrado de una empresa con personas
- **THEN** la empresa y sus relaciones siguen intactas y el sistema vuelve al submenu

#### Scenario: Desvinculación total y borrado
- **WHEN** el usuario elige desvincular a todas las personas y confirma con "s"
- **THEN** el sistema desvincula a todas las personas de la empresa, vacía su texto de
  empresa, borra la empresa y lo confirma