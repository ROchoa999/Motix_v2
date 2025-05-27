using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using Motix_v2.Presentation.WinUI.Views.Dialogs;

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

        private async void SalesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // aquí llamas a ViewModel para cargar clientes…
            await ViewModel.LoadCustomersAsync();
        }

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            // 1) Ejecutar la búsqueda en el ViewModel
            var results = await ViewModel.SearchCustomersAsync();

            // 2) Crear y mostrar la ventana modal de resultados
            var dlg = new SearchResultsWindow(results);
            dlg.Activate();

            // (Opcional: maximizar)
            IntPtr hWnd = WindowNative.GetWindowHandle(dlg);
        }

    }
}
