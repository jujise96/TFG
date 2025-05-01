using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioItem
    {
        Task<Item> ObtenerItemPorIdAsync(int id);
    }
    public class RepositorioItem : IRepositorioItem
    {
        private readonly string connectionString;
        public RepositorioItem(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        // Aquí puedes agregar métodos específicos para la entidad Item
        public async Task<Item> ObtenerItemPorIdAsync(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var item = await connection.QueryFirstOrDefaultAsync<Item>("SELECT * FROM Items WHERE Id = @id", new { id });
            return item;
        }
    }
}
