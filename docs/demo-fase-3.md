# Guion de demostración — Fase 3 (Relación persona-empresa)

Recorrido completo en una sola ejecución de la relación 1:N entre personas y empresas:
alta de personas con empresa opcional y validada (vacía, inexistente, ambigua y
existente), modificación (cambio de empresa y en blanco conserva la relación),
"asignar/cambiar empresa" y "desvincular empresa" desde el menú de personas, consulta
de plantilla desde el menú de empresas y borrado protegido de empresas con personas
relacionadas.

Cómo ejecutar:

```
dotnet run --project src/AgendaProfesional.csproj
```

Convenciones del guion:

- Cada línea es lo que hay que teclear seguido de <kbd>Enter</kbd>.
- `⟨vacío⟩` significa pulsar <kbd>Enter</kbd> sin escribir nada.
- Las líneas se leen en orden; el prompt indicado es el que espera esa entrada.

Los menús de esta fase son: personas `1-8` (6. Asignar/cambiar empresa, 7. Desvincular
empresa, 8. Volver) y empresas `1-7` (6. Ver personas de la empresa, 7. Volver).

## A. Preparación: altas de empresas

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `2` | `=== GESTIÓN DE EMPRESAS ===` |
| `Elige una opción (1-7):` | `1` | Alta |
| `Nombre comercial:` | `Casa Soluciones SL` | |
| `CIF ...:` | `A1234567X` | |
| `Teléfono ...:` | `+34 912345678` | |
| `Correo:` | `casa@soluciones.es` | |
| `Dirección:` | `Calle Mayor 1` | Resumen |
| `¿Confirmar el alta? (s/n):` | `s` | "Alta confirmada. Id asignado: 1." |
| `Elige una opción (1-7):` | `1` | Alta |
| `Nombre comercial:` | `Casa Del Norte SL` | |
| `CIF ...:` | `B2345678Y` | |
| `Teléfono ...:` | `+34 987654321` | |
| `Correo:` | `norte@corp.es` | |
| `Dirección:` | `Calle Menor 2` | Resumen |
| `¿Confirmar el alta? (s/n):` | `s` | "Alta confirmada. Id asignado: 2." |
| `Elige una opción (1-7):` | `7` | Vuelve al menú principal |

## B. Alta de persona con empresa opcional (4 caminos)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `1` | `=== GESTIÓN DE PERSONAS ===` |
| `Elige una opción (1-8):` | `1` | Alta |
| `Nombre/Apellidos/Teléfono/Correo:` | `Ana` / `López` / `+34 600111222` / `ana@test.com` | |
| `Empresa asignada:` | `⟨vacío⟩` | Alta **sin empresa**: "Alta confirmada. Id asignado: 1." |
| `Elige una opción (1-8):` | `1` | Alta |
| `Nombre/Apellidos/Teléfono/Correo:` | `Luis` / `García` / `+34 600111333` / `luis@test.com` | |
| `Empresa asignada:` | `Casa` | Lista numerada con las 2 candidatas (coincidencias parciales) |
| `Número de la empresa (1-2):` | `1` | Elige Casa Soluciones SL |
| (sin confirmación) | — | "Alta confirmada. Id asignado: 2." y relación con Id 1 |
| `Elige una opción (1-8):` | `1` | Alta |
| `Nombre/Apellidos/Teléfono/Correo:` | `Eva` / `Ruiz` / `+34 600111444` / `eva@test.com` | |
| `Empresa asignada:` | `Inexistente` | "No se ha encontrado ninguna empresa con ese nombre." y **repregunta solo ese campo** |
| `Empresa asignada:` | `⟨vacío⟩` | Alta sin empresa: "Alta confirmada. Id asignado: 3." |
| `Elige una opción (1-8):` | `1` | Alta |
| `Nombre/Apellidos/Teléfono/Correo:` | `Sara` / `Jiménez` / `+34 600111555` / `sara@test.com` | |
| `Empresa asignada:` | `⟨vacío⟩` | "Alta confirmada. Id asignado: 4." |
| `Elige una opción (1-8):` | `2` | Listado ordenado por apellidos: Luis → "Empresa: Casa Soluciones SL (Id 1)", el resto → "Empresa: Sin empresa" |
| `Elige una opción (1-8):` | `3` | Búsqueda |
| `Nombre:` / `Apellidos:` | `ana` / `lópez` | 1 coincidencia mostrando "Sin empresa" |

