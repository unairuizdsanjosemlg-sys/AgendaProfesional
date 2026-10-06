# personas Specification

## Purpose

Capacidad de la agenda de la consultora para mantener personas colaboradoras desde la
consola: alta, listado, búsqueda, modificación y baja, con validación de datos de contacto,
confirmaciones explícitas y un menú robusto que tolera entradas incorrectas.

## Requirements

### Requirement: Menú principal en bucle
El sistema SHALL mostrar un menú principal con opciones numeradas y claras (alta, listado,
búsqueda, modificación, baja y salir) y SHALL repetirlo en bucle hasta que el usuario elija
"Salir". Las opciones de búsqueda, modificación y baja SHALL localizar a la persona por nombre
y apellidos, y el menú de búsqueda SHALL NOT ofrecer ninguna alternativa de búsqueda por
identificador.

#### Scenario: Ciclo completo del menú
- **WHEN** el usuario termina una operación distinta de "Salir"
- **THEN** el sistema vuelve a mostrar el menú principal

#### Scenario: Salir del programa
- **WHEN** el usuario elige "Salir"
- **THEN** el sistema finaliza el programa de forma ordenada

#### Scenario: Menú sin búsqueda por identificador
- **WHEN** el sistema muestra el menú principal
- **THEN** ninguna opción ofrece buscar por `IdPersona` y la búsqueda se realiza siempre por nombre y apellidos

### Requirement: Alta de persona
El sistema SHALL permitir dar de alta una persona solicitando Nombre, Apellidos, Teléfono,
Correo y Empresa asignada; SHALL validar cada dato inmediatamente después de ser escrito,
repreguntando únicamente el campo que falla; SHALL rechazar cualquier campo vacío, incluida
la empresa asignada; SHALL asignar un `IdPersona` autogenerado e incremental no editable por
el usuario; y SHALL confirmar el alta al usuario.

#### Scenario: Alta válida
- **WHEN** el usuario introduce todos los datos obligatorios en formato válido y confirma
- **THEN** el sistema crea la persona con un `IdPersona` autogenerado y confirma el alta

#### Scenario: Alta con campos vacíos
- **WHEN** el usuario deja un campo obligatorio vacío
- **THEN** el sistema no avanza, repregunta únicamente ese campo e indica cuál es

#### Scenario: Alta con formato inválido
- **WHEN** el correo o el teléfono tienen un formato no válido
- **THEN** el sistema no guarda nada, indica qué regla ha fallado en ese campo y vuelve a
  preguntar solo ese campo, sin repetir el resto del formulario

#### Scenario: Empresa asignada obligatoria
- **WHEN** el usuario deja vacía la empresa asignada
- **THEN** el sistema no guarda nada y vuelve a preguntar únicamente la empresa asignada

### Requirement: Listado de personas
El sistema SHALL mostrar el listado de las personas registradas en orden legible y SHALL
indicar explícitamente cuando no hay personas registradas en lugar de mostrar una lista
vacía.

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
y exigir confirmación explícita (s/n) antes de guardar.

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
(s/n) antes de eliminar. Cuando no haya coincidencia, SHALL informarlo sin error ni cierre
inesperado y volver al menú.

#### Scenario: Baja confirmada
- **WHEN** el usuario localiza por nombre y apellidos una persona existente y confirma la eliminación
- **THEN** el sistema elimina el registro y lo confirma

#### Scenario: Baja cancelada
- **WHEN** el usuario no confirma la eliminación
- **THEN** el sistema mantiene el registro y vuelve al menú

#### Scenario: Baja sin coincidencias
- **WHEN** el nombre y los apellidos introducidos no coinciden con ninguna persona
- **THEN** el sistema lo informa sin error ni cierre inesperado y vuelve al menú

### Requirement: Entrada numérica y opciones de menú
El sistema SHALL tolerar entradas incorrectas en toda entrada numérica: la opción de menú, el
número de selección de un homónimo y la respuesta s/n de confirmación. Ante una entrada no
numérica, un número fuera de rango o una respuesta distinta de s/n, SHALL mostrar un aviso y
volver a pedir únicamente ese dato. Ninguna entrada inválida SHALL cerrar el programa ni lanzar
una excepción no controlada.

#### Scenario: Texto no numérico
- **WHEN** el usuario introduce texto no numérico donde se espera un número de opción
- **THEN** el sistema detecta el error, muestra un aviso y vuelve a pedir la opción

#### Scenario: Opción inexistente
- **WHEN** el usuario elige una opción de menú que no existe
- **THEN** el sistema lo indica y vuelve a mostrar el menú

