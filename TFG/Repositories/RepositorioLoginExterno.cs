using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;

namespace TFG.Repositories
{
    public interface IRepositorioLoginExterno
    {
        Task Eliminar(int UsuarioId, string loginprovider, string providerkey);
        Task Insertar(LoginExterno LE);
        Task<IEnumerable<LoginExterno>> ListadoLogins(int usuarioid);
        Task<LoginExterno> ObtenerLoginExterno(string loginProvider, string providerKey);
    }

    public class RepositorioLoginExterno : IRepositorioLoginExterno
    {
        private readonly string connectionString;
        public RepositorioLoginExterno(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Insertar(LoginExterno LE)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"INSERT INTO LoginsExternos (UsuarioId, loginprovider, providerKey, providerDisplayName) 
                                            VALUES (@UsuarioId, @loginprovider, @providerKey, @providerDisplayName)", LE);
        }

        public async Task<LoginExterno> ObtenerLoginExterno(string loginProvider, string providerKey)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<LoginExterno>(@"SELECT * FROM LoginsExternos 
                    WHERE loginProvider = @loginProvider and providerKey = @providerKey", new { loginProvider = loginProvider, providerKey = providerKey });
        }

        public async Task<IEnumerable<LoginExterno>> ListadoLogins(int usuarioid)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<LoginExterno>(@"SELECT * FROM LoginsExternos WHERE UsuarioId = @UsuarioId", new { UsuarioId = usuarioid});
        }

        public async Task Eliminar(int UsuarioId, string loginprovider, string providerkey)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM LoginsExternos 
                WHERE UsuarioId = @UsuarioId and loginprovider = @loginprovider and providerkey = @providerkey", 
                new { UsuarioId = UsuarioId, loginprovider = loginprovider, providerkey = providerkey });
        }

    }
}
