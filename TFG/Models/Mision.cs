using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TFG.Models.Validaciones;

namespace TFG.Models
{
    public class Mision
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }
        [Required]
        public int JuegoId { get; set; }
        public Juego Juego { get; set; }
        [AntiInjeciones]
        public string Nombre { get; set; }
        [AntiInjeciones]
        public string Descripcion { get; set; }

        public string Imagen { get; set; }
        [AntiInjeciones]
        public string StartTrigger { get; set; }
        [AntiInjeciones]
        public string Bugs { get; set; }

        [Required]
        public TipoQuest TipoQuest { get; set; }

        public ICollection<UsuarioMisionCompletada> UsuariosQueLaCompletaron { get; set; } = new List<UsuarioMisionCompletada>();

    }
}
