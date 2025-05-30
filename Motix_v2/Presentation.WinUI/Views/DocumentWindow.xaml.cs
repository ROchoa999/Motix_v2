using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class DocumentWindow : Window
    {

        public DocumentWindow()
        {
            InitializeComponent();

            ViewModel = App.Host.Services
                .GetRequiredService<DocumentViewModel>();

            // Carga inicial
            ViewModel.LoadCommand.Execute(null);

            // Fijar tamaño y propiedades de ventana
            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(1000, 700));
            if (appWindow.Presenter is OverlappedPresenter p)
            {
                p.IsResizable = false;
                p.IsMaximizable = false;
                p.IsMinimizable = false;
            }
        }

        private void OnClearFiltersClicked(object sender, RoutedEventArgs e)
        {
            // Limpiamos las fechas en el VM y recargamos todo
            ViewModel.StartDate = null;
            ViewModel.EndDate = null;
            ViewModel.LoadCommand.Execute(null);
        }


        public DocumentViewModel ViewModel { get; }

        private void OnVolverClicked(object sender, RoutedEventArgs e)
            => this.Close();
    }
}
