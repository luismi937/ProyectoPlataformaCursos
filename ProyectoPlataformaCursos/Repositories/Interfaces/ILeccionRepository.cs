using ProyectoPlataformaCursos.Models;

namespace ProyectoPlataformaCursos.Repositories.Interfaces
{
    public interface ILeccionRepository
    {
        Task<IEnumerable<Leccion>> GetByCursoAsync(int idCurso);
        Task<Leccion?> GetByIdAsync(int id);
        Task<Leccion> CreateAsync(Leccion leccion);
        Task UpdateAsync(Leccion leccion);
        Task DeleteAsync(int id);
    }
}
