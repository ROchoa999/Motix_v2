using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI;
using System.Collections.Generic;
using Motix_v2.Domain.Entities;

namespace Motix_v2.Presentation.WinUI.Views.Dialogs
{
    public sealed partial class SearchResultsWindow : Window
    {
        public SearchResultsWindow(IEnumerable<Customer> results)
        {
            InitializeComponent();

            ViewModel = new SearchResultsViewModel(results);

            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.SelectedItem))
                    ButtonSeleccionar.IsEnabled = ViewModel.SelectedItem != null;
            };

            // Opcional: fijar tamaño y deshabilitar máximizar/minimizar
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

        public SearchResultsViewModel ViewModel { get; }

        private void OnCerrarClicked(object sender, RoutedEventArgs e)
            => this.Close();

        private void OnSeleccionarClicked(object sender, RoutedEventArgs e)
        {
            // Notificar al llamador y cerrar
            ViewModel.ConfirmSelection();
            this.Close();
        }

        private void DataGridResults_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.ConfirmSelection();
                this.Close();
            }
        }
    }
}
