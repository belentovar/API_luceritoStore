using Microsoft.Data.SqlClient;
using System.Reflection;

namespace API_luceritoStore.Data
{
    public class AccesoDatos
    {
        public string cadenaConexion = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=DB_luceritoStore;Integrated Security=True;Trust Server Certificate=True";

        public SqlConnection ObtenerConexion()
        {
            SqlConnection sqlConnection = new SqlConnection(cadenaConexion);

            try
            {
                sqlConnection.Open();
                return sqlConnection;
            }
            catch (Exception ex) {
                return sqlConnection;
            }
        }

        public SqlConnection CerrarConexion()
        {
            SqlConnection sqlConnection = new SqlConnection(cadenaConexion);
            sqlConnection.Close();
            return sqlConnection;
        }
    }
}
