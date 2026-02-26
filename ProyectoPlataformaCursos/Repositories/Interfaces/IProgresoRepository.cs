using ProyectoPlataformaCursos.Models;

namespace ProyectoPlataformaCursos.Repositories.Interfaces
{
    public interface IProgresoRepository
    {
        Task<IEnumerable<Progreso>> GetByUsuarioAsync(int idUsuario);
        Task<IEnumerable<Progreso>> GetByUsuarioCursoAsync(int idUsuario, int idCurso);
        Task<Progreso?> GetByUsuarioLeccionAsync(int idUsuario, int idLeccion);
        Task<Progreso> CreateAsync(Progreso progreso);
        Task UpdateAsync(Progreso progreso);
        Task<int> GetCompletedCountAsync(int idUsuario, int idCurso);
    }
}
