using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using TFG.Models.Validaciones;

namespace TFG.Models
{
    public class IniciarSesionViewModel
    {
        [Required(ErrorMessage = "El correo electrónico o nombre de usuario es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico o nombre de usuario")]
        [AntiInjeciones]
        public string mailusername { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Column(TypeName = "NVarChar(Max)")]
        [Display(Name = "Contraseña")]
        [AntiInjeciones]
        public string Contrasena { get; set; }
    }

    public class RegistrarUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50)]
        [Display(Name = "Nombre de Usuario")]
        [RegularExpression(@"^[^@]+$", ErrorMessage = "El nombre de usuario no puede contener el carácter '@'")]
        [AntiInjeciones]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        [AntiInjeciones]
        public string Nombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Apellido")]
        [AntiInjeciones]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico")]
        [AntiInjeciones]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Column(TypeName = "NVarChar(Max)")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        [AntiInjeciones]
        public string Telefono { get; set; }

        [StringLength(50)]
        [Display(Name = "País")]
        [AntiInjeciones]
        public string Pais { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? F_Nacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Código Plus de Google: https://maps.google.com/pluscodes/")]
        [AntiInjeciones]
        public string GooglePlusCode { get; set; }

        public int? PerfilPic { get; set; }
    }


    public class ModificarUsuarioViewModel
    {
        // Este campo puede ser útil para identificar al usuario a modificar
        [AntiInjeciones]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        [AntiInjeciones]
        public string Nombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Apellido")]
        [AntiInjeciones]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico")]
        [AntiInjeciones]
        public string Correo { get; set; }

        // En una modificación de usuario, puedes hacer la contraseña opcional si no se desea cambiar
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string? Contrasena { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        [AntiInjeciones]
        public string Telefono { get; set; }

        [StringLength(50)]
        [Display(Name = "País")]
        [AntiInjeciones]
        public string Pais { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? F_Nacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Código Plus de Google: https://maps.google.com/pluscodes/")]
        [AntiInjeciones]
        public string GooglePlusCode { get; set; }

        public int? PerfilPic { get; set; }
        public ICollection<LoginExterno> loginexternos { get; set; }
    }

    public class ElementoUsuarioViewModel()
    {
        public int idJuego { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool completada { get; set; }
        public int? PerfilPic { get; set; }
        public decimal Progreso { get; set; } = -1;
        public string tipo { get; set; } // Puede ser "Mision", "Item" o "Juego"
    }


    public class PassForgotenViewModel()
    {
        [Required(ErrorMessage = "El Correo es obligatorio")]
        [EmailAddress(ErrorMessage = "¿Como puedo enviarte un mensaje si no me indicas a donde?")]
        [AntiInjeciones]
        public string Correo { get; set; }
    }

    public class RecuperarContraseñaViewModel()
    {
        [Required(ErrorMessage = "El Correo es obligatorio")]
        [EmailAddress(ErrorMessage = "¿Como puedo enviarte un mensaje si no me indicas a donde?")]
        [AntiInjeciones]
        public string Correo { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }
        public string Codigo { get; set; }
        [AntiInjeciones]
        public string IntentoCodigo { get; set; }
    }


    public class MisionViewModel()
    {
        public int idJuego { get; set; }
        public int Id { get; set; }
        [AntiInjeciones]
        public string IdElem { get; set; }
        [AntiInjeciones]
        public string Nombre { get; set; }
        [AntiInjeciones]
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public IFormFile? ImagenFile { get; set; }
        [AntiInjeciones]
        public string StartTrigger { get; set; }
        [AntiInjeciones]
        public string Bugs { get; set; }
        public TipoQuest TipoQuest { get; set; }
        public bool Completada { get; set; }
        public decimal Progreso { get; set; } = -1;
    }

    public class ItemViewModel()
    {
        public int Id { get; set; }
        public string IdElem { get; set; }
        public int JuegoId { get; set; }
        [AntiInjeciones]
        public string Nombre { get; set; }
        [AntiInjeciones]
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public IFormFile? ImagenFile { get; set; }
        [AntiInjeciones]
        public string Bugs { get; set; }
        public TipoItem TipoItem { get; set; }
        public bool Completada { get; set; }
        public decimal Progreso { get; set; } = -1;
    }


    public class JuegoViewModel()
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Id del Elemento es requerido.")]
        [AntiInjeciones]
        public string IdElem { get; set; }

        [Required(ErrorMessage = "El Nombre del juego es requerido.")]
        [AntiInjeciones]
        public string Nombre { get; set; }
        [AntiInjeciones]
        public string Descripcion { get; set; }

        public string Imagen { get; set; }
        public IFormFile? ImagenFile { get; set; }
        public string Bugs { get; set; }
        public decimal Progreso { get; set; } = -1;
    }

    public class CajaDeComentariosViewModel
    {
        [AntiInjeciones]
        public string TipoEntidad { get; set; }
        public int EntidadId { get; set; }
        public string Imagen { get; set; }
        public IFormFile? ImagenFile { get; set; } // New property for image upload
        public List<ComentarioViewModel> Comentarios { get; set; }
        public int TotalComentarios { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }

    public class ComentarioViewModel
    {
        public int Id { get; set; }
        public int? JuegoId { get; set; }
        public TipoEntidad TipoEntidad { get; set; }
        public int? ComentarioPadreId { get; set; }
        public int EntidadId { get; set; }
        [AntiInjeciones]
        public string Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UserId { get; set; }
        public string Imagen { get; set; }
        public IFormFile? ImagenFile { get; set; } // New property for image upload
        [AntiInjeciones]
        public string NombreUsuario { get; set; } // Nueva propiedad
        public int likes { get; set; } = 0; // Nueva propiedad para contar likes
        public int dislikes { get; set; } = 0; // Nueva propiedad para contar dislikes
        public int? UserReaction { get; set; } // null = sin reacción, 1 = like, 0 = dislike
        public List<ComentarioViewModel> Respuestas { get; set; } // Para la visualización anidada

    }

    public class RankingsViewModel
    {
        [AntiInjeciones]
        public String nombreusuario { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int TotalComentarios { get; set; }
    }

}

