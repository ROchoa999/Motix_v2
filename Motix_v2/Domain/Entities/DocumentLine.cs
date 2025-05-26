using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motix_v2.Domain.Entities
{
    [Table("lineasdocumento")]
    public class DocumentLine
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("documentoid")]
        public string DocumentoId { get; set; } = null!;

        [Column("piezaid")]
        public int? PiezaId { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("preciounitario")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column("totallinea")]
        public decimal TotalLinea { get; set; }
    }
}
