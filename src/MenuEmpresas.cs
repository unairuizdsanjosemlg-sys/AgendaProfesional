namespace AgendaProfesional;

public static class MenuEmpresas
{
    // Cada operación devuelve true para seguir en el submenu y false cuando
    // 'cancelar' obliga a volver al menú principal.
    public static void Ejecutar(GestorEmpresas gestor)
    {
        while (!Validaciones.EntradaAgotada)
        {
            Console.WriteLine();
            Console.WriteLine("=== GESTIÓN DE EMPRESAS ===");
            Console.WriteLine("1. Alta de empresa");
            Console.WriteLine("2. Listado");
            Console.WriteLine("3. Búsqueda");
            Console.WriteLine("4. Modificar");
            Console.WriteLine("5. Baja");
            Console.WriteLine("6. Volver al menú principal");
            Console.WriteLine();

            int opcion = Validaciones.LeerEntero("Elige una opción (1-6): ", 1, 6);

            if (Validaciones.EntradaAgotada)
            {
                return;
            }

            switch (opcion)
            {
                case 1:
                    if (!Alta(gestor)) return;
                    break;
                case 2:
                    if (!Listado(gestor)) return;
                    break;
                case 3:
                    if (!Busqueda(gestor)) return;
                    break;
                case 4:
                    if (!Modificar(gestor)) return;
                    break;
                case 5:
                    if (!Baja(gestor)) return;
                    break;
                case 6:
                    return;
            }
        }
    }

    private static bool Alta(GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- ALTA DE EMPRESA ---");
        Console.WriteLine($"Todos los campos son obligatorios. Escribe '{Validaciones.PalabraCancelar}' para volver al menú principal.");
        Console.WriteLine();

        string? nombreComercial = Validaciones.LeerCampoObligatorio("Nombre comercial: ", Validaciones.ValidarTexto);
        if (nombreComercial == null)
        {
            Console.WriteLine("Alta cancelada.");
            return false;
        }

        string? cif = Validaciones.LeerCampoObligatorio("CIF (letra o dígito + 7 dígitos + control): ",
            valor => ValidarCifUnico(gestor, valor, null));
        if (cif == null)
        {
            Console.WriteLine("Alta cancelada.");
            return false;
        }

        string? telefono = Validaciones.LeerCampoObligatorio(
            "Teléfono (con prefijo internacional, p. ej. +34 600111222): ", Validaciones.ValidarTelefono);
        if (telefono == null)
        {
            Console.WriteLine("Alta cancelada.");
            return false;
        }

        string? correo = Validaciones.LeerCampoObligatorio("Correo: ", Validaciones.ValidarCorreo);
        if (correo == null)
        {
            Console.WriteLine("Alta cancelada.");
            return false;
        }

        string? direccion = Validaciones.LeerCampoObligatorio("Dirección: ", Validaciones.ValidarTexto);
        if (direccion == null)
        {
            Console.WriteLine("Alta cancelada.");
            return false;
        }

        Console.WriteLine();
        Console.WriteLine("Resumen del alta:");
        Console.WriteLine($"  Nombre comercial: {nombreComercial}");
        Console.WriteLine($"  CIF: {cif}");
        Console.WriteLine($"  Teléfono: {telefono}");
        Console.WriteLine($"  Correo: {correo}");
        Console.WriteLine($"  Dirección: {direccion}");
        Console.WriteLine();

        switch (Validaciones.ConfirmarCancelable("¿Confirmar el alta? (s/n): "))
        {
            case Validaciones.Confirmacion.Si:
                Empresa? empresa = gestor.AltaEmpresa(nombreComercial, cif, telefono, correo, direccion);
                if (empresa == null)
                {
                    Console.WriteLine("No se pudo dar de alta: el CIF ya está registrado.");
                    return true;
                }

                Console.WriteLine($"Alta confirmada. Id asignado: {empresa.IdEmpresa}.");
                return true;

            case Validaciones.Confirmacion.No:
                Console.WriteLine("Alta cancelada.");
                return true;

            default:
                Console.WriteLine("Alta cancelada.");
                return false;
        }
    }

