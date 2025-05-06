using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioUsuarios
    {
        Task CrearUsuario(Usuario usuario);
        Task<Usuario> ObtenerUsuarioPorId(int id);
        Task<Usuario> ObtenerUsuarioPorNombreusuario(string nombreUsuario);
        Task<Usuario> ObtenerUsuarioPorCorreo(string correo);
        Task ActualizarRolUsuario(int id, int? rolid);
        Task ActualizarUsuario(Usuario user);
        Task EliminarUsuario(Usuario user);
    }
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            // Verificar si el nombre de usuario ya existe TODO
            usuario.securityStamp = Guid.NewGuid().ToString();
            var id = await connection.QueryFirstOrDefaultAsync<int>(@"INSERT INTO Usuarios ( securityStamp, NombreUsuario, Nombre, Apellido, Correo, Contrasena, Telefono, Pais, F_Nacimiento, GooglePlusCode ) 
                                                                    VALUES (@securityStamp, @NombreUsuario, @Nombre, @Apellido, @Correo, @Contrasena, @Telefono, @Pais, @F_Nacimiento, @GooglePlusCode ); select SCOPE_IDENTITY()", usuario);
            usuario.Id = id;
        }
        public async Task<Usuario> ObtenerUsuarioPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(@"SELECT * FROM Usuarios WHERE Id = @Id", new { Id = id });
        }

        public async Task<Usuario> ObtenerUsuarioPorNombreusuario(string nombreUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(@"SELECT * FROM Usuarios WHERE NombreUsuario = @NombreUsuario", new { NombreUsuario = nombreUsuario });
        }

        public async Task<Usuario> ObtenerUsuarioPorCorreo(string correo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(@"SELECT * FROM Usuarios WHERE Correo = @Correo", new { Correo = correo });
        }

        public async Task ActualizarRolUsuario(int id, int? rolid)
        {
            
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Usuarios SET RolId = @RolId WHERE Id = @Id", new { RolId = rolid,Id=id});
        }

        public async Task ActualizarUsuario(Usuario user)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"
        UPDATE Usuarios 
        SET NombreUsuario = @NombreUsuario,
            Nombre = @Nombre, 
            Apellido = @Apellido,
            Correo = @Correo, 
            Contrasena = @Contrasena,
            Telefono = @Telefono,
            Pais = @Pais,
            F_Nacimiento = @F_Nacimiento,
            GooglePlusCode = @GooglePlusCode
        WHERE Id = @Id", user);
        }

        public async Task EliminarUsuario(Usuario user)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("DELETE FROM Usuarios WHERE Id = @Id", new { user.Id });
        }
    }
}
