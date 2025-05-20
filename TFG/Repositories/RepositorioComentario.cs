using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioComentario
    {
        Task<ComentarioViewModel> ObtenerComentarioPorIdAsync(int id);
        Task<IEnumerable<ComentarioViewModel>> ObtenerComentariosPorEntidad(TipoEntidad tipoEntidad, int entidadId);
        Task<bool> GuardarComentario(ComentarioViewModel comentario);
        //public Task<bool> ModificarComentario(Comentario comentario)
        public Task<bool> EliminarComentario(int id);
    }

    public class RepositorioComentario : IRepositorioComentario
    {
        private readonly string connectionString;

        public RepositorioComentario(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ComentarioViewModel> ObtenerComentarioPorIdAsync(int id)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<ComentarioViewModel>(@"
                    SELECT c.Id AS Id,
                           c.Mensaje AS Mensaje,
                           c.FechaCreacion AS FechaCreacion,
                           c.UserId AS UserId,
                           u.NombreUsuario AS NombreUsuario,
                           c.ComentarioPadreId AS ComentarioPadreId
                    FROM Comentario c
                    LEFT JOIN Usuarios u ON c.UserId = u.Id
                    WHERE c.Id = @Id
                    ORDER BY c.FechaCreacion ASC",
        new { Id = id});
        }

        public async Task<IEnumerable<ComentarioViewModel>> ObtenerComentariosPorEntidad(TipoEntidad tipoEntidad, int entidadId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ComentarioViewModel>(@"
                    SELECT c.Id AS Id,
                           c.Mensaje AS Mensaje,
                           c.FechaCreacion AS FechaCreacion,
                           c.UserId AS UserId,
                           u.NombreUsuario AS NombreUsuario,
                           c.ComentarioPadreId AS ComentarioPadreId
                    FROM Comentario c
                    LEFT JOIN Usuarios u ON c.UserId = u.Id
                    WHERE c.TipoEntidad = @TipoEntidad AND c.EntidadId = @EntidadId
                    ORDER BY c.FechaCreacion ASC",
        new { TipoEntidad = tipoEntidad, EntidadId = entidadId });
        }

        public async Task<bool> GuardarComentario(ComentarioViewModel comentario)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"
                    INSERT INTO Comentario (JuegoId, TipoEntidad, EntidadId, ComentarioPadreId, Mensaje, FechaCreacion, UserId)
                    VALUES (@JuegoId, @TipoEntidad, @EntidadId, @ComentarioPadreId, @Mensaje, @FechaCreacion, @UserId)",
                    comentario);
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error de SQL al guardar el comentario: {ex.Message}");
                return false;
            }
        }



        // public async Task<bool> ModificarComentario(Comentario comentario)
        // {
        //     using var connection = new SqlConnection(connectionString);
        //     var rowsAffected = await connection.ExecuteAsync(@"
        //         UPDATE Comentario
        //         SET Mensaje = @Mensaje,
        //             FechaModificacion = @FechaModificacion // Si tienes esta columna
        //         WHERE Id = @Id", comentario);
        //     return rowsAffected > 0;
        // }


        public async Task<bool> EliminarComentario(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var rowsAffected = await connection.ExecuteAsync("DELETE FROM Comentario WHERE Id = @Id", new { Id = id });
            return rowsAffected > 0;
        }
    }
}