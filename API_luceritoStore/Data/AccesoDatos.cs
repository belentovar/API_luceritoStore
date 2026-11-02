using Microsoft.Data.SqlClient;
using System.Reflection;

namespace API_luceritoStore.Data
{
    public class AccesoDatos
    {
        public string cadenaConexion = string.Empty;

        public AccesoDatos(IConfiguration configuration)
        {
             cadenaConexion = configuration.GetConnectionString("cadenaConexion")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'cadenaConexion'.");

        }
        public SqlConnection ObtenerConexion()
        {
            SqlConnection sqlConnection = new SqlConnection(cadenaConexion);

            try
            {
                sqlConnection.Open();
                return sqlConnection;
            }
            catch (Exception)
            {
                throw; // ???
            }

        }

        public void CerrarConexion(SqlConnection conexion)
        {
            if (conexion != null)
            {
                conexion.Close();
            }
        }
    }
}
