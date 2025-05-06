using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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
        public int Id { get; set; }
        public string Nombre { get; set; }
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
}
