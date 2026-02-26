using ProyectoPlataformaCursos.Models;

namespace ProyectoPlataformaCursos.Repositories.Interfaces
{
    public interface ICursoRepository
    {
        Task<IEnumerable<Curso>> GetAllAsync();
        Task<IEnumerable<Curso>> GetActivosAsync();
        Task<IEnumerable<Curso>> GetByProfesorAsync(int idProfesor);
        Task<Curso?> GetByIdAsync(int id);
        Task<Curso?> GetByIdWithLeccionesAsync(int id);
        Task<Curso> CreateAsync(Curso curso);
        Task UpdateAsync(Curso curso);
        Task DeleteAsync(int id);
    }
}
