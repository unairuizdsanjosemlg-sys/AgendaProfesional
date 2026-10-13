# estructura-codigo Specification

## Purpose

Define cómo debe organizarse internamente el programa de agenda para ser mantenible:
responsabilidad única por función, separación entre menús, lectura/validación y operaciones
de gestión, ausencia de duplicación, contratos coherentes, un punto de entrada breve y sin
estado global mutable, con un README que documente el reparto de responsabilidades.

## Requirements

### Requirement: Responsabilidad única y nombres descriptivos

Cada responsabilidad del programa SHALL residir en una función cuyo nombre describa lo que
hace, de modo que al leer el nombre se entienda su efecto sin abrir el cuerpo. Una función
SHALL NOT acumular responsabilidades heterogéneas (por ejemplo, leer de consola, validar y
modificar el estado a la vez). Los nombres SHALL ser coherentes con el dominio y el idioma
del proyecto.

#### Scenario: El nombre describe la responsabilidad
- **WHEN** se revisa cualquier función del programa
- **THEN** su nombre describe la acción o decisión que realiza y su cuerpo no mezcla
  responsabilidades no relacionadas

#### Scenario: Operación de gestión acotada
- **WHEN** se revisa la función que ejecuta una operación de alta, búsqueda, modificación o
  baja
- **THEN** esa función delega la lectura de datos, la validación y el acceso a datos en
  funciones especializadas en lugar de implementarlos en línea

### Requirement: Separación entre menús, lectura/validación y operaciones de gestión

El programa SHALL organizar la interacción en responsabilidades separadas: (a) los menús,
que muestran opciones y coordinan la operación elegida; (b) la lectura y validación de
entrada, que obtiene líneas, valida campo a campo y gestiona `cancelar`, confirmaciones y
entrada agotada; y (c) las operaciones de gestión, que aplican las reglas de dominio sobre
los gestores y no interactúan con la consola. Ninguna función de gestión SHALL escribir
prompts ni leer del usuario, y ninguna función de menú SHALL reimplementar reglas de
validación.

#### Scenario: La gestión no toca la consola
- **WHEN** se revisa una operación de gestión (alta, listado, búsqueda, modificación o baja)
- **THEN** no lee de consola ni imprime prompts, y recibe sus datos como parámetros

#### Scenario: El menú no valida datos de dominio
- **WHEN** se revisa una función de menú
- **THEN** muestra las opciones, lee la opción elegida y delega la validación de campos y la
  operación de gestión en las funciones especializadas

#### Scenario: Entrada centralizada
- **WHEN** cualquier módulo (personas o empresas) necesita leer un campo, una confirmación o
  un número de selección
- **THEN** usa el mismo componente de lectura y validación en lugar de implementar su propia
  lectura con `Console.ReadLine`

### Requirement: Punto de entrada breve y coordinador

El punto de entrada del programa SHALL limitarse a preparar las dependencias y arrancar la
ejecución, y el bucle del menú principal SHALL residir en una función de coordinación
separada que encadene el flujo entre submenús sin contener reglas de negocio ni lógica de
campos.

#### Scenario: Punto de entrada mínimo
- **WHEN** se revisa el punto de entrada `Program.cs` (instrucciones de nivel superior)
- **THEN** solo crea los gestores y el componente de entrada, arranca el coordinador del
  menú principal y no contiene operaciones de personas ni de empresas

#### Scenario: Coordinador de flujo
- **WHEN** se revisa la función que ejecuta el menú principal
- **THEN** se limita a mostrar opciones, leer la opción y delegar en los submenús y en la
  salida, sin implementar las operaciones de cada submenú

### Requirement: Ausencia de código duplicado

Una misma regla, formato o algoritmo que aparezca en más de un punto del programa SHALL
resolverse mediante un único helper reutilizable. El programa SHALL NOT contener dos
implementaciones equivalentes de la misma operación (por ejemplo, normalización de valores
"vacío conserva", resumen de cambios, o localización con desambiguación de coincidencias).

