using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motix.Domain.Entities
{
    // Mapea la clase a la tabla en minúscula
    [Table("usuarios")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Required]
        [Column("usuario")]
        public string Usuario { get; set; } = null!;

        [Required]
        [Column("contrasenahash")]
        public string ContrasenaHash { get; set; } = null!;

        [Column("rolid")]
        public int RolId { get; set; }
    }
}
