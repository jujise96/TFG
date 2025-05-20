using TFG.Models; // Reemplaza con tu namespace de modelos
using System.Collections.Generic;
using System.Threading.Tasks;
using TFG.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace TFG.Services
{
    public interface IComentarioService
    {
        Task<IEnumerable<ComentarioViewModel>> ObtenerComentariosPorEntidad(TipoEntidad tipoEntidad, int entidadId);
        Task<bool> GuardarComentario(ComentarioViewModel nuevoComentario);
        Task<ComentarioViewModel> ObtenerComentariosPorId(int comentarioId);
        Task<bool> EliminarComentario(int idcomentario);
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

        public async Task<ComentarioViewModel> ObtenerComentariosPorId(int comentarioId)
        {
            return await _comentarioRepository.ObtenerComentarioPorIdAsync(comentarioId);
        }

        public async Task<bool> EliminarComentario(int idcomentario)
        {
            return await _comentarioRepository.EliminarComentario(idcomentario);
        }

    }
}