#### Scenario: Regla compartida con una sola implementación
- **WHEN** la misma regla se necesita en el módulo de personas y en el de empresas
- **THEN** existe una única función que la implementa y ambos módulos la reutilizan

#### Scenario: Desambiguación reutilizada
- **WHEN** una operación debe localizar un registro con cero, una o varias coincidencias
- **THEN** usa la misma función de localización y desambiguación que el resto de operaciones
  en lugar de repetir el algoritmo

#### Scenario: Resumen de cambios reutilizado
- **WHEN** se muestran los cambios pendientes al modificar una persona o una empresa
- **THEN** ambos casos usan el mismo helper de resumen de cambios

### Requirement: Contratos coherentes de parámetros y retorno

Las funciones SHALL declarar parámetros explícitos y valores de retorno tipados que
distingan los resultados posibles de una operación (por ejemplo, continuar en el submenú,
volver al menú principal o cancelar) en lugar de un `bool` cuyo significado dependa del
contexto. Módulos equivalentes SHALL usar las mismas convenciones de contrato.

#### Scenario: Resultado explícito de operación
- **WHEN** una operación de menú puede terminar continuando en el submenú o volviendo al
  menú principal por una cancelación
- **THEN** comunica ese resultado con un tipo explícito, no con un `bool` ambiguo

#### Scenario: Mismos contratos entre módulos equivalentes
- **WHEN** se comparan operaciones equivalentes del módulo de personas y del de empresas
- **THEN** declaran parámetros y devuelven resultados siguiendo las mismas convenciones

### Requirement: Sin estado global mutable

El programa SHALL NOT usar variables globales mutables. El estado compartido de la
interacción (por ejemplo, la detección de que la entrada se ha agotado) SHALL encapsularse
en una instancia que se pasa explícitamente a quien la necesita. Las constantes inmutables
y los miembros estáticos sin estado mutable quedan excluidos de esta restricción.

#### Scenario: Estado de entrada encapsulado
- **WHEN** se revisa cómo se detecta que la entrada del usuario se ha agotado
- **THEN** el estado reside en una instancia concreta inyectada en los menús y no en un
  campo estático mutable compartido

#### Scenario: Sin campos estáticos mutables
- **WHEN** se revisan los miembros estáticos del programa
- **THEN** no existe ningún campo o propiedad estática mutable; solo constantes o miembros
  sin estado mutable

### Requirement: Evidencia documentada del reparto de responsabilidades

El repositorio SHALL incluir un `README.md` en la raíz que describa brevemente el reparto de
responsabilidades de las funciones principales y cómo se relacionan las capas de menús,
lectura/validación y gestión.

#### Scenario: README de responsabilidades
- **WHEN** se abre la raíz del repositorio
- **THEN** existe un `README.md` que enumera las funciones principales y explica qué
  responsabilidad cubre cada una y cómo se conectan las capas

#### Scenario: README coherente con el código
- **WHEN** se contrasta el README con la organización real del código
- **THEN** las responsabilidades descritas corresponden a los componentes existentes

### Requirement: Preservación del comportamiento observable

La refactorización SHALL NOT alterar el comportamiento observable del programa: los prompts,
los mensajes, el orden de los menús y el formato de la salida SHALL permanecer idénticos a
la versión anterior, de modo que los guiones de demostración sigan siendo válidos.

#### Scenario: Salida idéntica
- **WHEN** se ejecuta cualquiera de los guiones de demostración documentados
- **THEN** la secuencia de prompts, mensajes y formato coincide exactamente con la versión
  anterior, sin cambios de texto ni de orden

#### Scenario: Flujos funcionales intactos
- **WHEN** se realizan las operaciones de alta, listado, búsqueda, modificación, baja,
  asignación y desvinculación
- **THEN** producen los mismos resultados y mensajes que antes de la refactorización