using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class ProductoForVenta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Stock { get; set; }
        public decimal IvaPct { get; set; }
        public decimal UtilidadPct { get; set; }

        public override string ToString() => Nombre; // por si el Combo usa ToString
    }
}
