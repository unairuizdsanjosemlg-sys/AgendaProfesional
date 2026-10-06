namespace AgendaProfesional;

public class GestorEmpresas
{
    private readonly List<Empresa> _empresas = new();
    private int _siguienteId = 1;

    public Empresa? AltaEmpresa(string nombreComercial, string cif, string telefono, string correoElectronico, string direccion)
    {
        if (ExisteCif(cif))
        {
            return null;
        }

        Empresa empresa = new(
            _siguienteId,
            (nombreComercial ?? string.Empty).Trim(),
            (cif ?? string.Empty).Trim(),
            (telefono ?? string.Empty).Trim(),
            (correoElectronico ?? string.Empty).Trim(),
            (direccion ?? string.Empty).Trim());

        _siguienteId++;
        _empresas.Add(empresa);
        return empresa;
    }

    public List<Empresa> Listar()
    {
        return _empresas
            .OrderBy(e => e.NombreComercial, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public List<Empresa> BuscarPorTexto(string texto)
    {
        string busca = (texto ?? string.Empty).Trim();

        if (busca.Length == 0)
        {
            return new List<Empresa>();
        }

        return _empresas
            .Where(e => Validaciones.Coincide(e.NombreComercial, busca)
                     || string.Equals(e.CIF, busca, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public bool Modificar(int id, string? nombreComercial, string? cif, string? telefono, string? correoElectronico, string? direccion)
    {
        Empresa? empresa = BuscarPorId(id);
        if (empresa == null)
        {
            return false;
        }

        string nuevoCif = ValorIndicadoONuevo(cif, empresa.CIF);
        if (!string.Equals(nuevoCif, empresa.CIF, StringComparison.OrdinalIgnoreCase) && ExisteCif(nuevoCif, id))
        {
            return false;
        }

        empresa.NombreComercial = ValorIndicadoONuevo(nombreComercial, empresa.NombreComercial);
        empresa.CIF = nuevoCif;
        empresa.Telefono = ValorIndicadoONuevo(telefono, empresa.Telefono);
        empresa.CorreoElectronico = ValorIndicadoONuevo(correoElectronico, empresa.CorreoElectronico);
        empresa.Direccion = ValorIndicadoONuevo(direccion, empresa.Direccion);
        return true;
    }

    public bool Eliminar(int id)
    {
        Empresa? empresa = BuscarPorId(id);
        if (empresa == null)
        {
            return false;
        }

        _empresas.Remove(empresa);
        return true;
    }

    /// Indica si ya existe una empresa con ese CIF (excluyendo la que se está editando).
    public bool ExisteCif(string cif, int? idExcluido = null)
    {
        string buscado = (cif ?? string.Empty).Trim();

        return _empresas.Any(e =>
            string.Equals(e.CIF, buscado, StringComparison.OrdinalIgnoreCase)
            && (idExcluido == null || e.IdEmpresa != idExcluido.Value));
    }

    private Empresa? BuscarPorId(int id)
    {
        return _empresas.FirstOrDefault(e => e.IdEmpresa == id);
    }

    private static string ValorIndicadoONuevo(string? valorIndicado, string valorActual)
    {
        return string.IsNullOrWhiteSpace(valorIndicado) ? valorActual : valorIndicado.Trim();
    }
}
