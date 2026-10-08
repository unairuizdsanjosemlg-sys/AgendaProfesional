# persona-empresa Specification

## Purpose

Mantiene la relación interna entre personas y empresas de la agenda por identificadores:
asignación y cambio de empresa, desvinculación, presentación de la empresa asociada a una
persona y consulta de la plantilla de una empresa, protegiendo el borrado de empresas con
personas vinculadas.

## Requirements

### Requirement: Relación persona-empresa por identificadores
El sistema SHALL mantener una relación interna entre personas y empresas almacenada como
pares de identificadores (`IdPersona`, `IdEmpresa`) en un array. Una persona SHALL poder no
tener empresa asignada o tener exactamente una; una empresa SHALL poder tener de cero a
muchas personas. El usuario SHALL operar siempre con nombres de empresa, y el sistema SHALL
guardar, sustituir y consultar la relación mediante los identificadores. Cuando en una alta
o una modificación de persona se introduzca el nombre de una empresa existente, el sistema
SHALL registrar o sustituir la relación con los identificadores de la persona y la empresa.

#### Scenario: Asignación guardada por identificadores
- **WHEN** el usuario asigna una empresa existente a una persona
- **THEN** el sistema guarda internamente el par (`IdPersona`, `IdEmpresa`) y no el texto tecleado

#### Scenario: Una única empresa por persona
- **WHEN** se asigna una empresa a una persona que ya tenía otra
- **THEN** la relación anterior se sustituye por la nueva y la persona queda con una única empresa

#### Scenario: Persona sin empresa
- **WHEN** una persona se da de alta sin empresa, nunca ha sido asignada o ha sido desvinculada
- **THEN** la persona no tiene ninguna entrada de relación y el sistema la presenta como "Sin empresa"

#### Scenario: Alta de persona con empresa existente
- **WHEN** el usuario introduce en el alta el nombre de una empresa que existe en el catálogo
- **THEN** el sistema crea la persona y registra la relación con los identificadores de ambas

#### Scenario: Modificación con empresa existente
- **WHEN** al modificar una persona se introduce el nombre de otra empresa existente
- **THEN** el sistema sustituye la relación por la de la nueva empresa y actualiza el texto de empresa de la persona

### Requirement: Asignar o cambiar empresa desde el menú de personas
El sistema SHALL ofrecer en el submenu de personas la operación para asignar o cambiar la
empresa de una persona. La persona SHALL localizarse por nombre y apellidos con las mismas
reglas de búsqueda y desambiguación que el resto de operaciones de personas. A continuación
el sistema SHALL pedir el nombre de la empresa y SHALL buscarla en el catálogo con
coincidencia tolerante (subcadena, sin distinguir acentos ni mayúsculas/minúsculas): con una
única coincidencia la SHALL usar directamente, con varias SHALL mostrar una lista numerada
con sus `IdEmpresa` y pedir el número elegido, y sin ninguna SHALL informarlo y volver al
submenu sin cambiar nada. Antes de aplicar el cambio, el sistema SHALL mostrar la persona y
la empresa elegida y SHALL exigir confirmación explícita (s/n). La palabra `cancelar` en
cualquier pregunta SHALL abortar la operación sin guardar y volver al menú principal.

#### Scenario: Asignación a persona sin empresa
- **WHEN** el usuario localiza una persona sin empresa, elige una empresa existente y confirma
- **THEN** el sistema crea la relación, actualiza el texto de empresa de la persona y lo confirma

#### Scenario: Cambio de empresa
- **WHEN** el usuario localiza una persona con empresa, elige otra empresa existente y confirma
- **THEN** el sistema sustituye la relación por la nueva empresa

#### Scenario: Empresa sin coincidencias
- **WHEN** el nombre de empresa introducido no coincide con ninguna empresa del catálogo
- **THEN** el sistema lo informa y vuelve al submenu de personas sin modificar la relación

#### Scenario: Varias empresas coincidentes
- **WHEN** el nombre introducido coincide con varias empresas del catálogo
- **THEN** el sistema muestra la lista numerada de candidatos con su `IdEmpresa` y pide el número elegido

#### Scenario: Confirmación denegada
- **WHEN** el usuario responde "n" a la confirmación del cambio de empresa
- **THEN** el sistema no modifica la relación y vuelve al submenu

#### Scenario: Operación cancelada
- **WHEN** el usuario escribe `cancelar` en la localización, en el nombre de empresa o en la confirmación
- **THEN** el sistema no guarda ningún cambio y vuelve al menú principal

### Requirement: Desvincular persona de su empresa
El sistema SHALL ofrecer en el submenu de personas la operación para desvincular a una
persona de su empresa. La persona SHALL localizarse por nombre y apellidos con las mismas
reglas del resto de operaciones. Si la persona no tiene empresa, el sistema SHALL informarlo
y volver al submenu sin error. Si la persona tiene empresa, el sistema SHALL mostrar la
persona con su empresa actual y SHALL exigir confirmación explícita (s/n); al confirmar, la
relación SHALL eliminarse del array y el campo de texto `Empresa asignada` de la persona
SHALL quedar vacío. Sin confirmación, la relación se conserva. La palabra `cancelar` en
cualquier pregunta SHALL abortar la operación sin guardar y volver al menú principal.

#### Scenario: Desvinculación confirmada
- **WHEN** el usuario localiza una persona con empresa y confirma la desvinculación
- **THEN** el sistema elimina la relación, vacía el texto de empresa de la persona y lo confirma

