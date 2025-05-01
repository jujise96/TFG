using TFG.Migrations;
using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{

    public interface IMisionService
    {
        // Aquí puedes agregar métodos específicos para la entidad Misión
        // Task<Mision> ObtenerMisionPorIdAsync(int id);
        Task<Mision> ObtenerMisionesPorIdAsync(int id);
    }
    public class MisionService : IMisionService
    {

        private readonly IRepositorioMision _repositorioMision;
        public MisionService(IRepositorioMision repositorioMision)
        {
            _repositorioMision = repositorioMision;
        }

        public async Task<Mision> ObtenerMisionesPorIdAsync(int id)
        {
            return await _repositorioMision.ObtenerMisionPorId(id);
        }
    }
}
