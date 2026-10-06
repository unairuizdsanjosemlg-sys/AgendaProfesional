# Tasks

## 1. Modelo y validación de empresa

- [x] 1.1 Crear `src/Empresa.cs` con `IdEmpresa` (solo lectura), `NombreComercial`, `CIF`,
      `Telefono`, `CorreoElectronico` y `Direccion` siguiendo el estilo de `Persona.cs`;
      verificar con `dotnet build src/AgendaProfesional.csproj` sin errores.
- [x] 1.2 Añadir `Validaciones.ValidarCif` (obligatorio + formato `letra/dígito + 7 dígitos
      + control`) sin modificar ningún método existente; verificar con `dotnet build` y con
      `git diff src/Validaciones.cs` que solo aparece el predicado nuevo.

## 2. Gestor de empresas

- [x] 2.1 Crear `src/GestorEmpresas.cs` con `AltaEmpresa` (id incremental no reutilizado y
      rechazo de CIF duplicado devolviendo un error claro) al estilo de `GestorPersonas`;
      verificar con `dotnet build` sin errores.
- [x] 2.2 Implementar en `GestorEmpresas` el `Listar()` ordenado por `NombreComercial`
      (OrdinalIgnoreCase) y el `BuscarPorTexto()` (subcadena de nombre con
      `Validaciones.Coincide` o CIF exacto ignorando mayúsculas); verificar con
      `dotnet build` sin errores.
- [x] 2.3 Implementar en `GestorEmpresas` `Modificar` (actualización parcial vacío-conserva,
      con revalidación de unicidad de CIF) y `Eliminar` al estilo de `GestorPersonas`;
      verificar con `dotnet build` sin errores.

## 3. Menú de empresas

- [x] 3.1 Crear `src/MenuEmpresas.cs` con el submenu en bucle (alta, listado, búsqueda,
      modificación, baja, "Volver al menú principal") usando `Validaciones.LeerEntero`;
      verificar ejecutando: entrada no numérica y opción inexistente muestran aviso sin
      cerrar, y "Volver" regresa al menú principal sin salir de la aplicación.
- [x] 3.2 Implementar el alta con lectura campo a campo (`LeerCampoObligatorio`), rechazo de
      CIF duplicado repreguntando solo ese campo, resumen + confirmación y aviso con el id
      asignado; verificar ejecutando: alta válida informa el id, y NombreComercial/CIF
      vacíos, CIF con mal formato o repetido, y teléfono/correo inválidos repreguntan solo
      el campo fallido.
- [x] 3.3 Implementar el listado con cabecera alineada, orden por NombreComercial e id
      visible; verificar ejecutando: con el catálogo vacío muestra "no hay empresas" y con
      empresas la lista sale ordenada y alineada.
- [x] 3.4 Implementar la búsqueda con un único prompt "Nombre comercial o CIF", mostrando
      todas las coincidencias o el mensaje de sin resultados; verificar ejecutando:
      subcadena sin acentos/mayúsculas encuentra, CIF exacto encuentra, y texto sin
      coincidencias informa sin error.
- [x] 3.5 Implementar la modificación (localizar → mostrar datos actuales → vacío-conserva
      → sin cambios → resumen con la empresa afectada → confirmación s/n); verificar
      ejecutando: modificar existente guarda solo lo indicado, localización sin
      coincidencias informa, todos los campos vacíos informa "sin cambios" y responder "n"
      no guarda.
- [x] 3.6 Implementar la baja (localizar → mostrar registro afectado con id → confirmación
      s/n → eliminar definitivamente); verificar ejecutando: confirmada elimina, cancelada
      conserva, y sin coincidencias informa sin error.

## 4. Menú principal e integración con la Fase 1

- [x] 4.1 Reestructurar `src/Program.cs`: menú principal "1. Personas / 2. Empresas /
      3. Salir" con bucle propio para el submenu de personas y "Volver al menú principal"
      (las operaciones de personas quedan sin cambio de comportamiento); verificar
      ejecutando: "Salir" es la única vía de cierre y el texto no numérico u opción
      inexistente del menú principal muestran aviso sin cerrar.
- [x] 4.2 Regresión de la Fase 1: ejecutar y comprobar que alta, listado, búsqueda,
      modificación y baja de personas funcionan igual que con la etiqueta `fase-1`
      (incluidos `cancelar`, vacío-conserva y homónimos); verificar que el submenu de
      personas vuelve al menú principal sin cerrar la aplicación.

## 5. Verificación final

- [x] 5.1 Recorrer el escenario completo de empresas en una sola ejecución: crear dos
      empresas (una con homónimo de nombre), listar, buscar por nombre y por CIF, modificar
      un campo y cancelar una modificación, dar de baja una empresa y cancelar otra, y
      probar entradas inválidas en todos los menús; verificar que no se produce ningún
      cierre inesperado ni excepción.
- [x] 5.2 Redactar el guion de demostración (inputs exactos a teclear) que cubre el
      recorrido del paso 5.1 y los flujos de personas; verificar que el guion se puede
      seguir tal cual descrito.
- [x] 5.3 Verificar con `git tag` que `fase-1` sigue existiendo y crear la etiqueta
      `fase-2` sobre el commit final de la funcionalidad.
