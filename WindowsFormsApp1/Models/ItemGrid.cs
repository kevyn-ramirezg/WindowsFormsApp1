using System;

namespace WindowsFormsApp1.Models
{
    public class ItemGrid
    {
        public decimal ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }   // uso decimal para empatar con NUMBER
        public decimal PrecioUnit { get; set; }
        public decimal Subtotal { get { return Cantidad * PrecioUnit; } }
    }
}