    private static bool Listado(GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- LISTADO DE EMPRESAS ---");

        List<Empresa> empresas = gestor.Listar();
        if (empresas.Count == 0)
        {
            Console.WriteLine("No hay empresas registradas en la agenda.");
            return true;
        }

        Console.WriteLine(string.Format("{0,-4} {1,-24} {2,-11} {3,-16} {4,-30} {5}",
            "Id", "Nombre comercial", "CIF", "Teléfono", "Correo", "Dirección"));

        foreach (Empresa empresa in empresas)
        {
            Console.WriteLine(string.Format("{0,-4} {1,-24} {2,-11} {3,-16} {4,-30} {5}",
                empresa.IdEmpresa, empresa.NombreComercial, empresa.CIF,
                empresa.Telefono, empresa.CorreoElectronico, empresa.Direccion));
        }

        return true;
    }

    private static bool Busqueda(GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- BÚSQUEDA DE EMPRESAS ---");
        Console.WriteLine("Coincidencia parcial por nombre comercial o CIF completo.");
        Console.WriteLine($"Escribe '{Validaciones.PalabraCancelar}' para volver al menú principal.");
        Console.WriteLine();

        string? texto = Validaciones.LeerCampoObligatorio("Nombre comercial o CIF: ", Validaciones.ValidarTexto);
        if (texto == null)
        {
            Console.WriteLine("Búsqueda cancelada.");
            return false;
        }

        List<Empresa> resultados = gestor.BuscarPorTexto(texto);
        if (resultados.Count == 0)
        {
            Console.WriteLine("No hay coincidencias para la búsqueda.");
            return true;
        }

        Console.WriteLine($"Coincidencias encontradas ({resultados.Count}):");
        foreach (Empresa empresa in resultados)
        {
            Console.WriteLine(EmpresaTexto(empresa));
        }

        return true;
    }

    private static bool Modificar(GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- MODIFICAR EMPRESA ---");

        Empresa? empresa = LocalizarEmpresa(gestor, out bool cancelado);
        if (cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return false;
        }

        if (empresa == null)
        {
            return true;
        }

        Console.WriteLine("Datos actuales:");
        Console.WriteLine(EmpresaTexto(empresa));
        Console.WriteLine($"Escribe '{Validaciones.PalabraCancelar}' para volver al menú principal; " +
                          "un campo en blanco conserva su valor actual.");
        Console.WriteLine();

        string? nombreComercial = Validaciones.LeerCampoModificacion("Nombre comercial: ", Validaciones.ValidarTexto);
        if (nombreComercial == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return false;
        }

        string? cif = Validaciones.LeerCampoModificacion("CIF: ",
            valor => ValidarCifUnico(gestor, valor, empresa.IdEmpresa));
        if (cif == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return false;
        }

        string? telefono = Validaciones.LeerCampoModificacion(
            "Teléfono (con prefijo internacional, p. ej. +34 600111222): ", Validaciones.ValidarTelefono);
        if (telefono == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return false;
        }

        string? correo = Validaciones.LeerCampoModificacion("Correo: ", Validaciones.ValidarCorreo);
        if (correo == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return false;
        }

        string? direccion = Validaciones.LeerCampoModificacion("Dirección: ", Validaciones.ValidarTexto);
        if (direccion == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return false;
        }

        if (nombreComercial.Length == 0 && cif.Length == 0 && telefono.Length == 0
            && correo.Length == 0 && direccion.Length == 0)
        {
            Console.WriteLine("No se ha introducido ningún cambio.");
            return true;
        }

        Console.WriteLine();
        Console.WriteLine("Empresa afectada:");
        Console.WriteLine(EmpresaTexto(empresa));
        Console.WriteLine("Resumen de cambios:");
        MostrarResumen(empresa, nombreComercial, cif, telefono, correo, direccion);

        switch (Validaciones.ConfirmarCancelable("¿Guardar los cambios? (s/n): "))
        {
            case Validaciones.Confirmacion.Si:
                bool modificado = gestor.Modificar(
                    empresa.IdEmpresa, nombreComercial, cif, telefono, correo, direccion);
                Console.WriteLine(modificado
                    ? "Empresa modificada correctamente."
                    : "No se pudo modificar la empresa.");
                return true;

            case Validaciones.Confirmacion.No:
                Console.WriteLine("Modificación cancelada.");
                return true;

            default:
                Console.WriteLine("Modificación cancelada.");
                return false;
        }
    }

