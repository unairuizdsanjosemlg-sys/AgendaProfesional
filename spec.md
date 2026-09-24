# Especificación — Fase 1: Gestión de Personas

## Proyecto: Agenda de Consultoría (C#)

| Campo       | Valor                                            |
| ----------- | ------------------------------------------------ |
| Autor       | Unai Ruiz Dávalos                                |
| Metodología | Spec Driven Development (SDD)                    |
| Documento   | `spec.md` — 1 de N (ver §11 próximos documentos) |
| Fase        | 1 de N — Gestión de Personas                     |
| Estado      | Borrador para revisión                           |
| Versión     | 0.1                                              |
| Fecha       | 2026-09-24                                       |

---

## 1. Resumen y contexto

Aplicación de consola en C# para una consultora, que funcionará como agenda interna
de personas y empresas colaboradoras. Este documento especifica **únicamente la
Fase 1**, centrada en la gestión de Personas (alta, listado, búsqueda, modificación
y baja), pero el diseño se plantea con visión de futuro para no bloquear la Fase 2
(Empresas y relación 1:N).

## 2. Objetivos de aprendizaje vinculados (RA)

| RA                                                                         | Cómo se cubre en esta fase                                                 |
| -------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| RA1 — Diseñar una consola usable con menús, validaciones y mensajes claros | §5 (FR-001, FR-012), §4 (escenarios de menú)                               |
| RA2 — Aplicar colecciones, funciones y clases para un programa mantenible  | §8 (arquitectura: `Persona`, `GestorPersonas`, `Program`)                  |
| RA3 — Modelar datos relacionados (1 empresa : N personas)                  | §6 y §8.5 (diseño preparado para la relación, aunque no se implemente aún) |

## 3. Alcance

### 3.1 Incluido en esta fase

- Alta, listado, búsqueda (por Id y por texto), modificación y baja de Personas.
- Validación de correo y teléfono.
- Menú principal robusto en bucle.
- Confirmación explícita antes de modificar o eliminar.

### 3.2 Fuera de alcance en esta fase (pero contemplado en el diseño)

- Entidad `Empresa` como clase gestionada (alta/baja/listado de empresas).
- Validación de que la empresa asignada exista realmente (relación 1:N funcional).
- Persistencia en fichero o base de datos (Fase 1 = solo memoria, ver NFR-001).
- Autenticación, roles o control de acceso.

## 4. Escenarios principales (historias de usuario)

**US-1 — Alta de persona**
Como usuario de la agenda, quiero dar de alta una persona con sus datos de contacto
para tener su información disponible en el sistema.

- Dado que elijo "Alta" e introduzco todos los datos obligatorios en formato válido,
  al confirmar el sistema crea la persona con un `IdPersona` autogenerado.
- Dado que el correo o el teléfono tienen un formato inválido, al intentar confirmar
  el sistema rechaza el alta, indica qué campo falla y permite corregirlo.
- Dado que dejo un campo obligatorio vacío, el sistema no avanza y me indica cuál falta.
  **US-2 — Listado**
  Como usuario, quiero ver un listado ordenado de todas las personas para revisar la
  agenda de un vistazo.
- Dado que no hay personas registradas, el sistema muestra un mensaje claro (no una
  lista vacía sin explicación).
- Dado que hay personas registradas, se muestran ordenadas (p. ej. por apellidos) y
  con formato legible en consola.
  **US-3 — Búsqueda**
  Como usuario, quiero localizar una persona por Id o por texto para no recorrer todo
  el listado manualmente.
- Dado un `IdPersona` existente, el sistema muestra esa persona.
- Dado un texto que coincide parcialmente con nombre o apellidos (sin distinguir
  mayúsculas/minúsculas), el sistema muestra todas las coincidencias.
- Dado que no hay coincidencias, el sistema lo indica explícitamente.
  **US-4 — Modificación**
  Como usuario, quiero corregir los datos de una persona ya existente.
