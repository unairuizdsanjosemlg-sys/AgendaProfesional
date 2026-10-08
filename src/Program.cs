using AgendaProfesional;

GestorPersonas gestorPersonas = new();
GestorEmpresas gestorEmpresas = new();
GestorRelaciones gestorRelaciones = new();
bool salir = false;

while (!salir)
{
    Console.WriteLine();
    Console.WriteLine("=== AGENDA DE CONSULTORÍA ===");
    Console.WriteLine("1. Personas");
    Console.WriteLine("2. Empresas");
    Console.WriteLine("3. Salir");
    Console.WriteLine();

    int opcion = Validaciones.LeerEntero("Elige una opción (1-3): ", 1, 3);

    if (Validaciones.EntradaAgotada)
    {
        Console.WriteLine("Hasta pronto.");
        break;
    }

    switch (opcion)
    {
        case 1:
            MenuPersonas(gestorPersonas, gestorEmpresas, gestorRelaciones);
            break;
        case 2:
            MenuEmpresas.Ejecutar(gestorEmpresas, gestorPersonas, gestorRelaciones);
            break;
        case 3:
            salir = true;
            Console.WriteLine("Hasta pronto.");
            break;
        default:
            Console.WriteLine("Opción inexistente. Inténtalo de nuevo.");
            break;
    }
}

static void MenuPersonas(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    while (!Validaciones.EntradaAgotada)
    {
        Console.WriteLine();
        Console.WriteLine("=== GESTIÓN DE PERSONAS ===");
        Console.WriteLine("1. Alta de persona");
        Console.WriteLine("2. Listado");
        Console.WriteLine("3. Búsqueda");
        Console.WriteLine("4. Modificar");
        Console.WriteLine("5. Baja");
        Console.WriteLine("6. Asignar/cambiar empresa");
        Console.WriteLine("7. Desvincular empresa");
        Console.WriteLine("8. Volver al menú principal");
        Console.WriteLine();

        int opcion = Validaciones.LeerEntero("Elige una opción (1-8): ", 1, 8);

        if (Validaciones.EntradaAgotada)
        {
            return;
        }

        switch (opcion)
        {
            case 1:
                if (!AltaPersona(gestorPersonas, gestorEmpresas, gestorRelaciones)) return;
                break;
            case 2:
                if (!Listado(gestorPersonas, gestorEmpresas, gestorRelaciones)) return;
                break;
            case 3:
                if (!Busqueda(gestorPersonas, gestorEmpresas, gestorRelaciones)) return;
                break;
            case 4:
                if (!Modificar(gestorPersonas, gestorEmpresas, gestorRelaciones)) return;
                break;
            case 5:
                if (!Baja(gestorPersonas, gestorEmpresas, gestorRelaciones)) return;
                break;
            case 6:
                if (!AsignarEmpresa(gestorPersonas, gestorEmpresas, gestorRelaciones)) return;
                break;
            case 7:
                if (!DesvincularEmpresa(gestorPersonas, gestorEmpresas, gestorRelaciones)) return;
                break;
            case 8:
                return;
        }
    }
}

static (string Etiqueta, string Mensaje, Func<string, string?> Validar)[] CamposPersona()
{
    return
    [
        ("Nombre", "Nombre: ", Validaciones.ValidarTexto),
        ("Apellidos", "Apellidos: ", Validaciones.ValidarTexto),
        ("Teléfono", "Teléfono (con prefijo internacional, p. ej. +34 600111222): ", Validaciones.ValidarTelefono),
        ("Correo", "Correo: ", Validaciones.ValidarCorreo)
    ];
}

