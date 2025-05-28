using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using Microsoft.UI.Xaml.Input;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class PartWindowViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public XamlUICommand SearchCommand { get; }

        // --- Criterios de búsqueda ---
        public string SearchInternalReference { get; set; } = string.Empty;
        public string SearchName { get; set; } = string.Empty;

        // Colección enlazada al DataGrid
        public ObservableCollection<Part> Parts { get; }
            = new ObservableCollection<Part>();

        public PartWindowViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
                p =>
                    (string.IsNullOrWhiteSpace(SearchInternalReference)
                        || p.ReferenciaInterna.Contains(SearchInternalReference)) &&
                    (string.IsNullOrWhiteSpace(SearchName)
                        || (p.Nombre ?? string.Empty).Contains(SearchName)),
                ct);

            // Refresca la colección para el DataGrid
            Parts.Clear();
            foreach (var part in resultados)
                Parts.Add(part);
        }
    }
}
