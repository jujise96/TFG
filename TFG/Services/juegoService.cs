using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{
    public interface IJuegoService
    {
        Task<Juego> ObtenerJuegoPorIdAsync(int id);
        Task<List<ElementoUsuarioViewModel>> ListarJuegos();
        Task<List<ElementoUsuarioViewModel>> ObtenerQuestsPorJuegoAsync(int juegoId);
        Task<List<ElementoUsuarioViewModel>> ObtenerItemsPorJuegoAsync(int juegoId);
        Task<ElementoUsuarioViewModel> ObtenerTrucoPorJuegoAsync(int juegoId);
        Task<bool> EliminarJuego(int idElemento);
        Task<bool> crearjuego(Juego juego);
        Task<bool> ModificarJuego(Juego juego);
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
        public async Task<List<ElementoUsuarioViewModel>> ObtenerQuestsPorJuegoAsync(int juegoId)
        {
            return await _repositorioJuego.ObtenerQuestsPorJuego(juegoId);
        }
        public async Task<List<ElementoUsuarioViewModel>> ObtenerItemsPorJuegoAsync(int juegoId)
        {
            return await _repositorioJuego.ObtenerItemsPorJuego(juegoId);
        }
        public async Task<ElementoUsuarioViewModel> ObtenerTrucoPorJuegoAsync(int juegoId)
        {
            return await _repositorioJuego.ObtenerTrucoPorJuego(juegoId);
        }

        public async Task<List<ElementoUsuarioViewModel>> ListarJuegos()
        {
            return await _repositorioJuego.ListarJuegos();
        }

        public async Task<bool> EliminarJuego(int idElemento)
        {
            return await _repositorioJuego.EliminarJuego(idElemento);
        }

        public async Task<bool> crearjuego(Juego juego)
        {
            return await _repositorioJuego.crearjuego(juego);
        }

        public async Task<bool> ModificarJuego(Juego juego)
        {
            return await _repositorioJuego.ModificarJuego(juego);
        }
    }
}
