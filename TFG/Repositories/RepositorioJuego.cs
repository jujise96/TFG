﻿using Dapper;
using Microsoft.Data.SqlClient;
using TFG.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TFG.Repositories
{
    public interface IRepositorioJuego
    {
        public Task<List<ElementoUsuarioViewModel>> ListarJuegos();
        public Task<Juego> ObtenerJuegoPorId(int id);
        public Task<List<ElementoUsuarioViewModel>> ObtenerQuestsPorJuego(int id);
        public Task<List<ElementoUsuarioViewModel>> ObtenerItemsPorJuego(int id);
        public Task<ElementoUsuarioViewModel> ObtenerTrucoPorJuego(int id);
        Task<bool> EliminarJuego(int idElemento);
        Task<bool> crearjuego(Juego juego);
        Task<bool> ModificarJuego(Juego juego);
        Task<List<string>> ObtenerUrlsImagenesHijosDeJuego(int juegoId);
        Task<string> IdaNombre(int idJuego);
    }
    public class RepositorioJuego : IRepositorioJuego
    {
        private readonly string connectionString;
        public RepositorioJuego(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> crearjuego(Juego juego)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"
                        INSERT INTO Juego (IdElem, Nombre, Descripcion, Imagen, Bugs)
                        VALUES (@IdElem, @Nombre, @Descripcion, @Imagen, @Bugs);", new { juego.IdElem, juego.Nombre, juego.Descripcion, juego.Imagen, juego.Bugs });
            }
            catch
            {
                // Manejo de excepciones si es necesario
                return false;
            }
            return true;
        }

        public async Task<bool> EliminarJuego(int idElemento)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                // Eliminar misiones relacionadas
                await connection.ExecuteAsync("DELETE FROM Mision WHERE JuegoId = @Id", new { Id = idElemento });

                // Eliminar trucos relacionados
                await connection.ExecuteAsync("DELETE FROM Truco WHERE JuegoId = @Id", new { Id = idElemento });

                // Eliminar items relacionados
                await connection.ExecuteAsync("DELETE FROM Items WHERE JuegoId = @Id", new { Id = idElemento });

                // Eliminar comentarios relacionados
                await connection.ExecuteAsync("DELETE FROM Comentario WHERE JuegoId = @Id", new { Id = idElemento });


                // Finalmente, eliminar el juego
                await connection.ExecuteAsync("DELETE FROM Juego WHERE Id = @Id", new { Id = idElemento });
            }
            catch
            {
                // Manejo de excepciones si es necesario
                return false;
            }


            return true;
        }

        public async Task<string> IdaNombre(int idJuego)
        {
            using var connection = new SqlConnection(connectionString);
            var juego = await connection.QueryFirstOrDefaultAsync<string>("SELECT Nombre FROM Juego WHERE Id = @Id", new { Id = idJuego });
            return juego;
        }

        public async Task<List<ElementoUsuarioViewModel>> ListarJuegos()
        {
            using var connection = new SqlConnection(connectionString);
            var juegos = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT ID, Nombre FROM Juego");
            return juegos.ToList();
        }

        public async Task<bool> ModificarJuego(Juego juego)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                var affectedRows = await connection.ExecuteAsync(@" UPDATE Juego SET 
                IdElem = @IdElem, Nombre = @Nombre, Descripcion = @Descripcion, Imagen = @Imagen, Bugs = @Bugs
            WHERE Id = @Id;", new { juego.IdElem, juego.Nombre, juego.Descripcion, juego.Imagen, juego.Bugs, juego.Id });

                return affectedRows > 0; // Devuelve true si al menos una fila fue modificada
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ElementoUsuarioViewModel>> ObtenerItemsPorJuego(int id)
        {
            using var connection = new SqlConnection(connectionString);
            //var items = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre FROM Items WHERE JuegoId = @id", new { id });
            var items = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre, TipoItem AS subtipo FROM Items WHERE JuegoId = @id", new { id });
            return items.ToList();
        }


        public async Task<Juego> ObtenerJuegoPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var juego = await connection.QueryFirstOrDefaultAsync<Juego>("SELECT * FROM Juego WHERE Id = @id", new { id });
            return juego;
        }

        public async Task<List<ElementoUsuarioViewModel>> ObtenerQuestsPorJuego(int id)
        {
            using var connection = new SqlConnection(connectionString);
            //var quest = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre FROM Mision WHERE JuegoId = @id", new { id });
            var quest = await connection.QueryAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre, TipoQuest AS subtipo FROM Mision WHERE JuegoId = @id", new { id });
            return quest.ToList();
        }



        public async Task<ElementoUsuarioViewModel> ObtenerTrucoPorJuego(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var truco = await connection.QueryFirstOrDefaultAsync<ElementoUsuarioViewModel>("SELECT Id, Nombre FROM Truco WHERE JuegoId = @id", new { id });
            return truco;
        }

        public async Task<List<string>> ObtenerUrlsImagenesHijosDeJuego(int juegoId)
        {
            string sqlQuery = @"
                SELECT Imagen FROM Mision WHERE JuegoId = @JuegoId AND Imagen IS NOT NULL AND Imagen != ''
                UNION ALL
                SELECT Imagen FROM Items WHERE JuegoId = @JuegoId AND Imagen IS NOT NULL AND Imagen != ''";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Dapper ejecutará la consulta combinada y mapeará todos los resultados
                // a una lista de strings (las URLs de las imágenes).
                var imageUrls = await connection.QueryAsync<string>(
                    sqlQuery,
                    new { JuegoId = juegoId } // Pasamos el parámetro para la consulta
                );

                // Convertimos el IEnumerable<string> a List<string> y lo devolvemos
                return imageUrls.ToList();
            }
        }
    }
}
