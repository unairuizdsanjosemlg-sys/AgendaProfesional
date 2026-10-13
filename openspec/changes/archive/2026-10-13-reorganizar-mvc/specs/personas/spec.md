# Spec Delta

## MODIFIED Requirements

### Requirement: Alta de persona

El sistema SHALL permitir dar de alta una persona solicitando Nombre, Apellidos, Teléfono,
Correo y Empresa asignada; SHALL validar cada dato inmediatamente después de ser escrito,
repreguntando únicamente el campo que falla; SHALL rechazar cualquier campo vacío excepto la
empresa asignada, que es opcional y declara explícitamente la posibilidad de quedarse sin
empresa; cuando la empresa asignada se rellene, SHALL corresponder al nombre de una empresa
existente del catálogo (con la misma coincidencia tolerante y desambiguación que la búsqueda
de empresas), repreguntando el campo con un aviso mientras no sea así o hasta que se deje en
blanco; SHALL asignar un `IdPersona` autogenerado e incremental no editable por el usuario;
y SHALL confirmar el alta al usuario. **La operación se ejecuta a través de `PersonaService`
que coordina internamente el repositorio de personas y `GestorRelaciones`; la UI no accede
directamente al gestor.**

#### Scenario: Alta válida
- **WHEN** el usuario introduce todos los datos obligatorios en formato válido y confirma
- **THEN** el sistema crea la persona con un `IdPersona` autogenerado y confirma el alta

#### Scenario: Alta con campos vacíos
- **WHEN** el usuario deja vacío un campo obligatorio (Nombre, Apellidos, Teléfono o Correo)
- **THEN** el sistema no avanza, repregunta únicamente ese campo e indica cuál es

#### Scenario: Alta con formato inválido
- **WHEN** el correo o el teléfono tienen un formato no válido
- **THEN** el sistema no guarda nada, indica qué regla ha fallado en ese campo y vuelve a
  preguntar solo ese campo, sin repetir el resto del formulario

#### Scenario: Empresa asignada obligatoria
- **WHEN** el usuario rellena el campo de empresa asignada en el alta
- **THEN** el sistema la valida obligatoriamente contra el catálogo de empresas y no da el
  alta hasta que corresponda a una empresa existente o el campo se deje en blanco

### Requirement: Listado de personas

El sistema SHALL mostrar el listado de las personas registradas en orden legible y SHALL
indicar explícitamente cuando no hay personas registradas en lugar de mostrar una lista
vacía. **La UI obtiene la lista mediante `PersonaService.Listar()`.**

#### Scenario: Listado con personas registradas
- **WHEN** hay personas registradas
- **THEN** el sistema las muestra ordenadas y con formato legible en consola

#### Scenario: Listado sin personas
- **WHEN** no hay personas registradas
- **THEN** el sistema muestra un mensaje claro indicando que no hay personas

### Requirement: Modificación de persona

El sistema SHALL permitir corregir los datos de una persona localizándola por su nombre y
apellidos; SHALL mostrar los datos actuales antes de preguntar por los cambios; SHALL pedir
cada campo por turnos (Nombre, Apellidos, Teléfono, Correo, Empresa asignada) conservando el
valor actual cuando la respuesta es vacía y modificando únicamente los campos con respuesta
no vacía; SHALL informar y volver al menú sin error cuando no hay ninguna coincidencia o
cuando el usuario no elige entre varios candidatos; SHALL informar que no hay cambios cuando
todos los campos se dejan vacíos; y SHALL mostrar un resumen de los campos que van a cambiar
y exigir confirmación explícita (s/n) antes de guardar. **La operación se ejecuta a través
de `PersonaService.Modificar()`.**

#### Scenario: Modificar persona existente
- **WHEN** el usuario localiza por nombre y apellidos una persona existente, escribe al menos
  un campo nuevo y confirma el resumen de cambios
- **THEN** el sistema guarda solo los campos indicados y el resto conserva su valor anterior

#### Scenario: Campo vacío conserva el valor
- **WHEN** el usuario deja vacío uno de los campos al modificar
- **THEN** el sistema conserva el valor actual de ese campo y no lo sustituye por una cadena vacía

#### Scenario: Todos los campos vacíos
- **WHEN** el usuario deja vacíos todos los campos al modificar
- **THEN** el sistema informa que no hay ningún cambio y vuelve al menú sin guardar ni pedir confirmación

#### Scenario: Modificación sin coincidencias
- **WHEN** el nombre y los apellidos introducidos no coinciden con ninguna persona
- **THEN** el sistema lo informa y vuelve al menú sin error

#### Scenario: Modificación cancelada en la confirmación
- **WHEN** el usuario responde que no a la confirmación del resumen de cambios
- **THEN** el sistema no guarda ningún cambio y vuelve al menú

### Requirement: Baja de persona

El sistema SHALL permitir eliminar una persona localizándola por su nombre y apellidos; SHALL
mostrar el registro afectado, incluido su `IdPersona`; y SHALL exigir confirmación explícita
(s/n) antes de eliminar. Al confirmar, el sistema SHALL eliminar también la entrada de
relación persona-empresa de esa persona si existe, de modo que no queden referencias
huérfanas. Cuando no haya coincidencia, SHALL informarlo sin error ni cierre inesperado y
volver al menú. **La operación se ejecuta a través de `PersonaService.Eliminar()` que
coordina con `GestorRelaciones`.**

#### Scenario: Baja confirmada
- **WHEN** el usuario localiza por nombre y apellidos una persona existente y confirma la eliminación
- **THEN** el sistema elimina el registro y lo confirma

