using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using Motix_v2.Presentation.WinUI.Views.Dialogs;
using Motix_v2.Domain.Entities;
using Microsoft.UI.Xaml.Controls;

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class SalesWindow : Window
    {
        public SalesViewModel ViewModel { get; }

        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MAXIMIZE = 3;
        public SalesWindow()
        {
            this.InitializeComponent();
            ViewModel = App.Host.Services.GetRequiredService<SalesViewModel>();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
        }

        private void AbrirVentana(Window w, bool cerrarActual = true)
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(w);
            ShowWindow(hWnd, SW_MAXIMIZE);
            w.Activate();
            //if (cerrarActual) this.Close();
        }

        private void Reparto_Click(object sender, RoutedEventArgs e) => AbrirVentana(new DeliveryWindow());
        private void Documentos_Click(object sender, RoutedEventArgs e) => AbrirVentana(new DocumentWindow());
        private void Stock_Click(object sender, RoutedEventArgs e) => AbrirVentana(new StockWindow());
        private void Login_Click(object sender, RoutedEventArgs e) => AbrirVentana(new LoginWindow());

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            // Ejecutar la búsqueda en el ViewModel
            var results = await ViewModel.SearchCustomersAsync();
            var dlg = new SearchResultsWindow(results);

            dlg.XamlRoot = layoutRoot.XamlRoot;
            var dialogResult = await dlg.ShowAsync();

            // Si el usuario seleccionó un cliente, vuelca sus datos en los TextBoxes
            if (dlg.SelectedCustomer is Customer c)
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
            }
        }

        private void ButtonAlbaran_Click(object sender, RoutedEventArgs e)
        {
            TextBoxClientId.Text = string.Empty;
            TextBoxName.Text = string.Empty;
            TextBoxCifNif.Text = string.Empty;
            TextBoxPhone.Text = string.Empty;
            TextBoxEmail.Text = string.Empty;
            TextBoxAddress.Text = string.Empty;

            // Opcional: si usas bindings TwoWay, limpia también el ViewModel
            ViewModel.SearchClientId = string.Empty;
            ViewModel.SearchName = string.Empty;
            ViewModel.SearchCifNif = string.Empty;
            ViewModel.SearchPhone = string.Empty;
            ViewModel.SearchEmail = string.Empty;
        }

        private void ButtonAddLine_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartWindow();
            window.Activate();
        }

        private void ButtonEditLine_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartWindow();
            window.Activate();
        }

        private void ButtonRemoveLine_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartWindow();
            window.Activate();
        }

    }
}
