using AgendaProfesional.Models;
using AgendaProfesional.Services.Shared;
using AgendaProfesional.UI;

namespace AgendaProfesional.Services.Repositories;

public class PersonaRepository
{
    private readonly List<Persona> _personas = new();
    private int _siguienteId = 1;

    public Persona Add(string nombre, string apellidos, string telefono, string correo, string empresaAsignada)
    {
        Persona persona = new(
            _siguienteId,
            (nombre ?? string.Empty).Trim(),
            (apellidos ?? string.Empty).Trim(),
            (telefono ?? string.Empty).Trim(),
            (correo ?? string.Empty).Trim(),
            (empresaAsignada ?? string.Empty).Trim());

        _siguienteId++;
        _personas.Add(persona);
        return persona;
    }

    public List<Persona> GetAll()
    {
        return _personas
            .OrderBy(p => p.Apellidos, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Nombre, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public Persona? GetById(int id)
    {
        return _personas.FirstOrDefault(p => p.IdPersona == id);
    }

    public List<Persona> FindByNombre(string nombre, string apellidos)
    {
        string buscaNombre = (nombre ?? string.Empty).Trim();
        string buscaApellidos = (apellidos ?? string.Empty).Trim();

        if (buscaNombre.Length == 0 || buscaApellidos.Length == 0)
        {
            return new List<Persona>();
        }

        return _personas
            .Where(p => Validaciones.Coincide(p.Nombre, buscaNombre)
                     && Validaciones.Coincide(p.Apellidos, buscaApellidos))
            .ToList();
    }

    public bool Update(int id, string? nombre, string? apellidos, string? telefono, string? correo, string? empresaAsignada)
    {
        Persona? persona = GetById(id);
        if (persona == null)
        {
            return false;
        }

        persona.Nombre = ValorIndicadoONuevo.Obtener(nombre, persona.Nombre);
        persona.Apellidos = ValorIndicadoONuevo.Obtener(apellidos, persona.Apellidos);
        persona.Telefono = ValorIndicadoONuevo.Obtener(telefono, persona.Telefono);
        persona.Correo = ValorIndicadoONuevo.Obtener(correo, persona.Correo);
        persona.EmpresaAsignada = ValorIndicadoONuevo.Obtener(empresaAsignada, persona.EmpresaAsignada);
        return true;
    }

    public bool Delete(int id)
    {
        Persona? persona = GetById(id);
        if (persona == null)
        {
            return false;
        }

        _personas.Remove(persona);
        return true;
    }
}