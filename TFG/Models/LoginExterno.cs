using System.ComponentModel.DataAnnotations;

namespace TFG.Models
{
    public class LoginExterno
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int usuarioId { get; set; }
        [Required]
        [MaxLength(100)]
        public string loginprovider { get; set; }
        [Required]
        [MaxLength(100)]
        public string providerKey { get; set; }
        [MaxLength(100)]
        public string providerDisplayName { get; set; }
    }
}
