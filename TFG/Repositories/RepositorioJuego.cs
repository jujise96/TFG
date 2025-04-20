using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioJuego
    {
        public Task<Juego> ObtenerJuegoPorId(int id);
        public Task<List<string>> ObtenerQuestsPorJuego(int id);
        public Task<List<string>> ObtenerItemsPorJuego(int id);
        public Task<string> ObtenerTrucoPorJuego(int id);
    }
    public class RepositorioJuego : IRepositorioJuego
    {
        private readonly string connectionString;
        public RepositorioJuego(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public Task<List<string>> ObtenerItemsPorJuego(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Juego> ObtenerJuegoPorId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> ObtenerQuestsPorJuego(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> ObtenerTrucoPorJuego(int id)
        {
            throw new NotImplementedException();
        }
    }
}
