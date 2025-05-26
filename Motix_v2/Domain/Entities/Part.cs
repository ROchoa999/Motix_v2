using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motix_v2.Domain.Entities
{
    [Table("piezas")]
    public class Part
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("referencia")]
        public string Referencia { get; set; } = null!;

        [Required]
        [Column("referenciainterna")]
        public string ReferenciaInterna { get; set; } = null!;

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("precioventa")]
        public decimal? PrecioVenta { get; set; }

        [Column("unidadmedida")]
        public string? UnidadMedida { get; set; }

        [Column("stock")]
        public int Stock { get; set; }
    }
}
