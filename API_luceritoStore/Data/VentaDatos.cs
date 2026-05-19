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
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            SqlTransaction transaccion = conexion.BeginTransaction();

            try
            {
                string consulta = @"INSERT INTO Ventas (idUsuario, fecha, total, estado)
                                   VALUES (@idUsuario, @fecha, @total, @estado);
                                   SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(consulta, conexion, transaccion);

                cmd.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
                cmd.Parameters.AddWithValue("@fecha", venta.Fecha);
                cmd.Parameters.AddWithValue("@total", venta.Total);
                cmd.Parameters.AddWithValue("@estado", venta.Estado);

                int idGenerado = Convert.ToInt32(cmd.ExecuteScalar());

                venta.IdVenta = idGenerado;

                DetalleVentaDatos detalleDatos = new DetalleVentaDatos();

                foreach (DetalleVenta detalle in venta.Detalle)
                {
                    detalleDatos.insertarDetalle(idGenerado, detalle, conexion, transaccion);
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

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
            sqlCommand.Parameters.AddWithValue("@estado", Estado);
            sqlCommand.Parameters.AddWithValue("@idVenta", IdVenta);

            int resultado = sqlCommand.ExecuteNonQuery();

            acceso.CerrarConexion(conexion);

            return resultado;

        }

        public Venta? obtenerVentaPorId(int idVenta)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            string consulta = "select idVenta, idUsuario, fecha, total, estado from Ventas where idVenta = @idventa";

            SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
            sqlCommand.Parameters.AddWithValue("@idventa", idVenta);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            if(sqlDataReader.Read()) {

                Venta venta = new Venta()
                {
                    IdVenta = (int)sqlDataReader["idVenta"],
                    IdUsuario = (int)sqlDataReader["idUsuario"],
                    Fecha = (DateTime)sqlDataReader["fecha"],
                    Total = (decimal)sqlDataReader["total"],
                    Estado = sqlDataReader["estado"].ToString(),
                    Detalle = new List<DetalleVenta>()
                };
                return venta;
            }

            acceso.CerrarConexion(conexion);
            return null;
        }
    }
}
