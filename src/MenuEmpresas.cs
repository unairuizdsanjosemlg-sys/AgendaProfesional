using AgendaProfesional.Ayudas;

namespace AgendaProfesional;

public static class MenuEmpresas
{
    public static ResultadoOperacion Ejecutar(
        Consola consola,
        GestorEmpresas gestor,
        GestorPersonas gestorPersonas,
        GestorRelaciones gestorRelaciones)
    {
        while (!consola.EntradaAgotada)
        {
            Console.WriteLine();
            Console.WriteLine("=== GESTIÓN DE EMPRESAS ===");
            Console.WriteLine("1. Alta de empresa");
            Console.WriteLine("2. Listado");
            Console.WriteLine("3. Búsqueda");
            Console.WriteLine("4. Modificar");
            Console.WriteLine("5. Baja");
            Console.WriteLine("6. Ver personas de la empresa");
            Console.WriteLine("7. Volver al menú principal");
            Console.WriteLine();

            int opcion = consola.LeerEntero("Elige una opción (1-7): ", 1, 7);

            if (consola.EntradaAgotada)
            {
                return ResultadoOperacion.VolverAlMenuPrincipal;
            }

            ResultadoOperacion resultado = opcion switch
            {
                1 => Alta(consola, gestor),
                2 => Listado(consola, gestor),
                3 => Busqueda(consola, gestor),
                4 => Modificar(consola, gestor),
                5 => Baja(consola, gestor, gestorPersonas, gestorRelaciones),
                6 => VerPersonas(consola, gestor, gestorPersonas, gestorRelaciones),
                7 => ResultadoOperacion.VolverAlMenuPrincipal,
                _ => ResultadoOperacion.Continuar
            };

            if (resultado == ResultadoOperacion.VolverAlMenuPrincipal)
            {
                return ResultadoOperacion.VolverAlMenuPrincipal;
            }
        }

        return ResultadoOperacion.VolverAlMenuPrincipal;
    }

