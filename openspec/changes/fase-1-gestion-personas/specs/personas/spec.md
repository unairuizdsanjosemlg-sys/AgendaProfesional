# Spec Delta

## Purpose

Capacidad de la agenda de la consultora para mantener personas colaboradoras desde la
consola: alta, listado, búsqueda, modificación y baja, con validación de datos de contacto,
confirmaciones explícitas y un menú robusto que tolera entradas incorrectas.

## ADDED Requirements

### Requirement: Menú principal en bucle
El sistema SHALL mostrar un menú principal con opciones numeradas y claras (alta, listado,
búsqueda, modificación, baja y salir) y SHALL repetirlo en bucle hasta que el usuario elija
"Salir".

#### Scenario: Ciclo completo del menú
- **WHEN** el usuario termina una operación distinta de "Salir"
- **THEN** el sistema vuelve a mostrar el menú principal

#### Scenario: Salir del programa
- **WHEN** el usuario elige "Salir"
- **THEN** el sistema finaliza el programa de forma ordenada

### Requirement: Alta de persona
El sistema SHALL permitir dar de alta una persona solicitando Nombre, Apellidos, Teléfono,
Correo y Empresa asignada; SHALL validar los datos antes de guardar; SHALL asignar un
`IdPersona` autogenerado e incremental no editable por el usuario; y SHALL confirmar el alta
al usuario.

#### Scenario: Alta válida
- **WHEN** el usuario introduce todos los datos obligatorios en formato válido y confirma
- **THEN** el sistema crea la persona con un `IdPersona` autogenerado y confirma el alta

#### Scenario: Alta con campos vacíos
- **WHEN** el usuario deja un campo obligatorio vacío
- **THEN** el sistema no avanza e indica qué campo falta

#### Scenario: Alta con formato inválido
- **WHEN** el correo o el teléfono tienen un formato no válido
- **THEN** el sistema rechaza el alta, indica qué campo falla y permite corregirlo

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

### Requirement: Búsqueda por identificador
El sistema SHALL permitir localizar una persona por su `IdPersona`, mostrando la persona
exacta si existe o indicando explícitamente que no existe.

#### Scenario: Búsqueda de Id existente
- **WHEN** el usuario introduce un `IdPersona` existente
- **THEN** el sistema muestra esa persona

#### Scenario: Búsqueda de Id inexistente
- **WHEN** el usuario introduce un `IdPersona` que no existe
- **THEN** el sistema lo indica explícitamente

### Requirement: Búsqueda por texto
El sistema SHALL permitir buscar por texto con coincidencias parciales en nombre o apellidos
sin distinguir mayúsculas/minúsculas y SHALL indicar explícitamente cuando no hay
coincidencias.

#### Scenario: Coincidencias parciales
- **WHEN** el usuario introduce un texto que coincide parcialmente con el nombre o los apellidos de una o más personas
- **THEN** el sistema muestra todas las coincidencias

#### Scenario: Sin coincidencias
- **WHEN** no hay coincidencias para el texto introducido
- **THEN** el sistema lo indica explícitamente

### Requirement: Modificación de persona
El sistema SHALL permitir corregir los datos de una persona localizándola por su Id; SHALL
mostrar los datos actuales y exigir confirmación explícita antes de guardar; SHALL re-validar
correo y teléfono si se modifican; y SHALL informar sin error cuando el Id no existe.

#### Scenario: Modificar persona existente
- **WHEN** el usuario localiza por Id una persona existente, cambia sus datos y confirma
- **THEN** el sistema guarda los cambios y re-valida los campos de contacto

#### Scenario: Modificar con Id inexistente
- **WHEN** el usuario introduce un Id que no existe
- **THEN** el sistema lo informa y vuelve al menú sin error

### Requirement: Baja de persona
El sistema SHALL permitir eliminar una persona localizándola por su Id; SHALL mostrar el
registro afectado y exigir confirmación explícita (s/n) antes de eliminar; y SHALL informar
sin error cuando el Id no existe.

#### Scenario: Baja confirmada
- **WHEN** el usuario confirma la eliminación de una persona existente
- **THEN** el sistema elimina el registro y lo confirma

#### Scenario: Baja cancelada
- **WHEN** el usuario no confirma la eliminación
- **THEN** el sistema mantiene el registro y vuelve al menú

#### Scenario: Baja con Id inexistente
- **WHEN** el usuario introduce un Id que no existe
- **THEN** el sistema lo informa sin error ni cierre inesperado

### Requirement: Entrada numérica y opciones de menú
El sistema SHALL tolerar entradas incorrectas: SHALL detectar texto no numérico donde se
espera un número de opción, mostrar un aviso y volver a pedir la opción; y SHALL indicar y
repetir el menú si la opción elegida no existe. Ninguna entrada inválida SHALL cerrar el
programa ni lanzar una excepción no controlada.

#### Scenario: Texto no numérico
- **WHEN** el usuario introduce texto no numérico donde se espera un número de opción
- **THEN** el sistema detecta el error, muestra un aviso y vuelve a pedir la opción

#### Scenario: Opción inexistente
- **WHEN** el usuario elige una opción de menú que no existe
- **THEN** el sistema lo indica y vuelve a mostrar el menú

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
El sistema SHALL validar el teléfono como no vacío, con solo dígitos y una longitud típica de
9 (formato España).

#### Scenario: Teléfono válido
- **WHEN** el teléfono no está vacío, contiene solo dígitos y tiene longitud 9
- **THEN** el sistema acepta el teléfono

#### Scenario: Teléfono no válido
- **WHEN** el teléfono está vacío, contiene caracteres no numéricos o su longitud no es 9
- **THEN** el sistema rechaza el teléfono e indica el problema

### Requirement: Empresa asignada
El sistema SHALL permitir registrar la empresa asignada a una persona como texto libre, sin
validarla contra un catálogo de empresas en esta fase, y SHALL permitir dejarla vacía.

#### Scenario: Empresa opcional y libre
- **WHEN** el usuario deja el campo de empresa vacío o introduce un texto libre
- **THEN** el sistema lo guarda tal cual sin validar contra un catálogo