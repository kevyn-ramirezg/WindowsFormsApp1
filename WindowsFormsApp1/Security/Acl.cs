using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1.Security
{
    public static class Acl
    {
        // Niveles de rol esperados:
        // 3 = Administrador (todo)
        // 2 = Paramétrico   (entidades + transacciones + reportes + utilidades; sin Usuarios/Bitácora)
        // 1 = Esporádico    (solo lectura en entidades + reportes; sin transacciones ni utilidades)

        // ¿Puedo ABRIR una pantalla / menú?
        public static bool CanOpen(Feature f)
        {
            int n = Session.Nivel; // usa tu Session actual

            if (n >= 3) return true; // Admin hace todo

            switch (f)
            {
                // Entidades: 1+ puede abrir. (El 1 verá solo lectura porque botones se deshabilitan con Acl.Can)
                case Feature.Categorias:
                case Feature.Clientes:
                case Feature.Productos:
                    return n >= 1;

                // Transacciones: 2+
                case Feature.Ventas:
                case Feature.Creditos:
                    return n >= 2;

                // Usuarios y Bitácora: solo Admin
                case Feature.Usuarios:
                case Feature.Bitacora:
                    return n >= 3;

                // Reportes: 1+
                case Feature.ReporteFactura:
                case Feature.ReporteVentasMes:
                case Feature.ReporteIvaTrimestre:
                case Feature.ReporteMorosos:
                case Feature.ReporteTopProductos:
                case Feature.ReporteExistenciasBajas:
                    return n >= 1;

                // Utilidades: 2+  (si quieres permitir calculadora al 1, cambia a "return n >= 1;")
                case Feature.Util_Calculadora:
                case Feature.Util_Calendario:
                case Feature.Util_ExportarCsv:
                    return n >= 2;

                default:
                    return false;
            }
        }

        // ¿Puedo EJECUTAR una acción concreta (botón/operación)?
        public static bool Can(Feature action)
        {
            int n = Session.Nivel;

            if (n >= 3) return true; // Admin todo

            switch (action)
            {
                // CRUD de entidades (crear/editar/borrar): 2+
                case Feature.CategoriasCreate:
                case Feature.CategoriasUpdate:
                case Feature.CategoriasDelete:
                case Feature.ClientesCreate:
                case Feature.ClientesUpdate:
                case Feature.ClientesDelete:
                case Feature.ProductosCreate:
                case Feature.ProductosUpdate:
                case Feature.ProductosDelete:
                    return n >= 2;

                // Transacciones: 2+
                case Feature.VentasCreate:
                case Feature.VentasAnular:
                case Feature.CreditosPagar:
                    return n >= 2;

                // Usuarios: solo Admin
                case Feature.UsuariosCreate:
                case Feature.UsuariosUpdate:
                case Feature.UsuariosDelete:
                case Feature.UsuariosResetPwd:
                case Feature.UsuariosToggleActivo:
                    return n >= 3;

                // Reportes: 1+
                case Feature.ReporteFactura:
                case Feature.ReporteVentasMes:
                case Feature.ReporteIvaTrimestre:
                case Feature.ReporteMorosos:
                case Feature.ReporteTopProductos:
                case Feature.ReporteExistenciasBajas:
                    return n >= 1;

                // Utilidades: 2+
                case Feature.Util_Calculadora:
                case Feature.Util_Calendario:
                case Feature.Util_ExportarCsv:
                    return n >= 2;

                default:
                    return false;
            }
        }

        // Lanza excepción si NO se puede abrir (útil en Load del form para cerrarlo de una)
        public static void Require(Feature feature)
        {
            if (!CanOpen(feature))
                throw new UnauthorizedAccessException("No tienes permisos para acceder a esta opción.");
        }
    }
}
