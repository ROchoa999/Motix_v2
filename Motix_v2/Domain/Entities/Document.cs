using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motix_v2.Domain.Entities
{
    [Table("documentos")]
    public class Document
    {
        [Key]
        [Column("id", TypeName = "varchar(36)")]
        public string Id { get; set; } = null!;

        [Column("clienteid")]
        public int ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Customer Cliente { get; set; } = null!;

        [Column("usuarioid")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("fecha", TypeName = "timestamptz")]
        public DateTimeOffset Fecha { get; set; }

        [Required]
        [Column("tipodocumento")]
        public string TipoDocumento { get; set; } = null!;

        [Required]
        [Column("estadopago")]
        public string EstadoPago { get; set; } = null!;

        [Column("formapagoid")]
        public int FormaPagoId { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Required]
        [Column("estadoreparto")]
        public string EstadoReparto { get; set; } = null!;

        [Required]
        [Column("base", TypeName = "numeric(18,2)")]
        public decimal BaseImponible { get; set; }

        [Required]
        [Column("iva", TypeName = "numeric(18,2)")]
        public decimal Iva { get; set; }

        [Required]
        [Column("total", TypeName = "numeric(18,2)")]
        public decimal Total { get; set; }

        public ICollection<DocumentLine> Lines { get; set; } = new List<DocumentLine>();

        [NotMapped]
        public string FechaFormateada
        {
            get
            {
                return Fecha
                    .ToLocalTime()
                    .ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
