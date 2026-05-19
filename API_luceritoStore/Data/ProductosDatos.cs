using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace API_luceritoStore.Data
{
    public class ProductosDatos
    {

        public List<Producto> listaDeProductos(SqlCommand sqlCommand) /// LISTA DE PRODUCTOS PARA NO REPETIR CODIGO 
        {
            List<Producto> productos = new List<Producto>();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                productos.Add(new Producto()
                {
                    IdProducto = (int)sqlDataReader["idProducto"],
                    IdCategoria = (int)sqlDataReader["idCategoria"],
                    NombreProducto = sqlDataReader["nombreProducto"].ToString(),
                    Precio = (decimal)sqlDataReader["precio"],
                    Detalle = sqlDataReader["detalle"].ToString(),
                    ImagenProducto = sqlDataReader["imagenProducto"].ToString(),
                    Stock = (int)sqlDataReader["stock"],
                });
            }

            return productos;
        }


        public List<Producto> listarProductos() /// ---------------------------> LISTAR PRODUCTOS
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();


            string consulta = "SELECT * FROM Productos";
            
            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);              
                return listaDeProductos(sqlCommand);

            }
            catch (Exception) {
                throw;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public Producto? obtenerProductoPorId(int idProducto) /// --------> OBTENER PRODUCTO POR ID
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "SELECT * FROM Productos WHERE idProducto = @idProd";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idProd", idProducto);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.Read())
                {
                    Producto producto = new Producto()
                    {
                        IdProducto = (int)sqlDataReader["idProducto"],
                        IdCategoria = (int)sqlDataReader["idCategoria"],
                        NombreProducto = sqlDataReader["nombreProducto"].ToString(),
                        Precio = (decimal)sqlDataReader["precio"],
                        Detalle = sqlDataReader["detalle"].ToString(),
                        ImagenProducto = sqlDataReader["imagenProducto"].ToString(),
                        Stock = (int)sqlDataReader["stock"],
                    };
                    return producto;
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


        public List<Producto> listarProductosPorCategoria(int idCategoria)/// ------------------> LISTAR PRODUCTOS POR CATEGORIA
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "select * from Productos where idCategoria = @idCat";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idCat", idCategoria);
                
                return listaDeProductos(sqlCommand); ;
            }
            catch (Exception) {
                throw;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public List<Producto> listarProductosPorNombre(string nombreProduc)/// ------------------> LISTAR PRODUCTOS POR NOMBRE
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            string consulta = "select * from Productos where nombreProducto like @p1 or " +
                              "nombreProducto like @p2 or " +
                              "nombreProducto like @p3";
            
            try{

                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@p1", "%" + nombreProduc);
                sqlCommand.Parameters.AddWithValue("@p2", "%" + nombreProduc + "%");
                sqlCommand.Parameters.AddWithValue("@p3", nombreProduc + "%");

                return listaDeProductos(sqlCommand);
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

        public int eliminarProducto(int idProduc) /// ------------------> BORRAR PRODUCTO POR ID
        {
            string consulta = "delete from Productos where idProducto = @idProduc";
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idProduc", idProduc);
                int filasAfectadas = sqlCommand.ExecuteNonQuery();

                return filasAfectadas;
            }
            catch (Exception) {
                throw;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public int actualizarProducto(Producto producto) /// -------- ACTUALIZAR PRODUCTO
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = @"UPDATE Productos 
                        SET idCategoria = @idCategoria,
                            nombreProducto = @nombre,
                            precio = @precio,
                            detalle = @detalle,
                            imagenProducto = @imagen,
                            stock = @stock
                        WHERE idProducto = @idProd";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);

                sqlCommand.Parameters.AddWithValue("@idProd", producto.IdProducto);
                sqlCommand.Parameters.AddWithValue("@idCategoria", producto.IdCategoria);
                sqlCommand.Parameters.AddWithValue("@nombre", producto.NombreProducto);
                sqlCommand.Parameters.AddWithValue("@precio", producto.Precio);
                sqlCommand.Parameters.AddWithValue("@detalle", producto.Detalle);
                sqlCommand.Parameters.AddWithValue("@imagen", producto.ImagenProducto);
                sqlCommand.Parameters.AddWithValue("@stock", producto.Stock);

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


    }
}
