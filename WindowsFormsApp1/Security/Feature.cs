namespace WindowsFormsApp1.Security
{
    // Macros = pantallas/menús completos
    // Granulares = botones/acciones dentro de cada pantalla
    public enum Feature
    {
        // ===== Macros por pantalla =====
        Categorias,
        Clientes,
        Productos,
        Ventas,
        Creditos,
        Usuarios,        // NUEVO (solo Admin)
        Bitacora,        // NUEVO (solo Admin)

        // Reportes (como macros porque abren pantallas)
        ReporteFactura,
        ReporteVentasMes,
        ReporteIvaTrimestre,
        ReporteMorosos,            // NUEVO
        ReporteTopProductos,       // NUEVO
        ReporteExistenciasBajas,   // NUEVO

        // Utilidades (si las quieres gobernar por permisos)
        Util_Calculadora,          // NUEVO
        Util_Calendario,           // NUEVO
        Util_ExportarCsv,          // NUEVO

        // ===== Acciones granulares en entidades =====
        CategoriasCreate, CategoriasUpdate, CategoriasDelete,
        ClientesCreate, ClientesUpdate, ClientesDelete,
        ProductosCreate, ProductosUpdate, ProductosDelete,

        // ===== Acciones granulares en transacciones =====
        VentasCreate, VentasAnular,
        CreditosPagar,

        // ===== Acciones granulares en Usuarios =====
        UsuariosCreate, UsuariosUpdate, UsuariosDelete, UsuariosResetPwd, UsuariosToggleActivo
    }
}
