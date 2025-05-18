using Microsoft.EntityFrameworkCore;
using TFG.Models;

namespace TFG.Data
{
    public class TFGContext : DbContext
    {
        public TFGContext(DbContextOptions<TFGContext> options)
            : base(options)
        {
        }
        // Aquí puedes agregar las entidades que deseas mapear a la base de datos
        // public DbSet<Entidad> Entidades { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Administrador> Administradores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aquí puedes configurar las entidades y sus relaciones
            // Ejemplo: modelBuilder.Entity<Entidad>().ToTable("NombreTabla");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Administrador>().ToTable("Administradores");
            modelBuilder.Entity<LoginExterno>().ToTable("LoginsExternos");
            modelBuilder.Entity<Roles>().ToTable("Roles");
            modelBuilder.Entity<Juego>().ToTable("Juego");
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Mision>().ToTable("Mision");
            modelBuilder.Entity<Truco>().ToTable("Truco");
            modelBuilder.Entity<Comentario>().ToTable("Comentario"); // O el nombre que quieras para la tabla



        }
    }
}
