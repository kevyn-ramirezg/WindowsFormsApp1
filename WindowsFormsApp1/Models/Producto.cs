using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Producto
    {
        public decimal Id { get; set; }
        public decimal CategoriaId { get; set; }
        public string Nombre { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Stock { get; set; }
        // (opcional) Nombre de la categoría para mostrar en grids
        public string Categoria { get; set; }
    }
}
