using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TFG.Repositories
{
    public interface IRepositorioJuego
    {
        public Task<List<ElementoUsuarioViewModel>> ListarJuegos();
        public Task<Juego> ObtenerJuegoPorId(int id);
        public Task<List<ElementoUsuarioViewModel>> ObtenerQuestsPorJuego(int id);
        public Task<List<ElementoUsuarioViewModel>> ObtenerItemsPorJuego(int id);
        public Task<ElementoUsuarioViewModel> ObtenerTrucoPorJuego(int id);
    }
    public class RepositorioJuego : IRepositorioJuego
    {
        private readonly string connectionString;
        public RepositorioJuego(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<ElementoUsuarioViewModel>> ListarJuegos()
        {
            using var connection = new SqlConnection(connectionString);
            var juegos = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT ID, Nombre FROM Juego");
            return juegos.ToList();
        }

        public async Task<List<ElementoUsuarioViewModel>> ObtenerItemsPorJuego(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var items = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre FROM Items WHERE JuegoId = @id", new { id });
            return items.ToList();
        }


        public async Task<Juego> ObtenerJuegoPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var juego = await connection.QueryFirstOrDefaultAsync<Juego>("SELECT * FROM Juego WHERE Id = @id", new { id });
            return juego;
        }

        public async Task<List<ElementoUsuarioViewModel>> ObtenerQuestsPorJuego(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var quest = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre FROM Mision WHERE JuegoId = @id", new { id });
            return quest.ToList();
        }

        public async Task<ElementoUsuarioViewModel> ObtenerTrucoPorJuego(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var truco = await connection.QueryFirstOrDefaultAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre FROM Truco WHERE JuegoId = @id", new { id });
            return truco;
        }
    }
}
