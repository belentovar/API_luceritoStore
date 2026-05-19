using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Data
{
    public class favoritosDatos
    {

        public bool existeFavorito(int idUsuario, int idProducto)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "select * from Favoritos where idUsuario=@idUsua and idProducto=@idProd";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@IdUsua", idUsuario);
                sqlCommand.Parameters.AddWithValue("@IdProd", idProducto);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                return reader.Read();
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
           
        }

        public bool agregarFavoritos(int idUsuario, int idProducto)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "insert into Favoritos (IdUsuario, IdProducto) values (@IdUsua, @IdProd)";

            if(existeFavorito(idUsuario, idProducto))
            {
                return false;
            }

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@IdUsua", idUsuario);
                sqlCommand.Parameters.AddWithValue("@IdProd", idProducto);
                
                sqlCommand.ExecuteNonQuery();
                return true;    
            }
            catch(Exception) {
                throw;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
                
        }

        public int eliminarFavorito(int idFavorito)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "DELETE FROM Favoritos WHERE idFavorito = @idFavorito";

            try
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@idFavorito", idFavorito);

                return comando.ExecuteNonQuery();
                
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public List<Producto> listarFavoritosPorUsuario(int idUsuario)
        {
            List<Producto> productos = new List<Producto>();
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "select p.idProducto, p.idCategoria, p.nombreProducto, p.precio, p.detalle, p.imagenProducto, p.stock"
                              +" from Favoritos f INNER JOIN Productos p on f.idProducto = p.idProducto WHERE f.idUsuario = @idUsua";

            try
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@idUsua", idUsuario);

                SqlDataReader sqlDataReader = comando.ExecuteReader();

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
                        Stock = (int)sqlDataReader["stock"]
                    });
                }

                return productos;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
            
        }

    }
}
