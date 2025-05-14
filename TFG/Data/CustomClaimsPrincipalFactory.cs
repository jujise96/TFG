using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TFG.Models;

namespace TFG.Data
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<Usuario>
    {
        public CustomClaimsPrincipalFactory(UserManager<Usuario> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Usuario user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            // Aquí puedes agregar claims personalizados si es necesario
            if (!string.IsNullOrEmpty(user.Nombre))
            {
                identity.AddClaim(new Claim("NombreReal", user.Nombre));
            }
            if (user.Id != 0)
            {
                identity.AddClaim(new Claim("IdUsuario", user.Id.ToString()));
            }

            var roles = await UserManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            return identity;
        }
    }
}
