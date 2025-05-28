namespace TFG.Models
{
    public class UsuarioComentarioLike
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ComentarioId { get; set; }
        // Relación con Usuario
        public Usuario Usuario { get; set; }
        // Relación con Comentario
        public Comentario Comentario { get; set; }

        // Marca si el usuario ha dado like al comentario
        public bool Like { get; set; } //true para like, false para dislike
    }
}
