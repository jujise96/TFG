using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{

    public interface ITrucoService
    {
        // Aquí puedes agregar métodos específicos para la entidad Truco
        Task<Truco> ObtenerTrucoPorIdAsync(int id);
    }
    public class TrucoService : ITrucoService
    {
        private readonly IRepositorioTruco _repositorioTruco;
        public TrucoService(IRepositorioTruco repositorioTruco)
        {
            _repositorioTruco = repositorioTruco;
        }
        // Aquí puedes agregar métodos específicos para la entidad Truco
        public async Task<Truco> ObtenerTrucoPorIdAsync(int id)
        {
            return await _repositorioTruco.ObtenerTrucoPorIdAsync(id);
        }
    }
}
