using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AgendaProfesional;

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

    /// Se activa al agotarse la entrada (Ctrl+Z o consola cerrada) para no dejar bucles colgados.
    public static bool EntradaAgotada { get; private set; }

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

    public static string? LeerCampoObligatorio(string mensaje, Func<string, string?> validar)
    {
        while (!EntradaAgotada)
        {
            string? linea = LeerLinea(mensaje);

            if (EsCancelacion(linea))
            {
                return null;
            }

            string valor = (linea ?? string.Empty).Trim();
            string? error = validar(valor);

            if (error == null)
            {
                return valor;
            }

            if (!EntradaAgotada)
            {
                Console.WriteLine(error);
            }
        }

        return null;
    }

    public static string? LeerCampoModificacion(string mensaje, Func<string, string?> validar)
    {
        while (!EntradaAgotada)
        {
            string? linea = LeerLinea(mensaje);

            if (EsCancelacion(linea))
            {
                return null;
            }

            string valor = (linea ?? string.Empty).Trim();

            if (valor.Length == 0)
            {
                return string.Empty;
            }

            string? error = validar(valor);

            if (error == null)
            {
                return valor;
            }

            if (!EntradaAgotada)
            {
                Console.WriteLine(error);
            }
        }

        return null;
    }

    public static int LeerEntero(string mensaje, int minimo, int maximo)
    {
        while (!EntradaAgotada)
        {
            string? linea = LeerLinea(mensaje);

            if (int.TryParse(linea, out int valor) && valor >= minimo && valor <= maximo)
            {
                return valor;
            }

            if (!EntradaAgotada)
            {
                Console.WriteLine($"Entrada no válida: introduce un número entre {minimo} y {maximo}.");
            }
        }

        return minimo;
    }

    public static int? LeerNumeroCancelable(string mensaje, int minimo, int maximo)
    {
        while (!EntradaAgotada)
        {
            string? linea = LeerLinea(mensaje);

            if (EsCancelacion(linea))
            {
                return null;
            }

            if (int.TryParse(linea, out int valor) && valor >= minimo && valor <= maximo)
            {
                return valor;
            }

            if (!EntradaAgotada)
            {
                Console.WriteLine($"Entrada no válida: introduce un número entre {minimo} y {maximo} " +
                                  $"o escribe '{PalabraCancelar}' para volver al menú.");
            }
        }

        return null;
    }

    public static bool Confirmar(string mensaje)
    {
        while (!EntradaAgotada)
        {
            string? linea = LeerLinea(mensaje);

            // 'cancelar' equivale a no confirmar: la operación se abandona sin guardar nada.
            if (EsCancelacion(linea))
            {
                return false;
            }

            string respuesta = (linea ?? string.Empty).Trim();

            if (respuesta.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (respuesta.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!EntradaAgotada)
            {
                Console.WriteLine($"Responde s (sí) o n (no), o escribe '{PalabraCancelar}' para volver al menú.");
            }
        }

        return false;
    }

    private static string? LeerLinea(string mensaje)
    {
        Console.Write(mensaje);
        string? linea = Console.ReadLine();

        if (linea == null)
        {
            EntradaAgotada = true;
        }

        return linea;
    }
}