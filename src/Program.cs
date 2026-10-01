using AgendaProfesional;

GestorPersonas gestor = new();
bool salir = false;

while (!salir)
{
    Console.WriteLine();
    Console.WriteLine("=== AGENDA DE CONSULTORÍA ===");
    Console.WriteLine("1. Alta de persona");
    Console.WriteLine("2. Listado");
    Console.WriteLine("3. Búsqueda");
    Console.WriteLine("4. Modificar");
    Console.WriteLine("5. Baja");
    Console.WriteLine("6. Salir");
    Console.WriteLine();

    int opcion = Validaciones.LeerEntero("Elige una opción (1-6): ", 1, 6);

    if (Validaciones.EntradaAgotada)
    {
        Console.WriteLine("Hasta pronto.");
        break;
    }

    switch (opcion)
    {
        case 1:
            AltaPersona(gestor);
            break;
        case 2:
            Listado(gestor);
            break;
        case 3:
            Busqueda(gestor);
            break;
        case 4:
            Modificar(gestor);
            break;
        case 5:
            Baja(gestor);
            break;
        case 6:
            salir = true;
            Console.WriteLine("Hasta pronto.");
            break;
        default:
            Console.WriteLine("Opción inexistente. Inténtalo de nuevo.");
            break;
    }
}

static string PersonaTexto(Persona persona)
{
    return $"Id {persona.IdPersona} | {persona.Nombre} {persona.Apellidos} | " +
           $"Tlf: {persona.Telefono} | Correo: {persona.Correo} | " +
           $"Empresa: {persona.EmpresaAsignada}";
}

static (string Etiqueta, string Mensaje, Func<string, string?> Validar)[] CamposPersona()
{
    return
    [
        ("Nombre", "Nombre: ", Validaciones.ValidarTexto),
        ("Apellidos", "Apellidos: ", Validaciones.ValidarTexto),
        ("Teléfono", "Teléfono (con prefijo internacional, p. ej. +34 600111222): ", Validaciones.ValidarTelefono),
        ("Correo", "Correo: ", Validaciones.ValidarCorreo),
        ("Empresa asignada", "Empresa asignada: ", Validaciones.ValidarTexto)
    ];
}

