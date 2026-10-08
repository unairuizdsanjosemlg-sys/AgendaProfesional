namespace AgendaProfesional;

public readonly record struct RelacionAsignacion(int IdPersona, int IdEmpresa);

public class GestorRelaciones
{
    private const int CapacidadInicial = 4;

    private RelacionAsignacion[] _relaciones = new RelacionAsignacion[CapacidadInicial];
    private int _contador;

    public void Asignar(int idPersona, int idEmpresa)
    {
        int indice = IndiceDe(idPersona);

        if (indice >= 0)
        {
            _relaciones[indice] = new RelacionAsignacion(idPersona, idEmpresa);
            return;
        }

        if (_contador == _relaciones.Length)
        {
            Array.Resize(ref _relaciones, _relaciones.Length * 2);
        }

        _relaciones[_contador++] = new RelacionAsignacion(idPersona, idEmpresa);
    }

    public bool Desvincular(int idPersona)
    {
        int indice = IndiceDe(idPersona);
        if (indice < 0)
        {
            return false;
        }

        _relaciones[indice] = _relaciones[_contador - 1];
        _contador--;
        return true;
    }

    public int? IdEmpresaDe(int idPersona)
    {
        int indice = IndiceDe(idPersona);
        return indice >= 0 ? _relaciones[indice].IdEmpresa : null;
    }

    public bool TienePersonas(int idEmpresa)
    {
        for (int i = 0; i < _contador; i++)
        {
            if (_relaciones[i].IdEmpresa == idEmpresa)
            {
                return true;
            }
        }

        return false;
    }

    public List<int> PersonasDe(int idEmpresa)
    {
        List<int> personas = new();

        for (int i = 0; i < _contador; i++)
        {
            if (_relaciones[i].IdEmpresa == idEmpresa)
            {
                personas.Add(_relaciones[i].IdPersona);
            }
        }

        return personas;
    }

    public int DesvincularTodas(int idEmpresa)
    {
        int desvinculadas = 0;
        int escritura = 0;

        for (int lectura = 0; lectura < _contador; lectura++)
        {
            if (_relaciones[lectura].IdEmpresa == idEmpresa)
            {
                desvinculadas++;
            }
            else
            {
                _relaciones[escritura++] = _relaciones[lectura];
            }
        }

        _contador = escritura;
        return desvinculadas;
    }

    public void EliminarRelacionesDe(int idPersona)
    {
        Desvincular(idPersona);
    }

    private int IndiceDe(int idPersona)
    {
        for (int i = 0; i < _contador; i++)
        {
            if (_relaciones[i].IdPersona == idPersona)
            {
                return i;
            }
        }

        return -1;
    }
}