namespace AgendaProfesional.Ayudas;

public static class ResumenCambios
{
    public static void Mostrar((string Etiqueta, string Actual, string Nuevo)[] cambios)
    {
        foreach ((string etiqueta, string actual, string nuevo) in cambios)
        {
            if (nuevo.Length > 0 && !string.Equals(actual, nuevo, StringComparison.Ordinal))
            {
                Console.WriteLine($"  {etiqueta}: {actual} -> {nuevo}");
            }
        }
    }
}