static bool LeerCamposPersona(bool esModificacion, GestorEmpresas gestorEmpresas,
    out PersonaDatos datos, out Empresa? empresaElegida)
{
    (string Etiqueta, string Mensaje, Func<string, string?> Validar)[] campos = CamposPersona();
    string[] valores = new string[campos.Length + 1];

    for (int i = 0; i < campos.Length; i++)
    {
        string? valor = esModificacion
            ? Validaciones.LeerCampoModificacion(campos[i].Mensaje, campos[i].Validar)
            : Validaciones.LeerCampoObligatorio(campos[i].Mensaje, campos[i].Validar);

        if (valor == null)
        {
            datos = new PersonaDatos(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            empresaElegida = null;
            return false;
        }

        valores[i] = valor;
    }

    empresaElegida = LeerEmpresaOpcional(gestorEmpresas, out bool cancelado);
    if (cancelado)
    {
        datos = new PersonaDatos(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        return false;
    }

    valores[campos.Length] = empresaElegida?.NombreComercial ?? string.Empty;

    datos = new PersonaDatos(valores[0], valores[1], valores[2], valores[3], valores[4]);
    return true;
}

static Empresa? LeerEmpresaOpcional(GestorEmpresas gestorEmpresas, out bool cancelado)
{
    cancelado = false;

    while (!Validaciones.EntradaAgotada)
    {
        string? valor = Validaciones.LeerCampoModificacion("Empresa asignada: ", Validaciones.ValidarTexto);

        if (valor == null)
        {
            cancelado = true;
            return null;
        }

        if (valor.Length == 0)
        {
            return null;
        }

        Empresa? empresa = ResolverEmpresa(gestorEmpresas, valor, out bool cancelada);
        if (cancelada)
        {
            cancelado = true;
            return null;
        }

        if (empresa != null)
        {
            return empresa;
        }
    }

    cancelado = true;
    return null;
}

static Empresa? ResolverEmpresa(GestorEmpresas gestorEmpresas, string texto, out bool cancelado)
{
    cancelado = false;

    List<Empresa> coincidencias = gestorEmpresas.BuscarPorTexto(texto);

    if (coincidencias.Count == 0)
    {
        Console.WriteLine("No se ha encontrado ninguna empresa con ese nombre.");
        return null;
    }

    if (coincidencias.Count == 1)
    {
        return coincidencias[0];
    }

    Console.WriteLine("Hay varias empresas coincidentes. Elige una:");
    for (int i = 0; i < coincidencias.Count; i++)
    {
        Console.WriteLine($"  {i + 1}. {Presentacion.EmpresaTexto(coincidencias[i])}");
    }

    int? numero = Validaciones.LeerNumeroCancelable(
        $"Número de la empresa (1-{coincidencias.Count}): ", 1, coincidencias.Count);

    if (numero == null)
    {
        cancelado = true;
        return null;
    }

    return coincidencias[numero.Value - 1];
}

static Empresa? PedirEmpresa(GestorEmpresas gestorEmpresas, out bool cancelado)
{
    cancelado = false;

    string? texto = Validaciones.LeerCampoObligatorio("Nombre de la empresa: ", Validaciones.ValidarTexto);
    if (texto == null)
    {
        cancelado = true;
        return null;
    }

    return ResolverEmpresa(gestorEmpresas, texto, out cancelado);
}

static bool AltaPersona(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    Console.WriteLine();
    Console.WriteLine("--- ALTA DE PERSONA ---");
    Console.WriteLine("Todos los campos son obligatorios, excepto la 'Empresa asignada', que es opcional y, " +
                      "si se rellena, debe corresponder a una empresa ya registrada.");
    Console.WriteLine($"Escribe '{Validaciones.PalabraCancelar}' para volver al menú.");
    Console.WriteLine();

    if (!LeerCamposPersona(esModificacion: false, gestorEmpresas, out PersonaDatos datos, out Empresa? empresaElegida))
    {
        Console.WriteLine("Alta cancelada.");
        return false;
    }

    Persona persona = gestorPersonas.AltaPersona(datos.Nombre, datos.Apellidos, datos.Telefono, datos.Correo, datos.EmpresaAsignada);

    if (empresaElegida != null)
    {
        gestorRelaciones.Asignar(persona.IdPersona, empresaElegida.IdEmpresa);
    }

    Console.WriteLine($"Alta confirmada. Id asignado: {persona.IdPersona}.");
    return true;
}

static bool Listado(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    Console.WriteLine();
    Console.WriteLine("--- LISTADO DE PERSONAS ---");

    List<Persona> personas = gestorPersonas.Listar();
    if (personas.Count == 0)
    {
        Console.WriteLine("No hay personas registradas en la agenda.");
        return true;
    }

    foreach (Persona persona in personas)
    {
        Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));
    }

    return true;
}

static List<Persona>? PedirCoincidencias(GestorPersonas gestorPersonas)
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

    return gestorPersonas.BuscarPorNombre(nombre, apellidos);
}

static Persona? LocalizarPersona(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas,
    GestorRelaciones gestorRelaciones, out bool cancelado)
{
    cancelado = false;

    List<Persona>? coincidencias = PedirCoincidencias(gestorPersonas);
    if (coincidencias == null)
    {
        cancelado = true;
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
        Console.WriteLine($"  {i + 1}. {Presentacion.PersonaTexto(coincidencias[i], gestorRelaciones, gestorEmpresas)}");
    }

    int? numero = Validaciones.LeerNumeroCancelable(
        $"Número de la persona (1-{coincidencias.Count}): ", 1, coincidencias.Count);

    if (numero == null)
    {
        cancelado = true;
        Console.WriteLine("Operación cancelada.");
        return null;
    }

    return coincidencias[numero.Value - 1];
}

