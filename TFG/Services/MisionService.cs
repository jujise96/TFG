using TFG.Models;
using TFG.Repositories;

namespace TFG.Services
{

    public interface IMisionService
    {
        // Aquí puedes agregar métodos específicos para la entidad Misión
        // Task<Mision> ObtenerMisionPorIdAsync(int id);
        Task<Mision> ObtenerMisionesPorIdAsync(int id);
        Task CompletarMision(int idMision, int idUsuario);
        Task DescompletarMision(int idMision, int idUsuario);
        Task<IEnumerable<UsuarioMisionCompletada>> ObtenerQuestsPorJuegoyUsuario(int idJuego, int idUsuario);
        Task<bool> EliminarMision(int idElemento, int idjuego);
        Task<bool> crearmision(Mision mision);
        Task<bool> ModificarMision(Mision mision);
        Task<decimal> ProgresoMision(int idJuego, int idUsuario);
    }
    public class MisionService : IMisionService
    {

        private readonly IRepositorioMision _repositorioMision;
        public MisionService(IRepositorioMision repositorioMision)
        {
            _repositorioMision = repositorioMision;
        }

        public async Task CompletarMision(int idMision, int idUsuario)
        {
            await _repositorioMision.CompletarMision(idMision, idUsuario);
        }

        public async Task<bool> crearmision(Mision mision)
        {
            return await _repositorioMision.crearmision(mision);
        }

        public async Task DescompletarMision(int idMision, int idUsuario)
        {
            await _repositorioMision.DescompletarMision(idMision, idUsuario);
        }

        public async Task<bool> EliminarMision(int idElemento, int idjuego)
        {
            return await _repositorioMision.EliminarMision(idElemento, idjuego);
        }

        public async Task<bool> ModificarMision(Mision mision)
        {
            return await _repositorioMision.ModificarMision(mision);
        }

        public async Task<Mision> ObtenerMisionesPorIdAsync(int id)
        {
            return await _repositorioMision.ObtenerMisionPorId(id);
        }

        public async Task<IEnumerable<UsuarioMisionCompletada>> ObtenerQuestsPorJuegoyUsuario(int idJuego, int idUsuario)
        {
            return await _repositorioMision.ObtenerQuestsPorJuegoyUsuario(idJuego,idUsuario);
        }

        public async Task<decimal> ProgresoMision(int idJuego, int idUsuario)
        {
            return await _repositorioMision.ProgresoMision(idJuego, idUsuario);
        }
    }
}
