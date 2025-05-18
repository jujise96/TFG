using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace TFG.Models
{
    public class IniciarSesionViewModel
    {
        [Required(ErrorMessage = "El correo electrónico o nombre de usuario es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico o nombre de usuario")]
        public string mailusername { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Column(TypeName = "NVarChar(Max)")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }
    }

    public class RegistrarUsuarioViewModel
    {
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

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? F_Nacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Código Plus de Google: https://maps.google.com/pluscodes/")]
        public string GooglePlusCode { get; set; }
    }


    public class ModificarUsuarioViewModel
    {
        // Este campo puede ser útil para identificar al usuario a modificar
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

        // En una modificación de usuario, puedes hacer la contraseña opcional si no se desea cambiar
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string? Contrasena { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [StringLength(50)]
        [Display(Name = "País")]
        public string Pais { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? F_Nacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Código Plus de Google: https://maps.google.com/pluscodes/")]
        public string GooglePlusCode { get; set; }
    }

    public class ElementoUsuarioViewModel()
    {
        public int idJuego { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool completada { get; set; }
    }


    public class PassForgotenViewModel()
    {
        [Required(ErrorMessage = "El Correo es obligatorio")]
        [EmailAddress(ErrorMessage = "¿Como puedo enviarte un mensaje si no me indicas a donde?")]
        public string Correo { get; set; }
    }

    public class RecuperarContraseñaViewModel()
    {
        [Required(ErrorMessage = "El Correo es obligatorio")]
        [EmailAddress(ErrorMessage = "¿Como puedo enviarte un mensaje si no me indicas a donde?")]
        public string Correo { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }
        public string Codigo { get; set; }
        public string IntentoCodigo { get; set; }
    }


    public class MisionViewModel()
    {
        public int idJuego { get; set; }
        public int Id { get; set; }
        public string IdElem { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string StartTrigger { get; set; }
        public string Bugs { get; set; }
        public TipoQuest TipoQuest { get; set; }
        public bool Completada { get; set; }
    }

    public class ItemViewModel()
    {
        public int Id { get; set; }
        public string IdElem { get; set; }
        public int JuegoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Bugs { get; set; }
        public TipoItem TipoItem { get; set; }
        public bool Completada { get; set; }
    }


    public class JuegoViewModel()
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Id del Elemento es requerido.")]
        public string IdElem { get; set; }

        [Required(ErrorMessage = "El Nombre del juego es requerido.")]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string Imagen { get; set; }
        public string Bugs { get; set; }
    }

    public class CajaDeComentariosViewModel
    {
        public string TipoEntidad { get; set; }
        public int EntidadId { get; set; }
        public List<ComentarioViewModel> Comentarios { get; set; }
        public int TotalComentarios { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }

    public class ComentarioViewModel
    {
        public int Id { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UserId { get; set; }
        public int? ComentarioPadreId { get; set; }
        public List<ComentarioViewModel> Respuestas { get; set; } // Para la visualización anidada
    }
}

