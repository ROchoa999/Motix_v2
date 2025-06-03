using QDocument = QuestPDF.Fluent.Document;
using DomainDoc = Motix_v2.Domain.Entities.Document;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Motix_v2.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;

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
            {
                // 1) Ruta física al logo dentro de la carpeta Assets
                //    (Asumimos que, al desplegar la app, existe una carpeta "Assets" 
                //     junto al ejecutable con el fichero PNG).
                string logoPath = Path.Combine(
                    System.AppContext.BaseDirectory,
                    "Assets",
                    "Square44x44Logo.altform-unplated_targetsize-256.png"
                );

                // 2) Creamos el documento con QuestPDF
                var pdf = QDocument.Create(container =>
                {
                    container.Page(page =>
                    {
                        // 2.1) Tamaño de página A5 y márgenes
                        page.Size(PageSizes.A5.Landscape());
                        page.Margin(20);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        // 2.2) ENCABEZADO: Logo + datos de la empresa
                        page.Header()
                            .PaddingHorizontal(20)
                            .PaddingBottom(10)
                            .Row(row =>
                            {
                                // 1) Columna 1: Logo (más margen a la izquierda si quieres)
                                row.ConstantItem(50)
                                   .Height(50)
                                   .AlignMiddle()
                                   .AlignLeft()
                                   .Image(logoPath);

                                // 2) Espacio flexible que ocupa todo lo que quede entre logo y datos de la empresa
                                row.RelativeItem();

                                // 3) Columna 2: Nombre y datos de la empresa, anclados a la derecha
                                //    (anchura fija de 200; ajusta si crees que necesitas más o menos)
                                row.ConstantItem(200)
                                   .AlignMiddle()
                                   .AlignRight()   // el bloque entero se coloca en el extremo derecho
                                   .Column(stack =>
                                   {
                                       stack.Spacing(2);

                                       stack.Item()
                                            .Text("Motix S.L.")
                                            .SemiBold()
                                            .FontSize(16)
                                            .FontColor(Colors.Black);

                                       stack.Item()
                                            .Text("C/ Industria, 123")
                                            .FontSize(10)
                                            .FontColor(Colors.Grey.Medium);

                                       stack.Item()
                                            .Text("CIF: B-12345678")
                                            .FontSize(10)
                                            .FontColor(Colors.Grey.Medium);

                                       stack.Item()
                                            .Text("Tel: (+34) 912 345 678")
                                            .FontSize(10)
                                            .FontColor(Colors.Grey.Medium);
                                   });
                            });

                        // 2.3) CONTENIDO PRINCIPAL
                        page.Content()
                            .Column(main =>
                            {
                                main.Spacing(5);

                                // (A) Tabla de líneas dentro de un borde (sin cambios)
                                main.Item().Padding(5)
                                    .Border(1).BorderColor(Colors.Grey.Lighten2)
                                    .Column(column =>
                                    {
                                        column.Spacing(2);
                                        column.Item().Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(8);
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(3);
                                                columns.RelativeColumn(3);
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell().Element(CellStyle).Text("Ref.").FontSize(10).Bold();
                                                header.Cell().Element(CellStyle).Text("Descripción").FontSize(10).Bold();
                                                header.Cell().Element(CellStyle).AlignRight().Text("P.U. (€)").FontSize(10).Bold();
                                                header.Cell().Element(CellStyle).AlignRight().Text("Cant.").FontSize(10).Bold();
                                                header.Cell().Element(CellStyle).AlignRight().Text("Importe (€)").FontSize(10).Bold();
                                            });

                                            foreach (var line in lines)
                                            {
                                                table.Cell().Element(CellStyle).Text(line.Pieza.ReferenciaInterna).FontSize(10);
                                                table.Cell().Element(CellStyle).Text(line.Pieza.Nombre).FontSize(10);
                                                table.Cell().Element(CellStyle).AlignRight().Text($"{line.PrecioUnitario:F2}").FontSize(10);
                                                table.Cell().Element(CellStyle).AlignRight().Text($"{line.Cantidad}").FontSize(10);
                                                table.Cell().Element(CellStyle).AlignRight().Text($"{(line.PrecioUnitario * line.Cantidad):F2}").FontSize(10);
                                            }
                                        });
                                    });                                
                            });


                        // 2.4) PIE DE PÁGINA
                        page.Footer()
                            .Column(footer =>
                            {
                                footer.Spacing(5);

                                // ── Fila superior: Cliente (65 %) + Totales (35 %) ──
                                footer.Item()
                                    .Row(row =>
                                    {
                                        // 65 %: datos del cliente (igual que antes)
                                        row.RelativeItem(65)
                                            .PaddingLeft(10)
                                            .Column(col =>
                                            {
                                                col.Spacing(2);
                                                col.Item().Text($"{customer.Nombre}")
                                                           .SemiBold().FontSize(12);
                                                col.Item().Text($"{customer.Direccion}")
                                                           .FontSize(10);
                                                col.Item().Text($"CIF/NIF: {customer.CifNif}")
                                                           .FontSize(10);
                                                col.Item().Text($"Tel: {customer.Telefono}")
                                                           .FontSize(10);
                                                col.Item().Text($"Email: {customer.Email}")
                                                           .FontSize(10);
                                            });

                                        // 35 %: totales divididos en dos columnas (título / valor), ambas right-aligned
                                        row.RelativeItem(35)
                                            .AlignRight()
                                            .Column(col =>
                                            {
                                                col.Spacing(2);

                                                // 1.a) Row interna con 2 columnas para títulos y valores
                                                col.Item()
                                                   .Row(inner =>
                                                   {
                                                       // 1.a.i) Columna de títulos
                                                       inner.RelativeItem()
                                                            .Column(titles =>
                                                            {
                                                                titles.Spacing(2);
                                                                titles.Item().AlignLeft()
                                                                              .Text("Total con IVA:")
                                                                              .FontSize(10);
                                                                titles.Item().AlignLeft()
                                                                              .Text("IVA (21%):")
                                                                              .FontSize(10);
                                                                titles.Item().AlignLeft()
                                                                              .Text("Base Imponible:")
                                                                              .FontSize(10)
                                                                              .Bold();
                                                            });

                                                       // 1.a.ii) Columna de valores correspondientes
                                                       inner.RelativeItem()
                                                            .Column(values =>
                                                            {
                                                                values.Spacing(2);
                                                                values.Item().AlignRight()
                                                                               .Text($"{document.Total:F2} €")
                                                                               .FontSize(10);
                                                                values.Item().AlignRight()
                                                                               .Text($"{document.Iva:F2} €")
                                                                               .FontSize(10);
                                                                values.Item().AlignRight()
                                                                               .Text($"{document.BaseImponible:F2} €")
                                                                               .FontSize(10)
                                                                               .Bold();
                                                            });
                                                   });

                                                // 1.b) Fila inferior: ID del albarán
                                                col.Item()
                                                   .AlignLeft()
                                                   .Text($"\nAlbarán: {document.Id}")
                                                   .FontSize(10)
                                                   .SemiBold();
                                            });
                                    });

                                // ── Pie de página ──
                                footer.Item()
                                    .AlignCenter()
                                    .Text("Generado por Motix")
                                    .FontSize(8)
                                    .FontColor(Colors.Grey.Medium);
                            });

                    });
                });

                // 3) Generar y guardar el PDF en la ruta indicada
                pdf.GeneratePdf(outputPath);
                return Task.FromResult(outputPath);
            }

        }
        private IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(5)
                .PaddingHorizontal(2);
        }
    }
}
