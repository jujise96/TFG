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

        public async Task DescompletarMision(int idMision, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM UsuarioMisionCompletada
            WHERE UsuarioId =@UsuarioId AND MisionId=@MisionId ", new { UsuarioId = idUsuario, MisionId = idMision});
        }

        public async Task<bool> EliminarMision(int idElemento, int idjuego)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
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
                WHERE umc.UsuarioId = @UsuarioId AND m.JuegoId = @JuegoId  ", new { UsuarioId= idUsuario, JuegoId= idJuego });
            return misiones;
        }
        // Aquí puedes agregar métodos específicos para la entidad Misión
    }
}
