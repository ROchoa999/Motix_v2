using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Motix_v2.Domain.Entities;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Motix_v2.Presentation.WinUI.Views.Dialogs
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResultsWindow : ContentDialog
    {
        public Customer? SelectedCustomer { get; private set; }

        public SearchResultsWindow(IEnumerable<Customer> customers)
        {
            this.InitializeComponent();
            DataGridResults.ItemsSource = customers;
        }

        // 1) Doble clic sobre una fila
        private void DataGridResults_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ConfirmSelection();
        }

        // 2) Click en el botón “Seleccionar” (SecondaryButton)
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ConfirmSelection();
        }

        // Lógica común que cierra el diálogo dejando la selección
        private void ConfirmSelection()
        {
            if (DataGridResults.SelectedItem is Customer c)
            {
                SelectedCustomer = c;
                Hide(); // cierra el ContentDialog
            }
        }
    }
}