#### Scenario: Número de homónimo inválido
- **WHEN** el usuario introduce un número fuera del rango de la lista de homónimos
- **THEN** el sistema lo indica y vuelve a pedir el número sin repetir la lista de candidatos

#### Scenario: Confirmación distinta de s/n
- **WHEN** el usuario responde con algo distinto de s o n en una confirmación
- **THEN** el sistema lo indica y vuelve a pedir la confirmación

### Requirement: Validación de correo
El sistema SHALL validar el correo como no vacío, con una `@` y un dominio con al menos un
punto.

#### Scenario: Correo válido
- **WHEN** el correo no está vacío, contiene una `@` y el dominio tiene al menos un punto
- **THEN** el sistema acepta el correo

#### Scenario: Correo no válido
- **WHEN** el correo está vacío, no contiene `@` o el dominio no tiene punto
- **THEN** el sistema rechaza el correo e indica el problema

### Requirement: Validación de teléfono
El sistema SHALL validar el teléfono como un único campo de texto que incluye obligatoriamente
el prefijo internacional: un carácter `+`, seguido de 1 a 3 dígitos de prefijo, seguido de un
único espacio opcional, seguido de 6 a 12 dígitos de número. Solo se admitirán dígitos y ese
único `+`; cualquier otra letra o carácter especial SHALL ser rechazado indicando la regla que
ha fallado.

#### Scenario: Teléfono válido con prefijo
- **WHEN** el teléfono tiene el formato `+` seguido de 1 a 3 dígitos de prefijo y de 6 a 12
  dígitos de número, con un único espacio opcional entre ambos
- **THEN** el sistema acepta el teléfono

#### Scenario: Teléfono sin prefijo
- **WHEN** el teléfono no empieza por `+`
- **THEN** el sistema lo rechaza, explica que el prefijo internacional es obligatorio y
  vuelve a preguntar únicamente el teléfono

#### Scenario: Teléfono con caracteres no permitidos
- **WHEN** el teléfono contiene letras u otros caracteres especiales distintos del `+`
- **THEN** el sistema lo rechaza indicando la regla y vuelve a preguntar únicamente el teléfono

#### Scenario: Teléfono con longitudes no válidas
- **WHEN** el prefijo tiene más de 3 dígitos o el número tiene menos de 6 o más de 12 dígitos
- **THEN** el sistema lo rechaza indicando la regla y vuelve a preguntar únicamente el teléfono

### Requirement: Empresa asignada
El sistema SHALL tratar la empresa asignada a una persona como texto libre obligatorio, sin
validarla contra un catálogo de empresas en esta fase. En el alta, una empresa asignada vacía
no SHALL aceptarse. En la modificación, una empresa asignada vacía SHALL conservar el valor
actual en lugar de dejarlo en blanco.

#### Scenario: Empresa obligatoria en el alta
- **WHEN** el usuario deja el campo de empresa vacío en el alta
- **THEN** el sistema no guarda nada y vuelve a preguntar únicamente la empresa asignada

#### Scenario: Empresa obligatoria en la modificación
- **WHEN** el usuario deja vacío el campo de empresa al modificar
- **THEN** el sistema conserva la empresa asignada actual

#### Scenario: Texto libre sin catálogo
- **WHEN** el usuario introduce un texto libre como empresa asignada
- **THEN** el sistema lo guarda tal cual sin validarlo contra un catálogo

### Requirement: Búsqueda por nombre y apellidos
El sistema SHALL localizar personas pidiendo el nombre y los apellidos en dos preguntas
separadas. La coincidencia SHALL ser parcial (subcadena), SHALL ignorar mayúsculas/minúsculas
y SHALL ignorar acentos tanto en lo tecleado como en lo almacenado. Una persona SHALL
coincidir solo cuando el nombre **y** los apellidos coincidan (condición AND). La búsqueda
SHALL mostrar todas las coincidencias y SHALL informar explícitamente cuando no haya ninguna.

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

### Requirement: Desambiguación de homónimos
Cuando la búsqueda por nombre y apellidos devuelve exactamente una coincidencia, el sistema
SHALL usarla directamente sin pedir selección. Cuando devuelve varias, el sistema SHALL mostrar
una lista temporal numerada (1, 2, 3...) con los datos de cada candidato y SHALL pedir al
usuario el número del que quiere elegir, validando esa respuesta antes de continuar.

#### Scenario: Una única coincidencia
- **WHEN** la búsqueda devuelve una sola persona
- **THEN** el sistema la selecciona directamente sin pedir un número

