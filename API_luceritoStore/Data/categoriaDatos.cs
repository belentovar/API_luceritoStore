using Microsoft.Data.SqlClient;

namespace API_luceritoStore.Data
{
    public class categoriaDatos
    {
        public bool ExisteCategoria(int id)
        {
            AccesoDatos acceso = new AccesoDatos();
            string consulta = "SELECT TOP 1 1 FROM CategoriaProductos WHERE idCategoria = @id";

            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@id", id);

                object resultado = sqlCommand.ExecuteScalar();
                return resultado != null;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }
    }
}
