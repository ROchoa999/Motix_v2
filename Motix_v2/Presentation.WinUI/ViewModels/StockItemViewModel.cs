using System.ComponentModel;
using System.Runtime.CompilerServices;
using Motix_v2.Domain.Entities;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class StockItemViewModel : INotifyPropertyChanged
    {
        private readonly Part _entity;
        private int _stock;

        public StockItemViewModel(Part part)
        {
            _entity = part;
            _stock = part.Stock;
        }

        public Part Entity => _entity;
        public int Id => _entity.Id;
        public string ReferenciaInterna => _entity.ReferenciaInterna;  // :contentReference[oaicite:1]{index=1}
        public string Nombre => _entity.Nombre;
        public string? Descripcion => _entity.Descripcion;
        public decimal? PrecioVenta => _entity.PrecioVenta;
        public int Stock
        {
            get => _stock;
            set
            {
                if (_stock != value)
                {
                    _stock = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
