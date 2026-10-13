# Design

## Context

Ver `proposal.md` — Why. Estado actual relevante:

- `src/Program.cs` (624 líneas) mezcla el bucle del menú principal, el submenú de personas,
  la lectura de formularios, las operaciones de gestión de personas, la clase
  `Presentacion` y el record `PersonaDatos`.
- `src/MenuEmpresas.cs` (494 líneas) repite el mismo patrón para empresas.
- `src/Validaciones.cs` es a la vez reglas puras y capa de lectura con el estado global
  mutable estático `EntradaAgotada`.
- Duplicación detectada: `ValorIndicadoONuevo` (idéntica en `GestorPersonas` y
  `GestorEmpresas`); la búsqueda "0/1/varias coincidencias" está implementada tres veces
  (`ResolverEmpresa` en `Program`, `LocalizarEmpresa` en `MenuEmpresas`,
  `LocalizarPersona` en `Program`); el bucle "resumen de cambios" está dos veces
  (`MostrarResumen` de persona y de empresa); los formularios de campos se leen con bucles
  equivalentes y repetidos.
- El proyecto no tiene pruebas automatizadas: la verificación es la ejecución de
  `docs/demo-fase-2.md` y `docs/demo-fase-3.md`, con salida idéntica byte a byte.

## Goals / Non-Goals

**Goals:**

- Repartir el código en capas con responsabilidad única: menús, entrada/validación,
  operaciones de gestión y presentación.
- Dejar `Program.cs` como arranque mínimo y un coordinador breve del menú principal.
- Eliminar la duplicación con helpers compartidos.
- Sustituir `bool` ambiguos por un resultado de operación explícito.
- Eliminar el estado global mutable `EntradaAgotada` encapsulándolo en una instancia.
- Añadir `README.md` con el reparto de responsabilidades.

**Non-Goals:**

- Cambiar cualquier comportamiento observable, texto de prompts o mensajes.
- Añadir funcionalidad nueva, persistencia o empresas/personas adicionales.
- Introducir un framework de pruebas o dependencias externas.
- Reorganizar los proyectos/namespace (`AgendaProfesional`) ni cambiar `Persona`/`Empresa`.

## Decisions

### D1. Estructura de archivos destino

```
src/
  Program.cs            -> arranque: crea dependencias y llama al coordinador (pocas líneas)
  MenuPrincipal.cs      -> coordinador del menú principal (bucle + despacho a submenús)
  MenuPersonas.cs       -> submenú de personas y sus operaciones de menú
  MenuEmpresas.cs       -> submenú de empresas y sus operaciones de menú
  Consola.cs            -> capa de entrada: leer línea, validar campo, cancelar, confirmar,
                           entrada agotada (estado de instancia)
  Presentacion.cs       -> formato de persona y empresa para consola
  PersonaDatos.cs       -> datos de formulario de persona (vacío = conservar)
  EmpresaDatos.cs       -> datos de formulario de empresa (vacío = conservar)
  ResultadoOperacion.cs -> enum Continuar | VolverAlMenuPrincipal (+ Confirmacion)
  Ayudas/
    LocalizarRegistro.cs -> localización con desambiguación 0/1/varias (genérico)
    ResumenCambios.cs    -> helper común de resumen de cambios
    ValorIndicadoONuevo.cs -> normalización "vacío conserva" compartida por los gestores
  Validaciones.cs       -> SOLO reglas puras (predicados y normalización de texto), sin estado
  GestorPersonas.cs     -> lógica de negocio de personas (sin consola)
  GestorEmpresas.cs     -> lógica de negocio de empresas (sin consola)
  GestorRelaciones.cs   -> relación persona-empresa (sin cambios funcionales)
  Persona.cs / Empresa.cs -> modelos (se mantienen)
```

*Alternativa considerada:* un único `App` estático con todos los menús. Rechazada porque
reintroduce acoplamiento y estado estático.

### D2. Inyección de dependencias por parámetro (sin globales)

`Program.cs` crea `Consola`, `GestorPersonas`, `GestorEmpresas` y `GestorRelaciones`, y se
los pasa a `MenuPrincipal.Ejecutar`, que a su vez los pasa a los submenús. `Consola` guarda
`EntradaAgotada` como propiedad de instancia. `Validaciones` deja de tener estado: solo
funciones puras (`ValidarTexto`, `ValidarCorreo`, `ValidarTelefono`, `ValidarCif`,
`EsCorreoValido`, `EsTelefonoValido`, `SinAcentos`, `Coincide`, `EsCancelacion`) y
constantes.

