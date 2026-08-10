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
                    Codigo = sqlDataReader["codigo"].ToString(),
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


            string consulta = "SELECT * FROM Productos where activo=1";
            
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
                        Codigo = sqlDataReader["codigo"].ToString(),
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

            string consulta = "select * from Productos where idCategoria = @idCat and activo = 1";

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
            string consulta = "select * from Productos where (nombreProducto like @p1 or " +
                              "nombreProducto like @p2 or " +
                              "nombreProducto like @p3) and activo = 1";
            
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

        public int darDeBajaProducto(int idProduc) /// ------------------> BORRAR PRODUCTO POR ID
        {
            string consulta = "update Productos set activo = 0 where idProducto = @idProduc";
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

        public int darDeALtaProducto(int idProduc) 
        {
            string consulta = "update Productos set activo = 1 where idProducto = @idProduc";
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idProduc", idProduc);
                int filasAfectadas = sqlCommand.ExecuteNonQuery();

                return filasAfectadas;
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

        public int actualizarProducto(Producto producto) /// -------- ACTUALIZAR PRODUCTO
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = @"UPDATE Productos
                        SET idCategoria = @idCategoria,
                            codigo = @codigo,
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
                sqlCommand.Parameters.AddWithValue("@codigo", producto.Codigo);
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


        public int AgregarProducto(Producto producto) /// ------------------> AGREGAR PRODUCTO
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            // activo se fija en 1: un producto recien creado siempre nace activo.
            string consulta = @"INSERT INTO Productos (idCategoria, codigo, nombreProducto, precio, detalle, imagenProducto, stock, activo)
                        VALUES (@idCategoria, @codigo, @nombre, @precio, @detalle, @imagen, @stock, 1)";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idCategoria", producto.IdCategoria);
                sqlCommand.Parameters.AddWithValue("@codigo", producto.Codigo);
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


        public int descontarStock(int idProducto, int cantidad, SqlConnection conn, SqlTransaction transaccion) /// -------- DESCONTAR STOCK (ATOMICO)
        {
            // UPDATE guardado: solo descuenta si hay stock suficiente (WHERE stock >= @cantidad).
            // Corre dentro de la transaccion de la venta. Devuelve las filas afectadas: si es 0,
            // no habia stock suficiente (o no existe el producto) y la venta se debe abortar.
            // Asi la verificacion y el descuento son una sola operacion: no hay sobreventa.
            string consulta = @"UPDATE Productos SET stock = stock - @cantidad
                        WHERE idProducto = @idProducto AND stock >= @cantidad";

            SqlCommand cmd = new SqlCommand(consulta, conn, transaccion);
            cmd.Parameters.AddWithValue("@cantidad", cantidad);
            cmd.Parameters.AddWithValue("@idProducto", idProducto);

            return cmd.ExecuteNonQuery();
        }

        // Se busca por codigo y no por nombre: el codigo es la clave unica del producto.
        // Dos productos distintos pueden llamarse igual (dos ediciones de un libro, dos
        // remeras negras de marcas distintas), asi que el nombre no sirve para detectar
        // duplicados. La garantia real la da el UNIQUE de la columna codigo; esto solo
        // evita el viaje al INSERT y devuelve un mensaje entendible.
        public bool ExisteProducto(string codigo)
        {
            string consulta = "SELECT TOP 1 1 FROM Productos where codigo=@codigo";

            AccesoDatos accesoDatos = new AccesoDatos();
            SqlConnection conexion = accesoDatos.ObtenerConexion();
            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@codigo", codigo);
                object existe = sqlCommand.ExecuteScalar();
                return existe != null;
            }
            finally
            {
                accesoDatos.CerrarConexion(conexion);
            }
        }
    }
}
