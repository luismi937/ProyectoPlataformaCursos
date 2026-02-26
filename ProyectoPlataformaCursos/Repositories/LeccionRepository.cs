using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Repositories
{
    public class LeccionRepository : ILeccionRepository
    {
        private readonly ApplicationDbContext _context;

        public LeccionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Leccion>> GetByCursoAsync(int idCurso)
        {
            return await _context.Lecciones
                .Where(l => l.IdCurso == idCurso)
                .OrderBy(l => l.Orden)
                .ToListAsync();
        }

        public async Task<Leccion?> GetByIdAsync(int id)
        {
            return await _context.Lecciones
                .Include(l => l.Curso)
                .FirstOrDefaultAsync(l => l.IdLeccion == id);
        }

        public async Task<Leccion> CreateAsync(Leccion leccion)
        {
            _context.Lecciones.Add(leccion);
            await _context.SaveChangesAsync();
            return leccion;
        }

        public async Task UpdateAsync(Leccion leccion)
        {
            _context.Entry(leccion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var leccion = await _context.Lecciones.FindAsync(id);
            if (leccion != null)
            {
                _context.Lecciones.Remove(leccion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
