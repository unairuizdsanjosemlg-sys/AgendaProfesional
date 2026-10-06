# Guion de demostración — Fase 2 (Gestión de empresas)

Recorrido completo en una sola ejecución: altas con errores y homónimos, listado,
búsqueda, modificación (cambio, "n" y vacío-conserva), bajas (confirmada, cancelada y
sin coincidencias), interrupciones con `cancelar`, entradas inválidas en todos los menús
y regresión de los flujos de personas de la Fase 1.

Cómo ejecutar:

```
dotnet run --project src/AgendaProfesional.csproj
```

Convenciones del guion:

- Cada línea es lo que hay que teclear seguido de <kbd>Enter</kbd>.
- `⟨vacío⟩` significa pulsar <kbd>Enter</kbd> sin escribir nada.
- Las líneas se leen en orden; el prompt indicado es el que espera esa entrada.

## A. Empresas

### A1. Entradas inválidas del menú principal y del submenu

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `xyz` | Aviso "Entrada no válida: introduce un número entre 1 y 3." y repregunta |
| `Elige una opción (1-3):` | `2` | Abre `=== GESTIÓN DE EMPRESAS ===` |
| `Elige una opción (1-6):` | `x` | Aviso "Entrada no válida: introduce un número entre 1 y 6." y repregunta |
| `Elige una opción (1-6):` | `9` | Mismo aviso y repregunta |

### A2. Alta válida con errores de campo (solo repregunta el campo fallido)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-6):` | `1` | `--- ALTA DE EMPRESA ---` |
| `Nombre comercial:` | `Consultora Delta` | Avanza al CIF |
| `CIF (letra o dígito + 7 dígitos + control):` | `B12345678` | Avanza al teléfono |
| `Teléfono ...:` | `600111222` | Aviso (empieza por `+`) y repregunta solo el teléfono |
| `Teléfono ...:` | `+34 600111222` | Avanza al correo |
| `Correo:` | `correomalo` | Aviso (`@` + dominio con punto) y repregunta solo el correo |
| `Correo:` | `info@delta.com` | Avanza a la dirección |
| `Dirección:` | `C/ Sol 5` | Muestra el resumen del alta |
| `¿Confirmar el alta? (s/n):` | `s` | "Alta confirmada. Id asignado: 1." y vuelve al submenu |

### A3. Alta de homónimo (mismo nombre, CIF distinto)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-6):` | `1` | `--- ALTA DE EMPRESA ---` |
| `Nombre comercial:` | `Consultora Delta` | (permitido: los nombres duplicados se aceptan) |
| `CIF ...:` | `B87654321` | |
| `Teléfono ...:` | `+34 611222333` | |
| `Correo:` | `contacto@delta.com` | |
| `Dirección:` | `Avda. Luna 10` | Resumen |
| `¿Confirmar el alta? (s/n):` | `s` | "Alta confirmada. Id asignado: 2." |

### A4. CIF repetido y `cancelar` a mitad del alta

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-6):` | `1` | Alta |
| `Nombre comercial:` | `Otra Empresa` | |
| `CIF ...:` | `B12345678` | "Ese CIF ya está registrado por otra empresa." y repregunta solo el CIF |
| `CIF ...:` | `cancelar` | "Alta cancelada." y vuelve al **menú principal** (no cierra) |

### A5. Listado y búsquedas

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-6):` | `2` | Listado con cabecera alineada y las 2 empresas (orden por nombre comercial) |
| `Elige una opción (1-6):` | `3` | `--- BÚSQUEDA DE EMPRESAS ---` |
| `Nombre comercial o CIF:` | `consultora delta` | 2 coincidencias (subcadena sin mayúsculas) |
| `Elige una opción (1-6):` | `3` | Búsqueda |
| `Nombre comercial o CIF:` | `b87654321` | 1 coincidencia (CIF completo en minúsculas) |
| `Elige una opción (1-6):` | `3` | Búsqueda |
| `Nombre comercial o CIF:` | `zzzzz` | "No hay coincidencias para la búsqueda." sin error |

### A6. Modificar: homónimos, selección fuera de rango y cambio confirmado

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-6):` | `4` | `--- MODIFICAR EMPRESA ---` |
| `Nombre comercial o CIF:` | `consultora delta` | Lista numerada con las 2 candidatas |
| `Número de la empresa (1-2):` | `5` | Aviso fuera de rango y repregunta |
| `Número de la empresa (1-2):` | `2` | Muestra los datos actuales de la empresa 2 |
| `Nombre comercial:` | `⟨vacío⟩` | Conserva |
| `CIF ...:` | `⟨vacío⟩` | Conserva |
| `Teléfono ...:` | `⟨vacío⟩` | Conserva |
| `Correo:` | `nuevocorreo@delta.com` | Valor nuevo |
| `Dirección:` | `⟨vacío⟩` | Conserva |
| `¿Guardar los cambios? (s/n):` | `s` | Resumen con la empresa afectada; "Empresa modificada correctamente." |

### A7. Modificar: responder "n" en la confirmación (permanece en el submenu)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-6):` | `4` | Modificar |
| `Nombre comercial o CIF:` | `B12345678` | Coincidencia única: datos actuales directamente |
| `Nombre comercial:` | `⟨vacío⟩` | |
| `CIF ...:` | `⟨vacío⟩` | |
| `Teléfono ...:` | `⟨vacío⟩` | |
| `Correo:` | `correocambiado@delta.com` | |
| `Dirección:` | `⟨vacío⟩` | Resumen de cambios |
| `¿Guardar los cambios? (s/n):` | `n` | "Modificación cancelada." y **sigue en el submenu de empresas** |

