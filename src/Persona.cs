namespace AgendaProfesional;

public class Persona
{
    public int IdPersona { get; }
    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
    public string EmpresaAsignada { get; set; }

    public Persona(int idPersona, string nombre, string apellidos, string telefono, string correo, string empresaAsignada)
    {
        IdPersona = idPersona;
        Nombre = nombre;
        Apellidos = apellidos;
        Telefono = telefono;
        Correo = correo;
        EmpresaAsignada = empresaAsignada ?? string.Empty;
    }
}