    private static bool Baja(GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- BAJA DE EMPRESA ---");

        Empresa? empresa = LocalizarEmpresa(gestor, out bool cancelado);
        if (cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return false;
        }

        if (empresa == null)
        {
            return true;
        }

        Console.WriteLine("Registro afectado:");
        Console.WriteLine(EmpresaTexto(empresa));

        switch (Validaciones.ConfirmarCancelable("¿Eliminar definitivamente? (s/n): "))
        {
            case Validaciones.Confirmacion.Si:
                gestor.Eliminar(empresa.IdEmpresa);
                Console.WriteLine("Empresa eliminada.");
                return true;

            case Validaciones.Confirmacion.No:
                Console.WriteLine("Baja cancelada.");
                return true;

            default:
                Console.WriteLine("Baja cancelada.");
                return false;
        }
    }

    private static Empresa? LocalizarEmpresa(GestorEmpresas gestor, out bool cancelado)
    {
        cancelado = false;

        string? texto = Validaciones.LeerCampoObligatorio("Nombre comercial o CIF: ", Validaciones.ValidarTexto);
        if (texto == null)
        {
            cancelado = true;
            return null;
        }

        List<Empresa> coincidencias = gestor.BuscarPorTexto(texto);

        if (coincidencias.Count == 0)
        {
            Console.WriteLine("No se ha encontrado ninguna empresa con ese nombre o CIF.");
            return null;
        }

        if (coincidencias.Count == 1)
        {
            return coincidencias[0];
        }

        Console.WriteLine("Hay varias empresas coincidentes. Elige una:");
        for (int i = 0; i < coincidencias.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {EmpresaTexto(coincidencias[i])}");
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

    private static string? ValidarCifUnico(GestorEmpresas gestor, string valor, int? idExcluido)
    {
        string? error = Validaciones.ValidarCif(valor);
        if (error != null)
        {
            return error;
        }

        return gestor.ExisteCif(valor, idExcluido)
            ? "Ese CIF ya está registrado por otra empresa."
            : null;
    }

    private static void MostrarResumen(Empresa empresa, string nombreComercial, string cif,
        string telefono, string correo, string direccion)
    {
        (string Etiqueta, string Actual, string Nuevo)[] cambios =
        [
            ("Nombre comercial", empresa.NombreComercial, nombreComercial),
            ("CIF", empresa.CIF, cif),
            ("Teléfono", empresa.Telefono, telefono),
            ("Correo", empresa.CorreoElectronico, correo),
            ("Dirección", empresa.Direccion, direccion)
        ];

        foreach ((string etiqueta, string actual, string nuevo) in cambios)
        {
            if (nuevo.Length > 0 && !string.Equals(actual, nuevo, StringComparison.Ordinal))
            {
                Console.WriteLine($"  {etiqueta}: {actual} -> {nuevo}");
            }
        }
    }

    private static string EmpresaTexto(Empresa empresa)
    {
        return $"Id {empresa.IdEmpresa} | {empresa.NombreComercial} | CIF: {empresa.CIF} | " +
               $"Tlf: {empresa.Telefono} | Correo: {empresa.CorreoElectronico} | " +
               $"Dirección: {empresa.Direccion}";
    }
}
