using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Data
{
    public class CarritoDatos
    {
        public Carrito obtenerCarritoPorUsuario(int idUsuario)
        {
            AccesoDatos conexion = new AccesoDatos();
            Carrito carrito = new Carrito();

            string consulta = "SELECT idCarrito, idUsuario FROM Carritos WHERE idUsuario = @idUsuario";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
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

            conexion.CerrarConexion();
            return carrito;
        }

        public int crearCarrito(int idUsuario)
        {
            AccesoDatos conexion = new AccesoDatos();

            string consulta = @"INSERT INTO Carritos (idUsuario, fechaCreacion)
                        VALUES (@idUsuario, @fechaCreacion);
                        SELECT SCOPE_IDENTITY();";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());

            sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
            sqlCommand.Parameters.AddWithValue("@fechaCreacion", DateTime.Now);

            int idGenerado = Convert.ToInt32(sqlCommand.ExecuteScalar());

            conexion.CerrarConexion();
            return idGenerado;  //devuelde el id del carrito nuevo
        }


        public int vaciarCarrito(int idCarrito)
        {
            AccesoDatos conexion = new AccesoDatos();

            string consulta = "DELETE FROM CarritoProductos WHERE idCarrito = @idCarrito";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());

            sqlCommand.Parameters.AddWithValue("@idCarrito", idCarrito);

            int filas = sqlCommand.ExecuteNonQuery();

            conexion.CerrarConexion();
            return filas;
        }
    }
}
