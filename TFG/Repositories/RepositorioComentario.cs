using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioComentario
    {
        Task<ComentarioViewModel> ObtenerComentarioPorIdAsync(int id);
        Task<IEnumerable<ComentarioViewModel>> ObtenerComentariosPorEntidad(TipoEntidad tipoEntidad, int entidadId, int? userId = null);
        Task<bool> GuardarComentario(ComentarioViewModel comentario);
        //public Task<bool> ModificarComentario(Comentario comentario)
        public Task<bool> EliminarComentario(int id);
        public Task<bool> LikeComentario(int idusurio, int idcomentario, bool like);
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
                   c.ComentarioPadreId AS ComentarioPadreId,
                   c.JuegoId AS JuegoId,       -- Asegurarse de seleccionar si es necesaria
                   c.TipoEntidad AS TipoEntidad, -- Asegurarse de seleccionar si es necesaria
                   c.EntidadId AS EntidadId,   -- Asegurarse de seleccionar si es necesaria
                   -- Conteo de likes
                   ISNULL(SUM(CASE WHEN ucl.[Like] = 1 THEN 1 ELSE 0 END), 0) AS likes,
                   -- Conteo de dislikes
                   ISNULL(SUM(CASE WHEN ucl.[Like] = 0 THEN 1 ELSE 0 END), 0) AS dislikes
            FROM Comentario c
            LEFT JOIN Usuarios u ON c.UserId = u.Id
            LEFT JOIN UsuarioComentarioLike ucl ON c.Id = ucl.ComentarioId
            WHERE c.Id = @Id
            GROUP BY c.Id, c.Mensaje, c.FechaCreacion, c.UserId, u.NombreUsuario, c.ComentarioPadreId,
                     c.JuegoId, c.TipoEntidad, c.EntidadId -- Todas las columnas no agregadas deben estar aquí
            ORDER BY c.FechaCreacion ASC",
        new { Id = id });
        }

        // Modifica tu servicio/repositorio de Comentarios
        public async Task<IEnumerable<ComentarioViewModel>> ObtenerComentariosPorEntidad(TipoEntidad tipoEntidad, int entidadId, int? userId = null)
        {
            using var connection = new SqlConnection(connectionString);
            string sql = @"
        SELECT c.Id AS Id,
               c.Mensaje AS Mensaje,
               c.FechaCreacion AS FechaCreacion,
               c.UserId AS UserId,
               u.NombreUsuario AS NombreUsuario,
               c.ComentarioPadreId AS ComentarioPadreId,
               c.JuegoId AS JuegoId,
               c.TipoEntidad AS TipoEntidad,
               c.EntidadId AS EntidadId,
               ISNULL(SUM(CASE WHEN ucl_total.[Like] = 1 THEN 1 ELSE 0 END), 0) AS likes,
               ISNULL(SUM(CASE WHEN ucl_total.[Like] = 0 THEN 1 ELSE 0 END), 0) AS dislikes,
               -- Obtener la reacción del usuario actual
               ucl_user.[Like] AS UserReaction
        FROM Comentario c
        LEFT JOIN Usuarios u ON c.UserId = u.Id
        LEFT JOIN UsuarioComentarioLike ucl_total ON c.Id = ucl_total.ComentarioId
        LEFT JOIN UsuarioComentarioLike ucl_user ON c.Id = ucl_user.ComentarioId AND ucl_user.UsuarioId = @CurrentUserId
        WHERE c.TipoEntidad = @TipoEntidad AND c.EntidadId = @EntidadId
        GROUP BY c.Id, c.Mensaje, c.FechaCreacion, c.UserId, u.NombreUsuario, c.ComentarioPadreId,
                 c.JuegoId, c.TipoEntidad, c.EntidadId, ucl_user.[Like] -- ucl_user.[Like] también en GROUP BY
        ORDER BY c.FechaCreacion ASC";

            // Pasa el userId a la consulta si está disponible
            return await connection.QueryAsync<ComentarioViewModel>(sql, new
            {
                TipoEntidad = tipoEntidad,
                EntidadId = entidadId,
                CurrentUserId = userId // Pasa el ID del usuario logueado aquí
            });
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

        public async Task<bool> LikeComentario(int idUsuario, int idComentario, bool newReaction)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction(); // Usar transacción para seguridad

            try
            {
                // 1. Obtener la reacción actual del usuario para este comentario
                int? existingReaction = await connection.QueryFirstOrDefaultAsync<int?>(
                    "SELECT [Like] FROM UsuarioComentarioLike WHERE UsuarioId = @UsuarioId AND ComentarioId = @ComentarioId",
                    new { UsuarioId = idUsuario, ComentarioId = idComentario }, transaction);

                int rowsAffected;

                if (existingReaction.HasValue)
                {
                    // Si ya existe una reacción:
                    if (existingReaction.Value == (newReaction ? 1 : 0))
                    {
                        // Si la nueva reacción es la misma que la existente, ELIMINAR la reacción
                        rowsAffected = await connection.ExecuteAsync(
                            "DELETE FROM UsuarioComentarioLike WHERE UsuarioId = @UsuarioId AND ComentarioId = @ComentarioId",
                            new { UsuarioId = idUsuario, ComentarioId = idComentario }, transaction);
                    }
                    else
                    {
                        // Si la nueva reacción es diferente, ACTUALIZAR la reacción
                        rowsAffected = await connection.ExecuteAsync(
                            "UPDATE UsuarioComentarioLike SET [Like] = @Like WHERE UsuarioId = @UsuarioId AND ComentarioId = @ComentarioId",
                            new { UsuarioId = idUsuario, ComentarioId = idComentario, Like = newReaction ? 1 : 0 }, transaction);
                    }
                }
                else
                {
                    // Si no existe ninguna reacción, INSERTAR la nueva reacción
                    rowsAffected = await connection.ExecuteAsync(
                        "INSERT INTO UsuarioComentarioLike (UsuarioId, ComentarioId, [Like]) VALUES (@UsuarioId, @ComentarioId, @Like)",
                        new { UsuarioId = idUsuario, ComentarioId = idComentario, Like = newReaction ? 1 : 0 }, transaction);
                }

                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al dar like/dislike al comentario: {ex.Message}");
                transaction.Rollback(); // Revertir si hay un error
                                        // Log the exception
                                        // Console.WriteLine($"Error al gestionar la reacción del comentario: {ex.Message}");
                return false;
            }
        }
    }
}
