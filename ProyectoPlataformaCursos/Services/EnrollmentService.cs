using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Models.ViewModels;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Services
{
    public class EnrollmentService
    {
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IProgresoRepository _progresoRepository;

        public EnrollmentService(
            IInscripcionRepository inscripcionRepository,
            ICursoRepository cursoRepository,
            IProgresoRepository progresoRepository)
        {
            _inscripcionRepository = inscripcionRepository;
            _cursoRepository = cursoRepository;
            _progresoRepository = progresoRepository;
        }

        public async Task<bool> InscribirUsuarioAsync(int idUsuario, int idCurso)
        {
            if (await _inscripcionRepository.ExistsAsync(idUsuario, idCurso))
            {
                return false;
            }

            var inscripcion = new Inscripcion
            {
                IdUsuario = idUsuario,
                IdCurso = idCurso,
                FechaInscripcion = DateTime.Now,
                Estado = "ACTIVO"
            };

            await _inscripcionRepository.CreateAsync(inscripcion);
            return true;
        }

        public async Task<IEnumerable<CursoViewModel>> GetMisCursosAsync(int idUsuario)
        {
            var inscripciones = await _inscripcionRepository.GetByUsuarioAsync(idUsuario);
            var viewModels = new List<CursoViewModel>();

            foreach (var inscripcion in inscripciones)
            {
                if (inscripcion.Curso != null)
                {
                    var numLecciones = inscripcion.Curso.Lecciones?.Count ?? 0;
                    var completadas = 0;

                    if (numLecciones > 0)
                    {
                        completadas = await _progresoRepository.GetCompletedCountAsync(idUsuario, inscripcion.IdCurso);
                    }

                    var viewModel = new CursoViewModel
                    {
                        IdCurso = inscripcion.Curso.IdCurso,
                        Titulo = inscripcion.Curso.Titulo,
                        Descripcion = inscripcion.Curso.Descripcion,
                        NombreProfesor = inscripcion.Curso.Profesor != null ?
                            $"{inscripcion.Curso.Profesor.Nombre} {inscripcion.Curso.Profesor.Apellidos}" : "Sin asignar",
                        FechaCreacion = inscripcion.Curso.FechaCreacion,
                        Activo = inscripcion.Curso.Activo,
                        NumeroLecciones = numLecciones,
                        EstaInscrito = true,
                        ProgresoPercentage = numLecciones > 0 ? (int)((completadas * 100.0) / numLecciones) : 0
                    };

                    viewModels.Add(viewModel);
                }
            }

            return viewModels;
        }
    }
}
