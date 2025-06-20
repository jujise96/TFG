﻿using Microsoft.AspNetCore.Identity;
using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{
    public class PersistenciaUsuario : IUserStore<Usuario>, IUserEmailStore<Usuario>, IUserPasswordStore<Usuario>, IUserRoleStore<Usuario>, IUserLoginStore<Usuario>
    {
        private readonly IRepositorioUsuarios _usuario;
        private readonly IRepositorioRol _rol;
        private readonly IRepositorioLoginExterno _loginExterno;

        public PersistenciaUsuario(IRepositorioUsuarios usuario, IRepositorioRol rol, IRepositorioLoginExterno loginExterno)
        {
            _usuario = usuario;
            _rol = rol;
            _loginExterno = loginExterno;
        }

        public async Task<IdentityResult> AddLoginAsync(Usuario user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(login);

            var loginExterno = new LoginExterno()
            {
                usuarioId = user.Id,
                loginprovider = login.LoginProvider,
                providerKey = login.ProviderKey,
                providerDisplayName = login.ProviderDisplayName
            };

            await _loginExterno.Insertar(loginExterno);
            return IdentityResult.Success;
        }

        public async Task AddToRoleAsync(Usuario user, string roleName, CancellationToken cancellationToken)
        {
            var rol = await _rol.ObtenerRolPorNombre(roleName);
            if (rol == null)
            {
                throw new InvalidOperationException("El rol no existe");
            }
            else
            {
                user.RolId = rol.Id;
                await _usuario.ActualizarRolUsuario(user.Id, user.RolId);
            }
        }

        public async Task<IdentityResult> CreateAsync(Usuario user, CancellationToken cancellationToken)
        {
            await _usuario.CrearUsuario(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Usuario user, CancellationToken cancellationToken)
        {
            await _usuario.EliminarUsuario(user);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<Usuario> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _usuario.ObtenerUsuarioPorCorreo(normalizedEmail);
        }

        public Task<Usuario> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return _usuario.ObtenerUsuarioPorId(int.Parse(userId));
        }

        public async Task<Usuario> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(loginProvider);
            ArgumentNullException.ThrowIfNull(providerKey);

            var loginExterno = await _loginExterno.ObtenerLoginExterno(loginProvider, providerKey);
            if (loginExterno == null)
            {
                return null;
            }
            return await _usuario.ObtenerUsuarioPorId(loginExterno.usuarioId);
        }

        public Task<Usuario> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return _usuario.ObtenerUsuarioPorNombreusuario(normalizedUserName);
        }

        public Task<string> GetEmailAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Correo);
        }

        public Task<bool> GetEmailConfirmedAsync(Usuario user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(Usuario user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);

            var logins = await _loginExterno.ListadoLogins(user.Id);
            return logins.Select(l => new UserLoginInfo(l.loginprovider, l.providerKey, l.providerDisplayName)).ToList();
        }

        public Task<string> GetNormalizedEmailAsync(Usuario user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(Usuario user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Contrasena);
        }

        public async Task<IList<string>> GetRolesAsync(Usuario user, CancellationToken cancellationToken)
        {
            if (user.RolId.HasValue)
            {
                var rol = await _rol.ObtenerRolPorId(user.RolId.Value);
                return new List<string> { rol?.Nombre };
            }
            return new List<string>();
        }

        public Task<string> GetUserIdAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NombreUsuario);
        }

        public Task<IList<Usuario>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(Usuario user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsInRoleAsync(Usuario user, string roleName, CancellationToken cancellationToken)
        {
            var rol = await _rol.ObtenerRolPorNombre(roleName);
            return (rol != null && user.RolId == rol.Id);
        }

        public Task RemoveFromRoleAsync(Usuario user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveLoginAsync(Usuario user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(loginProvider);
            ArgumentNullException.ThrowIfNull(providerKey);

            await _loginExterno.Eliminar(user.Id, loginProvider, providerKey);
        }

        public Task SetEmailAsync(Usuario user, string email, CancellationToken cancellationToken)
        {
            user.Correo = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(Usuario user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(Usuario user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(Usuario user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(Usuario user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Contrasena = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(Usuario user, string userName, CancellationToken cancellationToken)
        {
            user.NombreUsuario = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(Usuario user, CancellationToken cancellationToken)
        {
            await _usuario.ActualizarUsuario(user);
            return IdentityResult.Success;
        }

        Task IUserLoginStore<Usuario>.AddLoginAsync(Usuario user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            return AddLoginAsync(user, login, cancellationToken);
        }
    }
}
