using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Data
{
    public class CarritoDatos
    {
        // Devuelve null si el usuario no tiene carrito, igual que obtenerProductoPorId y
        // obtenerLocalidadPorId. Antes se inicializaba con new Carrito() antes del reader,
        // asi que nunca devolvia null: al usuario sin carrito le llegaba un objeto con
        // idCarrito = 0, y el chequeo == null del service no entraba jamas.
        public Carrito? obtenerCarritoPorUsuario(int idUsuario)
        {
            string consulta = "SELECT idCarrito, idUsuario FROM Carritos WHERE idUsuario = @idUsuario";

            AccesoDatos acceso = new AccesoDatos(); // <-
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
            AccesoDatos acceso = new AccesoDatos();

            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = @"INSERT INTO Carritos (idUsuario, fechaCreacion)
                        VALUES (@idUsuario, @fechaCreacion);
                        SELECT SCOPE_IDENTITY();";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                sqlCommand.Parameters.AddWithValue("@fechaCreacion", DateTime.Now);

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
            AccesoDatos acceso = new AccesoDatos();
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
