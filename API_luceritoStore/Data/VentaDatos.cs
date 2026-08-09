using API_luceritoStore.Models;
using API_luceritoStore.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;
namespace API_luceritoStore.Data
{
    public class VentaDatos
    {
        public int insertarVenta(Venta venta) //----------FIJARSE COMO FUNCIONA DESCONTARSTOCK ACA
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            SqlTransaction transaccion = conexion.BeginTransaction();

            try
            {
                string consulta = @"INSERT INTO Ventas (idUsuario, fecha, total, estado, idLocalidad, direccion, metodoPago, tipoEntrega)
                   VALUES (@idUsuario, @fecha, @total, @estado, @idLocalidad, @direccion, @metodoPago, @tipoEntrega);
                   SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(consulta, conexion, transaccion);

                cmd.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
                cmd.Parameters.AddWithValue("@fecha", venta.Fecha);
                cmd.Parameters.AddWithValue("@total", venta.Total);
                cmd.Parameters.AddWithValue("@estado", venta.Estado);
                cmd.Parameters.AddWithValue("@idLocalidad", venta.IdLocalidad == 0 ? (object)DBNull.Value : venta.IdLocalidad);
                cmd.Parameters.AddWithValue("@direccion", string.IsNullOrWhiteSpace(venta.Direccion) ? (object)DBNull.Value : venta.Direccion);
                cmd.Parameters.AddWithValue("@metodoPago", venta.MetodoPago);
                cmd.Parameters.AddWithValue("@tipoEntrega", venta.TipoEntrega);

                int idGenerado = Convert.ToInt32(cmd.ExecuteScalar());

                venta.IdVenta = idGenerado;

                DetalleVentaDatos detalleDatos = new DetalleVentaDatos();

                ProductosDatos productosDatos = new ProductosDatos();


                foreach (DetalleVenta detalle in venta.Detalle)
                {
                    detalleDatos.insertarDetalle(idGenerado, detalle, conexion, transaccion);


                    // Se descuenta el stock DENTRO de la misma transaccion. Si el UPDATE guardado
                    // no afecta ninguna fila es que no hay stock suficiente: se lanza excepcion y
                    // el catch hace Rollback, cancelando venta + detalles + descuentos juntos.
                    int filasStock = productosDatos.descontarStock(detalle.idProducto, detalle.Cantidad, conexion, transaccion);
                    if (filasStock == 0)
                    {
                        throw new Exception("No hay suficiente stock para el producto " + detalle.idProducto);
                    }

                }

                transaccion.Commit();

                return idGenerado;
            }
            catch
            {
                transaccion.Rollback();
                throw;
            }
            finally{
                acceso.CerrarConexion(conexion);
            }
        }

        public int actualizarEstadoVenta(int IdVenta, string Estado)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            string consulta = "update Ventas set estado = @estado where idVenta = @idVenta";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@estado", Estado);
                sqlCommand.Parameters.AddWithValue("@idVenta", IdVenta);

                return sqlCommand.ExecuteNonQuery();
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

        public Venta? obtenerVentaPorId(int idVenta)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            string consulta = "SELECT v.idVenta, v.idUsuario, v.fecha, v.total, v.estado, " +
               "v.idLocalidad, v.direccion, v.metodoPago, v.tipoEntrega, " +
               "l.nombreLocalidad, pr.nombreProvincia " +
               "FROM Ventas v " +
               "LEFT JOIN Localidades l ON v.idLocalidad = l.idLocalidad " +
               "LEFT JOIN Provincias pr ON l.idProvincia = pr.idProvincia " +
               "WHERE v.idVenta = @idventa";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idventa", idVenta);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.Read())
                {
                    Venta venta = new Venta()
                    {
                        IdVenta = (int)sqlDataReader["idVenta"],
                        IdUsuario = (int)sqlDataReader["idUsuario"],
                        Fecha = (DateTime)sqlDataReader["fecha"],
                        Total = (decimal)sqlDataReader["total"],
                        Estado = sqlDataReader["estado"].ToString(),
                        TipoEntrega = sqlDataReader["tipoEntrega"].ToString(),
                        MetodoPago = sqlDataReader["metodoPago"].ToString(),
                        IdLocalidad = sqlDataReader["idLocalidad"] == DBNull.Value ? 0 : (int)sqlDataReader["idLocalidad"],
                        Direccion = sqlDataReader["direccion"] == DBNull.Value ? null : sqlDataReader["direccion"].ToString(),
                        NombreLocalidad = sqlDataReader["nombreLocalidad"] == DBNull.Value ? null : sqlDataReader["nombreLocalidad"].ToString(),
                        NombreProvincia = sqlDataReader["nombreProvincia"] == DBNull.Value ? null : sqlDataReader["nombreProvincia"].ToString(),
                        Detalle = new List<DetalleVenta>()
                    };
                    return venta;
                }  

                return null;
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

        // SIN USO POR AHORA. No lo llama nadie
        public List<Venta> listarVentasPorUsuario(int idUsuario)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            // Mismos JOINs que obtenerVentaPorId para traer los nombres derivados.
            // Ordenado por fecha descendente: el historial muestra primero lo mas reciente.
            // El otro metodo parecido es listarVentasConDetallePorUsuario en detalleventadatos.
            string consulta = "SELECT v.idVenta, v.idUsuario, v.fecha, v.total, v.estado, " +
               "v.idLocalidad, v.direccion, v.metodoPago, v.tipoEntrega, " +
               "l.nombreLocalidad, pr.nombreProvincia " +
               "FROM Ventas v " +
               "LEFT JOIN Localidades l ON v.idLocalidad = l.idLocalidad " +
               "LEFT JOIN Provincias pr ON l.idProvincia = pr.idProvincia " +
               "WHERE v.idUsuario = @idUsuario " +
               "ORDER BY v.fecha DESC";

            List<Venta> ventas = new List<Venta>();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    ventas.Add(new Venta()
                    {
                        IdVenta = (int)sqlDataReader["idVenta"],
                        IdUsuario = (int)sqlDataReader["idUsuario"],
                        Fecha = (DateTime)sqlDataReader["fecha"],
                        Total = (decimal)sqlDataReader["total"],
                        Estado = sqlDataReader["estado"].ToString(),
                        TipoEntrega = sqlDataReader["tipoEntrega"].ToString(),
                        MetodoPago = sqlDataReader["metodoPago"].ToString(),
                        IdLocalidad = sqlDataReader["idLocalidad"] == DBNull.Value ? 0 : (int)sqlDataReader["idLocalidad"],
                        Direccion = sqlDataReader["direccion"] == DBNull.Value ? null : sqlDataReader["direccion"].ToString(),
                        NombreLocalidad = sqlDataReader["nombreLocalidad"] == DBNull.Value ? null : sqlDataReader["nombreLocalidad"].ToString(),
                        NombreProvincia = sqlDataReader["nombreProvincia"] == DBNull.Value ? null : sqlDataReader["nombreProvincia"].ToString(),
                        Detalle = new List<DetalleVenta>()
                    });
                }

                return ventas;
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
