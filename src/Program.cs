using AgendaProfesional;
using AgendaProfesional.Models;
using AgendaProfesional.Services;
using AgendaProfesional.Services.Repositories;
using AgendaProfesional.UI;

Consola consola = new();
GestorRelaciones gestorRelaciones = new();
PersonaRepository personaRepository = new();
EmpresaRepository empresaRepository = new();
PersonaService personaService = new(personaRepository, empresaRepository, gestorRelaciones);
EmpresaService empresaService = new(empresaRepository, personaRepository, gestorRelaciones);

MenuPrincipal.Ejecutar(consola, personaService, empresaService);