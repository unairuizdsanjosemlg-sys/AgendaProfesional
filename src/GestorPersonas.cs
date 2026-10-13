using AgendaProfesional.UI.Ayudas;
using AgendaProfesional.Models;
using AgendaProfesional.UI;
using AgendaProfesional.Services.Shared;

namespace AgendaProfesional;

public class GestorPersonas
{
    private readonly List<Persona> _personas = new();
    private int _siguienteId = 1;

    public Persona AltaPersona(string nombre, string apellidos, string telefono, string correo, string empresaAsignada)
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

    public List<Persona> Listar()
    {
        return _personas
            .OrderBy(p => p.Apellidos, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Nombre, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public List<Persona> BuscarPorNombre(string nombre, string apellidos)
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

    public bool Modificar(int id, string? nombre, string? apellidos, string? telefono, string? correo, string? empresaAsignada)
    {
        Persona? persona = BuscarPorId(id);
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

    public bool Eliminar(int id)
    {
        Persona? persona = BuscarPorId(id);
        if (persona == null)
        {
            return false;
        }

        _personas.Remove(persona);
        return true;
    }

    public Persona? BuscarPorId(int id)
    {
        return _personas.FirstOrDefault(p => p.IdPersona == id);
    }
}