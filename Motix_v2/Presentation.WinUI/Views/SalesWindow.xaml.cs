using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using Motix_v2.Presentation.WinUI.Views.Dialogs;
using Motix_v2.Domain.Entities;
using Microsoft.UI.Xaml.Controls;
using Motix_v2.Infraestructure.Services;

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class SalesWindow : Window
    {
        public SalesViewModel ViewModel { get; }
        private Document _currentDocument;

        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")] private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);


        private const int SW_MAXIMIZE = 3;
        public SalesWindow()
        {
            this.InitializeComponent();
            ViewModel = App.Host.Services.GetRequiredService<SalesViewModel>();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            ClearAlbaranForm();
        }

        private async void ButtonGeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            // 1) Si no estamos en modo lectura (albarán incompleto), mostramos mensaje de error
            if (!ViewModel.IsReadOnlyMode)
            {
                if (this.Content is FrameworkElement root)
                {
                    var dlg = new ContentDialog
                    {
                        Title = "Albarán incompleto",
                        Content = "No se puede imprimir un PDF si el albarán no está completo.",
                        CloseButtonText = "OK",
                        XamlRoot = root.XamlRoot
                    };
                    await dlg.ShowAsync();
                }
                return;
            }

            // 2) Si estamos en modo lectura, invocamos el método del ViewModel para generar el PDF
            try
            {
                await ViewModel.GeneratePdfAsync();
            }
            catch (Exception ex)
            {
                // Si hay algún fallo en la generación del PDF, lo capturamos y mostramos al usuario
                if (this.Content is FrameworkElement root)
                {
                    var dlg = new ContentDialog
                    {
                        Title = "Error al generar PDF",
                        Content = $"Ha ocurrido un error al crear el PDF:\n{ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = root.XamlRoot
                    };
                    await dlg.ShowAsync();
                }
            }
        }


        private void Reparto_Click(object sender, RoutedEventArgs e) => ShowModal(new DeliveryWindow());
        private void Documentos_Click(object sender, RoutedEventArgs e) => ShowModal(new DocumentWindow());
        private void Stock_Click(object sender, RoutedEventArgs e) => ShowModal(new StockWindow());

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            // Ejecutar la búsqueda en el ViewModel
            var results = await ViewModel.SearchCustomersAsync();
            var dlg = new SearchResultsWindow(results);

            dlg.ViewModel.SelectionConfirmed += c =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    TextBoxClientId.Text = c.Id.ToString();
                    TextBoxName.Text = c.Nombre;
                    TextBoxCifNif.Text = c.CifNif ?? string.Empty;
                    TextBoxPhone.Text = c.Telefono ?? string.Empty;
                    TextBoxEmail.Text = c.Email ?? string.Empty;
                    TextBoxAddress.Text = c.Direccion ?? string.Empty;

                    // Sincronizar también el ViewModel
                    ViewModel.SearchClientId = c.Id.ToString();
                    ViewModel.SearchName = c.Nombre;
                    ViewModel.SearchCifNif = c.CifNif ?? string.Empty;
                    ViewModel.SearchPhone = c.Telefono ?? string.Empty;
                    ViewModel.SearchEmail = c.Email ?? string.Empty;

                    ViewModel.SelectedCustomer = c;
                });
            };

            ShowModal(dlg);
        }

        private void ButtonAlbaran_Click(object sender, RoutedEventArgs e)
        {
            ClearAlbaranForm();
        }

        private void ButtonAddLine_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddLineWindow(ViewModel.DocumentId, _currentDocument);
            window.LineCreated += linea => ViewModel.Lines.Add(linea);
            ShowModal(window);
        }

        private void ButtonEditLine_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridResults.SelectedItem is DocumentLine original)
            {
                var window = new AddLineWindow(ViewModel.DocumentId, _currentDocument, original);
                window.LineCreated += updated => ViewModel.EditLine(original, updated);
                ShowModal(window);
            }
        }

        private void ButtonRemoveLine_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridResults.SelectedItem is DocumentLine linea)
            {
                ViewModel.RemoveLine(linea);
            }
        }

        private void ShowModal(Window child)
        {
            var parentHwnd = WindowNative.GetWindowHandle(this);
            EnableWindow(parentHwnd, false);

            if (this.Content is FrameworkElement root)
                root.Opacity = 0.5;

            child.Closed += (s, e) =>
            {
                EnableWindow(parentHwnd, true);
                // — restaurar opacidad
                if (this.Content is FrameworkElement r)
                    r.Opacity = 1.0;
            };

            child.Activate();
        }

        private void ClearAlbaranForm()
        {
            // 1) Limpiar UI
            TextBoxClientId.Text = string.Empty;
            TextBoxName.Text = string.Empty;
            TextBoxCifNif.Text = string.Empty;
            TextBoxPhone.Text = string.Empty;
            TextBoxEmail.Text = string.Empty;
            TextBoxAddress.Text = string.Empty;

            // 2) Limpiar filtros en el VM (si usas TwoWay)
            ViewModel.SearchClientId = string.Empty;
            ViewModel.SearchName = string.Empty;
            ViewModel.SearchCifNif = string.Empty;
            ViewModel.SearchPhone = string.Empty;
            ViewModel.SearchEmail = string.Empty;

            // 3) Limpiar líneas de documento añadidas
            ViewModel.Lines.Clear();
            ViewModel.ClearInvoiceState();

            _currentDocument = new Document
            {
                Id = ViewModel.DocumentId
            };
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            var auth = App.Host.Services.GetRequiredService<AuthenticationService>();
            auth.CurrentUserName = string.Empty;

            var login = new LoginWindow();
            IntPtr hWndLogin = WindowNative.GetWindowHandle(login);
            ShowWindow(hWndLogin, SW_MAXIMIZE);
            login.Activate();

            this.Close();
        }


    }
}
