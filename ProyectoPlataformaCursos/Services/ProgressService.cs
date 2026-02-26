using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Models.ViewModels;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Services
{
    public class ProgressService
    {
        private readonly IProgresoRepository _progresoRepository;
        private readonly ILeccionRepository _leccionRepository;

        public ProgressService(
            IProgresoRepository progresoRepository,
            ILeccionRepository leccionRepository)
        {
            _progresoRepository = progresoRepository;
            _leccionRepository = leccionRepository;
        }

        public async Task<bool> MarcarLeccionCompletadaAsync(int idUsuario, int idLeccion)
        {
            var progreso = await _progresoRepository.GetByUsuarioLeccionAsync(idUsuario, idLeccion);

            if (progreso == null)
            {
                progreso = new Progreso
                {
                    IdUsuario = idUsuario,
                    IdLeccion = idLeccion,
                    Completado = true,
                    FechaUltimaActualizacion = DateTime.Now
                };
                await _progresoRepository.CreateAsync(progreso);
            }
            else if (!progreso.Completado)
            {
                progreso.Completado = true;
                progreso.FechaUltimaActualizacion = DateTime.Now;
                await _progresoRepository.UpdateAsync(progreso);
            }

            return true;
        }

        public async Task<IEnumerable<LeccionViewModel>> GetLeccionesCursoAsync(int idCurso, int idUsuario)
        {
            var lecciones = await _leccionRepository.GetByCursoAsync(idCurso);
            var viewModels = new List<LeccionViewModel>();

            foreach (var leccion in lecciones)
            {
                var progreso = await _progresoRepository.GetByUsuarioLeccionAsync(idUsuario, leccion.IdLeccion);

                var viewModel = new LeccionViewModel
                {
                    IdLeccion = leccion.IdLeccion,
                    IdCurso = leccion.IdCurso,
                    Titulo = leccion.Titulo,
                    Contenido = leccion.Contenido,
                    Orden = leccion.Orden,
                    Completado = progreso?.Completado ?? false,
                    TituloCurso = leccion.Curso?.Titulo ?? ""
                };

                viewModels.Add(viewModel);
            }

            return viewModels;
        }
    }
}
