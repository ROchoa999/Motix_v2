using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;
using Motix_v2.Domain.Entities;

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class DocumentWindow : Window
    {

        public DocumentWindow()
        {
            InitializeComponent();
            AppWindow.SetIcon("Assets\\IconoV1.ico");

            TableViewDocuments.SelectionChanged += TableViewDocuments_SelectionChanged;

            ViewModel = App.Host.Services.GetRequiredService<DocumentViewModel>();

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

        private void TableViewDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonAbrir.IsEnabled = TableViewDocuments.SelectedItem is Document;
        }

        private void ButtonAbrir_Click(object sender, RoutedEventArgs e)
        {
            if(ViewModel != null && TableViewDocuments.SelectedItem is Document doc)
            {
                ViewModel.SelectDocument(doc.Id);

                this.Close();
            }
        }

        private void TableViewDocuments_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel != null && TableViewDocuments.SelectedItem is Document doc)
            {
                ViewModel.SelectDocument(doc.Id);

                this.Close();
            }
        }
    }
}
