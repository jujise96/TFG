using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFG.Models
{
    using AspNetCoreGeneratedDocument;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TFG.Repositories;

    public interface IUsuario
    {
        Task CrearUsuario(Usuario usuario);
    }

    public class Usuario : IUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50)]
        [Display(Name = "Nombre de Usuario")]
        [RegularExpression(@"^[^@]+$", ErrorMessage = "El nombre de usuario no puede contener el carácter '@'")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Column(TypeName = "NVarChar(Max)")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [StringLength(50)]
        [Display(Name = "País")]
        public string Pais { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [StringLength(10)]
        [Display(Name = "Fecha de Nacimiento")]
        public string F_Nacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Código Plus de Google: https://maps.google.com/pluscodes/")]
        public string GooglePlusCode { get; set; }

        private readonly IRepositorioUsuarios repositorioUsuario;

        public Usuario()
        {
            // Constructor por defecto
        }
        public Usuario(IRepositorioUsuarios repositorioUsuario)
        {
            this.repositorioUsuario = repositorioUsuario;
        }

        public async Task CrearUsuario(Usuario usuario)
        {
            if (repositorioUsuario != null)
            {
                await repositorioUsuario.CrearUsuario(usuario);
            }
            else
            {
                throw new Exception("Repositorio de usuarios no inicializado");
            }
        }
    }


    public class Administrador : Usuario
    {
        [Required]
        [StringLength(200)]
        public string Direccion { get; set; }

        [Required]
        public int CP { get; set; }

        public Administrador()
        {
            // Constructor por defecto
        }
        public Administrador(IRepositorioUsuarios repositorioUsuario) : base(repositorioUsuario)
        {
        }
    }

}

