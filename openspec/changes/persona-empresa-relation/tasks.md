# Tasks

## 1. Núcleo de la relación

- [x] 1.1 Crear `src/GestorRelaciones.cs` con un array de pares `record struct RelacionAsignacion(int IdPersona, int IdEmpresa)` y las operaciones `Asignar`, `Desvincular`, `IdEmpresaDe`, `TienePersonas`, `PersonasDe`, `DesvincularTodas` y `EliminarRelacionesDe`; verificar con `dotnet build` que compila sin errores
- [x] 1.2 Hacer público `BuscarPorId` en `GestorPersonas` y `GestorEmpresas` para resolver nombres desde la presentación; verificar con `dotnet build` que el resto del código sigue compilando

## 2. Presentación de la empresa en personas

- [x] 2.1 Actualizar `PersonaTexto` y el formato del listado en `Program.cs` para mostrar primero el nombre comercial resuelto por `IdEmpresa` y después el `IdEmpresa` (o "Sin empresa" cuando no hay relación), pasándole `GestorRelaciones` y `GestorEmpresas`; verificar manualmente que el listado, la búsqueda y la confirmación de baja muestran "Sin empresa" para las personas aún sin relación

## 3. Alta y modificación con empresa validada

- [x] 3.1 Implementar en `Program.cs` el helper de selección de empresa (pedir nombre → `BuscarPorTexto` → 0: aviso; 1: usarla; N: lista numerada con `IdEmpresa` y `LeerNumeroCancelable`); verificar con `dotnet build`
- [x] 3.2 Ajustar el alta de persona: leer los 4 campos obligatorios con el flujo actual y la empresa con bucle propio (vacío aceptado = sin empresa; nombre inexistente: aviso y repregunta solo ese campo; varias coincidencias: desambiguar con lista; `cancelar`: aborta); al confirmar, si hubo empresa, crear la relación con `Asignar`; verificar manualmente los cuatro caminos (vacío, inexistente, ambigua y existente) comprobando en el listado de personas (grupo 2) que aparece el nombre y el `IdEmpresa` de la empresa elegida
- [x] 3.3 Ajustar la modificación de persona: en blanco conserva valor y relación; nombre de empresa válido sustituye la relación y el texto tras la confirmación del resumen; verificar manualmente que cambiar la empresa actualiza la relación y que dejarlo en blanco no la toca
- [x] 3.4 Hacer que la baja de persona llame a `EliminarRelacionesDe` antes de quitar la persona; verificar con `dotnet build` y dejar la comprobación de que la consulta de plantilla (grupo 5) no lista a esa persona para la integración del grupo 7
- [x] 3.5 Actualizar `docs/demo-fase-2.md` (y `spec.md` si menciona la empresa como obligatoria) donde cambian la opcionalidad del campo empresa y los rangos de menú; verificar que cada prompt y aviso documentado coincide con lo que muestra el programa

## 4. Asignar y desvincular desde el menú de personas

- [x] 4.1 Renumerar el submenu de personas a 8 opciones: añadir "6. Asignar/cambiar empresa" y "7. Desvincular empresa", dejando "8. Volver al menú principal"; verificar manualmente que el menú muestra las opciones nuevas y que las opciones 1-5 siguen abriendo los flujos anteriores
- [x] 4.2 Implementar "Asignar/cambiar empresa": `LocalizarPersona` → helper de selección de empresa → resumen (persona + empresa elegida) → `ConfirmarCancelable` → `Asignar` y actualización del texto; verificar manualmente los escenarios de asignación a persona sin empresa, cambio de empresa, empresa sin coincidencias, respuesta "n" y `cancelar`
- [x] 4.3 Implementar "Desvincular empresa": `LocalizarPersona` → si no tiene empresa, aviso y vuelta al submenu; si tiene, mostrar persona con empresa → `ConfirmarCancelable` → `Desvincular` y vaciado del texto `EmpresaAsignada`; verificar manualmente la desvinculación confirmada, la respuesta "n" y el aviso para persona sin empresa

## 5. Plantilla desde el menú de empresas

- [x] 5.1 Renumerar el submenu de empresas y añadir "6. Ver personas de la empresa" con "7. Volver al menú principal"; verificar manualmente que el resto de opciones 1-5 siguen funcionando
- [x] 5.2 Implementar la consulta: localizar la empresa con la búsqueda existente y listar sus personas con el mismo formato del listado de personas; mensaje explícito si no tiene ninguna; verificar manualmente los escenarios con plantilla, empresa vacía, localización sin coincidencias y `cancelar`

## 6. Baja de empresa protegida

- [x] 6.1 Implementar en `MenuEmpresas.Baja` el flujo protegido: tras confirmar "s", si `TienePersonas` mostrar aviso con el número de personas y las dos opciones (1 = cancelar el borrado, 2 = desvincular a todas para poder borrar) con `LeerNumeroCancelable`; la opción 2 exige además `ConfirmarCancelable` y ejecuta `DesvincularTodas` (vaciando el texto de cada persona) + `Eliminar`; sin personas, mantener el flujo actual; verificar manualmente el bloqueo, el borrado cancelado, la desvinculación total con confirmación ("s" y "n") y el borrado normal de una empresa sin plantilla

## 7. Verificación de integración

- [x] 7.1 Ejecutar `dotnet build` y confirmar compilación sin errores; recorrer a mano la regresión de los flujos de personas y empresas de las fases anteriores (altas, listados, búsquedas, modificaciones y bajas sin relación) comprobando que no han cambiado
- [x] 7.2 Documentar el recorrido completo de la relación persona-empresa en `docs/demo-fase-3.md` siguiendo el formato de tablas de `docs/demo-fase-2.md` y ejecutarlo de principio a fin, verificando que cada entrada produce el resultado esperado
