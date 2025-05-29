using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using System.Linq;
using System;
using Motix_v2.Infraestructure.Repositories;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Motix_v2.Infraestructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthenticationService _authService;
        public string DocumentId { get; } = Guid.NewGuid().ToString();
        public IReadOnlyList<string> PaymentMethods { get; } = new[] { "Tarjeta", "Efectivo" };
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // --- criterios de búsqueda ---
        public string SearchClientId { get; set; } = string.Empty;
        public string SearchName { get; set; } = string.Empty;
        public string SearchCifNif { get; set; } = string.Empty;
        public string SearchPhone { get; set; } = string.Empty;
        public string SearchEmail { get; set; } = string.Empty;

        // Colección de líneas de documento en memoria (sin persistir)
        public ObservableCollection<DocumentLine> Lines { get; }
            = new ObservableCollection<DocumentLine>();

        // Variables para almacenar los totales del albaran
        public decimal BaseImponible { get; private set; }
        public decimal Iva21 { get; private set; }
        public decimal TotalFactura { get; private set; }

        // Rellenar el nombre del vendedor con el usuario actual
        private string _vendedor = string.Empty;
        public string Vendedor
        {
            get => _vendedor;
            set
            {
                if (_vendedor != value)
                {
                    _vendedor = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Colección enlazada al DataGrid en la vista.
        /// </summary>
        public ObservableCollection<Customer> Items { get; }
            = new ObservableCollection<Customer>();

        public Task SaveAsync(CancellationToken ct = default) => _unitOfWork.SaveChangesAsync(ct);

        public SalesViewModel(IUnitOfWork unitOfWork, AuthenticationService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = App.Host.Services.GetRequiredService<AuthenticationService>();
            Vendedor = _authService.CurrentUserName;
            Lines.CollectionChanged += (s, e) => RecalculateTotals();
            RecalculateTotals();
        }

        public async Task<List<Customer>> SearchCustomersAsync(CancellationToken ct = default)
        {
            // 1) Recuperar todos
            var all = await _unitOfWork.Customers.GetAllAsync(ct);

            // 2) Filtrar según criterios
            var filtered = all.Where(c =>
                (string.IsNullOrWhiteSpace(SearchClientId) || c.Id.ToString() == SearchClientId) &&
                (string.IsNullOrWhiteSpace(SearchName) || c.Nombre.Contains(SearchName)) &&
                (string.IsNullOrWhiteSpace(SearchCifNif) || (c.CifNif ?? string.Empty).Contains(SearchCifNif)) &&
                (string.IsNullOrWhiteSpace(SearchPhone) || (c.Telefono ?? string.Empty).Contains(SearchPhone)) &&
                (string.IsNullOrWhiteSpace(SearchEmail) || (c.Email ?? string.Empty).Contains(SearchEmail))
            ).ToList();

            // 3) Refrescar el DataGrid
            Items.Clear();
            foreach (var cust in filtered)
                Items.Add(cust);

            return filtered;
        }

        /// <summary>Añade una línea al albarán en curso.</summary>
        public async Task AddLineAsync(DocumentLine line, CancellationToken ct = default)
        {
            line.DocumentoId = DocumentId;
            // accede al repositorio concreto para líneas
            var docRepo = (DocumentRepository)_unitOfWork.Documents;
            await docRepo.AddLineAsync(line, ct);
            Lines.Add(line);
        }


        /// <summary>Modifica una línea existente en el albarán en curso.</summary>
        public void EditLine(DocumentLine original, DocumentLine updated)
        {
            var index = Lines.IndexOf(original);
            if (index < 0) return;

            Lines.RemoveAt(index);
            Lines.Insert(index, updated);
        }

        /// <summary>Elimina una línea del albarán en curso.</summary>
        public void RemoveLine(DocumentLine line)
        {
            var docRepo = (DocumentRepository)_unitOfWork.Documents;
            docRepo.RemoveLine(line);
            Lines.Remove(line);
        }

        private string _selectedPaymentMethod = "Tarjeta";
        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                if (_selectedPaymentMethod != value)
                {
                    _selectedPaymentMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        private void RecalculateTotals()
        {
            BaseImponible = Lines.Sum(l => l.TotalLinea);

            Iva21 = Math.Round(BaseImponible * 0.21m, 2);

            TotalFactura = BaseImponible + Iva21;

            OnPropertyChanged(nameof(BaseImponible));
            OnPropertyChanged(nameof(Iva21));
            OnPropertyChanged(nameof(TotalFactura));
        }

    }
}
