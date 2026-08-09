using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Data
{
    public class DetalleVentaDatos
    {
        public List<Venta> listarVentasConDetallePorUsuario(int idUsuario)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            List<Venta> ventas = new List<Venta>();
            string consulta = "SELECT v.idVenta, v.idUsuario, v.fecha, v.total, v.estado, " +
                      "v.tipoEntrega, v.metodoPago, v.direccion, v.idLocalidad, " +
                      "l.nombreLocalidad, pr.nombreProvincia, " +
                      "dv.cantidad, dv.precioUnitario, dv.idDetalleVenta, " +
                      "p.idProducto, p.NombreProducto, p.Precio, p.Detalle, p.ImagenProducto " +
                      "FROM Ventas v " +
                      "LEFT JOIN Localidades l ON v.idLocalidad = l.idLocalidad " +
                      "LEFT JOIN Provincias pr ON l.idProvincia = pr.idProvincia " +
                      "LEFT JOIN DetalleVentas dv ON v.idVenta = dv.idVenta " +
                      "LEFT JOIN Productos p ON dv.idProducto = p.idProducto " +
                      "WHERE v.idUsuario = @idUsuario " +
                      "ORDER BY v.idVenta"; 
            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Venta? venta = ventas.FirstOrDefault(venta => venta.IdVenta == (int)sqlDataReader["idVenta"]);
                    if (venta == null)
                    {
                        venta = new Venta()
                        {
                            IdVenta = (int)sqlDataReader["idVenta"],
                            IdUsuario = (int)sqlDataReader["idUsuario"],
                            Fecha = (DateTime)sqlDataReader["fecha"],
                            Total = (decimal)sqlDataReader["total"],
                            Estado = sqlDataReader["estado"].ToString(),
                            TipoEntrega = sqlDataReader["tipoEntrega"].ToString(),
                            MetodoPago = sqlDataReader["metodoPago"].ToString(),
                            Direccion = sqlDataReader["direccion"] == DBNull.Value ? null : sqlDataReader["direccion"].ToString(),
                            IdLocalidad = sqlDataReader["idLocalidad"] == DBNull.Value ? 0 : (int)sqlDataReader["idLocalidad"],
                            NombreLocalidad = sqlDataReader["nombreLocalidad"] == DBNull.Value ? null : sqlDataReader["nombreLocalidad"].ToString(),
                            NombreProvincia = sqlDataReader["nombreProvincia"] == DBNull.Value ? null : sqlDataReader["nombreProvincia"].ToString(),
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


        public Venta? obtenerVentaConDetalle(int idVenta)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
           
            Venta? venta = null;
            string consulta = "SELECT v.idVenta, v.idUsuario, v.fecha, v.total, v.estado, " +
                      "v.tipoEntrega, v.metodoPago, v.direccion, v.idLocalidad, " +
                      "l.nombreLocalidad, pr.nombreProvincia, " +
                      "dv.cantidad, dv.precioUnitario, dv.idDetalleVenta, " +
                      "p.idProducto, p.NombreProducto, p.Precio, p.Detalle, p.ImagenProducto " +
                      "FROM Ventas v " +
                      "LEFT JOIN Localidades l ON v.idLocalidad = l.idLocalidad " +
                      "LEFT JOIN Provincias pr ON l.idProvincia = pr.idProvincia " +
                      "LEFT JOIN DetalleVentas dv ON v.idVenta = dv.idVenta " +
                      "LEFT JOIN Productos p ON dv.idProducto = p.idProducto " +
                      "WHERE v.idVenta = @idVenta " +
                      // Se ordena por el renglon de detalle, no por v.idVenta: al filtrar por una
                      // sola venta ese ORDER BY no ordenaba nada. Asi los renglones salen siempre
                      // en el mismo orden entre llamadas.
                      "ORDER BY dv.idDetalleVenta";
            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idVenta", idVenta);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    if (venta == null)
                    {
                        venta = new Venta()
                        {
                            IdVenta = (int)sqlDataReader["idVenta"],
                            IdUsuario = (int)sqlDataReader["idUsuario"],
                            Fecha = (DateTime)sqlDataReader["fecha"],
                            Total = (decimal)sqlDataReader["total"],
                            Estado = sqlDataReader["estado"].ToString(),
                            TipoEntrega = sqlDataReader["tipoEntrega"].ToString(),
                            MetodoPago = sqlDataReader["metodoPago"].ToString(),
                            Direccion = sqlDataReader["direccion"] == DBNull.Value ? null : sqlDataReader["direccion"].ToString(),
                            IdLocalidad = sqlDataReader["idLocalidad"] == DBNull.Value ? 0 : (int)sqlDataReader["idLocalidad"],
                            NombreLocalidad = sqlDataReader["nombreLocalidad"] == DBNull.Value ? null : sqlDataReader["nombreLocalidad"].ToString(),
                            NombreProvincia = sqlDataReader["nombreProvincia"] == DBNull.Value ? null : sqlDataReader["nombreProvincia"].ToString(),
                            Detalle = new List<DetalleVenta>()
                        };
                        
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
                                Precio = (decimal)sqlDataReader["precio"],
                                // Ya venian en el SELECT pero no se mapeaban. La imagen y la
                                // descripcion las necesita la pantalla de detalle de compra.
                                Detalle = sqlDataReader["detalle"].ToString(),
                                ImagenProducto = sqlDataReader["imagenProducto"].ToString()
                            }
                        });
                    }

                }
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
            // null si la venta no existe; con detalle cargado si existe.
            return venta;
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
