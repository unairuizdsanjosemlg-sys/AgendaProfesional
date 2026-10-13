namespace AgendaProfesional.UI;

public readonly record struct PersonaDatos(
    string Nombre,
    string Apellidos,
    string Telefono,
    string Correo,
    string EmpresaAsignada)
{
    public bool SinCambios =>
        Nombre.Length == 0
        && Apellidos.Length == 0
        && Telefono.Length == 0
        && Correo.Length == 0
        && EmpresaAsignada.Length == 0;
}