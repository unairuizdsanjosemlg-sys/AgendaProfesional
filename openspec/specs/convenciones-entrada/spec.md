# convenciones-entrada Specification

## Purpose

Conjunto de convenciones de project's console input and record-lookup behaviour that any current
or future module (Personas today, Empresas in Phase 2) must follow, so that interaction and
validation rules are consistent across the agenda and are not reinvented per module.

## Requirements

### Requirement: Localización por nombre legible
Todo módulo que deba localizar registros SHALL pedir al usuario un nombre legible por personas
(los datos que la persona reconocería), y SHALL NOT exigir un identificador interno crudo para
localizar, seleccionar o modificar un registro. Cada módulo SHALL definir en su especificación
qué campos constituyen ese nombre legible.

#### Scenario: El usuario nunca teclea el identificador
- **WHEN** el usuario busca, modifica o elimina un registro en cualquier módulo
- **THEN** el sistema pide los datos del nombre legible del módulo y no pide su identificador interno

#### Scenario: Nombre legible específico del módulo
- **WHEN** un módulo define sus propios datos localizadores
- **THEN** esos datos sustituyen al identificador interno en la búsqueda y en la selección

### Requirement: Coincidencia tolerante y desambiguación de homónimos
La búsqueda de un registro por nombre legible SHALL comparar de forma parcial (por subcadena),
sin distinguir mayúsculas/minúsculas y sin distinguir acentos, tanto el texto introducido como
el almacenado. Cuando un módulo/localización devuelva exactamente un candidato, el sistema
SHALL usarlo directamente. Cuando devuelva varios, el sistema SHALL mostrar una lista temporal
numerada y pedir al usuario el número del candidato, validando esa respuesta.

#### Scenario: Búsqueda sin acentos encuentra el registro con acentos
- **WHEN** el usuario escribe el nombre legible sin acentos y el registro almacenado lo tiene con acentos
- **THEN** el sistema lo encuentra

#### Scenario: Homónimos
- **WHEN** varios registros comparten el nombre legible buscado
- **THEN** el sistema muestra una lista numerada de candidatos y usa el que el usuario elija

#### Scenario: Candidatos numerados fuera de rango
- **WHEN** el usuario elige un número que no está en la lista de candidatos
- **THEN** el sistema lo indica y vuelve a pedir el número

#### Scenario: Sin resultados
- **WHEN** la búsqueda no devuelve ningún candidato
- **THEN** el sistema lo comunica explícitamente y no continúa la operación como si hubiera éxito

### Requirement: Validación inmediata por campo
Todo dato que el usuario escriba SHALL validarse en el momento en que se escribe. Ante un dato
inválido, el sistema SHALL indicar qué regla se ha incumplido y SHALL repreguntar únicamente ese
campo, sin volver a pedir el resto de los datos ya aceptados.

#### Scenario: Solo se repregunta el campo fallido
- **WHEN** el usuario escribe un dato con formato no válido en un módulo
- **THEN** el sistema señala la regla incumplida y vuelve a pedir solo ese campo

#### Scenario: Continuación tras corregir
- **WHEN** el usuario corrige el campo fallido con un valor válido
- **THEN** la operación continúa sin volver a pedir los campos ya aceptados

### Requirement: Cancelación universal con la palabra "cancelar"
La palabra `cancelar` (sin distinguir mayúsculas/minúsculas), escrita por el usuario en
cualquier pregunta de cualquier módulo, SHALL abortar la operación en curso sin guardar
cambios y devolver el control al menú principal. `cancelar` SHALL considerarse una palabra
reservada y nunca un valor de dato literal.

#### Scenario: Cancelación en cualquier punto
- **WHEN** el usuario escribe `cancelar` en cualquier pregunta de cualquier operación
- **THEN** el sistema aborta la operación sin guardar cambios y vuelve al menú principal

#### Scenario: Valor de dato no literal
- **WHEN** un módulo pide un texto donde `cancelar` podría ser un valor legítimo
- **THEN** el sistema lo interpreta como cancelación, no como el texto `cancelar`

### Requirement: Edición con vacío-conserva y confirmación
Al modificar un registro, un campo respondido en blanco SHALL conservar el valor actual en lugar
de vaciarlo. El módulo SHALL mostrar un resumen de los cambios pendientes y pedir confirmación
explícita antes de aplicarlos; cualquier modificación SHALL requerir confirmación del usuario.

#### Scenario: Blanco conserva el valor
- **WHEN** el usuario deja en blanco un campo al modificar un registro
- **THEN** ese campo mantiene su valor anterior

#### Scenario: Resumen antes de guardar
- **WHEN** el usuario introduce uno o más cambios
- **THEN** el sistema muestra qué campos van a cambiar y pide confirmación antes de guardarlos

#### Scenario: Confirmación denegada
- **WHEN** el usuario no confirma los cambios
- **THEN** el sistema no los aplica

### Requirement: Campos obligatorios salvo excepción explícita
Todos los campos de un registro SHALL ser obligatorios, de modo que en el alta nunca se acepte
una entrada vacía. Un módulo SHALL declarar un campo como opcional solo si su especificación lo
hace explícitamente; la ausencia de esa declaración implica campo obligatorio. Cuando no haya
datos no esenciales, el módulo SHALL NOT ofrecer mecanismos especiales para vaciar o limpiar un
valor en la modificación (el blanco ya conserva el valor anterior).

#### Scenario: Alta sin campos vacíos
- **WHEN** el usuario da de alta un registro y deja un campo en blanco
- **THEN** el sistema rechaza el alta y repregunta ese campo

#### Scenario: La opcionalidad requiere declaración
- **WHEN** la especificación de un módulo no declara ningún campo como opcional
- **THEN** todos sus campos se tratan como obligatorios

#### Scenario: Sin datos no esenciales
- **WHEN** un módulo no tiene campos opcionales
- **THEN** no ofrece ninguna entrada especial para borrar un valor, porque el blanco ya lo conserva

### Requirement: Identificador visible pero nunca escrito
Cada registro con identificador interno autogenerado SHALL mostrar dicho identificador siempre
que se presente el registro al usuario (listados, resultados, listas de selección y
confirmaciones), y SHALL mantenerlo no editable. El identificador SHALL servir para distinguir
registros, nunca como dato que el usuario deba teclear para localizarlos.

#### Scenario: Identificador visible en la presentación
- **WHEN** un registro se muestra al usuario
- **THEN** su identificador aparece junto a sus datos

#### Scenario: Identificador no editable
- **WHEN** el usuario modifica un registro
- **THEN** su identificador no cambia y no puede ser introducido por el usuario como selector
