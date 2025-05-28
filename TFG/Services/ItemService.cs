using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{
    public interface IItemService
    {
        Task<Item> ObtenerItemPorIdAsync(int id);
        Task CompletarItem(int itemId, int idUsuario);
        Task DescompletarItem(int itemId, int idUsuario);
        Task<IEnumerable<UsuarioItemCompletado>> ObtenerItemPorJuegoyUsuario(int idJuego, int idUsuario);
        Task<bool> EliminarItem(int idElemento, int Juegoid);
        Task<bool> CrearItem(Item item);
        Task<bool> ModificarItem(Item item);
        Task<decimal> ProgresoItem(int idJuego, int idUsuario);
    }
    public class ItemService : IItemService
    {
        private readonly IRepositorioItem _repositorioItem;
        public ItemService(IRepositorioItem repositorioItem)
        {
            _repositorioItem = repositorioItem;
        }

        public async Task<bool> ModificarItem(Item item)
        {
            return await _repositorioItem.ModificarItem(item);
        }

        public async Task CompletarItem(int itemId, int idUsuario)
        {
            await _repositorioItem.CompletarItem(itemId, idUsuario);
        }

        public async Task<bool> CrearItem(Item item)
        {

            return await _repositorioItem.CrearItem(item);
        }

        public async Task DescompletarItem(int itemId, int idUsuario)
        {
            await _repositorioItem.DescompletarItem(itemId, idUsuario);
        }

        public async Task<bool> EliminarItem(int idElemento, int Juegoid)
        {
            return await _repositorioItem.EliminarItem(idElemento, Juegoid);
        }

        public async Task<Item> ObtenerItemPorIdAsync(int id)
        {
            return await _repositorioItem.ObtenerItemPorIdAsync(id);
        }

        public async Task<IEnumerable<UsuarioItemCompletado>> ObtenerItemPorJuegoyUsuario(int idJuego, int idUsuario)
        {
            return await _repositorioItem.ObtenerItemPorJuegoyUsuario(idJuego, idUsuario);
        }

        public async Task<decimal> ProgresoItem(int idJuego, int idUsuario)
        {
            return await _repositorioItem.ProgresoItem(idJuego, idUsuario);
        }
    }
}
