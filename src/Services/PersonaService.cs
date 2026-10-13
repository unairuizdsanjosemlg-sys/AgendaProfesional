using AgendaProfesional.Models;
using AgendaProfesional.Services.Repositories;

namespace AgendaProfesional.Services;

public class PersonaService
{
    private readonly PersonaRepository _personaRepository;
    private readonly EmpresaRepository _empresaRepository;
    private readonly GestorRelaciones _gestorRelaciones;

    public PersonaService(PersonaRepository personaRepository, EmpresaRepository empresaRepository, GestorRelaciones gestorRelaciones)
    {
        _personaRepository = personaRepository;
        _empresaRepository = empresaRepository;
        _gestorRelaciones = gestorRelaciones;
    }

    public Persona AltaPersona(string nombre, string apellidos, string telefono, string correo, string empresaAsignada)
    {
        Persona persona = _personaRepository.Add(nombre, apellidos, telefono, correo, empresaAsignada);

        if (!string.IsNullOrWhiteSpace(empresaAsignada))
        {
            var empresas = _empresaRepository.FindByTexto(empresaAsignada);
            if (empresas.Count == 1)
            {
                _gestorRelaciones.Asignar(persona.IdPersona, empresas[0].IdEmpresa);
            }
        }

        return persona;
    }

    public List<Persona> ListarPersonas()
    {
        return _personaRepository.GetAll();
    }

    public List<Persona> BuscarPersonasPorNombre(string nombre, string apellidos)
    {
        return _personaRepository.FindByNombre(nombre, apellidos);
    }

    public bool ModificarPersona(int id, string? nombre, string? apellidos, string? telefono, string? correo, string? empresaAsignada)
    {
        return _personaRepository.Update(id, nombre, apellidos, telefono, correo, empresaAsignada);
    }

    public bool EliminarPersona(int id)
    {
        _gestorRelaciones.EliminarRelacionesDe(id);
        return _personaRepository.Delete(id);
    }

    public bool AsignarEmpresa(int idPersona, string nombreEmpresa)
    {
        var empresas = _empresaRepository.FindByTexto(nombreEmpresa);
        if (empresas.Count != 1)
        {
            return false;
        }

        _gestorRelaciones.Asignar(idPersona, empresas[0].IdEmpresa);

        var persona = _personaRepository.GetById(idPersona);
        if (persona != null)
        {
            persona.EmpresaAsignada = empresas[0].NombreComercial;
        }

        return true;
    }

    public bool DesvincularEmpresa(int idPersona)
    {
        if (_gestorRelaciones.IdEmpresaDe(idPersona) == null)
        {
            return false;
        }

        _gestorRelaciones.Desvincular(idPersona);

        var persona = _personaRepository.GetById(idPersona);
        if (persona != null)
        {
            persona.EmpresaAsignada = string.Empty;
        }

        return true;
    }
}