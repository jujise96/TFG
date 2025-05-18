using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioTruco
    {
        Task<bool> Creartruco(Truco truco);
        Task<bool> EliminarTruco(int idElemento, int idjuego);
        Task<bool> ModificarTruco(Truco truco);

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

        public async Task<bool> Creartruco(Truco truco)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"
                INSERT INTO Truco (IdElem, JuegoId, Nombre, Descripcion, Trucos)
                VALUES (@IdElem, @JuegoId, @Nombre, @Descripcion, @Trucos)",
                    truco); // Pasamos el objeto 'truco' directamente para los parámetros
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error de SQL al crear la misión: {ex.Message}");
                // Manejo de excepciones si es necesario
                return false;
            }
            catch
            {
                return false;
            }
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

        public async Task<bool> ModificarTruco(Truco truco)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                int rowsAffected = await connection.ExecuteAsync(@"
                UPDATE Truco
                SET IdElem = @IdElem,
                    JuegoId = @JuegoId,
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Trucos = @Trucos
                WHERE JuegoId = @JuegoId",
                    truco); // Pasamos el objeto 'truco' directamente para los parámetros

                return rowsAffected > 0; // Devuelve true si al menos una fila fue modificada
            }
            catch (SqlException ex)
            {
                // Manejo específico de excepciones de SQL (opcional, pero recomendado)
                Console.WriteLine(ex.Message);
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Truco> ObtenerTrucoPorIdAsync(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var truco = await connection.QueryFirstOrDefaultAsync<Truco>("SELECT * FROM Truco WHERE JuegoId = @id", new { id });
            return truco;
        }
    }
}
