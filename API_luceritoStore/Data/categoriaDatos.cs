using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;

namespace API_luceritoStore.Data
{
    public class categoriaDatos
    {
        private readonly AccesoDatos acceso;

        public categoriaDatos(AccesoDatos acceso)
        {
            this.acceso = acceso;
        }

        public bool ExisteCategoria(int id)
        {
            string consulta = "SELECT TOP 1 1 FROM CategoriaProductos WHERE idCategoria = @id";

            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@id", id);

                object resultado = sqlCommand.ExecuteScalar();
                return resultado != null;
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

        public List<CategoriaProducto> listarCategorias()
        {
            string consulta = "SELECT idCategoria, nombreCategoria, activo FROM CategoriaProductos WHERE activo = 1";
            SqlConnection conexion = acceso.ObtenerConexion();
            List<CategoriaProducto> categorias = new List<CategoriaProducto>();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    categorias.Add(new CategoriaProducto()
                    {
                        IdCategoria = (int)reader["idCategoria"],
                        NombreCategoria = reader["nombreCategoria"].ToString(),
                        Activo = (bool)reader["activo"]
                    });
                }
                return categorias;
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

        public int AgregarCategoria(CategoriaProducto categoria)
        {
            // activo se fija en 1: una categoria recien creada siempre nace activa.
            string consulta = "INSERT INTO CategoriaProductos (nombreCategoria, activo) VALUES (@nombre, 1)";
            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@nombre", categoria.NombreCategoria);
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

        public int darDeBajaCategoria(int idCategoria)
        {
            string consulta = "UPDATE CategoriaProductos SET activo = 0 WHERE idCategoria = @id";
            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@id", idCategoria);
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

        public int darDeAltaCategoria(int idCategoria)
        {
            string consulta = "UPDATE CategoriaProductos SET activo = 1 WHERE idCategoria = @id";
            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@id", idCategoria);
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
