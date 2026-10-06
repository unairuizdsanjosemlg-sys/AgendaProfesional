# Spec Delta

## Purpose

Capacidad de la agenda para mantener el catálogo de empresas colaboradoras desde la
consola: alta, listado, búsqueda, modificación y baja, con validación de CIF (formato y
unicidad), confirmaciones explícitas y un submenu propio que convive con el de personas
sin alterar el comportamiento de la Fase 1.

## ADDED Requirements

### Requirement: Menú de empresas en bucle
El sistema SHALL mostrar un submenu de empresas con opciones numeradas y claras (alta,
listado, búsqueda, modificación, baja y "Volver al menú principal") y SHALL repetirlo en
bucle hasta que el usuario elija "Volver al menú principal". Elegir "Volver al menú
principal" SHALL devolver el control al menú principal sin cerrar la aplicación; solo la
opción "Salir" del menú principal SHALL finalizar el programa. Ante una entrada no
numérica o una opción inexistente del submenu, el sistema SHALL mostrar un aviso y volver
a pedir la opción, sin cerrar el programa ni lanzar excepciones no controladas.

#### Scenario: Ciclo del submenu de empresas
- **WHEN** el usuario termina una operación del submenu distinta de "Volver al menú principal"
- **THEN** el sistema vuelve a mostrar el submenu de empresas

#### Scenario: Volver al menú principal sin salir
- **WHEN** el usuario elige "Volver al menú principal"
- **THEN** el sistema muestra el menú principal sin cerrar la aplicación

#### Scenario: Texto no numérico en el submenu
- **WHEN** el usuario introduce texto no numérico donde se espera una opción del submenu
- **THEN** el sistema muestra un aviso y vuelve a pedir la opción

#### Scenario: Opción inexistente en el submenu
- **WHEN** el usuario elige una opción que no existe en el submenu de empresas
- **THEN** el sistema lo indica y vuelve a mostrar el submenu

### Requirement: Alta de empresa
El sistema SHALL permitir dar de alta una empresa solicitando NombreComercial, CIF,
Teléfono, CorreoElectronico y Dirección; SHALL validar cada dato inmediatamente después
de ser escrito, repreguntando únicamente el campo que falla; SHALL rechazar cualquier
campo vacío; SHALL validar el formato del CIF y exigir que ninguna otra empresa del
catálogo tenga ya ese CIF; SHALL asignar un `IdEmpresa` autogenerado e incremental no
editable por el usuario; SHALL mostrar un resumen de los datos introducidos y pedir
confirmación antes de guardar; y al confirmar SHALL crear la empresa informando del
`IdEmpresa` asignado.

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
registradas en lugar de mostrar una lista vacía.

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
SHALL pedirse nunca como dato de búsqueda.

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

### Requirement: Localización de empresas para modificación y baja
Para modificar o dar de baja una empresa, el sistema SHALL localizarla con la misma búsqueda
por NombreComercial o CIF. Con exactamente una coincidencia la SHALL usar directamente sin
pedir selección; con varias, SHALL mostrar una lista temporal numerada con los datos de
cada candidato y SHALL pedir al usuario el número del que quiere elegir, validando esa
respuesta. Sin coincidencias, SHALL informarlo sin error ni cierre inesperado y volver al
menú.

#### Scenario: Una única coincidencia
- **WHEN** la búsqueda devuelve una sola empresa
- **THEN** el sistema la selecciona directamente sin pedir un número

#### Scenario: Varias coincidencias
- **WHEN** la búsqueda devuelve varias empresas
- **THEN** el sistema muestra la lista numerada de candidatos y espera a que el usuario elija uno

#### Scenario: Selección fuera de rango
- **WHEN** el usuario introduce un número que no está en la lista de candidatos
- **THEN** el sistema lo indica y vuelve a pedir el número, sin repetir la operación

#### Scenario: Localización sin coincidencias en modificación o baja
- **WHEN** el texto introducido no corresponde a ninguna empresa
- **THEN** el sistema informa de que no se ha encontrado ninguna empresa y vuelve al menú
  sin error

#### Scenario: Localización interrumpida con cancelar
- **WHEN** el usuario escribe `cancelar` en el texto de localización o al elegir entre
  varios candidatos
- **THEN** el sistema no modifica ni elimina nada y vuelve al menú principal

### Requirement: Modificación de empresa
El sistema SHALL permitir modificar una empresa localizada previamente; SHALL mostrar los
datos actuales antes de preguntar por los cambios; SHALL pedir cada campo por turnos
(NombreComercial, CIF, Teléfono, CorreoElectronico, Dirección) conservando el valor actual
cuando la respuesta es vacía y revalidando el valor nuevo con las mismas reglas del alta,
incluida la unicidad del CIF; SHALL informar que no hay cambios cuando todos los campos se
dejen vacíos; y SHALL mostrar un resumen de los campos que van a cambiar junto a la empresa
afectada y exigir confirmación explícita (s/n) antes de guardar.

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
antes de eliminar. Al confirmar, la empresa SHALL eliminarse de forma definitiva del
catálogo, sin bloqueos por relaciones (en esta fase no existen relaciones persona-empresa).
Sin confirmación, el registro SHALL mantenerse.

#### Scenario: Baja confirmada
- **WHEN** el usuario localiza una empresa existente y confirma la eliminación
- **THEN** el sistema elimina el registro del catálogo y lo confirma

#### Scenario: Baja cancelada
- **WHEN** el usuario no confirma la eliminación
- **THEN** el sistema mantiene el registro y vuelve al submenu de empresas

#### Scenario: Baja interrumpida con cancelar
- **WHEN** el usuario escribe `cancelar` en la confirmación de la baja
- **THEN** el sistema mantiene el registro y vuelve al menú principal

### Requirement: Validación de datos de empresa
Todos los campos de la empresa (NombreComercial, CIF, Teléfono, CorreoElectronico y
Dirección) SHALL ser obligatorios: en el alta no SHALL aceptarse ninguna respuesta vacía.
El CIF SHALL cumplir el formato de una letra o dígito inicial, 7 dígitos y un dígito o
letra de control, y SHALL ser único en el catálogo (dos empresas distintas no pueden
compartir CIF). El Teléfono y el CorreoElectronico SHALL cumplir las mismas reglas de
formato válidas para las personas. Cada dato SHALL validarse en el momento de escribirse,
indicando la regla incumplida y repreguntando únicamente el campo fallido.

#### Scenario: CIF con formato válido
- **WHEN** el CIF tiene letra o dígito inicial, 7 dígitos y dígito o letra de control
- **THEN** el sistema acepta el CIF

#### Scenario: Teléfono y correo con las reglas de personas
- **WHEN** el teléfono no empieza por `+` con prefijo internacional o el correo no tiene
  `@` con dominio con punto
- **THEN** el sistema rechaza el dato indicando la regla y vuelve a preguntar únicamente
  ese campo

#### Scenario: Reintento de un solo campo
- **WHEN** el usuario escribe un dato de empresa con formato no válido
- **THEN** el sistema explica qué regla ha fallado y vuelve a preguntar solo ese campo,
  sin repetir los ya aceptados
