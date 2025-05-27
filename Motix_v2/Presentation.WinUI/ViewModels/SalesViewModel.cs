using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using System.Linq;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class SalesViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        // --- criterios de búsqueda ---
        public string SearchClientId { get; set; } = string.Empty;
        public string SearchName { get; set; } = string.Empty;
        public string SearchCifNif { get; set; } = string.Empty;
        public string SearchPhone { get; set; } = string.Empty;
        public string SearchEmail { get; set; } = string.Empty;

        // Colección de líneas de documento en memoria (sin persistir)
        public ObservableCollection<DocumentLine> Lines { get; }
            = new ObservableCollection<DocumentLine>();

        /// <summary>
        /// Colección enlazada al DataGrid en la vista.
        /// </summary>
        public ObservableCollection<Customer> Items { get; }
            = new ObservableCollection<Customer>();

        public SalesViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        public void AddLine(DocumentLine line)
        {
            Lines.Add(line);
        }

        /// <summary>Modifica una línea existente en el albarán en curso.</summary>
        public void EditLine(DocumentLine line)
        {
            // Ejemplo simple: al editar, asume que la instancia ya está en Lines y Notifica cambios
            var index = Lines.IndexOf(line);
            if (index >= 0)
                Lines[index] = line;
        }

        /// <summary>Elimina una línea del albarán en curso.</summary>
        public void RemoveLine(DocumentLine line)
        {
            Lines.Remove(line);
        }

    }
}
