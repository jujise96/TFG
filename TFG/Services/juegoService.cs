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
        Task<List<string>> ObtenerUrlsImagenesHijosDeJuego(int juegoId);
        Task<decimal> ProgresoJuego(int idJuego, int idUsuario);
    }
    
    public class JuegoService : IJuegoService
    {
        private readonly IRepositorioJuego _repositorioJuego;
        private readonly IMisionService _misionService;
        private readonly IItemService _itemService;
        public JuegoService(IRepositorioJuego repositorioJuego, IMisionService misionService, IItemService itemService)
        {
            _repositorioJuego = repositorioJuego;
            _misionService = misionService;
            _itemService = itemService;
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

        public async Task<List<string>> ObtenerUrlsImagenesHijosDeJuego(int juegoId)
        {
            return await _repositorioJuego.ObtenerUrlsImagenesHijosDeJuego(juegoId);
        }

        public async Task<decimal> ProgresoJuego(int idJuego, int idUsuario)
        {
            decimal progresoMisiones = await _misionService.ProgresoMision(idJuego, idUsuario);

            // Obtener el progreso de items
            decimal progresoItems = await _itemService.ProgresoItem(idJuego, idUsuario);

            // Calcular la media de ambos progresos
            // Nos aseguramos de que la suma sea decimal para una división precisa
            decimal progresoTotal = (progresoMisiones + progresoItems) / 2m;

            // Opcional: Redondear el progreso total si lo necesitas a una cantidad específica de decimales.
            // Por ejemplo, para redondear a 3 decimales:
            progresoTotal = Math.Round(progresoTotal, 3);

            return progresoTotal;
        }
    }
}
