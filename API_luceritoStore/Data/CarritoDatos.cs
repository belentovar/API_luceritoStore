using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Data
{
    public class CarritoDatos
    {
        private readonly AccesoDatos acceso;

        public CarritoDatos(AccesoDatos acceso) { 
            this.acceso = acceso;
        }
        public Carrito? obtenerCarritoPorUsuario(int idUsuario)
        {
            string consulta = "SELECT idCarrito, idUsuario FROM Carritos WHERE idUsuario = @idUsuario";

            Carrito? carrito = null;

            SqlConnection conexion = acceso.ObtenerConexion(); // <-

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion); // <-
                sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    carrito = new Carrito()
                    {
                        idCarrito = (int)reader["idCarrito"],
                        idUsuario = (int)reader["idUsuario"],
                    };
                }
            }
            finally
            {
                acceso.CerrarConexion(conexion); // <-
            }


            return carrito;
        }

        public int crearCarrito(int idUsuario)
        {

            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = @"INSERT INTO Carritos (idUsuario)
                        VALUES (@idUsuario);
                        SELECT SCOPE_IDENTITY();";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);

                int idGenerado = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return idGenerado; 
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }


        public int vaciarCarrito(int idCarrito)
        {
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "DELETE FROM CarritoProductos WHERE idCarrito = @idCarrito";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idCarrito", idCarrito);
                int filas = sqlCommand.ExecuteNonQuery();
                return filas;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }
    }
}
