using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TFG.Models.Validaciones;

namespace TFG.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public int? JuegoId { get; set; }
        public string Imagen { get; set; }

        [Required]
        public TipoEntidad TipoEntidad { get; set; }
        [Required]
        public int EntidadId { get; set; }
        
        public int? ComentarioPadreId { get; set; }
        public Comentario ComentarioPadre { get; set; }
        public ICollection<Comentario> Respuestas { get; set; }
        [Required]
        [MaxLength(1000)] // Ejemplo de longitud máxima para el mensaje
        [AntiInjeciones]
        public string Mensaje { get; set; }
        

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public Usuario Usuario { get; set; }

        public ICollection<UsuarioComentarioLike> likes { get; set; } = new List<UsuarioComentarioLike>();
    }
}