- Dado un Id que no existe, el sistema lo informa y vuelve al menú sin error.
- Dado un Id existente, el sistema muestra los datos actuales, pide confirmación
  antes de guardar cambios, y re-valida correo/teléfono si se modifican.
  **US-5 — Baja**
  Como usuario, quiero eliminar una persona que ya no debe estar en la agenda.
- Antes de eliminar, el sistema muestra claramente el registro afectado y exige
  confirmación explícita (s/n).
- Dado un Id que no existe, el sistema lo informa sin error ni cierre inesperado.
  **US-6 — Robustez del menú**
  Como usuario, quiero que el programa no se cierre si me equivoco al escribir.
- Dado que introduzco texto no numérico donde se espera un número de opción, el
  sistema lo detecta, muestra un aviso y vuelve a pedir la opción.
- Dado que elijo una opción de menú inexistente, el sistema lo indica y repite el menú.

## 5. Requisitos funcionales

| ID     | Requisito                                                                                                                       |
| ------ | ------------------------------------------------------------------------------------------------------------------------------- |
| FR-001 | El menú principal se repite en bucle hasta elegir "Salir", mostrando opciones numeradas y claras.                               |
| FR-002 | El alta solicita Nombre, Apellidos, Teléfono, Correo y Empresa asignada, valida antes de guardar y confirma el alta al usuario. |
| FR-003 | `IdPersona` se asigna automáticamente de forma incremental; no es editable por el usuario.                                      |
| FR-004 | El listado muestra todas las personas ordenadas (p. ej. por apellidos) y con formato legible.                                   |
| FR-005 | La búsqueda por `IdPersona` devuelve la persona exacta o indica que no existe.                                                  |
| FR-006 | La búsqueda por texto localiza coincidencias parciales en nombre o apellidos.                                                   |
| FR-007 | Toda búsqueda sin resultados se comunica explícitamente al usuario.                                                             |
| FR-008 | Modificar o eliminar exige localizar primero el registro por Id; si no existe, se informa sin error.                            |
| FR-009 | Antes de modificar, se muestran los datos actuales y se pide confirmación explícita.                                            |
| FR-010 | Al modificar campos de contacto, se vuelve a validar su formato.                                                                |
| FR-011 | Antes de eliminar, se muestra el registro afectado y se exige confirmación explícita (s/n).                                     |
| FR-012 | Ninguna entrada no numérica ni opción de menú inexistente puede cerrar el programa o lanzar una excepción no controlada.        |
| FR-013 | El correo se valida como no vacío, con `@` y un dominio con al menos un punto. _(ASUNCIÓN — ver §9)_                            |
| FR-014 | El teléfono se valida como no vacío, solo dígitos, longitud típica de 9 (formato España). _(ASUNCIÓN — ver §9)_                 |
| FR-015 | "Empresa asignada" se guarda como texto libre en esta fase, sin validar contra un catálogo real de empresas.                    |

## 6. Modelo de datos (Fase 1)

### Persona

| Campo           | Tipo   | Obligatorio  | Notas                                |
| --------------- | ------ | ------------ | ------------------------------------ |
| IdPersona       | entero | autogenerado | incremental, gestionado internamente |
| Nombre          | texto  | sí           | no vacío                             |
| Apellidos       | texto  | sí           | no vacío                             |
| Telefono        | texto  | sí           | validado (FR-014)                    |
| Correo          | texto  | sí           | validado (FR-013)                    |
| EmpresaAsignada | texto  | no           | texto libre en Fase 1 (ver §8.5)     |

_(La entidad `Empresa` del documento original no se implementa en esta fase; ver §3.2 y §8.5.)_

## 7. Requisitos no funcionales

