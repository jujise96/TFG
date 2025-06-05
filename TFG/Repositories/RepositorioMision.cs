using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioMision
    {
        public Task<Mision> ObtenerMisionPorId(int id);
        public Task CompletarMision(int idMision, int idUsuario);
        public Task DescompletarMision(int idMision, int idUsuario);
        public Task<IEnumerable<UsuarioMisionCompletada>> ObtenerQuestsPorJuegoyUsuario(int idJuego, int idUsuario);
        Task<bool> EliminarMision(int idElemento, int idjuego);
        Task<bool> crearmision(Mision mision);
        Task<bool> ModificarMision(Mision mision);
        Task<decimal> ProgresoMision(int idJuego, int idUsuario);
    }

    public class RepositorioMision : IRepositorioMision
    {
        private readonly string connectionString;
        public RepositorioMision(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CompletarMision(int idMision, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            var fecha = DateTime.Now;
            await connection.ExecuteAsync(@"INSERT INTO UsuarioMisionCompletada(UsuarioId, MisionId, FechaCompletado) 
            values(@UsuarioId, @MisionId, @FechaCompletado)", new { UsuarioId = idUsuario, MisionId = idMision, FechaCompletado = fecha });
        }

        public async Task<bool> crearmision(Mision mision)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"
                INSERT INTO Mision (IdElem, JuegoId, Nombre, Descripcion, Imagen, StartTrigger, Bugs, TipoQuest)
                VALUES (@IdElem, @JuegoId, @Nombre, @Descripcion, @Imagen, @StartTrigger, @Bugs, @TipoQuest)",
                    mision); // Pasamos el objeto 'mision' directamente para los parámetros
            }
            catch
            {
                // Manejo de excepciones si es necesario
                return false;
            }

            return true;
        }

        public async Task DescompletarMision(int idMision, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM UsuarioMisionCompletada
            WHERE UsuarioId =@UsuarioId AND MisionId=@MisionId ", new { UsuarioId = idUsuario, MisionId = idMision });
        }

        public async Task<bool> EliminarMision(int idElemento, int idjuego)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                // Eliminar comentarios relacionados
                await connection.ExecuteAsync("DELETE FROM Comentario WHERE TipoEntidad = @TipoEntidadMision AND EntidadId = @IdElemento",
                new { TipoEntidadMision = (int)TipoEntidad.Mision, IdElemento = idElemento });
                // Eliminar mision
                await connection.ExecuteAsync(@"DELETE FROM Mision 
                    WHERE Id =@Id AND Juegoid=@Juegoid ", new { Id = idElemento, Juegoid = idjuego });
            }
            catch
            {
                // Manejo de excepciones si es necesario
                return false;
            }

            return true;
        }

        public async Task<bool> ModificarMision(Mision mision)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                int rowsAffected = await connection.ExecuteAsync(@"
                UPDATE Mision
                SET IdElem = @IdElem,
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Imagen = @Imagen,
                    StartTrigger = @StartTrigger,
                    Bugs = @Bugs,
                    TipoQuest = @TipoQuest
                WHERE Id = @Id",
                    mision); // Pasamos el objeto 'mision' directamente para los parámetros
            }
            catch (SqlException ex)
            {
                // Manejo de excepciones si es necesario
                Console.WriteLine(ex.Message);
            }
            catch
            {
                // Manejo de excepciones si es necesario
                return false;
            }

            return true;
        }

        public async Task<Mision> ObtenerMisionPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var mision = await connection.QueryFirstOrDefaultAsync<Mision>("SELECT * FROM Mision WHERE Id = @id", new { id });
            return mision;
        }

        public async Task<IEnumerable<UsuarioMisionCompletada>> ObtenerQuestsPorJuegoyUsuario(int idJuego, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            var misiones = await connection.QueryAsync<UsuarioMisionCompletada>(@"SELECT * FROM  UsuarioMisionCompletada umc 
                INNER JOIN  Mision m ON umc.MisionId = m.Id 
                WHERE umc.UsuarioId = @UsuarioId AND m.JuegoId = @JuegoId  ", new { UsuarioId = idUsuario, JuegoId = idJuego });
            return misiones;
        }

        public async Task<decimal> ProgresoMision(int idJuego, int idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);

                // La consulta SQL que calcula el porcentaje redondeado a 3 decimales
                string sql = @"
            SELECT
                CAST(ROUND(CAST(COUNT(UCC.MisionId) AS DECIMAL(18, 10)) * 100 / NULLIF(COUNT(M.Id), 0), 3) AS DECIMAL(18, 3)) AS PorcentajeMisionesCompletadas
            FROM
                Mision AS M
            LEFT JOIN
                UsuarioMisionCompletada AS UCC ON M.Id = UCC.MisionId
                AND UCC.UsuarioId = @idUsuario  -- Parámetro para el ID del usuario
            WHERE
                M.JuegoId = @idJuego; -- Parámetro para el ID del juego";

                decimal progreso = await connection.QuerySingleOrDefaultAsync<decimal>(sql, new { idUsuario = idUsuario, idJuego = idJuego });

                return progreso;
            }
            catch
            {
                return -1;
            }
            
        }
        // Aquí puedes agregar métodos específicos para la entidad Misión
    }
}
