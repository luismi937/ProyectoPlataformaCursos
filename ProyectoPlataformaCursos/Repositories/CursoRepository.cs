using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Repositories
{
    public class CursoRepository : ICursoRepository
    {
        private readonly ApplicationDbContext _context;

        public CursoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Curso>> GetAllAsync()
        {
            return await _context.Cursos
                .Include(c => c.Profesor)
                .Include(c => c.Lecciones)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> GetActivosAsync()
        {
            return await _context.Cursos
                .Include(c => c.Profesor)
                .Include(c => c.Lecciones)
                .Where(c => c.Activo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> GetByProfesorAsync(int idProfesor)
        {
            return await _context.Cursos
                .Include(c => c.Lecciones)
                .Where(c => c.IdProfesor == idProfesor)
                .ToListAsync();
        }

        public async Task<Curso?> GetByIdAsync(int id)
        {
            return await _context.Cursos
                .Include(c => c.Profesor)
                .Include(c => c.Lecciones)
                .FirstOrDefaultAsync(c => c.IdCurso == id);
        }

        public async Task<Curso?> GetByIdWithLeccionesAsync(int id)
        {
            return await _context.Cursos
                .Include(c => c.Profesor)
                .Include(c => c.Lecciones.OrderBy(l => l.Orden))
                .FirstOrDefaultAsync(c => c.IdCurso == id);
        }

        public async Task<Curso> CreateAsync(Curso curso)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            return curso;
        }

        public async Task UpdateAsync(Curso curso)
        {
            _context.Entry(curso).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
            }
        }
    }
}
