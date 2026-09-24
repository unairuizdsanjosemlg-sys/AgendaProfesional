namespace AgendaProfesional;

public static class Validaciones
{
    private const int LongitudTelefonoEspania = 9;

    public static bool EsCorreoValido(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
        {
            return false;
        }

        string[] partes = correo.Split('@');
        if (partes.Length != 2)
        {
            return false;
        }

        string dominio = partes[1];
        return !string.IsNullOrWhiteSpace(dominio) && dominio.Contains('.');
    }

    public static bool EsTelefonoValido(string? telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            return false;
        }

        if (telefono.Length != LongitudTelefonoEspania)
        {
            return false;
        }

        return telefono.All(char.IsDigit);
    }

    public static int LeerEntero(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensaje);
            string? entrada = Console.ReadLine();

            if (int.TryParse(entrada, out int valor) && valor >= minimo && valor <= maximo)
            {
                return valor;
            }

            Console.WriteLine($"Entrada no válida: introduce un número entre {minimo} y {maximo}.");
        }
    }
}