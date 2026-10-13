# Agenda de Consultoría

Aplicación de consola en C# (.NET) para la gestión de personas y empresas colaboradoras de una consultora. Desarrollada por fases con Spec Driven Development (SDD).

## Ejecución

```bash
dotnet run --project src/AgendaProfesional.csproj
```

## Arquitectura y reparto de responsabilidades

El programa se organiza en capas con responsabilidad única:

### Punto de entrada
- **`Program.cs`** — Arranque mínimo: crea las dependencias (`Consola`, gestores) y delega en el coordinador del menú principal.

### Coordinación de menús
- **`MenuPrincipal.cs`** — Bucle del menú principal (Personas / Empresas / Salir) y despacho a submenús.
- **`MenuPersonas.cs`** — Submenú de personas con sus 7 operaciones (alta, listado, búsqueda, modificar, baja, asignar/cambiar empresa, desvincular).
- **`MenuEmpresas.cs`** — Submenú de empresas con sus 6 operaciones (alta, listado, búsqueda, modificar, baja, ver personas de la empresa).

Todos los menús reciben una instancia de `Consola` y devuelven `ResultadoOperacion` (`Continuar` | `VolverAlMenuPrincipal`) para controlar el flujo sin `bool` ambiguos.

### Capa de entrada y validación
- **`Consola.cs`** — Encapsula el estado de entrada (`EntradaAgotada`) y expone métodos de lectura con validación inmediata campo a campo:
  - `LeerCampoObligatorio` / `LeerCampoModificacion` — lectura con repregunta solo del campo fallido, soporte de `cancelar`.
  - `LeerEntero` / `LeerNumeroCancelable` — lectura numérica robusta.
  - `Confirmar` / `ConfirmarCancelable` — confirmaciones s/n con distinción de `cancelar`.
- **`Validaciones.cs`** — Reglas puras (sin estado): predicados de formato (`ValidarCorreo`, `ValidarTelefono`, `ValidarCif`, `ValidarTexto`), normalización (`SinAcentos`, `Coincide`), detección de `cancelar`. Sin variables globales mutables.

### Datos de formulario
- **`PersonaDatos.cs`** — Record con los 5 campos del formulario de persona; `string.Empty` = conservar valor actual en modificación; propiedad `SinCambios`.
- **`EmpresaDatos.cs`** — Equivalente para empresas (5 campos).

### Helpers compartidos (eliminan duplicación)
- **`Ayudas/LocalizarRegistro.cs`** — Localización genérica 0/1/varias coincidencias con desambiguación numerada; usada por personas y empresas.
- **`Ayudas/ResumenCambios.cs`** — Helper único para mostrar resumen de cambios pendientes (comparación ordinal, solo campos con valor nuevo).
- **`Ayudas/ValorIndicadoONuevo.cs`** — Normalización "vacío conserva" compartida por `GestorPersonas` y `GestorEmpresas`.

### Gestores de dominio (sin I/O de consola)
- **`GestorPersonas.cs`** — Lógica de negocio: `AltaPersona`, `Listar`, `BuscarPorNombre`, `Modificar`, `Eliminar`, `BuscarPorId`.
- **`GestorEmpresas.cs`** — Lógica de negocio: `AltaEmpresa`, `Listar`, `BuscarPorTexto`, `Modificar`, `Eliminar`, `ExisteCif`, `BuscarPorId`.
- **`GestorRelaciones.cs`** — Relación 1:N persona-empresa por identificadores: `Asignar`, `Desvincular`, `IdEmpresaDe`, `PersonasDe`, `DesvincularTodas`, `EliminarRelacionesDe`.

### Presentación
- **`Presentacion.cs`** — Formato de salida para consola: `PersonaTexto`, `EmpresaTexto`, `EmpresaDePersona` (resuelve nombre vigente por `IdEmpresa`).

### Modelos
- **`Persona.cs`** / **`Empresa.cs`** — Entidades de dominio con `Id` autogenerado e inmutable.

### Tipos de resultado
- **`ResultadoOperacion.cs`** — `ResultadoOperacion { Continuar, VolverAlMenuPrincipal }` y `Confirmacion { Si, No, Cancelado }`.

## Conexión entre capas

```
Program.cs
    └─ crea Consola + gestores
         └─ MenuPrincipal.Ejecutar(consola, gestores...)
              ├─ MenuPersonas.Ejecutar(consola, gestorPersonas, gestorEmpresas, gestorRelaciones)
              │    ├─ usa Consola para leer/validar
              │    ├─ usa Ayudas.LocalizarRegistro para buscar personas/empresas
              │    ├─ usa Ayudas.ResumenCambios para mostrar cambios
              │    └─ llama a GestorPersonas / GestorRelaciones (lógica pura)
              └─ MenuEmpresas.Ejecutar(consola, gestorEmpresas, gestorPersonas, gestorRelaciones)
                   ├─ usa Consola para leer/validar
                   ├─ usa Ayudas.LocalizarRegistro para buscar empresas
                   ├─ usa Ayudas.ResumenCambios para mostrar cambios
                   └─ llama a GestorEmpresas / GestorRelaciones (lógica pura)
```

## Convenciones de entrada (fijas para todos los módulos)

- Localización por nombre legible (nunca por `Id`).
- Coincidencia parcial, sin distinguir acentos ni mayúsculas/minúsculas.
- Validación inmediata campo a campo (repregunta solo el fallido).
- `cancelar` (cualquier mayúscula) aborta la operación y vuelve al menú principal.
- En modificación, campo en blanco = conserva valor actual.
- Resumen de cambios + confirmación explícita antes de guardar.
- `Id` visible en presentaciones, nunca editable ni solicitado al usuario.

## Verificación

Los guiones de demostración en `docs/demo-fase-2.md` y `docs/demo-fase-3.md` definen la salida esperada byte a byte. Ejecutar la aplicación y seguir los guiones valida la correcta preservación del comportamiento.