| ID      | Requisito                                                                                                                      |
| ------- | ------------------------------------------------------------------------------------------------------------------------------ |
| NFR-001 | Persistencia solo en memoria durante la ejecución; los datos se pierden al cerrar el programa (decisión de alcance de Fase 1). |
| NFR-002 | Nombres significativos, sangrado coherente, comentarios solo donde aporten contexto.                                           |
| NFR-003 | Entrega en carpeta o etiqueta independiente por fase; la Fase 1 debe seguir funcionando en fases posteriores.                  |
| NFR-004 | Interfaz de consola en español, con mensajes de éxito/error claros.                                                            |

## 8. Borrador técnico (arquitectura propuesta)

### 8.1 Estructura de archivos

```
/src
  Program.cs          -> punto de entrada, bucle del menú principal
  Persona.cs          -> modelo de datos
  GestorPersonas.cs   -> lógica de negocio (alta, listado, búsqueda, modificar, baja)
  Validaciones.cs     -> validación de correo, teléfono y entrada numérica
```

### 8.2 Modelo `Persona`

```csharp
public class Persona
{
    public int IdPersona { get; }              // solo lectura externa
    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
    public string EmpresaAsignada { get; set; } // texto libre en Fase 1, ver 8.5
}
```

### 8.3 `GestorPersonas` (responsabilidades)

- Almacenamiento interno: `List<Persona>` + contador `siguienteId`.
- `Persona AltaPersona(...)`
- `List<Persona> Listar()` — ordenado
- `Persona? BuscarPorId(int id)`
- `List<Persona> BuscarPorTexto(string texto)`
- `bool Modificar(int id, ...)`
- `bool Eliminar(int id)`

### 8.4 Bucle principal (pseudocódigo)

```
mientras (opcion != Salir):
    mostrar menú
    leer opción (validar que sea numérica, si no: aviso y repetir)
    según opción:
        1 Alta | 2 Listado | 3 Búsqueda | 4 Modificar | 5 Baja | 6 Salir
        otro -> aviso de opción inválida, repetir
```

### 8.5 Diseño con visión de futuro (Fase 2 — Empresas)

Para no bloquear la futura relación 1:N sin sobre-diseñar la Fase 1:

- `EmpresaAsignada` se expone como propiedad (no campo público directo), de modo que
  en la Fase 2 pueda sustituirse internamente por una referencia a `Empresa`
  (`IdEmpresa`) sin romper el resto del código que la usa.
- `GestorPersonas` no debe depender de una implementación concreta de almacenamiento,
  para poder relacionarse más adelante con un futuro `GestorEmpresas`.
- El `IdPersona` autoincremental gestionado internamente facilita usarlo como base
  de una clave foránea real en el futuro.

## 9. Supuestos y puntos abiertos (a confirmar)

| #   | Punto                                                                                         | Asunción usada en este documento                                                         |
| --- | --------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| 1   | Regla exacta de validación de teléfono/correo                                                 | Ver FR-013/FR-014. Ajustar si la asignatura exige un formato distinto.                   |
| 2   | Proyecto individual o en grupo, nombre de repositorio, convención de ramas/etiquetas por fase | No especificado; NFR-003 queda genérico. Indícalo para concretar el apartado de entrega. |
| 3   | Fecha límite de entrega                                                                       | No indicada; no se refleja en el documento.                                              |

## 10. Criterios de aceptación de la fase (evidencia)

Demostración en clase o captura de pantalla de una ejecución completa que incluya,
como mínimo: un alta, una búsqueda, una modificación y una baja, mostrando en cada
caso las confirmaciones y validaciones descritas en §5.

## 11. Próximos documentos sugeridos (SDD)

Este `spec.md` cubre el **qué** y un primer **cómo**. Si quieres seguir con el flujo
Spec Driven Development completo, los siguientes documentos naturales serían:

- `plan.md` — diseño técnico detallado (firmas de métodos completas, manejo de errores).
- `tasks.md` — desglose en tareas pequeñas y verificables para implementar la Fase 1.
  Dímelo cuando quieras y los generamos.
