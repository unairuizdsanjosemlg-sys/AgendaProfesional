using AgendaProfesional.Services;
using AgendaProfesional.UI;

namespace AgendaProfesional.UI;

public static class MenuPrincipal
{
    public static void Ejecutar(
        Consola consola,
        PersonaService personaService,
        EmpresaService empresaService)
    {
        bool salir = false;

        while (!salir && !consola.EntradaAgotada)
        {
            Console.WriteLine();
            Console.WriteLine("=== AGENDA DE CONSULTORÍA ===");
            Console.WriteLine("1. Personas");
            Console.WriteLine("2. Empresas");
            Console.WriteLine("3. Salir");
            Console.WriteLine();

            int opcion = consola.LeerEntero("Elige una opción (1-3): ", 1, 3);

            if (consola.EntradaAgotada)
            {
                Console.WriteLine("Hasta pronto.");
                break;
            }

            switch (opcion)
            {
                case 1:
                    var resultadoPersonas = MenuPersonas.Ejecutar(consola, personaService, empresaService);
                    if (resultadoPersonas == ResultadoOperacion.VolverAlMenuPrincipal && consola.EntradaAgotada)
                    {
                        Console.WriteLine("Hasta pronto.");
                        salir = true;
                    }
                    break;
                case 2:
                    var resultadoEmpresas = MenuEmpresas.Ejecutar(consola, empresaService, personaService);
                    if (resultadoEmpresas == ResultadoOperacion.VolverAlMenuPrincipal && consola.EntradaAgotada)
                    {
                        Console.WriteLine("Hasta pronto.");
                        salir = true;
                    }
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
    }
}