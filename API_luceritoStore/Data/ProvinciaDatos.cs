using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;

namespace API_luceritoStore.Data
{
    public class ProvinciaDatos
    {
        private readonly AccesoDatos acceso;

        public ProvinciaDatos(AccesoDatos acceso)
        {
            this.acceso = acceso;
        }
        public List<Provincia> ListarProvincias()
        {
            string consulta = "select idProvincia, nombreProvincia, activo from Provincias where activo=1";

            SqlConnection conexion = acceso.ObtenerConexion();
            List<Provincia> provincias = new List<Provincia>();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read()) {
                    provincias.Add(new Provincia()
                    {
                        IdProvincia = (int)reader["idProvincia"],
                        NombreProvincia = reader["nombreProvincia"].ToString()
                    });
                }   

            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }

            return provincias;

        }

        public int darDeBajaProvincia(int idProvincia)
        {
            string consulta = "update Provincias set activo=0 where idProvincia = @IdProvincia";

            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@IdProvincia", idProvincia);
                int filas = sqlCommand.ExecuteNonQuery();
                return filas;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public int darDeAltaProvincia(int idProvincia)
        {
            string consulta = "update Provincias set activo=1 where idProvincia = @IdProvincia";

            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@IdProvincia", idProvincia);
                int filas = sqlCommand.ExecuteNonQuery();
                return filas;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public int agregarProvincia(Provincia provincia)
        {
            string consulta = "insert into Provincias (nombreProvincia) values (@nombreProv)";

            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@nombreProv", provincia.NombreProvincia);
                int filas = sqlCommand.ExecuteNonQuery();
                return filas;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }
    }
}
