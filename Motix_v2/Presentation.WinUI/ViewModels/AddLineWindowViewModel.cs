using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class AddLineWindowViewModel : INotifyPropertyChanged
    {
        private readonly IUnitOfWork _unitOfWork;
        public XamlUICommand SearchCommand { get; }

        // --- Criterios de búsqueda ---
        public string SearchInternalReference { get; set; } = string.Empty;
        public string SearchName { get; set; } = string.Empty;

        // Colección enlazada al DataGrid
        public ObservableCollection<Part> Parts { get; }

        public AddLineWindowViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            Parts = new ObservableCollection<Part>();
            SearchCommand = new XamlUICommand();
            SearchCommand.ExecuteRequested += async (s, e) => await SearchAsync();
        }

        /// <summary>
        /// Ejecuta la búsqueda filtrando por ReferenciaInterna o Nombre.
        /// </summary>
        public async Task SearchAsync(CancellationToken ct = default)
        {
            // Recupera los registros que cumplan alguno de los criterios
            var resultados = await _unitOfWork.Parts.FindAsync(
                p => (string.IsNullOrWhiteSpace(SearchInternalReference)
                        || p.ReferenciaInterna.Contains(SearchInternalReference)) &&
                    (string.IsNullOrWhiteSpace(SearchName)
                        || (p.Nombre ?? string.Empty).Contains(SearchName)),
                ct);

            // Refresca la colección para el DataGrid
            Parts.Clear();
            foreach (var part in resultados)
                Parts.Add(part);
        }

        private Part? _selectedPart;
        public Part? SelectedPart
        {
            get => _selectedPart;
            set
            {
                if (_selectedPart != value)
                {
                    _selectedPart = value;
                    OnPropertyChanged();
                    // Cuando cambia, actualizamos el precio unitario y recalc total
                    UnitPrice = _selectedPart?.PrecioVenta ?? 0m;
                    RecalculateTotal();
                }
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            private set { _unitPrice = value; OnPropertyChanged(); }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    RecalculateTotal();
                }
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            private set { _totalPrice = value; OnPropertyChanged(); }
        }

        private void RecalculateTotal()
        {
            if (Quantity <= 0)
                TotalPrice = UnitPrice;
            else
                TotalPrice = Quantity * UnitPrice;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
