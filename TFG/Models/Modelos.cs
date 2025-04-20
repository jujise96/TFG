using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFG.Models
{
    using AspNetCoreGeneratedDocument;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TFG.Repositories;


    public class Usuario
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

        public int? RolId { get; set; }
        public Roles Rol { get; set; } = new Roles();

        public ICollection<UsuarioMisionCompletada>? MisionesCompletadas { get; set; } = new List<UsuarioMisionCompletada>();
        public ICollection<UsuarioItemCompletado>? ItemsCompletados { get; set; } = new List<UsuarioItemCompletado>();
    }


    public class Administrador : Usuario
    {
        [Required]
        [StringLength(200)]
        public string Direccion { get; set; }

        [Required]
        public int CP { get; set; }
    }


    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string NombreNormalizado { get; set; }
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }




    public enum TipoQuest
    {
        Principal,
        Secundaria,
        [Display(Name = "Sin marcar")]
        SinMarcar
    }
    public class Juego
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public List<string> Quest { get; set; } = new List<string>();
        public List<string> Item { get; set; } = new List<string>();
        public string Truco { get; set; }
    }

    public class Mision
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string Imagen { get; set; }

        public List<string> StartTrigger { get; set; } = new List<string>();
        public List<string> Bugs { get; set; } = new List<string>();

        [Required]
        public TipoQuest TipoQuest { get; set; }

        public ICollection<UsuarioMisionCompletada> UsuariosQueLaCompletaron { get; set; } = new List<UsuarioMisionCompletada>();

    }

    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string Imagen { get; set; }

        public List<string> Bugs { get; set; } = new List<string>();

        public ICollection<UsuarioItemCompletado> UsuariosQueLoCompletaron { get; set; } = new List<UsuarioItemCompletado>();

    }

    public class Truco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public List<string> Trucos { get; set; } = new List<string>();
    }


    public class UsuarioMisionCompletada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        public int MisionId { get; set; }
        public Mision Mision { get; set; }

        public DateTime FechaCompletado { get; set; } = DateTime.Now;
    }


    public class UsuarioItemCompletado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public DateTime FechaCompletado { get; set; } = DateTime.Now;
    }



}

