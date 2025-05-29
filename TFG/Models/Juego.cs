using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TFG.Models.Validaciones;

namespace TFG.Models
{
    public class Juego
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string IdElem { get; set; }
        [AntiInjeciones]
        public string Nombre { get; set; }
        [AntiInjeciones]
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        [AntiInjeciones]
        public string Bugs { get; set; }

        public ICollection<Mision> Misiones { get; set; } = new List<Mision>(); // CAMBIO: Mision -> Misiones
        public ICollection<Item> Items { get; set; } = new List<Item>();       // CAMBIO: Item -> Items
        public Truco Truco { get; set; }
    }
}
