using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TFG.Repositories
{
    public interface IRepositorioRol
    {
        public Task<IEnumerable<Roles>> ObtenerRoles();
        public Task<Roles> ObtenerRolPorId(int id);
        public Task<Roles> ObtenerRolPorNombre(string nombre);
        public Task<bool> ExisteRol(int id);
        public Task<bool> ExisteRol(string nombre);
        public Task InsertarRol(Roles rol);
    }
    public class RepositorioRol : IRepositorioRol
    {
        private readonly string connectionString;
        public RepositorioRol(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> ExisteRol(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM Roles WHERE Id = @Id", new { Id = id });
            return existe == 1;
        }

        public async Task<bool> ExisteRol(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM Roles WHERE Nombre = @Nombre", new { Nombre = nombre });
            return existe == 1;
        }

        public async Task InsertarRol(Roles rol)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"insert into Roles (Id, Nombre, NombreNormalizado) Values(@Id, @Nombre, @NombreNormalizado)", rol);
        }

        public async Task<IEnumerable<Roles>> ObtenerRoles()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<List<Roles>>(@"SELECT * FROM Roles");
        }

        public async Task<Roles> ObtenerRolPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Roles>(@"SELECT * FROM Roles WHERE Id = @Id", new { Id = id });
        }

        public async Task<Roles> ObtenerRolPorNombre(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Roles>(@"SELECT * FROM Roles WHERE Nombre = @Nombre", new { Nombre = nombre });
        }
    }
}
