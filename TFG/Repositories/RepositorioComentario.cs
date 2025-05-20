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
            await connection.OpenAsync(); // Es bueno asegurarse de que la conexión está abierta

            // Iniciar una transacción para asegurar que todas las eliminaciones son atómicas
            using var transaction = connection.BeginTransaction();
            try
            {
                // SQL Common Table Expression (CTE) para obtener todos los descendientes del comentario
                // incluyendo el comentario original a eliminar.
                var sql = @"
                    WITH ComentariosDescendientes AS (
                        SELECT Id
                        FROM Comentario
                        WHERE Id = @Id
                        UNION ALL
                        SELECT c.Id
                        FROM Comentario c
                        INNER JOIN ComentariosDescendientes cd ON c.ComentarioPadreId = cd.Id
                    )
                    DELETE FROM Comentario
                    WHERE Id IN (SELECT Id FROM ComentariosDescendientes);";

                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id }, transaction);

                // Si todo fue bien, confirma la transacción
                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                // Si algo falla, revierte la transacción
                transaction.Rollback();
                // Deberías usar un logger aquí para un manejo de errores más robusto
                Console.WriteLine($"Error de SQL al eliminar el comentario y sus descendientes: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Captura otras posibles excepciones y revierte
                transaction.Rollback();
                Console.WriteLine($"Error inesperado al eliminar el comentario y sus descendientes: {ex.Message}");
                return false;
            }
        }
    }
}