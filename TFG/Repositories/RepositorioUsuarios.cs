using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioUsuarios
    {
        Task CrearUsuario(Usuario usuario);
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
    }
}
