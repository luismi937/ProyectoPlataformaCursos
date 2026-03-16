using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;
using System.Security.Claims;

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

        public override Task<Usuario?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == normalizedUserName.ToUpper(), cancellationToken);
        }

        public override Task<string?> GetUserNameAsync(Usuario user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(string.IsNullOrWhiteSpace(user.UserName) ? user.Email : user.UserName);
        }

        public override Task SetUserNameAsync(Usuario user, string? userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.UserName = string.IsNullOrWhiteSpace(userName) ? user.Email : userName;
            return Task.CompletedTask;
        }

        public override Task<IList<Claim>> GetClaimsAsync(Usuario user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public override Task AddClaimsAsync(Usuario user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public override Task ReplaceClaimAsync(Usuario user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public override Task RemoveClaimsAsync(Usuario user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public override Task<IList<Usuario>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<IList<Usuario>>(new List<Usuario>());
        }
    }
}