static bool Busqueda(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    Console.WriteLine();
    Console.WriteLine("--- BÚSQUEDA DE PERSONAS ---");
    Console.WriteLine("La búsqueda se hace por nombre y apellidos, con coincidencia parcial.");
    Console.WriteLine($"Escribe '{Validaciones.PalabraCancelar}' para volver al menú.");
    Console.WriteLine();

    List<Persona>? resultados = PedirCoincidencias(gestorPersonas);
    if (resultados == null)
    {
        Console.WriteLine("Búsqueda cancelada.");
        return false;
    }

    if (resultados.Count == 0)
    {
        Console.WriteLine("No hay coincidencias para la búsqueda.");
        return true;
    }

    Console.WriteLine($"Coincidencias encontradas ({resultados.Count}):");
    foreach (Persona persona in resultados)
    {
        Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));
    }

    return true;
}

static bool Modificar(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    Console.WriteLine();
    Console.WriteLine("--- MODIFICAR PERSONA ---");

    Persona? persona = LocalizarPersona(gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
    if (cancelado)
    {
        return false;
    }

    if (persona == null)
    {
        return true;
    }

    Console.WriteLine("Datos actuales:");
    Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));
    Console.WriteLine($"Escribe '{Validaciones.PalabraCancelar}' para volver al menú; " +
                      "un campo en blanco conserva su valor actual.");
    Console.WriteLine();

    if (!LeerCamposPersona(esModificacion: true, gestorEmpresas, out PersonaDatos datos, out Empresa? empresaElegida))
    {
        Console.WriteLine("Modificación cancelada.");
        return false;
    }

    if (datos.SinCambios)
    {
        Console.WriteLine("No se ha introducido ningún cambio.");
        return true;
    }

    Console.WriteLine();
    Console.WriteLine("Resumen de cambios:");
    MostrarResumen(persona, datos, gestorEmpresas, gestorRelaciones);

    switch (Validaciones.ConfirmarCancelable("¿Guardar los cambios? (s/n): "))
    {
        case Validaciones.Confirmacion.Si:
            bool modificado = gestorPersonas.Modificar(
                persona.IdPersona, datos.Nombre, datos.Apellidos, datos.Telefono, datos.Correo, datos.EmpresaAsignada);

            if (empresaElegida != null)
            {
                gestorRelaciones.Asignar(persona.IdPersona, empresaElegida.IdEmpresa);
            }

            Console.WriteLine(modificado ? "Persona modificada correctamente." : "No se pudo modificar la persona.");
            return true;

        case Validaciones.Confirmacion.No:
            Console.WriteLine("Modificación cancelada.");
            return true;

        default:
            Console.WriteLine("Modificación cancelada.");
            return false;
    }
}

static void MostrarResumen(Persona persona, PersonaDatos datos,
    GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    (string Etiqueta, string Actual, string Nuevo)[] cambios =
    [
        ("Nombre", persona.Nombre, datos.Nombre),
        ("Apellidos", persona.Apellidos, datos.Apellidos),
        ("Teléfono", persona.Telefono, datos.Telefono),
        ("Correo", persona.Correo, datos.Correo),
        ("Empresa asignada", Presentacion.EmpresaDePersona(persona, gestorRelaciones, gestorEmpresas), datos.EmpresaAsignada)
    ];

    foreach ((string etiqueta, string actual, string nuevo) in cambios)
    {
        if (nuevo.Length > 0 && !string.Equals(actual, nuevo, StringComparison.Ordinal))
        {
            Console.WriteLine($"  {etiqueta}: {actual} -> {nuevo}");
        }
    }
}

static bool Baja(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    Console.WriteLine();
    Console.WriteLine("--- BAJA DE PERSONA ---");

    Persona? persona = LocalizarPersona(gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
    if (cancelado)
    {
        return false;
    }

    if (persona == null)
    {
        return true;
    }

    Console.WriteLine("Registro afectado:");
    Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));

    switch (Validaciones.ConfirmarCancelable("¿Eliminar definitivamente? (s/n): "))
    {
        case Validaciones.Confirmacion.Si:
            gestorRelaciones.EliminarRelacionesDe(persona.IdPersona);
            gestorPersonas.Eliminar(persona.IdPersona);
            Console.WriteLine("Persona eliminada.");
            return true;

        case Validaciones.Confirmacion.No:
            Console.WriteLine("Baja cancelada.");
            return true;

        default:
            Console.WriteLine("Baja cancelada.");
            return false;
    }
}

