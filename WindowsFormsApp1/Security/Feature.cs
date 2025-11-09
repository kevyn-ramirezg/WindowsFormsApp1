namespace WindowsFormsApp1.Security
{
    // Define las acciones que vamos a controlar con PERMISO.ACCION
    // Incluye "macro" features para pantallas completas y otros granulares.
    public enum Feature
    {
        // ===== Macros por pantalla (los que usan tus forms) =====
        Categorias,
        Clientes,
        Productos,
        Ventas,
        Creditos,

        // ===== Acciones granulares (útiles si quieres restringir por botón) =====
        CategoriasCreate,
        CategoriasUpdate,
        CategoriasDelete,

        ClientesCreate,
        ClientesUpdate,
        ClientesDelete,

        ProductosCreate,
        ProductosUpdate,
        ProductosDelete,

        VentasCreate,
        VentasAnular,

        CreditosPagar,

        // Reportes
        ReporteFactura,
        ReporteVentasMes,
        ReporteIvaTrimestre
    }
}