> Comprobación del formato del listado: se muestra primero el nombre comercial
> resuelto por `IdEmpresa` y, a continuación, el `IdEmpresa` entre paréntesis.

## C. Modificación de persona con empresa

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-8):` | `4` | Modificar |
| `Nombre:` / `Apellidos:` | `Ana` / `López` | Datos actuales |
| los 4 primeros campos | `⟨vacío⟩` ×4 | Conservan |
| `Empresa asignada:` | `Casa Del Norte` | Resumen: `Empresa asignada: Sin empresa -> Casa Del Norte SL` |
| `¿Guardar los cambios? (s/n):` | `s` | "Persona modificada correctamente." y relación nueva con Id 2 |
| `Elige una opción (1-8):` | `4` | Modificar |
| `Nombre:` / `Apellidos:` | `Luis` / `García` | Datos actuales |
| `Nombre:` | `Luis Miguel` | Cambio de nombre |
| los otros 3 campos + empresa | `⟨vacío⟩` ×4 | Conservan (la empresa en blanco **no toca la relación**) |
| `¿Guardar los cambios? (s/n):` | `s` | Resumen solo con el nombre; relación con Casa Soluciones intacta |
| `Elige una opción (1-8):` | `4` | Modificar |
| `Nombre:` / `Apellidos:` | `Eva` / `Ruiz` | Datos actuales |
| los 4 primeros campos | `⟨vacío⟩` ×4 | Conservan |
| `Empresa asignada:` | `Casa` | Lista numerada (ambigüedad) |
| `Número de la empresa (1-2):` | `2` | Resumen: `Empresa asignada: Sin empresa -> Casa Del Norte SL` |
| `¿Guardar los cambios? (s/n):` | `s` | "Persona modificada correctamente." |
| `Elige una opción (1-8):` | `2` | Listado: Ana y Eva → Casa Del Norte SL (Id 2); Luis Miguel → Casa Soluciones SL (Id 1); Sara → Sin empresa |

## D. Plantilla antes de la baja (empresa con 1 persona)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-8):` | `8` | Vuelve al menú principal |
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-7):` | `6` | Ver personas de la empresa |
| `Nombre comercial o CIF:` | `Casa Soluciones` | "Personas asociadas (1):" con Luis Miguel García |

## E. Baja de persona: quita su relación

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-7):` | `7` | Vuelve al menú principal |
| `Elige una opción (1-3):` | `1` | Personas |
| `Elige una opción (1-8):` | `5` | Baja |
| `Nombre:` / `Apellidos:` | `Luis` / `García` | Registro afectado (muestra Casa Soluciones SL Id 1) |
| `¿Eliminar definitivamente? (s/n):` | `s` | "Persona eliminada." y su relación eliminada |
| `Elige una opción (1-8):` | `8` | Vuelve al menú principal |
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-7):` | `6` | Ver personas de la empresa |
| `Nombre comercial o CIF:` | `Casa Soluciones` | "Esta empresa no tiene personas asignadas." (la baja de Luis ya no aparece) |

## F. Asignar/cambiar empresa (5 escenarios)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-7):` | `7` | Vuelve al menú principal |
| `Elige una opción (1-3):` | `1` | Personas |
| `Elige una opción (1-8):` | `6` | Asignar/cambiar empresa |
| `Nombre:` / `Apellidos:` | `Sara` / `Jiménez` | Persona sin empresa |
| `Nombre de la empresa:` | `Casa Del Norte` | Resumen de asignación |
| `¿Guardar la asignación? (s/n):` | `s` | "Persona asignada a la empresa 'Casa Del Norte SL'." |
| `Elige una opción (1-8):` | `6` | Asignar/cambiar empresa |
| `Nombre:` / `Apellidos:` | `Ana` / `López` | Persona con Casa Del Norte |
| `Nombre de la empresa:` | `Casa Soluciones` | **Cambio** de empresa |
| `¿Guardar la asignación? (s/n):` | `s` | "Persona asignada a la empresa 'Casa Soluciones SL'." |
| `Elige una opción (1-8):` | `6` | Asignar/cambiar empresa |
| `Nombre:` / `Apellidos:` | `Ana` / `López` | |
| `Nombre de la empresa:` | `XYZ Empresa Inexistente` | Aviso "No se ha encontrado ninguna empresa con ese nombre."; "No se ha realizado ningún cambio." y vuelve al submenu |
| `Elige una opción (1-8):` | `6` | Asignar/cambiar empresa |
| `Nombre:` / `Apellidos:` | `Ana` / `López` | |
| `Nombre de la empresa:` | `Casa Del Norte` | Resumen |
| `¿Guardar la asignación? (s/n):` | `n` | "Asignación cancelada." y sigue en el submenu (Ana sigue en Casa Soluciones) |
| `Elige una opción (1-8):` | `6` | Asignar/cambiar empresa |
| `Nombre:` / `Apellidos:` | `Ana` / `López` | |
| `Nombre de la empresa:` | `cancelar` | "Operación cancelada." y vuelve al **menú principal** |

