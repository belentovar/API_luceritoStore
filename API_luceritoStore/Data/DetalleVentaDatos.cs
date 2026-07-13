using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Data
{
    public class DetalleVentaDatos
    {
        public List<Venta> listarDetalleVenta(int idUsuario)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            List<Venta> ventas = new List<Venta>();

            string consulta = "SELECT v.idVenta, v.idUsuario, v.fecha, v.total, v.estado, " +
                  "dv.cantidad, dv.precioUnitario, dv.idDetalleVenta, " +
                  "p.idProducto, p.NombreProducto, p.Precio, p.Detalle, p.ImagenProducto " +
                  "FROM Ventas v " +
                  "LEFT JOIN DetalleVentas dv ON v.idVenta = dv.idVenta " +
                  "LEFT JOIN Productos p ON dv.idProducto = p.idProducto " +
                  "WHERE v.idUsuario = @idUsuario " +
                  "ORDER BY v.idVenta";

            // Se envuelve en try/finally para asegurar que la conexion se cierre
            // siempre, aunque falle la lectura (antes no habia y podia quedar abierta).
            try
            {
            SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
            sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            while(sqlDataReader.Read()) {

                Venta? venta = ventas.FirstOrDefault(venta => venta.IdVenta == (int)sqlDataReader["idVenta"]);

                if (venta == null) {
                    venta = new Venta()
                    {
                        IdVenta = (int)sqlDataReader["idVenta"],
                        IdUsuario = (int)sqlDataReader["idUsuario"],
                        Fecha = (DateTime)sqlDataReader["fecha"],
                        Total = (decimal)sqlDataReader["total"],
                        Estado = sqlDataReader["estado"].ToString(),
                        Detalle = new List<DetalleVenta>()
                    };

                    ventas.Add(venta);
                }

                if (sqlDataReader["idDetalleVenta"] != DBNull.Value)
                {
                    venta.Detalle.Add(new DetalleVenta()
                    {
                        IdDetalleVenta = (int)sqlDataReader["idDetalleVenta"],
                        IdVenta = (int)sqlDataReader["idVenta"],
                        Cantidad = (int)sqlDataReader["cantidad"],
                        PrecioUnitario = (decimal)sqlDataReader["precioUnitario"],
                        Producto = new Producto()
                        {
                            IdProducto = (int)sqlDataReader["idProducto"],
                            NombreProducto = sqlDataReader["nombreProducto"].ToString(),
                            Precio = (decimal)sqlDataReader["precio"]
                        }
                    });
                }
                    
            }

            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }

            return ventas;
        }

        public int insertarDetalle(int idVenta, DetalleVenta detalle, SqlConnection conn, SqlTransaction transaccion)
        {
            string consulta = @"INSERT INTO DetalleVentas (idVenta, idProducto, cantidad, precioUnitario)
                        VALUES (@idVenta, @idProducto, @cantidad, @precio)";

            SqlCommand cmd = new SqlCommand(consulta, conn, transaccion);

            cmd.Parameters.AddWithValue("@idVenta", idVenta);
            // Se usa detalle.idProducto (el id que envia el front) en lugar de detalle.Producto.IdProducto,
            // que era null cuando el front solo manda el id y provocaba NullReferenceException.
            cmd.Parameters.AddWithValue("@idProducto", detalle.idProducto);
            cmd.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@precio", detalle.PrecioUnitario);

            return cmd.ExecuteNonQuery();
        }

    }
}
