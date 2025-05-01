using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioMision
    {
        public Task<Mision> ObtenerMisionPorId(int id);

    }

    public class RepositorioMision : IRepositorioMision
    {
        private readonly string connectionString;
        public RepositorioMision(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Mision> ObtenerMisionPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var mision = await connection.QueryFirstOrDefaultAsync<Mision>("SELECT * FROM Mision WHERE Id = @id", new { id });
            return mision;
        }
        // Aquí puedes agregar métodos específicos para la entidad Misión
    }
}