static bool LeerCamposPersona(bool esModificacion, out PersonaDatos datos)
{
    (string Etiqueta, string Mensaje, Func<string, string?> Validar)[] campos = CamposPersona();
    string[] valores = new string[campos.Length];

    for (int i = 0; i < campos.Length; i++)
    {
        string? valor = esModificacion
            ? Validaciones.LeerCampoModificacion(campos[i].Mensaje, campos[i].Validar)
            : Validaciones.LeerCampoObligatorio(campos[i].Mensaje, campos[i].Validar);

        if (valor == null)
        {
            datos = new PersonaDatos(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            return false;
        }

        valores[i] = valor;
    }

    datos = new PersonaDatos(valores[0], valores[1], valores[2], valores[3], valores[4]);
    return true;
}

static void AltaPersona(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- ALTA DE PERSONA ---");
    Console.WriteLine($"Todos los campos son obligatorios. Escribe '{Validaciones.PalabraCancelar}' para volver al menú.");
    Console.WriteLine();

    if (!LeerCamposPersona(esModificacion: false, out PersonaDatos datos))
    {
        Console.WriteLine("Alta cancelada.");
        return;
    }

    Persona persona = gestor.AltaPersona(datos.Nombre, datos.Apellidos, datos.Telefono, datos.Correo, datos.EmpresaAsignada);
    Console.WriteLine($"Alta confirmada. Id asignado: {persona.IdPersona}.");
}

static void Listado(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- LISTADO DE PERSONAS ---");

    List<Persona> personas = gestor.Listar();
    if (personas.Count == 0)
    {
        Console.WriteLine("No hay personas registradas en la agenda.");
        return;
    }

    foreach (Persona persona in personas)
    {
        Console.WriteLine(PersonaTexto(persona));
    }
}

static List<Persona>? PedirCoincidencias(GestorPersonas gestor)
{
    string? nombre = Validaciones.LeerCampoObligatorio("Nombre: ", Validaciones.ValidarTexto);
    if (nombre == null)
    {
        return null;
    }

    string? apellidos = Validaciones.LeerCampoObligatorio("Apellidos: ", Validaciones.ValidarTexto);
    if (apellidos == null)
    {
        return null;
    }

    return gestor.BuscarPorNombre(nombre, apellidos);
}

static Persona? LocalizarPersona(GestorPersonas gestor)
{
    List<Persona>? coincidencias = PedirCoincidencias(gestor);
    if (coincidencias == null)
    {
        Console.WriteLine("Operación cancelada.");
        return null;
    }

    if (coincidencias.Count == 0)
    {
        Console.WriteLine("No se ha encontrado ninguna persona con ese nombre y apellidos.");
        return null;
    }

    if (coincidencias.Count == 1)
    {
        return coincidencias[0];
    }

    Console.WriteLine("Hay varias personas con ese nombre y apellidos. Elige una:");
    for (int i = 0; i < coincidencias.Count; i++)
    {
        Console.WriteLine($"  {i + 1}. {PersonaTexto(coincidencias[i])}");
    }

    int? numero = Validaciones.LeerNumeroCancelable(
        $"Número de la persona (1-{coincidencias.Count}): ", 1, coincidencias.Count);

    if (numero == null)
    {
        Console.WriteLine("Operación cancelada.");
        return null;
    }

    return coincidencias[numero.Value - 1];
}

static void Busqueda(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- BÚSQUEDA DE PERSONAS ---");
    Console.WriteLine("La búsqueda se hace por nombre y apellidos, con coincidencia parcial.");
    Console.WriteLine($"Escribe '{Validaciones.PalabraCancelar}' para volver al menú.");
    Console.WriteLine();

    List<Persona>? resultados = PedirCoincidencias(gestor);
    if (resultados == null)
    {
        Console.WriteLine("Búsqueda cancelada.");
        return;
    }

    if (resultados.Count == 0)
    {
        Console.WriteLine("No hay coincidencias para la búsqueda.");
        return;
    }

    Console.WriteLine($"Coincidencias encontradas ({resultados.Count}):");
    foreach (Persona persona in resultados)
    {
        Console.WriteLine(PersonaTexto(persona));
    }
}

static void Modificar(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- MODIFICAR PERSONA ---");

    Persona? persona = LocalizarPersona(gestor);
    if (persona == null)
    {
        return;
    }

    Console.WriteLine("Datos actuales:");
    Console.WriteLine(PersonaTexto(persona));
    Console.WriteLine($"Escribe '{Validaciones.PalabraCancelar}' para volver al menú; " +
                      "un campo en blanco conserva su valor actual.");
    Console.WriteLine();

    if (!LeerCamposPersona(esModificacion: true, out PersonaDatos datos))
    {
        Console.WriteLine("Modificación cancelada.");
        return;
    }

    if (datos.SinCambios)
    {
        Console.WriteLine("No se ha introducido ningún cambio.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Resumen de cambios:");
    MostrarResumen(persona, datos);

    if (!Validaciones.Confirmar("¿Guardar los cambios? (s/n): "))
    {
        Console.WriteLine("Modificación cancelada.");
        return;
    }

    bool modificado = gestor.Modificar(
        persona.IdPersona, datos.Nombre, datos.Apellidos, datos.Telefono, datos.Correo, datos.EmpresaAsignada);

    Console.WriteLine(modificado ? "Persona modificada correctamente." : "No se pudo modificar la persona.");
}

static void MostrarResumen(Persona persona, PersonaDatos datos)
{
    (string Etiqueta, string Actual, string Nuevo)[] cambios =
    [
        ("Nombre", persona.Nombre, datos.Nombre),
        ("Apellidos", persona.Apellidos, datos.Apellidos),
        ("Teléfono", persona.Telefono, datos.Telefono),
        ("Correo", persona.Correo, datos.Correo),
        ("Empresa asignada", persona.EmpresaAsignada, datos.EmpresaAsignada)
    ];

    foreach ((string etiqueta, string actual, string nuevo) in cambios)
    {
        if (nuevo.Length > 0 && !string.Equals(actual, nuevo, StringComparison.Ordinal))
        {
            Console.WriteLine($"  {etiqueta}: {actual} -> {nuevo}");
        }
    }
}

static void Baja(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- BAJA DE PERSONA ---");

    Persona? persona = LocalizarPersona(gestor);
    if (persona == null)
    {
        return;
    }

    Console.WriteLine("Registro afectado:");
    Console.WriteLine(PersonaTexto(persona));

    if (!Validaciones.Confirmar("¿Eliminar definitivamente? (s/n): "))
    {
        Console.WriteLine("Baja cancelada.");
        return;
    }

    gestor.Eliminar(persona.IdPersona);
    Console.WriteLine("Persona eliminada.");
}

readonly record struct PersonaDatos(
    string Nombre,
    string Apellidos,
    string Telefono,
    string Correo,
    string EmpresaAsignada)
{
    public bool SinCambios =>
        Nombre.Length == 0
        && Apellidos.Length == 0
        && Telefono.Length == 0
        && Correo.Length == 0
        && EmpresaAsignada.Length == 0;
}