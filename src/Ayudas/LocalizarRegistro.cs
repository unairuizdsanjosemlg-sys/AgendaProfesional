using System;
using System.Collections.Generic;

namespace AgendaProfesional.Ayudas;

public static class LocalizarRegistro
{
    public sealed record Resultado<T>(
        T? Registro,
        bool Cancelado,
        bool SinCoincidencias);

    public static Resultado<T> Ejecutar<T>(
        Consola consola,
        string mensajeBusqueda,
        Func<string, List<T>> buscar,
        Func<T, string> presentar,
        string etiquetaSinCoincidencias)
    {
        while (!consola.EntradaAgotada)
        {
            string? texto = consola.LeerCampoObligatorio(mensajeBusqueda, Validaciones.ValidarTexto);
            if (texto == null)
            {
                return new Resultado<T>(default, true, false);
            }

            List<T> coincidencias = buscar(texto);

            if (coincidencias.Count == 0)
            {
                Console.WriteLine(etiquetaSinCoincidencias);
                return new Resultado<T>(default, false, true);
            }

            if (coincidencias.Count == 1)
            {
                return new Resultado<T>(coincidencias[0], false, false);
            }

            Console.WriteLine($"Hay varias {etiquetaSinCoincidencias.ToLowerInvariant().Replace("no se ha encontrado ninguna ", "").Replace("no se ha encontrado ningún ", "")}coincidentes. Elige una:");
            for (int i = 0; i < coincidencias.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {presentar(coincidencias[i])}");
            }

            int? numero = consola.LeerNumeroCancelable(
                $"Número (1-{coincidencias.Count}): ", 1, coincidencias.Count);

            if (numero == null)
            {
                return new Resultado<T>(default, true, false);
            }

            return new Resultado<T>(coincidencias[numero.Value - 1], false, false);
        }

        return new Resultado<T>(default, true, false);
    }
}