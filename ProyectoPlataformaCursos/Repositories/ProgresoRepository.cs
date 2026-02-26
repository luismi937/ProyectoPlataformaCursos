using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Repositories
{
    public class ProgresoRepository : IProgresoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProgresoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Progreso>> GetByUsuarioAsync(int idUsuario)
        {
            return await _context.Progresos
                .Include(p => p.Leccion)
                .Where(p => p.IdUsuario == idUsuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<Progreso>> GetByUsuarioCursoAsync(int idUsuario, int idCurso)
        {
            return await _context.Progresos
                .Include(p => p.Leccion)
                .Where(p => p.IdUsuario == idUsuario && p.Leccion!.IdCurso == idCurso)
                .ToListAsync();
        }

        public async Task<Progreso?> GetByUsuarioLeccionAsync(int idUsuario, int idLeccion)
        {
            return await _context.Progresos
                .FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.IdLeccion == idLeccion);
        }

        public async Task<Progreso> CreateAsync(Progreso progreso)
        {
            _context.Progresos.Add(progreso);
            await _context.SaveChangesAsync();
            return progreso;
        }

        public async Task UpdateAsync(Progreso progreso)
        {
            _context.Entry(progreso).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCompletedCountAsync(int idUsuario, int idCurso)
        {
            return await _context.Progresos
                .Include(p => p.Leccion)
                .Where(p => p.IdUsuario == idUsuario && 
                           p.Leccion!.IdCurso == idCurso && 
                           p.Completado)
                .CountAsync();
        }
    }
}
