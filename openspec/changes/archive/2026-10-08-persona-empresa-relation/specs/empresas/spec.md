# Spec Delta

## MODIFIED Requirements

### Requirement: Menú de empresas en bucle
El sistema SHALL mostrar un submenu de empresas con opciones numeradas y claras (alta,
listado, búsqueda, modificación, baja, ver personas de la empresa y "Volver al menú
principal") y SHALL repetirlo en bucle hasta que el usuario elija "Volver al menú
principal". Elegir "Volver al menú principal" SHALL devolver el control al menú principal
sin cerrar la aplicación; solo la opción "Salir" del menú principal SHALL finalizar el
programa. Ante una entrada no numérica o una opción inexistente del submenu, el sistema
SHALL mostrar un aviso y volver a pedir la opción, sin cerrar el programa ni lanzar
excepciones no controladas.

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

#### Scenario: Submenu con la consulta de plantilla
- **WHEN** el usuario abre el submenu de empresas
- **THEN** además de alta, listado, búsqueda, modificación y baja, incluye la opción
  "Ver personas de la empresa"

### Requirement: Localización de empresas para modificación y baja
Para modificar, dar de baja una empresa o ver las personas de una empresa, el sistema SHALL
localizarla con la misma búsqueda por NombreComercial o CIF. Con exactamente una coincidencia
la SHALL usar directamente sin pedir selección; con varias, SHALL mostrar una lista temporal
numerada con los datos de cada candidato y SHALL pedir al usuario el número del que quiere
elegir, validando esa respuesta. Sin coincidencias, SHALL informarlo sin error ni cierre
inesperado y volver al menú.

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
palabra `cancelar` SHALL abortar la operación sin cambios y volver al menú principal.

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
