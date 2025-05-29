using Motix_v2.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class SearchResultsViewModel : INotifyPropertyChanged
    {

        public SearchResultsViewModel(IEnumerable<Customer> results)
        {
            Results = new ObservableCollection<Customer>(results);
        }
        public ObservableCollection<Customer> Results { get; }

        private Customer? _selectedItem;
        public Customer? SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSelect)); }
        }

        public bool CanSelect => SelectedItem != null;

        public void ConfirmSelection()
            => SelectionConfirmed?.Invoke(SelectedItem!);

        public event Action<Customer>? SelectionConfirmed;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }
}
