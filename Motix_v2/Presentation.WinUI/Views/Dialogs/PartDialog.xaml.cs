using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using System.Globalization;

namespace Motix_v2.Presentation.WinUI.Views.Dialogs
{
    public sealed partial class PartDialog : ContentDialog
    {
        private readonly IUnitOfWork _unitOfWork;
        public DocumentLine ResultLine { get; private set; }

        public PartDialog()
        {
            this.InitializeComponent();
            _unitOfWork = App.Host.Services.GetRequiredService<IUnitOfWork>();
            DataGridParts.ItemsSource = new List<Part>();
        }

        public PartDialog(DocumentLine existing) : this()
        {
            // obtener la pieza desde el repositorio (sincrónico para precarga)
            var part = _unitOfWork.Parts
                       .GetByIdAsync(existing.PiezaId)
                       .GetAwaiter()
                       .GetResult();
            if (part is not null)
            {
                // mostrar solo esa pieza y seleccionarla
                DataGridParts.ItemsSource = new List<Part> { part };
                DataGridParts.SelectedItem = part;

                // precargar precio unitario
                TextBlockUnitPrice.Text = part.PrecioVenta
                    ?.ToString("0.00", CultureInfo.CurrentCulture)
                    ?? "0.00";
            }

            // precargar cantidad y total
            TextBoxQuantity.Text = existing.Cantidad.ToString();
            TextBlockTotalPrice.Text = existing.TotalLinea
                .ToString("0.00", CultureInfo.CurrentCulture);

            // preparar el ResultLine con valores actuales
            ResultLine = new DocumentLine
            {
                PiezaId = existing.PiezaId,
                Cantidad = existing.Cantidad,
                PrecioUnitario = existing.PrecioUnitario,
                TotalLinea = existing.TotalLinea,
                DocumentoId = existing.DocumentoId
            };
        }

        private async Task SearchPartsAsync()
        {
            var refInterna = TextBoxSearchReferenciainterna.Text.Trim();
            var name = TextBoxSearchName.Text.Trim();

            var parts = await _unitOfWork.Parts.FindAsync(p =>
                (string.IsNullOrEmpty(refInterna) || p.ReferenciaInterna.Contains(refInterna)) &&
                (string.IsNullOrEmpty(name) || (p.Nombre ?? string.Empty).Contains(name))
            );

            DataGridParts.ItemsSource = parts;
        }

        private async void ButtonSearchPart_Click(object sender, RoutedEventArgs e)
        {
            await SearchPartsAsync();
        }

        private async void SearchPart_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await SearchPartsAsync();
                e.Handled = true;
            }
        }

        private void DataGridParts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridParts.SelectedItem is Part part)
            {
                // Mostrar precio unitario
                TextBlockUnitPrice.Text = part.PrecioVenta?.ToString("0.00") ?? "0.00";
            }
        }

        private void TextBoxQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Si hay una pieza seleccionada y la cantidad es válida...
            if (DataGridParts.SelectedItem is Part part
                && double.TryParse(TextBoxQuantity.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var qty))
            {
                // Calcula el total = cantidad × precio unitario
                var unitPrice = part.PrecioVenta ?? 0m;
                var total = qty * (double)unitPrice;
                // Muestra el total con dos decimales
                TextBlockTotalPrice.Text = total.ToString("0.00");
            }
            else
            {
                // Si no hay pieza o cantidad inválida, pon total a cero
                TextBlockTotalPrice.Text = "0.00";
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DataGridParts.SelectedItem is Part part
                && int.TryParse(TextBoxQuantity.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var qty))
            {
                ResultLine = new DocumentLine
                {
                    PiezaId = part.Id,
                    Cantidad = qty,
                    PrecioUnitario = part.PrecioVenta ?? 0m,
                    TotalLinea = (part.PrecioVenta ?? 0m) * qty,
                    DocumentoId = string.Empty  // se asignará más adelante
                };
            }
            else
            {
                // Cancela el cierre si no hay selección o cantidad inválida
                args.Cancel = true;
            }
        }

    }
}
