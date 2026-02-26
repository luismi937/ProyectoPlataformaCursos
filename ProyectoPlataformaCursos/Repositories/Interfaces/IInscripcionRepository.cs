using ProyectoPlataformaCursos.Models;

namespace ProyectoPlataformaCursos.Repositories.Interfaces
{
    public interface IInscripcionRepository
    {
        Task<IEnumerable<Inscripcion>> GetByUsuarioAsync(int idUsuario);
        Task<Inscripcion?> GetByUsuarioCursoAsync(int idUsuario, int idCurso);
        Task<Inscripcion> CreateAsync(Inscripcion inscripcion);
        Task<bool> ExistsAsync(int idUsuario, int idCurso);
    }
}
