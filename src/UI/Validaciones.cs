using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AgendaProfesional.UI;

public static class Validaciones
{
    public const string PalabraCancelar = "cancelar";

    public const int MinimoDigitosPrefijo = 1;
    public const int MaximoDigitosPrefijo = 3;
    public const int MinimoDigitosNumero = 6;
    public const int MaximoDigitosNumero = 12;

    private const string MensajeObligatorio = "Este campo es obligatorio.";

    private static readonly Regex FormatoTelefono = new(
        $@"^\+\d{{{MinimoDigitosPrefijo},{MaximoDigitosPrefijo}}} ?\d{{{MinimoDigitosNumero},{MaximoDigitosNumero}}}$",
        RegexOptions.CultureInvariant);

    // CIF: letra o dígito inicial, 7 dígitos y dígito o letra de control.
    private static readonly Regex FormatoCif = new(
        @"^[A-Za-z0-9]\d{7}[A-Za-z0-9]$",
        RegexOptions.CultureInvariant);

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
        return !string.IsNullOrWhiteSpace(telefono) && FormatoTelefono.IsMatch(telefono);
    }

    public static string SinAcentos(string? texto)
    {
        if (string.IsNullOrEmpty(texto))
        {
            return string.Empty;
        }

        // El plegado solo se aplica al comparar: lo almacenado conserva los acentos que escribió el usuario.
        string plegado = texto.ToUpperInvariant().Normalize(NormalizationForm.FormD);
        StringBuilder resultado = new(plegado.Length);

        foreach (char caracter in plegado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(caracter);
            }
        }

        return resultado.ToString();
    }

    public static bool Coincide(string? texto, string? fragmento)
    {
        string buscado = SinAcentos(fragmento);

        return buscado.Length > 0
            && SinAcentos(texto).Contains(buscado, StringComparison.OrdinalIgnoreCase);
    }

    public static bool EsCancelacion(string? linea)
    {
        return linea != null && linea.Trim().Equals(PalabraCancelar, StringComparison.OrdinalIgnoreCase);
    }

    public static string? ValidarTexto(string valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? MensajeObligatorio : null;
    }

    public static string? ValidarCorreo(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
        {
            return MensajeObligatorio;
        }

        return EsCorreoValido(correo)
            ? null
            : "Correo no válido: debe contener una @ y un dominio con punto.";
    }

    public static string? ValidarTelefono(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            return MensajeObligatorio;
        }

        string valor = telefono.Trim();

        if (!valor.StartsWith('+'))
        {
            return "Teléfono no válido: debe empezar por '+' seguido del prefijo internacional " +
                   $"({MinimoDigitosPrefijo}-{MaximoDigitosPrefijo} dígitos).";
        }

        string resto = valor[1..];
        bool caracteresInvalidos = resto.Contains('+')
                                   || resto.Count(c => c == ' ') > 1
                                   || resto.Any(c => !char.IsDigit(c) && c != ' ');

        if (caracteresInvalidos)
        {
            return "Teléfono no válido: solo se admiten dígitos, el '+' inicial y un único espacio opcional.";
        }

        return EsTelefonoValido(valor)
            ? null
            : $"Teléfono no válido: el prefijo necesita entre {MinimoDigitosPrefijo} y {MaximoDigitosPrefijo} dígitos " +
              $"y el número entre {MinimoDigitosNumero} y {MaximoDigitosNumero}.";
    }

    public static string? ValidarCif(string cif)
    {
        if (string.IsNullOrWhiteSpace(cif))
        {
            return MensajeObligatorio;
        }

        string valor = cif.Trim();

        return FormatoCif.IsMatch(valor)
            ? null
            : "CIF no válido: debe tener una letra o dígito inicial, 7 dígitos y un dígito o letra de control.";
    }
}