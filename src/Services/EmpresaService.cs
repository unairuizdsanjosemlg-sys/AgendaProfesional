using AgendaProfesional.Models;
using AgendaProfesional.Services.Repositories;

namespace AgendaProfesional.Services;

public class EmpresaService
{
    private readonly EmpresaRepository _empresaRepository;
    private readonly PersonaRepository _personaRepository;
    private readonly GestorRelaciones _gestorRelaciones;

    public EmpresaService(EmpresaRepository empresaRepository, PersonaRepository personaRepository, GestorRelaciones gestorRelaciones)
    {
        _empresaRepository = empresaRepository;
        _personaRepository = personaRepository;
        _gestorRelaciones = gestorRelaciones;
    }

    public Empresa? AltaEmpresa(string nombreComercial, string cif, string telefono, string correoElectronico, string direccion)
    {
        return _empresaRepository.Add(nombreComercial, cif, telefono, correoElectronico, direccion);
    }

    public List<Empresa> ListarEmpresas()
    {
        return _empresaRepository.GetAll();
    }

    public List<Empresa> BuscarEmpresasPorTexto(string texto)
    {
        return _empresaRepository.FindByTexto(texto);
    }

    public bool ModificarEmpresa(int id, string? nombreComercial, string? cif, string? telefono, string? correoElectronico, string? direccion)
    {
        return _empresaRepository.Update(id, nombreComercial, cif, telefono, correoElectronico, direccion);
    }

    public bool EliminarEmpresa(int id)
    {
        if (_gestorRelaciones.TienePersonas(id))
        {
            return false; // La lógica de desvinculación masiva está en la UI
        }

        return _empresaRepository.Delete(id);
    }

    public bool EliminarEmpresaConDesvinculacion(int id)
    {
        foreach (int idPersona in _gestorRelaciones.PersonasDe(id))
        {
            var persona = _personaRepository.GetById(idPersona);
            if (persona != null)
            {
                persona.EmpresaAsignada = string.Empty;
            }
        }

        _gestorRelaciones.DesvincularTodas(id);
        return _empresaRepository.Delete(id);
    }

    public List<Persona> VerPlantilla(int idEmpresa)
    {
        var ids = _gestorRelaciones.PersonasDe(idEmpresa);
        var todas = _personaRepository.GetAll();
        return todas.Where(p => ids.Contains(p.IdPersona)).ToList();
    }

    public bool TienePersonas(int idEmpresa)
    {
        return _gestorRelaciones.TienePersonas(idEmpresa);
    }

    public List<int> ObtenerPersonasVinculadas(int idEmpresa)
    {
        return _gestorRelaciones.PersonasDe(idEmpresa);
    }

    public bool ExisteCif(string cif, int? idExcluido = null)
    {
        return _empresaRepository.ExisteCif(cif, idExcluido);
    }
}