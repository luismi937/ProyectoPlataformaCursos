using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Models.ViewModels;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Services
{
    public class CourseService
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IProgresoRepository _progresoRepository;

        public CourseService(
            ICursoRepository cursoRepository,
            IInscripcionRepository inscripcionRepository,
            IProgresoRepository progresoRepository)
        {
            _cursoRepository = cursoRepository;
            _inscripcionRepository = inscripcionRepository;
            _progresoRepository = progresoRepository;
        }

        public async Task<IEnumerable<CursoViewModel>> GetAllCursosAsync(int? usuarioId = null)
        {
            var cursos = await _cursoRepository.GetActivosAsync();
            var viewModels = new List<CursoViewModel>();

            foreach (var curso in cursos)
            {
                var viewModel = await MapToCursoViewModel(curso, usuarioId);
                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public async Task<IEnumerable<CursoViewModel>> GetCursosByProfesorAsync(int idProfesor)
        {
            var cursos = await _cursoRepository.GetByProfesorAsync(idProfesor);
            var viewModels = new List<CursoViewModel>();

            foreach (var curso in cursos)
            {
                var viewModel = await MapToCursoViewModel(curso, null);
                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public async Task<Curso?> GetCursoByIdAsync(int id)
        {
            return await _cursoRepository.GetByIdWithLeccionesAsync(id);
        }

        public async Task<Curso> CreateCursoAsync(Curso curso)
        {
            curso.FechaCreacion = DateTime.Now;
            curso.Activo = true;
            curso.Precio = 0;
            curso.AceptaEfectivo = true;
            curso.AceptaTarjeta = true;
            curso.AceptaTransferencia = true;
            return await _cursoRepository.CreateAsync(curso);
        }

        public async Task UpdateCursoAsync(Curso curso)
        {
            await _cursoRepository.UpdateAsync(curso);
        }

        public async Task DeleteCursoAsync(int id)
        {
            await _cursoRepository.DeleteAsync(id);
        }

        private async Task<CursoViewModel> MapToCursoViewModel(Curso curso, int? usuarioId)
        {
            var viewModel = new CursoViewModel
            {
                IdCurso = curso.IdCurso,
                Titulo = curso.Titulo,
                Descripcion = curso.Descripcion,
                NombreProfesor = curso.Profesor != null ? 
                    $"{curso.Profesor.Nombre} {curso.Profesor.Apellidos}" : "Sin asignar",
                FechaCreacion = curso.FechaCreacion,
                Activo = curso.Activo,
                Precio = curso.Precio,
                AceptaEfectivo = curso.AceptaEfectivo,
                AceptaTarjeta = curso.AceptaTarjeta,
                AceptaTransferencia = curso.AceptaTransferencia,
                FormasPagoDisponibles = string.Join(", ",
                    new[]
                    {
                        curso.AceptaEfectivo ? "Efectivo" : null,
                        curso.AceptaTarjeta ? "Tarjeta" : null,
                        curso.AceptaTransferencia ? "Transferencia" : null
                    }.Where(x => x != null)),
                NumeroLecciones = curso.Lecciones?.Count ?? 0,
                EstaInscrito = false,
                ProgresoPercentage = 0
            };

            if (usuarioId.HasValue)
            {
                viewModel.EstaInscrito = await _inscripcionRepository.ExistsAsync(usuarioId.Value, curso.IdCurso);
                
                if (viewModel.EstaInscrito && viewModel.NumeroLecciones > 0)
                {
                    var completadas = await _progresoRepository.GetCompletedCountAsync(usuarioId.Value, curso.IdCurso);
                    viewModel.ProgresoPercentage = (int)((completadas * 100.0) / viewModel.NumeroLecciones);
                }
            }

            return viewModel;
        }
    }
}
