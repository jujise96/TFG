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
            var id = await connection.QueryFirstOrDefaultAsync<int>(@"INSERT INTO Usuarios ( NombreUsuario, Nombre, Apellido, Correo, Contrasena, Telefono, Pais, F_Nacimiento, GooglePlusCode ) 
                                                                    VALUES ( @NombreUsuario, @Nombre, @Apellido, @Correo, @Contrasena, @Telefono, @Pais, @F_Nacimiento, @GooglePlusCode ); select SCOPE_IDENTITY()", usuario);
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
    }
}
