namespace AgendaProfesional.Services.Shared;

public static class ValorIndicadoONuevo
{
    public static string Obtener(string? valorIndicado, string valorActual)
    {
        return string.IsNullOrWhiteSpace(valorIndicado) ? valorActual : valorIndicado.Trim();
    }
}