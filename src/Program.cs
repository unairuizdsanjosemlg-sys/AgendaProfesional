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

static string LeerTexto(string mensaje)
{
    Console.Write(mensaje);
    return Console.ReadLine() ?? string.Empty;
}

static bool Confirmar()
{
    while (true)
    {
        string? entrada = Console.ReadLine();
        if (entrada != null && entrada.Trim().ToLowerInvariant() == "s")
        {
            return true;
        }

        if (entrada != null && entrada.Trim().ToLowerInvariant() == "n")
        {
            return false;
        }

        Console.Write("Responde s (sí) o n (no): ");
    }
}

static string PersonaTexto(Persona persona)
{
    string empresa = string.IsNullOrWhiteSpace(persona.EmpresaAsignada)
        ? "-"
        : persona.EmpresaAsignada;

    return $"Id {persona.IdPersona} | {persona.Nombre} {persona.Apellidos} | " +
           $"Tlf: {persona.Telefono} | Correo: {persona.Correo} | Empresa: {empresa}";
}

static void AltaPersona(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- ALTA DE PERSONA ---");

    while (true)
    {
        string nombre = LeerTexto("Nombre: ");
        string apellidos = LeerTexto("Apellidos: ");
        string telefono = LeerTexto("Teléfono: ");
        string correo = LeerTexto("Correo: ");
        string empresa = LeerTexto("Empresa asignada: ");

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("El campo Nombre es obligatorio.");
            continue;
        }

        if (string.IsNullOrWhiteSpace(apellidos))
        {
            Console.WriteLine("El campo Apellidos es obligatorio.");
            continue;
        }

        if (!Validaciones.EsTelefonoValido(telefono))
        {
            Console.WriteLine("Teléfono no válido: debe contener exactamente 9 dígitos.");
            continue;
        }

        if (!Validaciones.EsCorreoValido(correo))
        {
            Console.WriteLine("Correo no válido: debe contener una @ y un dominio con punto.");
            continue;
        }

        Persona persona = gestor.AltaPersona(nombre, apellidos, telefono, correo, empresa);
        Console.WriteLine($"Alta confirmada. Id asignado: {persona.IdPersona}.");
        return;
    }
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

static void Busqueda(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- BÚSQUEDA DE PERSONAS ---");
    Console.WriteLine("1. Buscar por Id");
    Console.WriteLine("2. Buscar por texto");
    int tipo = Validaciones.LeerEntero("Elige (1-2): ", 1, 2);

    if (tipo == 1)
    {
        int id = Validaciones.LeerEntero("Id de la persona: ", 1, int.MaxValue);
        Persona? persona = gestor.BuscarPorId(id);
        if (persona == null)
        {
            Console.WriteLine($"No existe ninguna persona con Id {id}.");
            return;
        }

        Console.WriteLine(PersonaTexto(persona));
        return;
    }

    string texto = LeerTexto("Texto a buscar (coincide con nombre o apellidos): ");
    List<Persona> resultados = gestor.BuscarPorTexto(texto);
    if (resultados.Count == 0)
    {
        Console.WriteLine("No hay coincidencias para la búsqueda.");
        return;
    }

    foreach (Persona persona in resultados)
    {
        Console.WriteLine(PersonaTexto(persona));
    }
}

static void Modificar(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- MODIFICAR PERSONA ---");
    int id = Validaciones.LeerEntero("Id de la persona a modificar: ", 1, int.MaxValue);
    Persona? persona = gestor.BuscarPorId(id);
    if (persona == null)
    {
        Console.WriteLine($"No existe ninguna persona con Id {id}.");
        return;
    }

    Console.WriteLine("Datos actuales:");
    Console.WriteLine(PersonaTexto(persona));
    Console.Write("¿Modificar esta persona? (s/n): ");
    if (!Confirmar())
    {
        Console.WriteLine("Modificación cancelada.");
        return;
    }

    while (true)
    {
        string nombre = LeerTexto("Nombre: ");
        string apellidos = LeerTexto("Apellidos: ");
        string telefono = LeerTexto("Teléfono: ");
        string correo = LeerTexto("Correo: ");
        string empresa = LeerTexto("Empresa asignada: ");

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("El campo Nombre es obligatorio.");
            continue;
        }

        if (string.IsNullOrWhiteSpace(apellidos))
        {
            Console.WriteLine("El campo Apellidos es obligatorio.");
            continue;
        }

        if (!Validaciones.EsTelefonoValido(telefono))
        {
            Console.WriteLine("Teléfono no válido: debe contener exactamente 9 dígitos.");
            continue;
        }

        if (!Validaciones.EsCorreoValido(correo))
        {
            Console.WriteLine("Correo no válido: debe contener una @ y un dominio con punto.");
            continue;
        }

        bool modificado = gestor.Modificar(id, nombre, apellidos, telefono, correo, empresa);
        Console.WriteLine(modificado ? "Persona modificada correctamente." : "No se pudo modificar la persona.");
        return;
    }
}

static void Baja(GestorPersonas gestor)
{
    Console.WriteLine();
    Console.WriteLine("--- BAJA DE PERSONA ---");
    int id = Validaciones.LeerEntero("Id de la persona a eliminar: ", 1, int.MaxValue);
    Persona? persona = gestor.BuscarPorId(id);
    if (persona == null)
    {
        Console.WriteLine($"No existe ninguna persona con Id {id}.");
        return;
    }

    Console.WriteLine("Registro afectado:");
    Console.WriteLine(PersonaTexto(persona));
    Console.Write("¿Eliminar definitivamente? (s/n): ");
    if (!Confirmar())
    {
        Console.WriteLine("Baja cancelada.");
        return;
    }

    gestor.Eliminar(id);
    Console.WriteLine("Persona eliminada.");
}