using System;

namespace AgendaProfesional;

public sealed class Consola
{
    public const string PalabraCancelar = "cancelar";

    public bool EntradaAgotada { get; private set; }

    public string? LeerCampoObligatorio(string mensaje, Func<string, string?> validar)
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

    public string? LeerCampoModificacion(string mensaje, Func<string, string?> validar)
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

    public int LeerEntero(string mensaje, int minimo, int maximo)
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

    public int? LeerNumeroCancelable(string mensaje, int minimo, int maximo)
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

    public bool Confirmar(string mensaje)
    {
        while (!EntradaAgotada)
        {
            string? linea = LeerLinea(mensaje);

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

    public Confirmacion ConfirmarCancelable(string mensaje)
    {
        while (!EntradaAgotada)
        {
            string? linea = LeerLinea(mensaje);

            if (EsCancelacion(linea))
            {
                return Confirmacion.Cancelado;
            }

            string respuesta = (linea ?? string.Empty).Trim();

            if (respuesta.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                return Confirmacion.Si;
            }

            if (respuesta.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                return Confirmacion.No;
            }

            if (!EntradaAgotada)
            {
                Console.WriteLine($"Responde s (sí) o n (no), o escribe '{PalabraCancelar}' para volver al menú principal.");
            }
        }

        return Confirmacion.No;
    }

    private bool EsCancelacion(string? linea)
    {
        return linea != null && linea.Trim().Equals(PalabraCancelar, StringComparison.OrdinalIgnoreCase);
    }

    private string? LeerLinea(string mensaje)
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