using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [StringLength(10)]
        [Display(Name = "Fecha de Nacimiento")]
        public string F_Nacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Código Plus de Google: https://maps.google.com/pluscodes/")]
        public string GooglePlusCode { get; set; }
    }





}
