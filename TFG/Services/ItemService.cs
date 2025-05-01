using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{
    public interface IItemService
    {
        Task<Item> ObtenerItemPorIdAsync(int id);
    }
    public class ItemService : IItemService
    {
        private readonly IRepositorioItem _repositorioItem;
        public ItemService(IRepositorioItem repositorioItem)
        {
            _repositorioItem = repositorioItem;
        }
        public async Task<Item> ObtenerItemPorIdAsync(int id)
        {
            return await _repositorioItem.ObtenerItemPorIdAsync(id);
        }
    }
}
