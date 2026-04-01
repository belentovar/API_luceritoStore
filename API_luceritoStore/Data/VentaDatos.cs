using API_luceritoStore.Models;
using API_luceritoStore.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;
namespace API_luceritoStore.Data
{
    public class VentaDatos
    {
        public int insertarVenta(Venta venta)
        {
            AccesoDatos conexion = new AccesoDatos();
            SqlConnection conn = conexion.ObtenerConexion();

            SqlTransaction transaccion = conn.BeginTransaction();

            try
            {
                string consulta = @"INSERT INTO Ventas (idUsuario, fecha, total, estado)
                                   VALUES (@idUsuario, @fecha, @total, @estado);
                                   SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(consulta, conn, transaccion);

                cmd.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
                cmd.Parameters.AddWithValue("@fecha", venta.Fecha);
                cmd.Parameters.AddWithValue("@total", venta.Total);
                cmd.Parameters.AddWithValue("@estado", venta.Estado);

                int idGenerado = Convert.ToInt32(cmd.ExecuteScalar());

                venta.IdVenta = idGenerado;

                DetalleVentaDatos detalleDatos = new DetalleVentaDatos();

                foreach (DetalleVenta detalle in venta.Detalle)
                {
                    detalleDatos.insertarDetalle(idGenerado, detalle, conn, transaccion);
                }

                transaccion.Commit();

                return idGenerado;
            }
            catch
            {
                transaccion.Rollback();
                throw;
            }
        }

        public int actualizarEstadoVenta(int IdVenta, string Estado)
        {
            AccesoDatos conexion = new AccesoDatos();
            string consulta = "update Ventas set estado = @estado where idVenta = @idVenta";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
            sqlCommand.Parameters.AddWithValue("@estado", Estado);
            sqlCommand.Parameters.AddWithValue("@idVenta", IdVenta);

            conexion.CerrarConexion();
            return sqlCommand.ExecuteNonQuery();

        }

        public Venta obtenerVentaPorId(int idVenta)
        {
            AccesoDatos conexion = new AccesoDatos();
            Venta venta = new Venta(); // o Venta venta = null; ?
            string consulta = "select idVenta, idUsuario, fecha, total, estado from Ventas where idVenta = @idventa";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
            sqlCommand.Parameters.AddWithValue("@idventa", idVenta);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            if(sqlDataReader.Read()) {

                venta = new Venta()
                {
                    IdVenta = (int)sqlDataReader["idVenta"],
                    IdUsuario = (int)sqlDataReader["idUsuario"],
                    Fecha = (DateTime)sqlDataReader["fecha"],
                    Total = (decimal)sqlDataReader["total"],
                    Estado = sqlDataReader["estado"].ToString(),
                    Detalle = new List<DetalleVenta>()
                };
            }
            
            conexion.CerrarConexion();
            return venta;
        }
    }
}
