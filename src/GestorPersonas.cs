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

    public Persona? BuscarPorId(int id)
    {
        return _personas.FirstOrDefault(p => p.IdPersona == id);
    }

    public List<Persona> BuscarPorTexto(string texto)
    {
        string busqueda = (texto ?? string.Empty).Trim();
        if (busqueda.Length == 0)
        {
            return new List<Persona>();
        }

        return _personas
            .Where(p => p.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase)
                     || p.Apellidos.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public bool Modificar(int id, string nombre, string apellidos, string telefono, string correo, string empresaAsignada)
    {
        Persona? persona = BuscarPorId(id);
        if (persona == null)
        {
            return false;
        }

        persona.Nombre = (nombre ?? string.Empty).Trim();
        persona.Apellidos = (apellidos ?? string.Empty).Trim();
        persona.Telefono = (telefono ?? string.Empty).Trim();
        persona.Correo = (correo ?? string.Empty).Trim();
        persona.EmpresaAsignada = (empresaAsignada ?? string.Empty).Trim();
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
}