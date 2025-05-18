using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{

    public interface ITrucoService
    {
        Task<bool> EliminarTruco(int idElemento, int idjuego);

        // Aquí puedes agregar métodos específicos para la entidad Truco
        Task<Truco> ObtenerTrucoPorIdAsync(int id);
        Task<bool> ModificarTruco(Truco truco);
        Task<bool> Creartruco(Truco truco);
    }
    public class TrucoService : ITrucoService
    {
        private readonly IRepositorioTruco _repositorioTruco;
        public TrucoService(IRepositorioTruco repositorioTruco)
        {
            _repositorioTruco = repositorioTruco;
        }

        public async Task<bool> Creartruco(Truco truco)
        {
            return await _repositorioTruco.Creartruco(truco);
        }

        public async Task<bool> EliminarTruco(int idElemento, int idjuego)
        {
            return await _repositorioTruco.EliminarTruco(idElemento, idjuego);
        }

        public async Task<bool> ModificarTruco(Truco truco)
        {
            return await _repositorioTruco.ModificarTruco(truco);
        }

        // Aquí puedes agregar métodos específicos para la entidad Truco
        public async Task<Truco> ObtenerTrucoPorIdAsync(int id)
        {
            return await _repositorioTruco.ObtenerTrucoPorIdAsync(id);
        }
    }
}
