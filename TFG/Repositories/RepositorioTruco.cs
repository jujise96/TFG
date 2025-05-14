using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioTruco
    {
        Task<bool> EliminarTruco(int idElemento, int idjuego);

        // Aquí puedes agregar métodos específicos para la entidad Truco
        Task<Truco> ObtenerTrucoPorIdAsync(int id);
    }
    public class RepositorioTruco : IRepositorioTruco
    {
        private readonly string connectionString;
        public RepositorioTruco(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

            // Aquí puedes agregar métodos específicos para la entidad Truco
            // public async Task<Truco> ObtenerTrucoPorIdAsync(int id)
            // {
            //     using var connection = new SqlConnection(connectionString);
            //     var truco = await connection.QueryFirstOrDefaultAsync<Truco>("SELECT * FROM Truco WHERE Id = @id", new { id });
            //     return truco;
            // }
        }

        public async Task<bool> EliminarTruco(int idElemento, int idjuego)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"DELETE FROM Truco 
                    WHERE Id =@Id AND Juegoid=@Juegoid ", new { Id = idElemento, Juegoid = idjuego });
            }
            catch
            {
                // Manejo de excepciones si es necesario
                return false;
            }

            return true;
        }

        public async Task<Truco> ObtenerTrucoPorIdAsync(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var truco = await connection.QueryFirstOrDefaultAsync<Truco>("SELECT * FROM Truco WHERE Id = @id", new { id });
            return truco;
        }
    }
}
