using System.ComponentModel.DataAnnotations;

namespace TFG.Models
{
    public class UsuarioMisionCompletada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        public int MisionId { get; set; }
        public Mision Mision { get; set; }

        public DateTime FechaCompletado { get; set; } = DateTime.Now;
    }
}
