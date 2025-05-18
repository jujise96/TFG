using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFG.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Juego")]
        public int JuegoId { get; set; }
        public Juego Juego { get; set; }

        [Required]
        public TipoEntidad TipoEntidad { get; set; }
        [Required]
        public int EntidadId { get; set; }

        [ForeignKey("ComentarioPadre")]
        public int? ComentarioPadreId { get; set; }
        public Comentario ComentarioPadre { get; set; }

        [Required]
        [MaxLength(1000)] // Ejemplo de longitud máxima para el mensaje
        public string Mensaje { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
    }
}