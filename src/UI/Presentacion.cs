using AgendaProfesional.Models;

namespace AgendaProfesional.UI;

public static class Presentacion
{
    public static string EmpresaDePersona(Persona persona)
    {
        if (string.IsNullOrWhiteSpace(persona.EmpresaAsignada))
        {
            return "Sin empresa";
        }
        return $"{persona.EmpresaAsignada}";
    }

    public static string PersonaTexto(Persona persona)
    {
        return $"Id {persona.IdPersona} | {persona.Nombre} {persona.Apellidos} | " +
               $"Tlf: {persona.Telefono} | Correo: {persona.Correo} | " +
               $"Empresa: {EmpresaDePersona(persona)}";
    }

    public static string EmpresaTexto(Empresa empresa)
    {
        return $"Id {empresa.IdEmpresa} | {empresa.NombreComercial} | CIF: {empresa.CIF} | " +
               $"Tlf: {empresa.Telefono} | Correo: {empresa.CorreoElectronico} | " +
               $"Dirección: {empresa.Direccion}";
    }
}