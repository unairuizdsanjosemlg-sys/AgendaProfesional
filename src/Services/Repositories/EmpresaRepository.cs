using AgendaProfesional.Models;
using AgendaProfesional.Services.Shared;
using AgendaProfesional.UI;

namespace AgendaProfesional.Services.Repositories;

public class EmpresaRepository
{
    private readonly List<Empresa> _empresas = new();
    private int _siguienteId = 1;

    public Empresa? Add(string nombreComercial, string cif, string telefono, string correoElectronico, string direccion)
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

    public List<Empresa> GetAll()
    {
        return _empresas
            .OrderBy(e => e.NombreComercial, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public List<Empresa> FindByTexto(string texto)
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

    public bool Update(int id, string? nombreComercial, string? cif, string? telefono, string? correoElectronico, string? direccion)
    {
        Empresa? empresa = GetById(id);
        if (empresa == null)
        {
            return false;
        }

        string nuevoCif = ValorIndicadoONuevo.Obtener(cif, empresa.CIF);
        if (!string.Equals(nuevoCif, empresa.CIF, StringComparison.OrdinalIgnoreCase) && ExisteCif(nuevoCif, id))
        {
            return false;
        }

        empresa.NombreComercial = ValorIndicadoONuevo.Obtener(nombreComercial, empresa.NombreComercial);
        empresa.CIF = nuevoCif;
        empresa.Telefono = ValorIndicadoONuevo.Obtener(telefono, empresa.Telefono);
        empresa.CorreoElectronico = ValorIndicadoONuevo.Obtener(correoElectronico, empresa.CorreoElectronico);
        empresa.Direccion = ValorIndicadoONuevo.Obtener(direccion, empresa.Direccion);
        return true;
    }

    public bool Delete(int id)
    {
        Empresa? empresa = GetById(id);
        if (empresa == null)
        {
            return false;
        }

        _empresas.Remove(empresa);
        return true;
    }

    public bool ExisteCif(string cif, int? idExcluido = null)
    {
        string buscado = (cif ?? string.Empty).Trim();

        return _empresas.Any(e =>
            string.Equals(e.CIF, buscado, StringComparison.OrdinalIgnoreCase)
            && (idExcluido == null || e.IdEmpresa != idExcluido.Value));
    }

    public Empresa? GetById(int id)
    {
        return _empresas.FirstOrDefault(e => e.IdEmpresa == id);
    }
}