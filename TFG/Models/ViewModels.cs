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
}
