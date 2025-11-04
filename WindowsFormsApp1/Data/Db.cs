using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace WindowsFormsApp1.Data
{
    public static class Db
    {
        public static OracleConnection Open()
        {
            var cs = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            var cn = new OracleConnection(cs);
            cn.Open();
            return cn;
        }
    }
}
