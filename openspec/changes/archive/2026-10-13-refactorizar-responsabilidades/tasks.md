# Tasks

## 1. Capa de entrada y tipos compartidos

- [x] 1.1 Crear `src/Consola.cs` con el estado de instancia `EntradaAgotada` y los métodos de lectura/confirmación (`LeerCampoObligatorio`, `LeerCampoModificacion`, `LeerEntero`, `LeerNumeroCancelable`, `Confirmar`, `ConfirmarCancelable`, `LeerLinea`), moviendo los bucles actuales de `Validaciones` sin alterar sus mensajes; verificar con `dotnet build src/AgendaProfesional.csproj` que compila.
- [x] 1.2 Reducir `src/Validaciones.cs` a reglas puras (`ValidarTexto`, `ValidarCorreo`, `ValidarTelefono`, `ValidarCif`, `EsCorreoValido`, `EsTelefonoValido`, `SinAcentos`, `Coincide`, `EsCancelacion`) y constantes, eliminando el estado estático mutable y los métodos de lectura; verificar que no queda ningún campo/propiedad estática mutable y que compila.
- [x] 1.3 Añadir `src/ResultadoOperacion.cs` con `enum ResultadoOperacion { Continuar, VolverAlMenuPrincipal }` y trasladar `enum Confirmacion` desde `Validaciones`; verificar que compila.
- [x] 1.4 Mover `Presentacion` de `Program.cs` a `src/Presentacion.cs` y `PersonaDatos` a `src/PersonaDatos.cs`, y crear `src/EmpresaDatos.cs` con la misma forma que `PersonaDatos` (vacío = conservar) para el formulario de empresa; verificar que compila.
- [x] 1.5 Crear `src/Ayudas/LocalizarRegistro.cs` (localización genérica 0/1/varias con mensaje de "sin coincidencias" y etiqueta parametrizables), `src/Ayudas/ResumenCambios.cs` (helper único de resumen de cambios) y `src/Ayudas/ValorIndicadoONuevo.cs`; verificar que compila.

## 2. Migrar el menú de personas

- [x] 2.1 Crear `src/MenuPersonas.cs` extrayendo de `Program.cs` el bucle del submenú y las operaciones (alta, listado, búsqueda, modificar, baja, asignar/cambiar empresa, desvincular), usando `Consola`, `LocalizarRegistro`, `ResumenCambios` y `ResultadoOperacion`, conservando literales y orden de salida; verificar que compila.
- [x] 2.2 Crear `src/MenuPrincipal.cs` con el bucle del menú principal y el despacho a `MenuPersonas`/`MenuEmpresas`, y dejar `src/Program.cs` como arranque mínimo que crea `Consola`, los tres gestores y llama al coordinador; verificar con `dotnet build src/AgendaProfesional.csproj` y una ejecución manual de humo (menú principal → Personas → Volver → Salir) que arranca y sale correctamente.
- [x] 2.3 Comprobar que no queda lógica de personas en `Program.cs` y que `Program.cs` no supera la veintena de líneas; verificar por inspección del archivo.

## 3. Migrar el menú de empresas

- [x] 3.1 Refactorizar `src/MenuEmpresas.cs` para recibir `Consola` y usar `LocalizarRegistro`, `ResumenCambios`, `EmpresaDatos` y `ResultadoOperacion`, eliminando `LocalizarEmpresa`/`ValidarCifUnico`/`MostrarResumen` duplicados y conservando literales y orden; verificar que compila.
- [x] 3.2 Verificar que empresa y personas comparten una única implementación de localización con desambiguación y de resumen de cambios (sin copias locales); verificar por inspección de `MenuEmpresas.cs` y `MenuPersonas.cs`.

## 4. Deduplicar los gestores

- [x] 4.1 Reemplazar la copia privada `ValorIndicadoONuevo` de `src/GestorPersonas.cs` y `src/GestorEmpresas.cs` por el helper compartido de `src/Ayudas/ValorIndicadoONuevo.cs`; verificar que compila y que cada gestor conserva su comportamiento (un campo vacío mantiene el valor).
- [x] 4.2 Confirmar que ningún gestor escribe en consola ni lee del usuario (solo reciben parámetros y devuelven resultados); verificar por inspección de `src/GestorPersonas.cs`, `src/GestorEmpresas.cs` y `src/GestorRelaciones.cs`.

## 5. README de responsabilidades

- [x] 5.1 Crear `README.md` en la raíz del repositorio describiendo la pila (.NET consola), cómo ejecutar (`dotnet run --project src/AgendaProfesional.csproj`) y el reparto de responsabilidades de las funciones/componentes principales (menús, `Consola`, gestores, `Presentacion`, helpers); verificar que cada componente citado existe en el código.
- [x] 5.2 Verificar que el README explica cómo se conectan las capas (menú → Consola/helpers → gestor → Presentacion) y no contradice la estructura real; verificar por inspección contra los archivos de `src/`.

## 6. Verificación de integración

- [x] 6.1 Ejecutar el flujo completo de `docs/demo-fase-2.md` y comparar prompts, mensajes y formato con la versión previa a la refactorización; verificar que la salida es idéntica.
- [x] 6.2 Ejecutar el flujo completo de `docs/demo-fase-3.md` y comparar prompts, mensajes y formato con la versión previa; verificar que la salida es idéntica.
- [x] 6.3 Revisar que no hay variables globales mutables (sin campos/propiedades estáticas mutables) ni dos implementaciones equivalentes de la misma regla; verificar por búsqueda en `src/`.