#### Scenario: Persona sin empresa
- **WHEN** el usuario localiza una persona que no tiene empresa asignada
- **THEN** el sistema lo informa y vuelve al submenu sin modificar nada

#### Scenario: Confirmación denegada
- **WHEN** el usuario responde "n" a la confirmación de desvinculación
- **THEN** el sistema conserva la relación y vuelve al submenu

#### Scenario: Operación cancelada
- **WHEN** el usuario escribe `cancelar` en la localización o en la confirmación
- **THEN** el sistema no guarda ningún cambio y vuelve al menú principal

### Requirement: Presentación de la empresa de una persona
Cada vez que el sistema presente una persona (listado, resultados de búsqueda, lista de
homónimos, datos actuales en modificación o confirmación de baja), SHALL mostrar primero el
nombre de la empresa resuelto desde su `IdEmpresa` y, a continuación, ese `IdEmpresa`. Cuando
la persona no tenga relación, SHALL indicar explícitamente "Sin empresa". El nombre mostrado
SHALL ser el vigente en el catálogo, de modo que un cambio de nombre de la empresa se refleje
en todas las presentaciones. El campo de texto libre de la persona no SHALL presentarse como
la empresa de la persona cuando exista relación.

#### Scenario: Persona con empresa
- **WHEN** se presenta una persona que tiene empresa asignada
- **THEN** aparece primero el nombre comercial vigente de la empresa y después su `IdEmpresa`

#### Scenario: Persona sin empresa
- **WHEN** se presenta una persona sin relación con ninguna empresa
- **THEN** la presentación indica explícitamente "Sin empresa"

#### Scenario: Empresa renombrada
- **WHEN** el nombre comercial de la empresa cambia después de haber sido asignada
- **THEN** las presentaciones de la persona muestran el nombre nuevo resuelto por el `IdEmpresa`

### Requirement: Consultar las personas de una empresa
El sistema SHALL ofrecer en el submenu de empresas la operación para ver las personas de una
empresa. La empresa SHALL localizarse con la misma búsqueda por NombreComercial o CIF que el
resto de operaciones del submenu. Con la empresa elegida, el sistema SHALL listar su
plantilla en el mismo formato legible que el listado de personas, mostrando cada persona con
su `IdPersona` y su empresa. Si la empresa no tiene ninguna persona, SHALL informarlo
explícitamente. La palabra `cancelar` SHALL abortar la operación y volver al menú principal.

#### Scenario: Empresa con plantilla
- **WHEN** el usuario elige una empresa que tiene personas asociadas
- **THEN** el sistema lista esas personas con sus datos, su `IdPersona` y la empresa mostrada

#### Scenario: Empresa sin personas
- **WHEN** el usuario elige una empresa que no tiene personas asociadas
- **THEN** el sistema informa de que la empresa no tiene personas asignadas

#### Scenario: Localización sin coincidencias
- **WHEN** el texto introducido no corresponde a ninguna empresa
- **THEN** el sistema lo informa y vuelve al submenu sin error

#### Scenario: Operación cancelada
- **WHEN** el usuario escribe `cancelar` en la localización o en la selección entre candidatos
- **THEN** el sistema vuelve al menú principal sin mostrar la plantilla

### Requirement: Borrado de empresa con personas asociadas
Cuando el usuario confirme la intención de eliminar una empresa que tiene personas
asociadas, el sistema SHALL NOT borrarla. SHALL informar de que la empresa tiene personas
relacionadas, de que por eso no se puede borrar y de que la forma de borrarla es
desvincular primero a todas las personas; SHALL mostrar el número de personas afectadas y
SHALL ofrecer dos opciones: cancelar el borrado, o desvincular a todas las personas para
proceder al borrado. Si elige cancelar, no SHALL cambiar nada y SHALL volver al submenu. Si
elige desvincular, el sistema SHALL exigir confirmación explícita (s/n) antes de actuar; al
confirmar SHALL desvincular a todas las personas de esa empresa (vaciando también su texto
`Empresa asignada`) y SHALL borrar la empresa en el mismo acto, informando del resultado. La
palabra `cancelar` SHALL abortar la operación sin cambios y volver al menú principal.

#### Scenario: Borrado bloqueado por plantilla
- **WHEN** el usuario confirma la baja de una empresa que tiene personas asociadas
- **THEN** el sistema no la borra, informa del bloqueo, muestra el número de personas
  afectadas y ofrece las dos opciones (cancelar o desvincular todas y borrar)

#### Scenario: Cancelar el borrado
- **WHEN** el usuario elige cancelar el borrado
- **THEN** la empresa y sus relaciones siguen intactas y el sistema vuelve al submenu

#### Scenario: Desvincular todas y borrar tras confirmación
- **WHEN** el usuario elige desvincular a todas las personas y confirma con "s"
- **THEN** el sistema desvincula a todas las personas de la empresa, vacía su texto de
  empresa, borra la empresa y lo confirma

#### Scenario: Confirmación de desvinculación denegada
- **WHEN** el usuario elige desvincular y responde "n" en la confirmación
- **THEN** no se desvincula a nadie, no se borra la empresa y el sistema vuelve al submenu

#### Scenario: Empresa sin personas
- **WHEN** el usuario confirma la baja de una empresa sin personas asociadas
- **THEN** el sistema la borra con el flujo normal de confirmación, sin mostrar el aviso de bloqueo
