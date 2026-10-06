# Tasks

## 1. Setup del proyecto

- [x] 1.1 Crear el proyecto de consola con `dotnet new console -n AgendaProfesional -o src` y verificar que `dotnet build src/AgendaProfesional.csproj` compila sin errores.

## 2. Modelo de datos

- [x] 2.1 Implementar `src/Persona.cs` (IdPersona de solo lectura, Nombre, Apellidos, Telefono, Correo, EmpresaAsignada) y verificar que `dotnet build src/AgendaProfesional.csproj` compila sin errores y que `IdPersona` no es editable externamente.

## 3. Validaciones

- [x] 3.1 Implementar en `src/Validaciones.cs` la validación de correo (FR-013: no vacío, contiene `@`, dominio con punto) y verificar manualmente con `dotnet run` que un correo válido se acepta y uno vacío/sin `@`/sin punto se rechaza con mensaje claro.
- [x] 3.2 Implementar la validación de teléfono (FR-014: no vacío, solo dígitos, longitud 9) y la lectura numérica robusta (`int.TryParse` en bucle), y verificar manualmente con `dotnet run` que teléfonos vacíos, con letras o de longitud distinta de 9 se rechazan y que el texto no numérico no rompe el menú.

## 4. Lógica de negocio

- [x] 4.1 Implementar `src/GestorPersonas.cs` con `AltaPersona` (IdPersona autogenerado e incremental) y su almacenamiento interno en memoria, y verificar que `dotnet build src/AgendaProfesional.csproj` compila sin errores.
- [x] 4.2 Implementar `Listar` (ordenado por Apellidos y Nombre), `BuscarPorId` y `BuscarPorTexto` (coincidencia parcial sin distinguir mayúsculas/minúsculas), y verificar comportamiento en la demo del grupo 6.
- [x] 4.3 Implementar `Modificar` y `Eliminar` (con confirmación en la capa de consola), y verificar comportamiento en la demo del grupo 6.

## 5. Interfaz de consola

- [x] 5.1 Implementar en `src/Program.cs` el bucle del menú principal (1 Alta, 2 Listado, 3 Búsqueda, 4 Modificar, 5 Baja, 6 Salir) con aviso y repetición ante entrada no numérica u opción inexistente, y verificar que `dotnet run` arranca el menú y no se cierra con entradas inválidas.
- [x] 5.2 Implementar el alta por consola (solicitar los cinco campos, validar antes de guardar, confirmar el alta) y verificar manualmente con `dotnet run` los tres escenarios del requisito "Alta de persona".
- [x] 5.3 Implementar listado y búsquedas por consola y verificar manualmente con `dotnet run` los escenarios de listado con/sin personas y de búsqueda por Id y por texto con/sin coincidencias.
- [x] 5.4 Implementar modificación y baja por consola (mostrar datos actuales, confirmación explícita s/n antes de guardar/eliminar, informar si el Id no existe) y verificar manualmente con `dotnet run` los escenarios de ambos requisitos.

## 6. Verificación y entrega

- [x] 6.1 Ejecutar la demo completa del criterio de aceptación de la Fase 1 (un alta, una búsqueda, una modificación y una baja con sus confirmaciones y validaciones) y verificar que todos los escenarios de `specs/personas/spec.md` se cumplen.
- [x] 6.2 Crear la etiqueta git `fase-1` sobre el estado verificado y comprobar que el proyecto sigue compilando y arranca desde ese tag (NFR-003).