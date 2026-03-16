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

        public async Task<(bool Success, string Message)> MarcarLeccionCompletadaAsync(int idUsuario, int idLeccion, string? respuestaUsuario)
        {
            var leccion = await _leccionRepository.GetByIdAsync(idLeccion);
            if (leccion == null)
            {
                return (false, "La lección no existe");
            }

            var leccionesCurso = (await _leccionRepository.GetByCursoAsync(leccion.IdCurso)).OrderBy(l => l.Orden).ToList();
            var indiceLeccion = leccionesCurso.FindIndex(l => l.IdLeccion == idLeccion);

            if (indiceLeccion > 0)
            {
                var leccionAnterior = leccionesCurso[indiceLeccion - 1];
                var progresoAnterior = await _progresoRepository.GetByUsuarioLeccionAsync(idUsuario, leccionAnterior.IdLeccion);
                if (progresoAnterior?.Completado != true)
                {
                    return (false, "Debes completar la lección anterior antes de continuar");
                }
            }

            if (!string.IsNullOrWhiteSpace(leccion.RespuestaCorrecta))
            {
                if (string.IsNullOrWhiteSpace(respuestaUsuario))
                {
                    return (false, "Debes responder la pregunta para completar la lección");
                }

                if (!string.Equals(respuestaUsuario.Trim(), leccion.RespuestaCorrecta.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "Respuesta incorrecta. Revisa la lección e inténtalo de nuevo");
                }
            }

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

            return (true, "Lección completada correctamente");
        }

        public async Task<IEnumerable<LeccionViewModel>> GetLeccionesCursoAsync(int idCurso, int idUsuario)
        {
            var lecciones = await _leccionRepository.GetByCursoAsync(idCurso);
            var viewModels = new List<LeccionViewModel>();

            bool cadenaCompletada = true;
            foreach (var leccion in lecciones.OrderBy(l => l.Orden))
            {
                var progreso = await _progresoRepository.GetByUsuarioLeccionAsync(idUsuario, leccion.IdLeccion);
                bool completado = progreso?.Completado ?? false;
                bool puedeAcceder = leccion.Orden == 1 || cadenaCompletada || completado;

                var viewModel = new LeccionViewModel
                {
                    IdLeccion = leccion.IdLeccion,
                    IdCurso = leccion.IdCurso,
                    Titulo = leccion.Titulo,
                    Contenido = leccion.Contenido,
                    PreguntaEvaluacion = leccion.PreguntaEvaluacion,
                    Orden = leccion.Orden,
                    Completado = completado,
                    PuedeAcceder = puedeAcceder,
                    TituloCurso = leccion.Curso?.Titulo ?? ""
                };

                viewModels.Add(viewModel);
                cadenaCompletada = cadenaCompletada && completado;
            }

            return viewModels;
        }
    }
}
