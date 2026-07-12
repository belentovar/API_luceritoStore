using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Data
{
    public class CarritoDatos
    {
        public Carrito obtenerCarritoPorUsuario(int idUsuario)
        {
            string consulta = "SELECT idCarrito, idUsuario FROM Carritos WHERE idUsuario = @idUsuario";

            AccesoDatos acceso = new AccesoDatos(); // <-
            Carrito carrito = new Carrito();

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
            AccesoDatos acceso = new AccesoDatos();

            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = @"INSERT INTO Carritos (idUsuario, fechaCreacion)
                        VALUES (@idUsuario, @fechaCreacion);
                        SELECT SCOPE_IDENTITY();";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion);

            sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
            sqlCommand.Parameters.AddWithValue("@fechaCreacion", DateTime.Now);

            int idGenerado = Convert.ToInt32(sqlCommand.ExecuteScalar());

            acceso.CerrarConexion(conexion);
            return idGenerado;  //devuelde el id del carrito nuevo
        }


        public int vaciarCarrito(int idCarrito)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "DELETE FROM CarritoProductos WHERE idCarrito = @idCarrito";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion);

            sqlCommand.Parameters.AddWithValue("@idCarrito", idCarrito);

            int filas = sqlCommand.ExecuteNonQuery();

            acceso.CerrarConexion(conexion);
            return filas;
        }
    }
}
