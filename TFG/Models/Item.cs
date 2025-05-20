using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TFG.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }
        [Required]
        public int JuegoId { get; set; }
        public Juego Juego { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string Imagen { get; set; }

        public string Bugs { get; set; }
        public TipoItem TipoItem { get; set; }

        public ICollection<UsuarioItemCompletado> UsuariosQueLoCompletaron { get; set; } = new List<UsuarioItemCompletado>();

    }
}
