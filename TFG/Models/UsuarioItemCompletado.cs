using System.ComponentModel.DataAnnotations;

namespace TFG.Models
{
    public class UsuarioItemCompletado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public DateTime FechaCompletado { get; set; } = DateTime.Now;
    }
}