#### Scenario: Baja cancelada
- **WHEN** el usuario no confirma la eliminación
- **THEN** el sistema mantiene el registro y vuelve al menú

#### Scenario: Baja sin coincidencias
- **WHEN** el nombre y los apellidos introducidos no coinciden con ninguna persona
- **THEN** el sistema lo informa sin error ni cierre inesperado y vuelve al menú

#### Scenario: Baja de persona con empresa
- **WHEN** el usuario confirma la baja de una persona que tiene empresa asignada
- **THEN** el sistema elimina la persona y también su entrada en la relación con empresas

### Requirement: Búsqueda por nombre y apellidos

El sistema SHALL localizar personas pidiendo el nombre y los apellidos en dos preguntas
separadas. La coincidencia SHALL ser parcial (subcadena), SHALL ignorar mayúsculas/minúsculas
y SHALL ignorar acentos tanto en lo tecleado como en lo almacenado. Una persona SHALL
coincidir solo cuando el nombre **y** los apellidos coincidan (condición AND). La búsqueda
SHALL mostrar todas las coincidencias y SHALL informar explícitamente cuando no haya ninguna.
**La búsqueda se realiza mediante `PersonaService.BuscarPorNombre()`.**

#### Scenario: Coincidencia por subcadena
- **WHEN** el usuario introduce un fragmento del nombre y un fragmento de los apellidos que
  corresponden a una persona registrada
- **THEN** el sistema la incluye entre las coincidencias

#### Scenario: Coincidencia sin distinguir acentos
- **WHEN** el usuario escribe el nombre y los apellidos sin acentos y la persona registrada los
  tiene con acentos
- **THEN** el sistema la incluye entre las coincidencias

#### Scenario: Coincidencia sin distinguir mayúsculas
- **WHEN** el usuario escribe el nombre y los apellidos en minúsculas y la persona registrada los
  tiene en mayúsculas o con mayúscula inicial
- **THEN** el sistema la incluye entre las coincidencias

#### Scenario: Una coincidencia exige ambos campos
- **WHEN** el nombre introducido coincide con una persona pero los apellidos no
- **THEN** esa persona no aparece entre las coincidencias

#### Scenario: Varias coincidencias
- **WHEN** el nombre y los apellidos introducidos coinciden con varias personas
- **THEN** el sistema las muestra todas, cada una con su `IdPersona` visible

#### Scenario: Sin coincidencias
- **WHEN** no hay ninguna persona cuyo nombre y apellidos coincidan con lo introducido
- **THEN** el sistema lo indica explícitamente y vuelve al menú sin error

### Requirement: Empresa asignada

El sistema SHALL tratar la empresa asignada a una persona como un campo opcional cuya
opcionalidad se declara explícitamente, validado contra el catálogo de empresas. En el alta,
una empresa asignada vacía SHALL aceptarse y la persona quedará sin empresa; una empresa
asignada no vacía SHALL corresponder al nombre de una empresa existente del catálogo, con
coincidencia tolerante (subcadena, sin distinguir acentos ni mayúsculas/minúsculas), y con
desambiguación por lista numerada cuando coincida con varias; mientras el nombre no
corresponda a ninguna empresa existente, el sistema SHALL mostrar un aviso y volver a
preguntar únicamente ese campo. En la modificación, una empresa asignada vacía SHALL
conservar el valor actual (y su relación) en lugar de dejarlo en blanco, y una empresa no
vacía SHALL seguir las mismas reglas de existencia y desambiguación que en el alta,
sustituyendo la relación existente al validarse. Para quitar la empresa de una persona, el
usuario SHALL usar la operación "Desvincular empresa" del submenu de personas. **La
asignación/desvinculación se realiza mediante `PersonaService.AsignarEmpresa` y
`PersonaService.DesvincularEmpresa` que coordinan con `GestorRelaciones`.**

#### Scenario: Empresa obligatoria en el alta
- **WHEN** el usuario rellena el campo de empresa en el alta
- **THEN** el sistema valida obligatoriamente que corresponda a una empresa existente del
  catálogo antes de dar el alta

#### Scenario: Empresa obligatoria en la modificación
- **WHEN** el usuario deja vacío el campo de empresa al modificar
- **THEN** el sistema conserva la empresa asignada actual y su relación

#### Scenario: Texto libre sin catálogo
- **WHEN** el usuario intenta guardar un texto libre que no corresponde a ninguna empresa
  del catálogo en el alta o en la modificación
- **THEN** el sistema muestra un aviso y vuelve a preguntar únicamente la empresa asignada

#### Scenario: Alta sin empresa
- **WHEN** el usuario deja vacío el campo de empresa en el alta
- **THEN** el sistema acepta la alta y la persona queda sin empresa asignada

#### Scenario: Varias coincidencias en el campo
- **WHEN** el nombre introducido coincide con varias empresas del catálogo
- **THEN** el sistema muestra la lista numerada de candidatos con su `IdEmpresa` y pide el número elegido

#### Scenario: Relación por identificadores al validar
- **WHEN** el usuario introduce un nombre de empresa existente y el campo se valida
- **THEN** el sistema acepta el texto, lo guarda en la persona y registra la relación por identificadores

#### Scenario: Desvincular para quitar la empresa
- **WHEN** el usuario quiere quitar la empresa de una persona
- **THEN** el sistema no lo permite en el formulario de modificación y lo resuelve mediante
  la operación "Desvincular empresa" del submenu de personas