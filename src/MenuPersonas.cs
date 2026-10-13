using AgendaProfesional.Ayudas;

namespace AgendaProfesional;

public static class MenuPersonas
{
    public static ResultadoOperacion Ejecutar(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        while (!consola.EntradaAgotada)
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

            int opcion = consola.LeerEntero("Elige una opción (1-8): ", 1, 8);

            if (consola.EntradaAgotada)
            {
                return ResultadoOperacion.VolverAlMenuPrincipal;
            }

            ResultadoOperacion resultado = opcion switch
            {
                1 => AltaPersona(consola, gestorPersonas, gestorEmpresas, gestorRelaciones),
                2 => Listado(consola, gestorPersonas, gestorEmpresas, gestorRelaciones),
                3 => Busqueda(consola, gestorPersonas, gestorEmpresas, gestorRelaciones),
                4 => Modificar(consola, gestorPersonas, gestorEmpresas, gestorRelaciones),
                5 => Baja(consola, gestorPersonas, gestorEmpresas, gestorRelaciones),
                6 => AsignarEmpresa(consola, gestorPersonas, gestorEmpresas, gestorRelaciones),
                7 => DesvincularEmpresa(consola, gestorPersonas, gestorEmpresas, gestorRelaciones),
                8 => ResultadoOperacion.VolverAlMenuPrincipal,
                _ => ResultadoOperacion.Continuar
            };

            if (resultado == ResultadoOperacion.VolverAlMenuPrincipal)
            {
                return ResultadoOperacion.VolverAlMenuPrincipal;
            }
        }

