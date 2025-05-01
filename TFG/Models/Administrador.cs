using System.ComponentModel.DataAnnotations;

namespace TFG.Models
{
    public class Administrador : Usuario
    {
        [Required]
        [StringLength(200)]
        public string Direccion { get; set; }

        [Required]
        public int CP { get; set; }
    }
}
