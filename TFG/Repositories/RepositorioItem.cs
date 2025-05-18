using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioItem
    {
        Task<Item> ObtenerItemPorIdAsync(int id);
        Task CompletarItem(int itemId, int idUsuario);
        Task DescompletarItem(int itemId, int idUsuario);
        Task<IEnumerable<UsuarioItemCompletado>> ObtenerItemPorJuegoyUsuario(int idJuego, int idUsuario);
        Task<bool> EliminarItem(int idElemento, int Juegoid);
        Task<bool> CrearItem(Item item);
        Task<bool> ModificarItem(Item item);
    }
    public class RepositorioItem : IRepositorioItem
    {
        private readonly string connectionString;
        public RepositorioItem(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CompletarItem(int itemId, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            var fecha = DateTime.Now;
            await connection.ExecuteAsync(@"INSERT INTO UsuarioItemCompletado(UsuarioId, ItemId, FechaCompletado) 
            values(@UsuarioId, @ItemId, @FechaCompletado)", new { UsuarioId = idUsuario, ItemId = itemId, FechaCompletado = fecha });
        }

        public async Task<bool> CrearItem(Item item)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"
                INSERT INTO Items (IdElem, JuegoId, Nombre, Descripcion, Imagen, Bugs, TipoItem)
                VALUES (@IdElem, @JuegoId, @Nombre, @Descripcion, @Imagen, @Bugs, @TipoItem)",
                item); // Pasamos el objeto 'item' directamente para los parámetros

            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error de SQL al crear la misión: {ex.Message}");
                // Manejo de excepciones si es necesario
                return false;
            }

            return true;
        }

        public async Task DescompletarItem(int itemId, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM UsuarioItemCompletado
            WHERE UsuarioId =@UsuarioId AND ItemId=@ItemId ", new { UsuarioId = idUsuario, ItemId = itemId });
        }

        public async Task<bool> EliminarItem(int idElemento, int Juegoid)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"DELETE FROM Items 
                    WHERE Id =@Id AND Juegoid=@Juegoid ", new { Id = idElemento, Juegoid = Juegoid });
            }
            catch
            {
                // Manejo de excepciones si es necesario
                return false;
            }
            
            return true;
        }

        public async Task<bool> ModificarItem(Item item)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                int rowsAffected = await connection.ExecuteAsync(@"
                UPDATE Items
                SET IdElem = @IdElem,
                    JuegoId = @JuegoId,
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Imagen = @Imagen,
                    Bugs = @Bugs,
                    TipoItem = @TipoItem
                WHERE Id = @Id",
                    item); // Pasamos el objeto 'item' directamente para los parámetros

                return rowsAffected > 0; // Devuelve true si al menos una fila fue modificada
            }
            catch
            {
                // Manejo de otras excepciones si es necesario
                return false;
            }
        }

        // Aquí puedes agregar métodos específicos para la entidad Item
        public async Task<Item> ObtenerItemPorIdAsync(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var item = await connection.QueryFirstOrDefaultAsync<Item>("SELECT * FROM Items WHERE Id = @id", new { id });
            return item;
        }

        public async Task<IEnumerable<UsuarioItemCompletado>> ObtenerItemPorJuegoyUsuario(int idJuego, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            var items = await connection.QueryAsync<UsuarioItemCompletado>(@"SELECT * FROM  UsuarioItemCompletado uic 
                INNER JOIN  Items i ON uic.ItemId = i.Id 
                WHERE uic.UsuarioId = @UsuarioId AND i.JuegoId = @JuegoId  ", new { UsuarioId = idUsuario, JuegoId = idJuego });
            return items;
        }
    }
}
