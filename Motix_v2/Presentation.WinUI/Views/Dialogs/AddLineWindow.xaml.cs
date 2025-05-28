using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;
using Motix_v2.Presentation.WinUI.ViewModels;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.UI.Xaml.Printing;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Motix_v2.Presentation.WinUI.Views.Dialogs
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddLineWindow : Window
    {
        public AddLineWindowViewModel ViewModel { get; }
        public event Action<DocumentLine>? LineCreated;
        private readonly string _documentoId;
        private readonly Document _parentDocument;


        public AddLineWindow(string documentoId, Document parentDocument)
        {
            this.InitializeComponent();
            _documentoId = documentoId;
            _parentDocument = parentDocument;

            ViewModel = App.Host.Services.GetRequiredService<AddLineWindowViewModel>();

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new SizeInt32(1000, 700));

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
            }
        }

        public AddLineWindow(string documentoId, Document parentDocument, DocumentLine lineaToEdit) 
            : this(documentoId, parentDocument)   // llama al constructor existente
        {
            var uow = App.Host.Services.GetRequiredService<IUnitOfWork>();
            Part? part = lineaToEdit.PiezaId.HasValue
                ? uow.Parts.GetByIdAsync(lineaToEdit.PiezaId.Value).GetAwaiter().GetResult()
                : null;

            ViewModel.Parts.Clear();

            if (part != null)
            {
                ViewModel.Parts.Add(part);

                ViewModel.SelectedPart = part;
                ViewModel.Quantity = lineaToEdit.Cantidad;
                ViewModel.SearchInternalReference = part.ReferenciaInterna;
                ViewModel.SearchName = part.Nombre;

                TextBoxSearchReferenciainterna.IsEnabled = false;
                TextBoxSearchName.IsEnabled = false;
                ButtonSearchPart.IsEnabled = false;
            }
            else
            {
                ViewModel.SelectedPart = null;
                ViewModel.Quantity = 0;
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
            => this.Close();

        private async void OnConfirmClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedPart is null)
            {
                await ShowError("Pieza no seleccionada", "Debes seleccionar una pieza antes de continuar.");
                return;
            }
            if (!int.TryParse(TextBoxQuantity.Text, out int qty))
            {
                await ShowError("Cantidad inválida", "Introduce un número válido.");
                return;
            }

            if (qty <= 0)
            {
                await ShowError("Cantidad no permitida", "La cantidad debe ser mayor que cero.");
                return;
            }

            if (ViewModel.SelectedPart is not null && qty > ViewModel.SelectedPart.Stock)
            {
                await ShowError(
                    "Cantidad excede stock",
                    $"La cantidad ({qty}) supera el stock disponible ({ViewModel.SelectedPart.Stock})."
                );
                return;
            }

            ViewModel.Quantity = qty;

            var linea = new DocumentLine
            {
                DocumentoId = _documentoId,
                Document = _parentDocument,
                PiezaId = ViewModel.SelectedPart!.Id,
                Pieza = ViewModel.SelectedPart!,
                Cantidad = ViewModel.Quantity,
                PrecioUnitario = ViewModel.UnitPrice,
                TotalLinea = ViewModel.TotalPrice
            };

            LineCreated?.Invoke(linea);
            this.Close();
        }

        private async Task ShowError(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "Aceptar"
            };
            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}
