using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Presentation.WinUI.ViewModels;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;
using System;

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class DeliveryWindow : Window
    {
        public DeliveryWindow()
        {
            InitializeComponent();

            ViewModel = App.Host.Services.GetRequiredService<DeliveryViewModel>();
            this.Activate();

            // Carga inicial de datos
            ViewModel.LoadCommand.Execute(null);

            // Ajustes de tamaño y comportamiento de la ventana
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

        public DeliveryViewModel ViewModel { get; }

        private void OnBackClicked(object sender, RoutedEventArgs e)
            => this.Close();

        private async void OnRepartirClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirmación",
                Content = "¿Estás seguro de que deseas COMENZAR el reparto de estos albaranes?",
                PrimaryButtonText = "Sí",
                CloseButtonText = "No",
                // IMPORTANTE: asignar el XamlRoot para que el dialog se muestre correctamente
                XamlRoot = this.Content.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Ejecutar el comando de ViewModel
                if (ViewModel.StartDeliveryCommand.CanExecute(null))
                {
                    ViewModel.StartDeliveryCommand.Execute(null);
                }
                // Cerramos esta ventana para volver a la de ventas
                this.Close();
            }
        }
        private async void OnTerminarRepartoClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirmación",
                Content = "¿Estás seguro de que deseas FINALIZAR el reparto?",
                PrimaryButtonText = "Sí",
                CloseButtonText = "No",
                XamlRoot = this.Content.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (ViewModel.FinishDeliveryCommand.CanExecute(null))
                {
                    ViewModel.FinishDeliveryCommand.Execute(null);
                }
                this.Close();
            }
        }
    }
}