static bool AsignarEmpresa(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    Console.WriteLine();
    Console.WriteLine("--- ASIGNAR/CAMBIAR EMPRESA ---");

    Persona? persona = LocalizarPersona(gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
    if (cancelado)
    {
        Console.WriteLine("Operación cancelada.");
        return false;
    }

    if (persona == null)
    {
        return true;
    }

    Empresa? empresa = PedirEmpresa(gestorEmpresas, out cancelado);
    if (cancelado)
    {
        Console.WriteLine("Operación cancelada.");
        return false;
    }

    if (empresa == null)
    {
        Console.WriteLine("No se ha realizado ningún cambio.");
        return true;
    }

    Console.WriteLine();
    Console.WriteLine("Resumen de la asignación:");
    Console.WriteLine($"  Persona: {Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas)}");
    Console.WriteLine($"  Empresa: {Presentacion.EmpresaTexto(empresa)}");
    Console.WriteLine();

    switch (Validaciones.ConfirmarCancelable("¿Guardar la asignación? (s/n): "))
    {
        case Validaciones.Confirmacion.Si:
            gestorRelaciones.Asignar(persona.IdPersona, empresa.IdEmpresa);
            persona.EmpresaAsignada = empresa.NombreComercial;
            Console.WriteLine($"Persona asignada a la empresa '{empresa.NombreComercial}'.");
            return true;

        case Validaciones.Confirmacion.No:
            Console.WriteLine("Asignación cancelada.");
            return true;

        default:
            Console.WriteLine("Asignación cancelada.");
            return false;
    }
}

static bool DesvincularEmpresa(GestorPersonas gestorPersonas, GestorEmpresas gestorEmpresas, GestorRelaciones gestorRelaciones)
{
    Console.WriteLine();
    Console.WriteLine("--- DESVINCULAR EMPRESA ---");

    Persona? persona = LocalizarPersona(gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
    if (cancelado)
    {
        Console.WriteLine("Operación cancelada.");
        return false;
    }

    if (persona == null)
    {
        return true;
    }

    if (gestorRelaciones.IdEmpresaDe(persona.IdPersona) == null)
    {
        Console.WriteLine("Esa persona no tiene empresa asignada.");
        return true;
    }

    Console.WriteLine("Persona afectada:");
    Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));

    switch (Validaciones.ConfirmarCancelable("¿Desvincular a la persona de su empresa? (s/n): "))
    {
        case Validaciones.Confirmacion.Si:
            gestorRelaciones.Desvincular(persona.IdPersona);
            persona.EmpresaAsignada = string.Empty;
            Console.WriteLine("Persona desvinculada de su empresa.");
            return true;

        case Validaciones.Confirmacion.No:
            Console.WriteLine("Desvinculación cancelada.");
            return true;

        default:
            Console.WriteLine("Desvinculación cancelada.");
            return false;
    }
}

public static class Presentacion
{
    public static string EmpresaDePersona(Persona persona, GestorRelaciones relaciones, GestorEmpresas empresas)
    {
        int? idEmpresa = relaciones.IdEmpresaDe(persona.IdPersona);
        if (idEmpresa == null)
        {
            return "Sin empresa";
        }

        Empresa? empresa = empresas.BuscarPorId(idEmpresa.Value);
        return empresa == null
            ? "Sin empresa"
            : $"{empresa.NombreComercial} (Id {empresa.IdEmpresa})";
    }

    public static string PersonaTexto(Persona persona, GestorRelaciones relaciones, GestorEmpresas empresas)
    {
        return $"Id {persona.IdPersona} | {persona.Nombre} {persona.Apellidos} | " +
               $"Tlf: {persona.Telefono} | Correo: {persona.Correo} | " +
               $"Empresa: {EmpresaDePersona(persona, relaciones, empresas)}";
    }

    public static string EmpresaTexto(Empresa empresa)
    {
        return $"Id {empresa.IdEmpresa} | {empresa.NombreComercial} | CIF: {empresa.CIF} | " +
               $"Tlf: {empresa.Telefono} | Correo: {empresa.CorreoElectronico} | " +
               $"Dirección: {empresa.Direccion}";
    }
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