*Alternativa considerada:* mantener `Validaciones` estático con un `Reset()` al inicio.
Rechazada: sigue siendo estado global mutable y difícil de razonar.

### D3. Resultado de operación explícito

Se introduce `enum ResultadoOperacion { Continuar, VolverAlMenuPrincipal }`. Cada operación
de menú lo devuelve en lugar de `bool`:

- `Continuar`: la operación acabó y el submenú sigue mostrándose.
- `VolverAlMenuPrincipal`: llegó `cancelar` o se agotó la entrada; el submenú sale y el
  control vuelve al menú principal.

Esto reproduce exactamente la semántica actual de los `bool` (que hoy significan "seguir /
volver") pero con el significado explícito en el tipo. `Menus` compara el resultado para
decidir si continúa.

*Alternativa considerada:* mantener `bool` con nombres de constantes. Rechazada por
ambigüedad al leer la firma.

### D4. Helpers compartidos

- **Localización genérica**: `LocalizarRegistro<T>(Consola, Func<string, List<T>> buscar,
  Func<T, string> presentar, string etiqueta)` que pide el texto, resuelve 0/1/varias, muestra
  la lista numerada y devuelve `(T? Registro, bool Cancelado, bool SinCoincidencias)`. La usan
  personas y empresas, sustituyendo las tres copias actuales.
- **Resumen de cambios**: `ResumenCambios.Mostrar((string Etiqueta, string Actual, string
  Nuevo)[])` con la condición actual `nuevo.Length > 0 && actual != nuevo` (comparación
  ordinal), usada por persona y empresa.
- **Normalización "vacío conserva"**: `ValorIndicadoONuevo` compartido por ambos gestores.
- **Lectura de formulario**: una función común que recorre una lista de campos
  `(Etiqueta, Mensaje, Validar)` y devuelve los valores (o `null` si se canceló), usada por
  los formularios de persona y de empresa en alta y modificación. `EmpresaDatos` y
  `PersonaDatos` son records construidos por esa función.

### D5. Preservación literal de la salida

Todas las cadenas de `Console.WriteLine` / prompts se trasladan sin editarlas y en el mismo
orden. Donde la refactorización cambie el punto desde el que se imprime, se conserva el
orden observable exacto. Los guiones `docs/demo-fase-2.md` y `docs/demo-fase-3.md` se usan
como prueba de no regresión manual.

## Risks / Trade-offs

- **[Riesgo] Romper la salida byte a byte al mover impresiones** → Mitigación: no reformular
  literales, mantener el orden de escritura y validar con los dos guiones de demostración.
- **[Riesgo] La nueva localización genérica altere el matiz de "sin coincidencias" vs
  "cancelado"** → Mitigación: el helper devuelve explícitamente ambos estados y cada menú
  imprime el mismo mensaje que hoy.
- **[Riesgo] Regresión en el manejo de `EntradaAgotada` (Ctrl+Z / consola cerrada) al pasar
  de estático a instancia** → Mitigación: conservar la misma semántica (al leer `null` se
  marca agotada y todos los bucles salen), con un único propietario del estado.
- **[Riesgo] La refactorización es amplia y sin pruebas automatizadas** → Mitigación:
  avanzar por pasos, compilar tras cada paso y ejecutar los guiones al final. La falta de
  pruebas automatizadas se acepta como non-goal (no se introduce framework).
- **[Trade-off] Más archivos y más tipos (`ResultadoOperacion`, `EmpresaDatos`)** →
  aceptado a cambio de responsabilidades separadas y contratos explícitos, que es el
  objetivo del cambio.

## Migration Plan

1. Crear los componentes nuevos (`Consola`, `Presentacion`, `ResultadoOperacion`, records y
   helpers) extrayendo el código existente sin alterar literales.
2. Migrar `Program.cs`: mover el submenú de personas a `MenuPersonas`, extraer
   `MenuPrincipal` y dejar `Program.cs` como arranque.
3. Migrar `MenuEmpresas` a `Consola` y a los helpers compartidos.
4. Aplicar los helpers en los gestores (`ValorIndicadoONuevo`).
5. Compilar y ejecutar los guiones de demostración comparando la salida.
6. Escribir el `README.md` de responsabilidades.

Rollback: al ser un cambio puramente estructural y estar en git, revertir el commit de la
refactorización restaura la versión anterior.

## Open Questions

- Ninguna que afecte a las specs, al enfoque o al desglose de tareas.
