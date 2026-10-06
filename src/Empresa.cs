namespace AgendaProfesional;

public class Empresa
{
    public int IdEmpresa { get; }
    public string NombreComercial { get; set; }
    public string CIF { get; set; }
    public string Telefono { get; set; }
    public string CorreoElectronico { get; set; }
    public string Direccion { get; set; }

    public Empresa(int idEmpresa, string nombreComercial, string cif, string telefono, string correoElectronico, string direccion)
    {
        IdEmpresa = idEmpresa;
        NombreComercial = nombreComercial;
        CIF = cif;
        Telefono = telefono;
        CorreoElectronico = correoElectronico;
        Direccion = direccion ?? string.Empty;
    }
}
