using TFG.Models; // Reemplaza con tu namespace de modelos
using System.Collections.Generic;
using System.Threading.Tasks;
using TFG.Repositories;

namespace TFG.Services
{
    public interface IComentarioService
    {
        Task<IEnumerable<ComentarioViewModel>> ObtenerComentariosPorEntidad(TipoEntidad tipoEntidad, int entidadId);
        Task<bool> GuardarComentario(ComentarioViewModel nuevoComentario);
        // Otros métodos relacionados con comentarios...
    }

    public class ComentarioService : IComentarioService
    {
        private readonly IRepositorioComentario _comentarioRepository;

        public ComentarioService(IRepositorioComentario comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task<IEnumerable<ComentarioViewModel>> ObtenerComentariosPorEntidad(TipoEntidad tipoEntidad, int entidadId)
        {
            return await _comentarioRepository.ObtenerComentariosPorEntidad(tipoEntidad, entidadId);
        }

        public async Task<bool> GuardarComentario(ComentarioViewModel nuevoComentario)
        {
            // Aquí podrías agregar lógica de negocio adicional antes de guardar, si es necesario
            return await _comentarioRepository.GuardarComentario(nuevoComentario);
        }

        // Otros métodos del servicio...
    }
}