## G. Desvincular empresa (3 escenarios)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `1` | Personas |
| `Elige una opción (1-8):` | `7` | Desvincular empresa |
| `Nombre:` / `Apellidos:` | `Eva` / `Ruiz` | Muestra a Eva con Casa Del Norte |
| `¿Desvincular a la persona de su empresa? (s/n):` | `s` | "Persona desvinculada de su empresa." y texto `EmpresaAsignada` vaciado |
| `Elige una opción (1-8):` | `7` | Desvincular empresa |
| `Nombre:` / `Apellidos:` | `Sara` / `Jiménez` | (persona sin empresa) |
| (sin confirmación) | — | "Esa persona no tiene empresa asignada." y vuelve al submenu |
| `Elige una opción (1-8):` | `7` | Desvincular empresa |
| `Nombre:` / `Apellidos:` | `Ana` / `López` | Muestra a Ana con Casa Soluciones |
| `¿Desvincular a la persona de su empresa? (s/n):` | `n` | "Desvinculación cancelada." (Ana sigue vinculada) |
| `Elige una opción (1-8):` | `2` | Listado: Eva → Sin empresa; Ana → Casa Soluciones SL (Id 1); Sara → Casa Del Norte SL (Id 2) |

### G2. Baja de persona sin relación

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-8):` | `5` | Baja |
| `Nombre:` / `Apellidos:` | `Sara` / `Jiménez` | Registro afectado |
| `¿Eliminar definitivamente? (s/n):` | `s` | "Persona eliminada." |
| `Elige una opción (1-8):` | `8` | Vuelve al menú principal |

## H. Consulta de plantilla desde empresas

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-7):` | `6` | Ver personas de la empresa |
| `Nombre comercial o CIF:` | `Casa Soluciones` | "Personas asociadas (1):" con Ana López |
| `Elige una opción (1-7):` | `6` | Ver personas de la empresa |
| `Nombre comercial o CIF:` | `Casa Del Norte` | "Personas asociadas (1):" con Sara Jiménez |
| `Elige una opción (1-7):` | `6` | Ver personas de la empresa |
| `Nombre comercial o CIF:` | `zzz` | "No se ha encontrado ninguna empresa con ese nombre o CIF." |
| `Elige una opción (1-7):` | `6` | Ver personas de la empresa |
| `Nombre comercial o CIF:` | `cancelar` | "Operación cancelada." y vuelve al **menú principal** |

