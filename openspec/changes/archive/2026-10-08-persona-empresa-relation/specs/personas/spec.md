# Spec Delta

## MODIFIED Requirements

### Requirement: Menú principal en bucle
El sistema SHALL mostrar un menú principal con opciones numeradas y claras ("Personas",
"Empresas" y "Salir") y SHALL repetirlo en bucle hasta que el usuario elija "Salir". Elegir
"Personas" SHALL abrir un submenu con las operaciones sobre personas (alta, listado,
búsqueda, modificación, baja, asignar o cambiar empresa, desvincular empresa y "Volver al
menú principal"), el cual SHALL permanecer en bucle hasta que el usuario elija "Volver al
menú principal", devolviendo el control al menú principal sin cerrar la aplicación. Solo la
opción "Salir" del menú principal SHALL finalizar el programa. Las opciones de búsqueda,
modificación, baja, asignar o cambiar empresa y desvincular empresa del submenu de personas
SHALL localizar a la persona por nombre y apellidos, y el menú de búsqueda SHALL NOT ofrecer
ninguna alternativa de búsqueda por identificador.

#### Scenario: Menú principal con Personas y Empresas
- **WHEN** el sistema muestra el menú principal
- **THEN** ofrece las opciones numeradas "Personas", "Empresas" y "Salir"

#### Scenario: Entrada no numérica en el menú principal
- **WHEN** el usuario introduce texto no numérico donde se espera una opción del menú principal
- **THEN** el sistema detecta el error, muestra un aviso y vuelve a pedir la opción

#### Scenario: Opción inexistente en el menú principal
- **WHEN** el usuario elige una opción de menú que no existe
- **THEN** el sistema lo indica y vuelve a mostrar el menú

#### Scenario: Ciclo completo del menú
- **WHEN** el usuario termina una operación distinta de "Salir" y de "Volver al menú principal"
- **THEN** el sistema vuelve a mostrar el menú desde el que se partió (submenu de personas
  o menú principal) sin cerrar la aplicación

#### Scenario: Volver al menú principal sin salir
- **WHEN** el usuario elige "Volver al menú principal" desde el submenu de personas
- **THEN** el sistema muestra el menú principal sin cerrar la aplicación

#### Scenario: Salir del programa
- **WHEN** el usuario elige "Salir" en el menú principal
- **THEN** el sistema finaliza el programa de forma ordenada

#### Scenario: Menú sin búsqueda por identificador
- **WHEN** el usuario abre la búsqueda de personas
- **THEN** ninguna opción ofrece buscar por `IdPersona` y la búsqueda se realiza siempre por nombre y apellidos

#### Scenario: Submenu con las operaciones de relación
- **WHEN** el usuario abre el submenu de personas
- **THEN** además de alta, listado, búsqueda, modificación y baja, incluye las opciones
  "Asignar/cambiar empresa" y "Desvincular empresa"

### Requirement: Alta de persona
El sistema SHALL permitir dar de alta una persona solicitando Nombre, Apellidos, Teléfono,
Correo y Empresa asignada; SHALL validar cada dato inmediatamente después de ser escrito,
repreguntando únicamente el campo que falla; SHALL rechazar cualquier campo vacío excepto la
empresa asignada, que es opcional y declara explícitamente la posibilidad de quedarse sin
empresa; cuando la empresa asignada se rellene, SHALL corresponder al nombre de una empresa
existente del catálogo (con la misma coincidencia tolerante y desambiguación que la búsqueda
de empresas), repreguntando el campo con un aviso mientras no sea así o hasta que se deje en
blanco; SHALL asignar un `IdPersona` autogenerado e incremental no editable por el usuario;
y SHALL confirmar el alta al usuario.

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

### Requirement: Baja de persona
El sistema SHALL permitir eliminar una persona localizándola por su nombre y apellidos; SHALL
mostrar el registro afectado, incluido su `IdPersona`; y SHALL exigir confirmación explícita
(s/n) antes de eliminar. Al confirmar, el sistema SHALL eliminar también la entrada de
relación persona-empresa de esa persona si existe, de modo que no queden referencias
huérfanas. Cuando no haya coincidencia, SHALL informarlo sin error ni cierre inesperado y
volver al menú.

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
usuario SHALL usar la operación "Desvincular empresa" del submenu de personas.

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
- **THEN** el sistema acepta el alta y la persona queda sin empresa asignada

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
