using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TFG.Models
{
    public class Juego
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Bugs { get; set; }

        public ICollection<Mision> Misiones { get; set; } = new List<Mision>(); // CAMBIO: Mision -> Misiones
        public ICollection<Item> Items { get; set; } = new List<Item>();       // CAMBIO: Item -> Items
        public Truco Truco { get; set; }
    }
}