        return ResultadoOperacion.VolverAlMenuPrincipal;
    }

    private static (string Etiqueta, string Mensaje, Func<string, string?> Validar)[] CamposPersona()
    {
        return
        [
            ("Nombre", "Nombre: ", Validaciones.ValidarTexto),
            ("Apellidos", "Apellidos: ", Validaciones.ValidarTexto),
            ("Teléfono", "Teléfono (con prefijo internacional, p. ej. +34 600111222): ", Validaciones.ValidarTelefono),
            ("Correo", "Correo: ", Validaciones.ValidarCorreo)
        ];
    }

    private static bool LeerCamposPersona(
        Consola consola,
        bool esModificacion,
        GestorEmpresas gestorEmpresas,
        out PersonaDatos datos,
        out Empresa? empresaElegida)
    {
        var campos = CamposPersona();
        string[] valores = new string[campos.Length + 1];

        for (int i = 0; i < campos.Length; i++)
        {
            string? valor = esModificacion
                ? consola.LeerCampoModificacion(campos[i].Mensaje, campos[i].Validar)
                : consola.LeerCampoObligatorio(campos[i].Mensaje, campos[i].Validar);

            if (valor == null)
            {
                datos = new PersonaDatos(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                empresaElegida = null;
                return false;
            }

            valores[i] = valor;
        }

        empresaElegida = LeerEmpresaOpcional(consola, gestorEmpresas, out bool cancelado);
        if (cancelado)
        {
            datos = new PersonaDatos(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            return false;
        }

        valores[campos.Length] = empresaElegida?.NombreComercial ?? string.Empty;

        datos = new PersonaDatos(valores[0], valores[1], valores[2], valores[3], valores[4]);
        return true;
    }

    private static Empresa? LeerEmpresaOpcional(Consola consola, GestorEmpresas gestorEmpresas, out bool cancelado)
    {
        cancelado = false;

        while (!consola.EntradaAgotada)
        {
            string? valor = consola.LeerCampoModificacion("Empresa asignada: ", Validaciones.ValidarTexto);

            if (valor == null)
            {
                cancelado = true;
                return null;
            }

            if (valor.Length == 0)
            {
                return null;
            }

            var resultado = LocalizarRegistro.Ejecutar(
                consola,
                "Empresa asignada: ",
                t => gestorEmpresas.BuscarPorTexto(t),
                Presentacion.EmpresaTexto,
                "No se ha encontrado ninguna empresa con ese nombre.");

            if (resultado.Cancelado)
            {
                cancelado = true;
                return null;
            }

            if (resultado.SinCoincidencias)
            {
                continue;
            }

            return resultado.Registro;
        }

        cancelado = true;
        return null;
    }

    private static Empresa? PedirEmpresa(Consola consola, GestorEmpresas gestorEmpresas, out bool cancelado)
    {
        cancelado = false;

        var resultado = LocalizarRegistro.Ejecutar(
            consola,
            "Nombre de la empresa: ",
            t => gestorEmpresas.BuscarPorTexto(t),
            Presentacion.EmpresaTexto,
            "No se ha encontrado ninguna empresa con ese nombre o CIF.");

        if (resultado.Cancelado)
        {
            cancelado = true;
            return null;
        }

        if (resultado.SinCoincidencias)
        {
            return null;
        }

        return resultado.Registro;
    }

    private static ResultadoOperacion AltaPersona(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- ALTA DE PERSONA ---");
        Console.WriteLine("Todos los campos son obligatorios, excepto la 'Empresa asignada', que es opcional y, " +
                          "si se rellena, debe corresponder a una empresa ya registrada.");
        Console.WriteLine($"Escribe '{Consola.PalabraCancelar}' para volver al menú.");
        Console.WriteLine();

        if (!LeerCamposPersona(consola, esModificacion: false, gestorEmpresas, out PersonaDatos datos, out Empresa? empresaElegida))
        {
            Console.WriteLine("Alta cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        Persona persona = gestorPersonas.AltaPersona(datos.Nombre, datos.Apellidos, datos.Telefono, datos.Correo, datos.EmpresaAsignada);

        if (empresaElegida != null)
        {
            gestorRelaciones.Asignar(persona.IdPersona, empresaElegida.IdEmpresa);
        }

        Console.WriteLine($"Alta confirmada. Id asignado: {persona.IdPersona}.");
        return ResultadoOperacion.Continuar;
    }

    private static ResultadoOperacion Listado(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- LISTADO DE PERSONAS ---");

        List<Persona> personas = gestorPersonas.Listar();
        if (personas.Count == 0)
        {
            Console.WriteLine("No hay personas registradas en la agenda.");
            return ResultadoOperacion.Continuar;
        }

        foreach (Persona persona in personas)
        {
            Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));
        }

        return ResultadoOperacion.Continuar;
    }

    private static List<Persona>? PedirCoincidencias(Consola consola, GestorPersonas gestorPersonas)
    {
        string? nombre = consola.LeerCampoObligatorio("Nombre: ", Validaciones.ValidarTexto);
        if (nombre == null)
        {
            return null;
        }

        string? apellidos = consola.LeerCampoObligatorio("Apellidos: ", Validaciones.ValidarTexto);
        if (apellidos == null)
        {
            return null;
        }

        return gestorPersonas.BuscarPorNombre(nombre, apellidos);
    }

    private static Persona? LocalizarPersona(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones,
        out bool cancelado)
    {
        cancelado = false;

        List<Persona>? coincidencias = PedirCoincidencias(consola, gestorPersonas);
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

        int? numero = consola.LeerNumeroCancelable(
            $"Número de la persona (1-{coincidencias.Count}): ", 1, coincidencias.Count);

        if (numero == null)
        {
            cancelado = true;
            Console.WriteLine("Operación cancelada.");
            return null;
        }

        return coincidencias[numero.Value - 1];
    }

    private static ResultadoOperacion Busqueda(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- BÚSQUEDA DE PERSONAS ---");
        Console.WriteLine("La búsqueda se hace por nombre y apellidos, con coincidencia parcial.");
        Console.WriteLine($"Escribe '{Consola.PalabraCancelar}' para volver al menú.");
        Console.WriteLine();

        List<Persona>? resultados = PedirCoincidencias(consola, gestorPersonas);
        if (resultados == null)
        {
            Console.WriteLine("Búsqueda cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (resultados.Count == 0)
        {
            Console.WriteLine("No hay coincidencias para la búsqueda.");
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine($"Coincidencias encontradas ({resultados.Count}):");
        foreach (Persona persona in resultados)
        {
            Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));
        }

        return ResultadoOperacion.Continuar;
    }

    private static ResultadoOperacion Modificar(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- MODIFICAR PERSONA ---");

        Persona? persona = LocalizarPersona(consola, gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
        if (cancelado)
        {
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (persona == null)
        {
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine("Datos actuales:");
        Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));
        Console.WriteLine($"Escribe '{Consola.PalabraCancelar}' para volver al menú; " +
                          "un campo en blanco conserva su valor actual.");
        Console.WriteLine();

        if (!LeerCamposPersona(consola, esModificacion: true, gestorEmpresas, out PersonaDatos datos, out Empresa? empresaElegida))
        {
            Console.WriteLine("Modificación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (datos.SinCambios)
        {
            Console.WriteLine("No se ha introducido ningún cambio.");
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine();
        Console.WriteLine("Resumen de cambios:");
        MostrarResumen(persona, datos, gestorEmpresas, gestorRelaciones);

        switch (consola.ConfirmarCancelable("¿Guardar los cambios? (s/n): "))
        {
            case Confirmacion.Si:
                bool modificado = gestorPersonas.Modificar(
                    persona.IdPersona, datos.Nombre, datos.Apellidos, datos.Telefono, datos.Correo, datos.EmpresaAsignada);

                if (empresaElegida != null)
                {
                    gestorRelaciones.Asignar(persona.IdPersona, empresaElegida.IdEmpresa);
                }

                Console.WriteLine(modificado ? "Persona modificada correctamente." : "No se pudo modificar la persona.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Modificación cancelada.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Modificación cancelada.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }

    private static void MostrarResumen(
        Persona persona,
        PersonaDatos datos,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        (string Etiqueta, string Actual, string Nuevo)[] cambios =
        [
            ("Nombre", persona.Nombre, datos.Nombre),
            ("Apellidos", persona.Apellidos, datos.Apellidos),
            ("Teléfono", persona.Telefono, datos.Telefono),
            ("Correo", persona.Correo, datos.Correo),
            ("Empresa asignada", Presentacion.EmpresaDePersona(persona, gestorRelaciones, gestorEmpresas), datos.EmpresaAsignada)
        ];

        ResumenCambios.Mostrar(cambios);
    }

    private static ResultadoOperacion Baja(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- BAJA DE PERSONA ---");

        Persona? persona = LocalizarPersona(consola, gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
        if (cancelado)
        {
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (persona == null)
        {
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine("Registro afectado:");
        Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));

        switch (consola.ConfirmarCancelable("¿Eliminar definitivamente? (s/n): "))
        {
            case Confirmacion.Si:
                gestorRelaciones.EliminarRelacionesDe(persona.IdPersona);
                gestorPersonas.Eliminar(persona.IdPersona);
                Console.WriteLine("Persona eliminada.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Baja cancelada.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Baja cancelada.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }

    private static ResultadoOperacion AsignarEmpresa(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- ASIGNAR/CAMBIAR EMPRESA ---");

        Persona? persona = LocalizarPersona(consola, gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
        if (cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (persona == null)
        {
            return ResultadoOperacion.Continuar;
        }

        Empresa? empresa = PedirEmpresa(consola, gestorEmpresas, out cancelado);
        if (cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (empresa == null)
        {
            Console.WriteLine("No se ha realizado ningún cambio.");
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine();
        Console.WriteLine("Resumen de la asignación:");
        Console.WriteLine($"  Persona: {Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas)}");
        Console.WriteLine($"  Empresa: {Presentacion.EmpresaTexto(empresa)}");
        Console.WriteLine();

        switch (consola.ConfirmarCancelable("¿Guardar la asignación? (s/n): "))
        {
            case Confirmacion.Si:
                gestorRelaciones.Asignar(persona.IdPersona, empresa.IdEmpresa);
                persona.EmpresaAsignada = empresa.NombreComercial;
                Console.WriteLine($"Persona asignada a la empresa '{empresa.NombreComercial}'.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Asignación cancelada.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Asignación cancelada.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }

    private static ResultadoOperacion DesvincularEmpresa(
        Consola consola,
        GestorPersonas gestorPersonas,
        GestorEmpresas gestorEmpresas,
        GestorRelaciones gestorRelaciones)
    {
        Console.WriteLine();
        Console.WriteLine("--- DESVINCULAR EMPRESA ---");

        Persona? persona = LocalizarPersona(consola, gestorPersonas, gestorEmpresas, gestorRelaciones, out bool cancelado);
        if (cancelado)
        {
            Console.WriteLine("Operación cancelada.");
            return ResultadoOperacion.VolverAlMenuPrincipal;
        }

        if (persona == null)
        {
            return ResultadoOperacion.Continuar;
        }

        if (gestorRelaciones.IdEmpresaDe(persona.IdPersona) == null)
        {
            Console.WriteLine("Esa persona no tiene empresa asignada.");
            return ResultadoOperacion.Continuar;
        }

        Console.WriteLine("Persona afectada:");
        Console.WriteLine(Presentacion.PersonaTexto(persona, gestorRelaciones, gestorEmpresas));

        switch (consola.ConfirmarCancelable("¿Desvincular a la persona de su empresa? (s/n): "))
        {
            case Confirmacion.Si:
                gestorRelaciones.Desvincular(persona.IdPersona);
                persona.EmpresaAsignada = string.Empty;
                Console.WriteLine("Persona desvinculada de su empresa.");
                return ResultadoOperacion.Continuar;

            case Confirmacion.No:
                Console.WriteLine("Desvinculación cancelada.");
                return ResultadoOperacion.Continuar;

            default:
                Console.WriteLine("Desvinculación cancelada.");
                return ResultadoOperacion.VolverAlMenuPrincipal;
        }
    }
}