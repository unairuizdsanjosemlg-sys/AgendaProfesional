namespace AgendaProfesional;

public static class Presentacion
{
    public static string EmpresaDePersona(Persona persona, GestorRelaciones relaciones, GestorEmpresas empresas)
    {
        int? idEmpresa = relaciones.IdEmpresaDe(persona.IdPersona);
        if (idEmpresa == null)
        {
            return "Sin empresa";
        }

        Empresa? empresa = empresas.BuscarPorId(idEmpresa.Value);
        return empresa == null
            ? "Sin empresa"
            : $"{empresa.NombreComercial} (Id {empresa.IdEmpresa})";
    }

    public static string PersonaTexto(Persona persona, GestorRelaciones relaciones, GestorEmpresas empresas)
    {
        return $"Id {persona.IdPersona} | {persona.Nombre} {persona.Apellidos} | " +
               $"Tlf: {persona.Telefono} | Correo: {persona.Correo} | " +
               $"Empresa: {EmpresaDePersona(persona, relaciones, empresas)}";
    }

    public static string EmpresaTexto(Empresa empresa)
    {
        return $"Id {empresa.IdEmpresa} | {empresa.NombreComercial} | CIF: {empresa.CIF} | " +
               $"Tlf: {empresa.Telefono} | Correo: {empresa.CorreoElectronico} | " +
               $"Dirección: {empresa.Direccion}";
    }
}