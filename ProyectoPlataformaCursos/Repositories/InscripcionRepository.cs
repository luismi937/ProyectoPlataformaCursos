using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Repositories
{
    public class InscripcionRepository : IInscripcionRepository
    {
        private readonly ApplicationDbContext _context;

        public InscripcionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inscripcion>> GetByUsuarioAsync(int idUsuario)
        {
            return await _context.Inscripciones
                .Include(i => i.Curso)
                    .ThenInclude(c => c!.Profesor)
                .Include(i => i.Curso)
                    .ThenInclude(c => c!.Lecciones)
                .Where(i => i.IdUsuario == idUsuario)
                .ToListAsync();
        }

        public async Task<Inscripcion?> GetByUsuarioCursoAsync(int idUsuario, int idCurso)
        {
            return await _context.Inscripciones
                .Include(i => i.Curso)
                .FirstOrDefaultAsync(i => i.IdUsuario == idUsuario && i.IdCurso == idCurso);
        }

        public async Task<Inscripcion> CreateAsync(Inscripcion inscripcion)
        {
            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();
            return inscripcion;
        }

        public async Task<bool> ExistsAsync(int idUsuario, int idCurso)
        {
            return await _context.Inscripciones
                .AnyAsync(i => i.IdUsuario == idUsuario && i.IdCurso == idCurso);
        }
    }
}
