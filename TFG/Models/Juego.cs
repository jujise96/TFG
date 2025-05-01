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

        public ICollection<Mision> Mision { get; set; } = new List<Mision>();
        public ICollection<Item> Item { get; set; } = new List<Item>();
        public Truco Truco { get; set; } = new Truco();
    }
}
