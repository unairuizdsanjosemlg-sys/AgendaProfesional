namespace AgendaProfesional.UI;

public readonly record struct EmpresaDatos(
    string NombreComercial,
    string Cif,
    string Telefono,
    string CorreoElectronico,
    string Direccion)
{
    public bool SinCambios =>
        NombreComercial.Length == 0
        && Cif.Length == 0
        && Telefono.Length == 0
        && CorreoElectronico.Length == 0
        && Direccion.Length == 0;
}