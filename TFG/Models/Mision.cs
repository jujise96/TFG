using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string Imagen { get; set; }

        public string StartTrigger { get; set; }
        public string Bugs { get; set; }

        [Required]
        public TipoQuest TipoQuest { get; set; }

        public ICollection<UsuarioMisionCompletada> UsuariosQueLaCompletaron { get; set; } = new List<UsuarioMisionCompletada>();

    }
}