#### Scenario: Varias coincidencias en modificación o baja
- **WHEN** la búsqueda devuelve varias personas y el usuario está en modificación o baja
- **THEN** el sistema muestra la lista numerada de candidatos y espera a que elija uno

#### Scenario: Selección válida
- **WHEN** el usuario introduce el número de un candidato de la lista
- **THEN** el sistema continúa la operación sobre esa persona

#### Scenario: Selección fuera de rango
- **WHEN** el usuario introduce un número que no está en la lista
- **THEN** el sistema lo indica y vuelve a pedir el número, sin repetir el resto de la operación

#### Scenario: Lista temporal
- **WHEN** la operación termina
- **THEN** el sistema no conserva la lista de candidatos para operaciones posteriores

### Requirement: Validación inmediata campo a campo
El sistema SHALL validar cada dato en el momento en que el usuario lo escribe y SHALL repreguntar
únicamente el campo que ha fallado, nunca el formulario completo. El aviso de error SHALL
identificar qué regla se ha incumplido. En la modificación, una respuesta vacía SHALL considerarse
válida, conservar el valor actual y omitir la validación del campo. En el alta, ninguna respuesta
vacía SHALL considerarse válida para ningún campo.

#### Scenario: Reintento de un solo campo
- **WHEN** el usuario escribe un correo con formato no válido
- **THEN** el sistema explica qué regla ha fallado y vuelve a preguntar solo el correo, sin
  volver a preguntar nombre, apellidos, teléfono ni empresa

#### Scenario: Cambio de un campo correcto tras un error
- **WHEN** el usuario corrige el campo que falló y el nuevo valor es válido
- **THEN** el sistema continúa con el siguiente campo sin repetir los anteriores

#### Scenario: Campo vacío en modificación
- **WHEN** el usuario responde con una línea vacía a un campo de la modificación
- **THEN** el sistema lo acepta sin validar ese campo y lo mantiene sin cambios

#### Scenario: Campo vacío en alta
- **WHEN** el usuario responde con una línea vacía a un campo del alta
- **THEN** el sistema lo rechaza, lo indica y vuelve a preguntar únicamente ese campo

### Requirement: Cancelación de la operación en curso
El sistema SHALL tratar la palabra `cancelar`, escrita sin distinguir mayúsculas/minúsculas, como
una orden de cancelación válida en cualquier pregunta de cualquier operación. Al recibirla, el
sistema SHALL abortar la operación en curso sin guardar ningún cambio y SHALL volver al menú
principal.

#### Scenario: Cancelar durante el alta
- **WHEN** el usuario escribe `cancelar` en cualquiera de los campos del alta
- **THEN** el sistema no crea la persona y vuelve al menú principal

#### Scenario: Cancelar durante la búsqueda de nombre
- **WHEN** el usuario escribe `cancelar` al pedir el nombre o los apellidos
- **THEN** el sistema abandona la búsqueda y vuelve al menú principal

#### Scenario: Cancelar durante la desambiguación de homónimos
- **WHEN** el usuario escribe `cancelar` al elegir entre homónimos
- **THEN** el sistema abandona la operación sin modificar ni eliminar ninguna persona

#### Scenario: Cancelar durante la modificación
- **WHEN** el usuario escribe `cancelar` en un campo de la modificación o en la confirmación
- **THEN** el sistema no guarda ningún cambio y vuelve al menú principal

#### Scenario: Cancelar es una palabra reservada
- **WHEN** el usuario teclea `cancelar` donde se esperaba un valor de dato
- **THEN** el sistema interpreta la entrada como cancelación, no como un valor literal

### Requirement: Identificador mostrado pero nunca solicitado
El sistema SHALL mantener el `IdPersona` como dato interno, autogenerado de forma incremental y
no editable por el usuario. El sistema SHALL mostrarlo siempre que se presente una persona
(listado, resultados de búsqueda, lista de selección de homónimos y confirmación de baja) y
SHALL NOT pedirlo al usuario como forma de localizar, seleccionar o modificar un registro.

#### Scenario: Identificador visible en el listado
- **WHEN** el sistema muestra el listado de personas
- **THEN** cada persona aparece con su `IdPersona`

#### Scenario: Identificador no solicitado
- **WHEN** el usuario busca, modifica o da de baja una persona
- **THEN** el sistema nunca pide el `IdPersona` como dato de entrada

#### Scenario: Identificador no editable
- **WHEN** el usuario modifica cualquier campo de una persona
- **THEN** su `IdPersona` permanece igual y no puede ser cambiado por el usuario