    private static ResultadoOperacion Alta(Consola consola, GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- ALTA DE EMPRESA ---");
        Console.WriteLine($"Todos los campos son obligatorios. Escribe '{Consola.PalabraCancelar}' para volver al menú principal.");
        Console.WriteLine();

        string? nombreComercial = consola.LeerCampoObligatorio("Nombre comercial: ", Validaciones.ValidarTexto);
        if (nombreComercial == null)
        {
            Console.WriteLine("Alta cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? cif = LeerCif(consola, gestor, null);
        if (cif == null)
        {
            Console.WriteLine("Alta cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? telefono = consola.LeerCampoObligatorio(
            "Teléfono (con prefijo internacional, p. ej. +34 600111222): ", Validaciones.ValidarTelefono);
        if (telefono == null)
        {
            Console.WriteLine("Alta cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? correo = consola.LeerCampoObligatorio("Correo: ", Validaciones.ValidarCorreo);
        if (correo == null)
        {
            Console.WriteLine("Alta cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? direccion = consola.LeerCampoObligatorio("Dirección: ", Validaciones.ValidarTexto);
        if (direccion == null)
        {
            Console.WriteLine("Alta cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        Console.WriteLine();
        Console.WriteLine("Resumen del alta:");
        Console.WriteLine($"  Nombre comercial: {nombreComercial}");
        Console.WriteLine($"  CIF: {cif}");
        Console.WriteLine($"  Teléfono: {telefono}");
        Console.WriteLine($"  Correo: {correo}");
        Console.WriteLine($"  Dirección: {direccion}");
        Console.WriteLine();

        switch (consola.ConfirmarCancelable("¿Confirmar el alta? (s/n): "))
        {
            case Confirmacion.Si:
                Empresa? empresa = gestor.AltaEmpresa(nombreComercial, cif, telefono, correo, direccion);
                if (empresa == null)
                {
                    Console.WriteLine("No se pudo dar de alta: el CIF ya está registrado.");
                    return ResultadoOperacion.Continuar;
                }

                Console.WriteLine($"Alta confirmada. Id asignado: {empresa.IdEmpresa}.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Alta cancelada.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Alta cancelada.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }

    private static string? LeerCif(Consola consola, GestorEmpresas gestor, int? idExcluido)
    {
        while (!consola.EntradaAgotada)
        {
            string? valor = consola.LeerCampoObligatorio("CIF (letra o dígito + 7 dígitos + control): ",
                v => ValidarCifUnico(gestor, v, idExcluido));

            if (valor == null)
            {
                return null;
            }

            string? error = Validaciones.ValidarCif(valor);
            if (error != null)
            {
                Console.WriteLine(error);
                continue;
            }

            if (gestor.ExisteCif(valor, idExcluido))
            {
                Console.WriteLine("Ese CIF ya está registrado por otra empresa.");
                continue;
            }

            return valor.Trim();
        }

        return null;
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

    private static ResultadoOperacion Listado(Consola consola, GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- LISTADO DE EMPRESAS ---");

        List<Empresa> empresas = gestor.Listar();
        if (empresas.Count == 0)
        {
            Console.WriteLine("No hay empresas registradas en la agenda.");
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine(string.Format("{0,-4} {1,-24} {2,-11} {3,-16} {4,-30} {5}",
            "Id", "Nombre comercial", "CIF", "Teléfono", "Correo", "Dirección"));

        foreach (Empresa empresa in empresas)
        {
            Console.WriteLine(string.Format("{0,-4} {1,-24} {2,-11} {3,-16} {4,-30} {5}",
                empresa.IdEmpresa, empresa.NombreComercial, empresa.CIF,
                empresa.Telefono, empresa.CorreoElectronico, empresa.Direccion));
        }

        return ResultadoOperacion.Continuar;
    }

    private static ResultadoOperacion Busqueda(Consola consola, GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- BÚSQUEDA DE EMPRESAS ---");
        Console.WriteLine("Coincidencia parcial por nombre comercial o CIF completo.");
        Console.WriteLine($"Escribe '{Consola.PalabraCancelar}' para volver al menú principal.");
        Console.WriteLine();

        string? texto = consola.LeerCampoObligatorio("Nombre comercial o CIF: ", Validaciones.ValidarTexto);
        if (texto == null)
        {
            Console.WriteLine("Búsqueda cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        List<Empresa> resultados = gestor.BuscarPorTexto(texto);
        if (resultados.Count == 0)
        {
            Console.WriteLine("No hay coincidencias para la búsqueda.");
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine($"Coincidencias encontradas ({resultados.Count}):");
        foreach (Empresa empresa in resultados)
        {
            Console.WriteLine(Presentacion.EmpresaTexto(empresa));
        }

        return ResultadoOperacion.Continuar;
    }

    private static ResultadoOperacion Modificar(Consola consola, GestorEmpresas gestor)
    {
        Console.WriteLine();
        Console.WriteLine("--- MODIFICAR EMPRESA ---");

        var resultadoLocalizar = LocalizarRegistro.Ejecutar(
            consola,
            "Nombre comercial o CIF: ",
            t => gestor.BuscarPorTexto(t),
            Presentacion.EmpresaTexto,
            "No se ha encontrado ninguna empresa con ese nombre o CIF.");

        if (resultadoLocalizar.Cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (resultadoLocalizar.SinCoincidencias)
        {
            return ResultadoOperacion.Continuar;
        }

        Empresa? empresa = resultadoLocalizar.Registro!;

        Console.WriteLine("Datos actuales:");
        Console.WriteLine(Presentacion.EmpresaTexto(empresa));
        Console.WriteLine($"Escribe '{Consola.PalabraCancelar}' para volver al menú principal; " +
                          "un campo en blanco conserva su valor actual.");
        Console.WriteLine();

        string? nombreComercial = consola.LeerCampoModificacion("Nombre comercial: ", Validaciones.ValidarTexto);
        if (nombreComercial == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? cif = LeerCifModificacion(consola, gestor, empresa.IdEmpresa);
        if (cif == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? telefono = consola.LeerCampoModificacion(
            "Teléfono (con prefijo internacional, p. ej. +34 600111222): ", Validaciones.ValidarTelefono);
        if (telefono == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? correo = consola.LeerCampoModificacion("Correo: ", Validaciones.ValidarCorreo);
        if (correo == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        string? direccion = consola.LeerCampoModificacion("Dirección: ", Validaciones.ValidarTexto);
        if (direccion == null)
        {
            Console.WriteLine("Modificación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (nombreComercial.Length == 0 && cif.Length == 0 && telefono.Length == 0
            && correo.Length == 0 && direccion.Length == 0)
        {
            Console.WriteLine("No se ha introducido ningún cambio.");
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine();
        Console.WriteLine("Empresa afectada:");
        Console.WriteLine(Presentacion.EmpresaTexto(empresa));
        Console.WriteLine("Resumen de cambios:");
        MostrarResumen(empresa, nombreComercial, cif, telefono, correo, direccion);

        switch (consola.ConfirmarCancelable("¿Guardar los cambios? (s/n): "))
        {
            case Confirmacion.Si:
                bool modificado = gestor.Modificar(
                    empresa.IdEmpresa, nombreComercial, cif, telefono, correo, direccion);
                Console.WriteLine(modificado
                    ? "Empresa modificada correctamente."
                    : "No se pudo modificar la empresa.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Modificación cancelada.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Modificación cancelada.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }

    private static string? LeerCifModificacion(Consola consola, GestorEmpresas gestor, int idExcluido)
    {
        while (!consola.EntradaAgotada)
        {
            string? valor = consola.LeerCampoModificacion("CIF: ",
                v => ValidarCifUnico(gestor, v, idExcluido));

            if (valor == null)
            {
                return null;
            }

            if (valor.Length == 0)
            {
                return string.Empty;
            }

            string? error = Validaciones.ValidarCif(valor);
            if (error != null)
            {
                Console.WriteLine(error);
                continue;
            }

            if (gestor.ExisteCif(valor, idExcluido))
            {
                Console.WriteLine("Ese CIF ya está registrado por otra empresa.");
                continue;
            }

            return valor.Trim();
        }

        return null;
    }

    private static ResultadoOperacion Baja(
        Consola consola,
        GestorEmpresas gestor,
        GestorPersonas gestorPersonas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- BAJA DE EMPRESA ---");

        var resultadoLocalizar = LocalizarRegistro.Ejecutar(
            consola,
            "Nombre comercial o CIF: ",
            t => gestor.BuscarPorTexto(t),
            Presentacion.EmpresaTexto,
            "No se ha encontrado ninguna empresa con ese nombre o CIF.");

        if (resultadoLocalizar.Cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (resultadoLocalizar.SinCoincidencias)
        {
            return ResultadoOperacion.Continuar;
        }

        Empresa? empresa = resultadoLocalizar.Registro!;

        Console.WriteLine("Registro afectado:");
        Console.WriteLine(Presentacion.EmpresaTexto(empresa));

        switch (consola.ConfirmarCancelable("¿Eliminar definitivamente? (s/n): "))
        {
            case Confirmacion.Si:
                int personasVinculadas = gestorRelaciones.PersonasDe(empresa.IdEmpresa).Count;

                if (personasVinculadas > 0)
                {
                    return BajaConPlantilla(consola, gestor, gestorPersonas, gestorRelaciones, empresa, personasVinculadas);
                }

                gestor.Eliminar(empresa.IdEmpresa);
                Console.WriteLine("Empresa eliminada.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Baja cancelada.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Baja cancelada.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }

    private static ResultadoOperacion BajaConPlantilla(
        Consola consola,
        GestorEmpresas gestor,
        GestorPersonas gestorPersonas,
        GestorRelaciones gestorRelaciones,
        Empresa empresa,
        int personasVinculadas)
    {
        string textoPersonas = personasVinculadas == 1 ? "persona relacionada" : "personas relacionadas";

        Console.WriteLine();
        Console.WriteLine($"Esta empresa tiene {personasVinculadas} {textoPersonas}.");
        Console.WriteLine("No se puede borrar una empresa con personas relacionadas.");
        Console.WriteLine("Para poder borrarla, primero hay que desvincular a todas sus personas.");
        Console.WriteLine("Elige una opción:");
        Console.WriteLine("1. Cancelar el borrado");
        Console.WriteLine("2. Desvincular a todas las personas para poder borrarla");
        Console.WriteLine();

        int? opcion = consola.LeerNumeroCancelable("Elige una opción (1-2): ", 1, 2);
        if (opcion == null)
        {
            Console.WriteLine("Borrado cancelado.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (opcion.Value == 1)
        {
            Console.WriteLine("Borrado cancelado. La empresa y sus relaciones se mantienen.");
            return ResultadoOperacion.Continuar;
        }

        switch (consola.ConfirmarCancelable(
            $"¿Desvincular a las {personasVinculadas} {textoPersonas} y borrar la empresa? (s/n): "))
        {
            case Confirmacion.Si:
                foreach (int idPersona in gestorRelaciones.PersonasDe(empresa.IdEmpresa))
                {
                    Persona? persona = gestorPersonas.BuscarPorId(idPersona);
                    if (persona != null)
                    {
                        persona.EmpresaAsignada = string.Empty;
                    }
                }

                gestorRelaciones.DesvincularTodas(empresa.IdEmpresa);
                gestor.Eliminar(empresa.IdEmpresa);
                Console.WriteLine($"Empresa '{empresa.NombreComercial}' eliminada tras desvincular a sus {personasVinculadas} {textoPersonas}.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Borrado cancelado. La empresa no se ha eliminado.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Borrado cancelado.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }

    private static ResultadoOperacion VerPersonas(
        Consola consola,
        GestorEmpresas gestor,
        GestorPersonas gestorPersonas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- PERSONAS DE LA EMPRESA ---");

        var resultadoLocalizar = LocalizarRegistro.Ejecutar(
            consola,
            "Nombre comercial o CIF: ",
            t => gestor.BuscarPorTexto(t),
            Presentacion.EmpresaTexto,
            "No se ha encontrado ninguna empresa con ese nombre o CIF.");

        if (resultadoLocalizar.Cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (resultadoLocalizar.SinCoincidencias)
        {
            return ResultadoOperacion.Continuar;
        }

        Empresa? empresa = resultadoLocalizar.Registro!;

        List<int> idsPlantilla = gestorRelaciones.PersonasDe(empresa.IdEmpresa);

        Console.WriteLine($"Empresa: {Presentacion.EmpresaTexto(empresa)}");

        if (idsPlantilla.Count == 0)
        {
            Console.WriteLine("Esta empresa no tiene personas asignadas.");
            return ResultadoOperacion.Continuar;
        }

        List<Persona> plantilla = gestorPersonas.Listar()
            .Where(p => idsPlantilla.Contains(p.IdPersona))
            .ToList();

        Console.WriteLine($"Personas asociadas ({plantilla.Count}):");
        foreach (Persona persona in plantilla)
        {
            Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestor));
        }

        return ResultadoOperacion.Continuar;
    }

    private static void MostrarResumen(
        Empresa empresa,
        string nombreComercial,
        string cif,
        string telefono,
        string correo,
        string direccion)
    {
        (string Etiqueta, string Actual, string Nuevo)[] cambios =
        [
            ("Nombre comercial", empresa.NombreComercial, nombreComercial),
            ("CIF", empresa.CIF, cif),
            ("Teléfono", empresa.Telefono, telefono),
            ("Correo", empresa.CorreoElectronico, correo),
            ("Dirección", empresa.Direccion, direccion)
        ];

        ResumenCambios.Mostrar(cambios);
    }
}