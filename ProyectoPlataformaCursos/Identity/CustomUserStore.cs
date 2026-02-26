using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;

namespace ProyectoPlataformaCursos.Identity
{
    public class CustomUserStore : UserStore<Usuario, IdentityRole<int>, ApplicationDbContext, int>
    {
        public CustomUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }

        public override Task<Usuario> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Buscar por Email normal en lugar de NormalizedEmail
            return Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == normalizedEmail.ToUpper(), cancellationToken);
        }
    }
}
