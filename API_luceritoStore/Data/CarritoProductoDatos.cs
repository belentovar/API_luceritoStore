using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;

namespace API_luceritoStore.Data
{
    public class CarritoProductoDatos
    {
        public int agregarProducto(int idCarrito, int idProducto, int cantidad)
        {
            AccesoDatos conexion = new AccesoDatos();

            string consulta = @"INSERT INTO CarritoProductos (idCarrito, idProducto, cantidad)
                        VALUES (@idCarrito, @idProducto, @cantidad)";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());

            sqlCommand.Parameters.AddWithValue("@idCarrito", idCarrito);
            sqlCommand.Parameters.AddWithValue("@idProducto", idProducto);
            sqlCommand.Parameters.AddWithValue("@cantidad", cantidad);

            int filas = sqlCommand.ExecuteNonQuery();

            conexion.CerrarConexion();
            return filas;
        }

        public List<CarritoProducto> obtenerProductosCarrito(int idCarrito)
        {
            AccesoDatos conexion = new AccesoDatos();
            List<CarritoProducto> productos = new List<CarritoProducto>();

            string consulta = @"SELECT cp.idCarritoProducto, cp.idCarrito, cp.idProducto, cp.cantidad,
                        p.NombreProducto, p.Precio, p.ImagenProducto
                        FROM CarritoProductos cp
                        INNER JOIN Productos p ON cp.idProducto = p.idProducto
                        WHERE cp.idCarrito = @idCarrito";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
            sqlCommand.Parameters.AddWithValue("@idCarrito", idCarrito);

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                CarritoProducto carritoProducto = new CarritoProducto()
                {
                    idCarritoProducto = (int)reader["idCarritoProducto"],
                    idCarrito = (int)reader["idCarrito"],
                    idProducto = (int)reader["idProducto"],
                    cantidad = (int)reader["cantidad"],
                    Producto = new Producto()
                    {
                        IdProducto = (int)reader["idProducto"],
                        NombreProducto = reader["NombreProducto"].ToString(),
                        Precio = (decimal)reader["Precio"]
                    }
                };

                productos.Add(carritoProducto);
            }

            conexion.CerrarConexion();
            return productos;
        }

        public int actualizarCantidad(int idCarritoProducto, int cantidad)
        {
            AccesoDatos conexion = new AccesoDatos();

            string consulta = @"UPDATE CarritoProductos
                        SET cantidad = @cantidad
                        WHERE idCarritoProducto = @idCarritoProducto";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());

            sqlCommand.Parameters.AddWithValue("@cantidad", cantidad);
            sqlCommand.Parameters.AddWithValue("@idCarritoProducto", idCarritoProducto);

            int filas = sqlCommand.ExecuteNonQuery();

            conexion.CerrarConexion();
            return filas;
        }

        public int eliminarProducto(int idCarritoProducto)
        {
            AccesoDatos conexion = new AccesoDatos();

            string consulta = "DELETE FROM CarritoProductos WHERE idCarritoProducto = @idCarritoProducto";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());

            sqlCommand.Parameters.AddWithValue("@idCarritoProducto", idCarritoProducto);

            int filas = sqlCommand.ExecuteNonQuery();

            conexion.CerrarConexion();
            return filas;
        }
    }
}
