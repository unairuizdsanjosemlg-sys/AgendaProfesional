# Spec Delta

## MODIFIED Requirements

### Requirement: Menú principal en bucle
El sistema SHALL mostrar un menú principal con opciones numeradas y claras ("Personas",
"Empresas" y "Salir") y SHALL repetirlo en bucle hasta que el usuario elija "Salir". Elegir
"Personas" SHALL abrir un submenu con las operaciones sobre personas (alta, listado,
búsqueda, modificación, baja y "Volver al menú principal"), el cual SHALL permanecer en
bucle hasta que el usuario elija "Volver al menú principal", devolviendo el control al menú
principal sin cerrar la aplicación. Solo la opción "Salir" del menú principal SHALL
finalizar el programa. Las opciones de búsqueda, modificación y baja del submenu de
personas SHALL localizar a la persona por nombre y apellidos, y el menú de búsqueda SHALL
NOT ofrecer ninguna alternativa de búsqueda por identificador.

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
