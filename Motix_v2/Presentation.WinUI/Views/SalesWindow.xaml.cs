using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class SalesWindow : Window
    {
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private const int SW_MAXIMIZE = 3;
        public SalesWindow()
        {
            this.InitializeComponent();

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

    }
}