## I. Baja de empresa protegida

### I1. Bloqueo y opción 1 (cancelar el borrado)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-7):` | `5` | Baja |
| `Nombre comercial o CIF:` | `Casa Del Norte` | Registro afectado (tiene 1 persona) |
| `¿Eliminar definitivamente? (s/n):` | `s` | Aviso "Esta empresa tiene 1 persona relacionada." + las 2 opciones |
| `Elige una opción (1-2):` | `1` | "Borrado cancelado. La empresa y sus relaciones se mantienen." |

### I2. Opción 2 con "n" en la doble confirmación

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-7):` | `5` | Baja |
| `Nombre comercial o CIF:` | `Casa Del Norte` | Registro afectado |
| `¿Eliminar definitivamente? (s/n):` | `s` | Aviso + 2 opciones |
| `Elige una opción (1-2):` | `2` | Pide confirmación extra |
| `¿Desvincular a las 1 persona relacionada y borrar la empresa? (s/n):` | `n` | "Borrado cancelado. La empresa no se ha eliminado." |

### I3. Opción 2 con "s": desvincula a todas y borra

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-7):` | `5` | Baja |
| `Nombre comercial o CIF:` | `Casa Del Norte` | Registro afectado |
| `¿Eliminar definitivamente? (s/n):` | `s` | Aviso + 2 opciones |
| `Elige una opción (1-2):` | `2` | Pide confirmación extra |
| `¿Desvincular a las 1 persona relacionada y borrar la empresa? (s/n):` | `s` | "Empresa 'Casa Del Norte SL' eliminada tras desvincular a sus 1 persona relacionada." y el texto `EmpresaAsignada` de Sara queda vaciado |
| `Elige una opción (1-7):` | `2` | Listado: quedan Casa Soluciones SL e Id de la tercera alta |

### I4. Borrado normal de una empresa sin plantilla

> Para no dejar la agenda vacía, se da de alta previamente una tercera empresa
> (sin personas) que se borra por el flujo habitual.

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-7):` | `5` | Baja |
| `Nombre comercial o CIF:` | `Energía Limpia` | Registro afectado (sin personas) |
| `¿Eliminar definitivamente? (s/n):` | `s` | "Empresa eliminada." (sin bloqueo) |

### I5. `cancelar` y confirmación final con desvinculación

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-7):` | `5` | Baja |
| `Nombre comercial o CIF:` | `Casa Soluciones` | Registro afectado (1 persona: Ana) |
| `¿Eliminar definitivamente? (s/n):` | `s` | Aviso + 2 opciones |
| `Elige una opción (1-2):` | `cancelar` | "Borrado cancelado." y vuelve al **menú principal** |
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-7):` | `5` | Baja |
| `Nombre comercial o CIF:` | `Casa Soluciones` | Registro afectado |
| `¿Eliminar definitivamente? (s/n):` | `s` | Aviso + 2 opciones |
| `Elige una opción (1-2):` | `2` | Confirmación extra |
| `¿Desvincular a las 1 persona relacionada y borrar la empresa? (s/n):` | `s` | "Empresa 'Casa Soluciones SL' eliminada tras desvincular a sus 1 persona relacionada." |

## J. Comprobación final del vaciado de texto

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-7):` | `7` | Vuelve al menú principal |
| `Elige una opción (1-3):` | `1` | Personas |
| `Elige una opción (1-8):` | `2` | Listado: Ana, Eva y Sara muestran "Empresa: Sin empresa" (sus textos quedaron vaciados) |
| `Elige una opción (1-8):` | `8` | Vuelve al menú principal |
| `Elige una opción (1-3):` | `3` | "Hasta pronto." y termina |