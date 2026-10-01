# Tasks

Orden de implementación: primero las validaciones (se reusan en todas las capas), luego la
lógica de negocio, después los flujos de consola y por último la verificación manual. Sin
tareas de test automatizado: la verificación de esta fase es manual (`spec.md` §10).

## 1. Validaciones (`src/Validaciones.cs`)

- [x] 1.1 Añadir el helper de plegado de acentos (mayúsculas invariantes + `FormD` +
      eliminación de `NonSpacingMark`) y comprobar con `dotnet run` que comparar `"garcia"`
      con `"García"` devuelve coincidencia.
- [x] 1.2 Sustituir `EsTelefonoValido` por la regla con prefijo internacional (regex
      `^\+\d{1,3} ?\d{6,12}$`, constante para los rangos) y verificar que se aceptan
      `+34600111222`, `+34 600111222`, `+1 2125550123` y `+346001112223333` (15 dígitos,
      límite máximo), y se rechazan `600111222`, `+34600`, `+34  600111222` (dos espacios),
      `+346001112abc` y `+3460011122233333` (16 dígitos).
- [x] 1.3 Añadir el helper de lectura de línea que detecta la palabra reservada `cancelar`
      (tras `Trim()`, sin distinguir mayúsculas) antes de validar, y verificar que `cancelar`,
      `Cancelar` y `  cancelar  ` devuelven la señal de cancelación.
- [x] 1.4 Añadir el helper de campo obligatorio: bucle que valida una línea con un predicado y
      un mensaje de error en español, repreguntando **solo** ese campo, y verificar que un
      correo sin `@` se repregunta sin volver a pedir nombre, apellidos, teléfono ni empresa.
- [x] 1.5 Añadir el helper de campo "vacío conserva el valor": devuelve el valor actual cuando la
      línea está vacía, valida en el acto cualquier respuesta no vacía y repregunta solo el
      campo fallido.
- [x] 1.6 Extender la lectura numérica y la confirmación s/n al mismo bucle de re-pregunta
      (número de homónimo fuera de rango, respuesta distinta de s/n) y verificar que ninguna
      entrada inválida cierra el programa.

## 2. Lógica de negocio (`src/GestorPersonas.cs`)

- [x] 2.1 Añadir la búsqueda por nombre y apellidos (dos campos, condición AND, coincidencia
      parcial con plegado de acentos) y comprobar con `dotnet run` que devuelve todas las
      coincidencias, incluidas las escritas sin acentos.
- [x] 2.2 Retirar `BuscarPorTexto` (búsqueda por texto libre con condición OR) y comprobar que
      el proyecto sigue compilando y que el listado (opción 2) no ha cambiado.
- [x] 2.3 Convertir `Modificar` en una actualización parcial (solo los campos indicados) y
      verificar que los campos no indicados conservan su valor anterior.
- [x] 2.4 Confirmar que `Persona.cs` no necesita cambios (mismas propiedades, sin campos nuevos)
      y que `EmpresaAsignada` sigue siendo una propiedad asignable para la Fase 2.

## 3. Flujos de consola (`src/Program.cs`)

- [x] 3.1 Implementar la rutina compartida de localización por nombre y apellidos: 0 resultados →
      mensaje explícito y vuelta al menú; 1 resultado → se usa directamente; varios → lista
      temporal numerada (1, 2, 3...) con `IdPersona` visible y selección validada.
- [x] 3.2 Reescribir la búsqueda (opción 3) sin el sub-menú "Buscar por Id": preguntar Nombre y
      Apellidos, mostrar todas las coincidencias e informar si no hay ninguna.
- [x] 3.3 Reescribir la modificación (opción 4): localizar por nombre, mostrar los datos
      actuales, pedir los cinco campos con "vacío conserva", avisar si no hay cambios, mostrar el
      resumen de cambios y pedir confirmación s/n antes de guardar.
- [x] 3.4 Reescribir la baja (opción 5): localizar por nombre, mostrar el registro afectado con
      `IdPersona` y mantener la confirmación s/n antes de eliminar.
- [x] 3.5 Aplicar la validación inmediata por campo (R4) en el alta y en la modificación, y
      propagar `cancelar` en todas las operaciones (alta, búsqueda, homónimos, modificación,
      baja y confirmaciones) sin guardar nada.
- [x] 3.6 Actualizar los textos de consola en español: indicar el formato del teléfono con
      prefijo, anunciar la palabra reservada `cancelar`, quitar el marcador `-` de empresa (ya
      no hay campos vacíos) y mantener `IdPersona` visible en listado, resultados, selección y
      confirmaciones.

## 4. Documentación de proyecto

- [x] 4.1 Actualizar `openspec/project.md` para que referencie las convenciones permanentes
      (`convenciones-entrada`) como reglas de todo el proyecto y refleje el nuevo formato de
      teléfono y la obligatoriedad de `EmpresaAsignada`.

## 5. Verificación manual

- [x] 5.1 Ejecutar `dotnet build src/AgendaProfesional.csproj` en `src` y confirmar que compila
      sin errores ni avisos nuevos.
- [x] 5.2 Recorrer la lista manual que ejercita **todos** los escenarios de
      `specs/personas/spec.md` y `specs/convenciones-entrada/spec.md`: alta válida; alta con
      campo vacío; alta con teléfono sin prefijo; alta con correo inválido (repregunta solo ese
      campo); `cancelar` en el alta; listado con y sin personas; búsqueda por subcadena, sin
      acentos, sin mayúsculas, sin coincidencias y con un campo que no coincide (AND);
      búsqueda con varias coincidencias; modificación de un único campo conservando el resto;
      modificación con todos los campos vacíos ("no hay cambios"); modificación con resumen y
      confirmación s/n; modificación denegada; homónimo: lista numerada, selección válida y
      selección fuera de rango; baja confirmada, baja cancelada y baja sin coincidencias;
      entrada no numérica en la opción de menú; respuesta distinta de s/n; `cancelar` en búsqueda,
      homónimos, modificación y confirmación.
- [x] 5.3 Comprobar en la misma ejecución que `IdPersona` se ve en listado, resultados, lista de
      homónimos y confirmación de baja, y que nunca se pide como dato de entrada ni cambia al
      modificar.
- [x] 5.4 Ejecutar la demo completa del criterio de aceptación de `spec.md` §10 (alta, búsqueda,
      modificación y baja con sus confirmaciones y validaciones) y anotarlo como evidencia de la
      Fase 1.1.