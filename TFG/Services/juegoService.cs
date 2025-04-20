using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{
    public interface IJuegoService
    {
        Task<Juego> ObtenerJuegoPorIdAsync(int id);
        Task<List<string>> ObtenerQuestsPorJuegoAsync(int juegoId);
        Task<List<string>> ObtenerItemsPorJuegoAsync(int juegoId);
        Task<string> ObtenerTrucoPorJuegoAsync(int juegoId);
    }
    
    public class JuegoService : IJuegoService
    {
        private readonly IRepositorioJuego _repositorioJuego;
        public JuegoService(IRepositorioJuego repositorioJuego)
        {
            _repositorioJuego = repositorioJuego;
        }
        public async Task<Juego> ObtenerJuegoPorIdAsync(int id)
        {
            return await _repositorioJuego.ObtenerJuegoPorId(id);
        }
        public async Task<List<string>> ObtenerQuestsPorJuegoAsync(int juegoId)
        {
            return await _repositorioJuego.ObtenerQuestsPorJuego(juegoId);
        }
        public async Task<List<string>> ObtenerItemsPorJuegoAsync(int juegoId)
        {
            return await _repositorioJuego.ObtenerItemsPorJuego(juegoId);
        }
        public async Task<string> ObtenerTrucoPorJuegoAsync(int juegoId)
        {
            return await _repositorioJuego.ObtenerTrucoPorJuego(juegoId);
        }
    }
}
