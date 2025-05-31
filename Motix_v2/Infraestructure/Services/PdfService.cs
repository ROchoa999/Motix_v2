using QDocument = QuestPDF.Fluent.Document;
using DomainDoc = Motix_v2.Domain.Entities.Document;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Motix_v2.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Motix_v2.Infraestructure.Services
{
    public class PdfService
    {
        public Task<string> GenerateInvoicePdfAsync(
            DomainDoc document,
            Customer customer,
            IEnumerable<DocumentLine> lines,
            string outputPath)
        {
            // Definición básica del documento PDF usando QuestPDF
            var pdf = QDocument.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Encabezado
                    page.Header()
                        .Text($"Albarán {document.Id}")
                        .FontSize(20)
                        .SemiBold();

                    // Contenido principal
                    page.Content()
                        .Column(col =>
                        {
                            // Sección de Cliente
                            col.Item().Text($"Cliente: {customer.Nombre}");
                            col.Item().Text($"Dirección: {customer.Direccion}");
                            col.Item().Text($"Teléfono: {customer.Telefono}");
                            col.Item().Text($"Email: {customer.Email}");
                            col.Item().PaddingVertical(5).LineHorizontal(1);

                            // Tabla de líneas del albarán
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();    // Referencia
                                    columns.RelativeColumn();    // Descripción
                                    columns.ConstantColumn(60);   // Cantidad
                                    columns.ConstantColumn(80);   // Precio Unitario
                                    columns.ConstantColumn(80);   // Total Línea
                                });

                                // Encabezado de la tabla
                                table.Header(header =>
                                {
                                    header.Cell().Text("Referencia").SemiBold();
                                    header.Cell().Text("Descripción").SemiBold();
                                    header.Cell().AlignRight().Text("Cant.");
                                    header.Cell().AlignRight().Text("P.Unit.");
                                    header.Cell().AlignRight().Text("Total");
                                });

                                // Filas de datos
                                foreach (var line in lines)
                                {
                                    table.Cell().Text(line.Pieza.ReferenciaInterna);
                                    table.Cell().Text(line.Pieza.Nombre);
                                    table.Cell().AlignRight().Text(line.Cantidad.ToString());
                                    table.Cell().AlignRight().Text($"{line.PrecioUnitario:N2} €");
                                    table.Cell().AlignRight().Text($"{line.TotalLinea:N2} €");
                                }

                                // Pie de tabla con totales
                                table.Footer(footer =>
                                {
                                    footer.Cell().ColumnSpan(3);
                                    footer.Cell().AlignRight().Text("Base: ");
                                    footer.Cell().AlignRight().Text($"{document.BaseImponible:N2} €");

                                    footer.Cell().ColumnSpan(3);
                                    footer.Cell().AlignRight().Text("IVA: ");
                                    footer.Cell().AlignRight().Text($"{document.Iva:N2} €");

                                    footer.Cell().ColumnSpan(3);
                                    footer.Cell().AlignRight().Text("Total: ");
                                    footer.Cell().AlignRight().Text($"{document.Total:N2} €");
                                });
                            });
                        });

                    // Pie de página
                    page.Footer()
                        .AlignCenter()
                        .Text("Generado por Motix");
                });
            });

            // Generar y guardar el PDF en la ruta indicada
            pdf.GeneratePdf(outputPath);
            return Task.FromResult(outputPath);
        }
    }
}
