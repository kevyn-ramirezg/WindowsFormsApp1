namespace WindowsFormsApp1.Models
{
    // Modelo solo para la UI del carrito
    public class ItemCarrito
    {
        public decimal ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal Stock { get; set; }

        // Campos calculados
        public decimal IvaPct { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }
}
