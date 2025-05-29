using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using WinRT.Interop;
using CommunityToolkit.WinUI.UI.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Motix_v2.Presentation.WinUI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockWindow : Window
    {
        public StockWindow()
        {
            this.InitializeComponent();
            ViewModel = App.Host.Services
                .GetRequiredService<StockWindowViewModel>();

            // Opcional: dispara la carga al abrir
            ViewModel.LoadCommand.Execute(null);

            DataGridStock.RowEditEnding += OnStockRowEditEnding;

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

        public StockWindowViewModel ViewModel { get; }

        private void OnVolverClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnGuardarClicked(object sender, RoutedEventArgs e)
        {
            // Lógica de guardar cambios en StockItems aquí…
        }

        private void OnStockRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            ButtonGuardar.Visibility = Visibility.Visible;
        }
}
}