### A8. Modificar: todos los campos vacíos

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-6):` | `4` | Modificar |
| `Nombre comercial o CIF:` | `B12345678` | Datos actuales |
| los 5 campos | `⟨vacío⟩` ×5 | "No se ha introducido ningún cambio." (sin resumen ni confirmación) |

### A9. Modificar: `cancelar` al elegir entre candidatas (vuelve al menú principal)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-6):` | `4` | Modificar |
| `Nombre comercial o CIF:` | `consultora delta` | Lista numerada con las 2 candidatas |
| `Número de la empresa (1-2):` | `cancelar` | "Operación cancelada." y vuelve al **menú principal** |

### A10. Alta interrumpida con `cancelar` en la confirmación

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-6):` | `1` | Alta |
| `Nombre comercial:` | `Prueba Cancel` | |
| `CIF ...:` | `A12345678` | |
| `Teléfono ...:` | `+34 622333444` | |
| `Correo:` | `prueba@correo.es` | |
| `Dirección:` | `C/ Test 1` | Resumen |
| `¿Confirmar el alta? (s/n):` | `cancelar` | "Alta cancelada." y vuelve al **menú principal** (no crea la empresa) |

### A11. Bajas: cancelada, confirmada, sin coincidencias y con `cancelar`

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-6):` | `5` | `--- BAJA DE EMPRESA ---` |
| `Nombre comercial o CIF:` | `consultora delta` | Lista numerada con las 2 candidatas |
| `Número de la empresa (1-2):` | `1` | Registro afectado (empresa 1, Id 1) |
| `¿Eliminar definitivamente? (s/n):` | `n` | "Baja cancelada." y **sigue en el submenu** |
| `Elige una opción (1-6):` | `5` | Baja |
| `Nombre comercial o CIF:` | `B12345678` | Registro afectado |
| `¿Eliminar definitivamente? (s/n):` | `s` | "Empresa eliminada." |
| `Elige una opción (1-6):` | `5` | Baja |
| `Nombre comercial o CIF:` | `zzzzz` | "No se ha encontrado ninguna empresa con ese nombre o CIF." |
| `Elige una opción (1-6):` | `5` | Baja |
| `Nombre comercial o CIF:` | `cancelar` | "Operación cancelada." y vuelve al **menú principal** |

### A12. Listado final y vuelta al menú principal

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `2` | Empresas |
| `Elige una opción (1-6):` | `2` | Listado con **1 sola fila**: Consultora Delta (B87654321) con el correo `nuevocorreo@delta.com` |
| `Elige una opción (1-6):` | `6` | Vuelve al menú principal sin cerrar la aplicación |

## B. Personas (regresión de la Fase 1)

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `1` | `=== GESTIÓN DE PERSONAS ===` |
| `Elige una opción (1-6):` | `1` | `--- ALTA DE PERSONA ---` |
| `Nombre:` | `Ana` | |
| `Apellidos:` | `García` | |
| `Teléfono ...:` | `600111222` | Aviso y repregunta solo el teléfono |
| `Teléfono ...:` | `+34 600111222` | |
| `Correo:` | `ana@correo.es` | |
| `Empresa asignada:` | `Delta Consulting` | "Alta confirmada. Id asignado: 1." |
| `Elige una opción (1-6):` | `2` | Listado con la persona |
| `Elige una opción (1-6):` | `3` | `--- BÚSQUEDA DE PERSONAS ---` |
| `Nombre:` | `an` | |
| `Apellidos:` | `gar` | 1 coincidencia (coincidencia parcial) |
| `Elige una opción (1-6):` | `4` | Modificar |
| `Nombre:` | `ana` | |
| `Apellidos:` | `garcía` | Datos actuales |
| los 5 campos | `⟨vacío⟩` ×5 | "No se ha introducido ningún cambio." |
| `Elige una opción (1-6):` | `4` | Modificar |
| `Nombre:` | `ana` | |
| `Apellidos:` | `garcía` | Datos actuales |
| `Nombre:` | `⟨vacío⟩` | Conserva |
| `Apellidos:` | `⟨vacío⟩` | Conserva |
| `Teléfono ...:` | `⟨vacío⟩` | Conserva |
| `Correo:` | `⟨vacío⟩` | Conserva |
| `Empresa asignada:` | `Delta SL` | Resumen: `Empresa asignada: Delta Consulting -> Delta SL` |
| `¿Guardar los cambios? (s/n):` | `s` | "Persona modificada correctamente." |
| `Elige una opción (1-6):` | `5` | Baja |
| `Nombre:` | `ana` | |
| `Apellidos:` | `garcía` | Registro afectado |
| `¿Eliminar definitivamente? (s/n):` | `n` | "Baja cancelada." y sigue en el submenu |
| `Elige una opción (1-6):` | `5` | Baja |
| `Nombre:` | `ana` | |
| `Apellidos:` | `garcía` | Registro afectado |
| `¿Eliminar definitivamente? (s/n):` | `s` | "Persona eliminada." |
| `Elige una opción (1-6):` | `2` | "No hay personas registradas en la agenda." |
| `Elige una opción (1-6):` | `6` | Vuelve al menú principal sin cerrar |

## C. Salida

| Prompt | Entrada | Resultado esperado |
|---|---|---|
| `Elige una opción (1-3):` | `1` | Abre Personas |
| `Elige una opción (1-6):` | `1` | Alta de persona |
| `Nombre:` | `cancelar` | "Alta cancelada." y vuelve al menú principal |
| `Elige una opción (1-3):` | `9` | Aviso y repregunta |
| `Elige una opción (1-3):` | `aaa` | Aviso y repregunta |
| `Elige una opción (1-3):` | `3` | "Hasta pronto." y la aplicación termina (única forma de cerrar) |
