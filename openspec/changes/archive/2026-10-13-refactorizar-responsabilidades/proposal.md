# Proposal

## Why

`Program.cs` concentra el bucle principal, los menús, la lectura/validación de campos y
todas las operaciones de gestión de personas, mientras que `MenuEmpresas.cs` replica ese
mismo patrón para empresas. Esto mezcla responsabilidades, duplica lógica (resúmenes de
cambios, `ValorIndicadoONuevo`, desambiguación de coincidencias, lectura de formularios) y
deja el punto de entrada demasiado cargado, lo que dificulta mantener y extender la agenda.

## What Changes

- Mantener `Program.cs` (instrucciones de nivel superior) como mero arranque que delega en
  un coordinador breve del menú principal; el coordinador solo encadena flujo, no contiene
  reglas de negocio.
- Separar en componentes con responsabilidad única: **menús** (principal, personas,
  empresas), **lectura y validación de entrada** y **operaciones de gestión** sobre los
  gestores.
- Sustituir el estado global mutable `Validaciones.EntradaAgotada` por un componente de
  consola con estado de instancia, inyectado en los menús (sin variables globales salvo
  justificación documentada).
- Mover la presentación (`Presentacion`) y los datos de formulario (`PersonaDatos`) a sus
  propios archivos fuera de `Program.cs`.
- Eliminar duplicación con helpers compartidos: resumen de cambios, localización de un
  registro con desambiguación (0/1/varias), secuencias de lectura de campos y despacho de
  confirmaciones s/n/cancelar.
- Unificar los contratos de parámetros y valores de retorno con un tipo de resultado
  explícito en lugar de `bool` ambiguos.
- Añadir `README.md` en la raíz del repositorio con el reparto de responsabilidades de las
  funciones principales (evidencia).
- Sin cambios de comportamiento: mismos prompts, mensajes, orden de menús y formato de
  salida que la versión actual (idénticos byte a byte).

## Capabilities

### New Capabilities

- `estructura-codigo`: requisitos de organización interna y mantenibilidad del programa;
  responsabilidad única por función con nombres descriptivos, separación de capas (menús,
  lectura/validación y operaciones de gestión), `main` breve que solo coordina, ausencia de
  código duplicado, contratos coherentes de parámetros y retorno, ausencia de estado global
  mutable, y un README de reparto de responsabilidades como evidencia.

### Modified Capabilities

- Ninguna: la refactorización no altera ningún comportamiento observable, por lo que no
  cambian los requisitos de `personas`, `empresas`, `persona-empresa` ni
  `convenciones-entrada`.

## Impact

- Código afectado: `src/Program.cs`, `src/MenuEmpresas.cs`, `src/Validaciones.cs`,
  `src/GestorPersonas.cs`, `src/GestorEmpresas.cs`, `src/GestorRelaciones.cs`, más archivos
  nuevos para los menús, la entrada de consola, la presentación y los helpers compartidos.
- Nuevo archivo de evidencia: `README.md` en la raíz del repositorio.
- Modelos de dominio `Persona` y `Empresa`: sus miembros públicos se mantienen.
- Sin dependencias nuevas ni cambios en `docs/demo-fase-2.md` y `docs/demo-fase-3.md`: al
  preservarse la salida byte a byte, los guiones siguen siendo válidos.
