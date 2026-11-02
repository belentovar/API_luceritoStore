using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;

namespace API_luceritoStore.Data
{
    public class LocalidadDatos
    {
        private readonly AccesoDatos acceso;

        public LocalidadDatos(AccesoDatos acceso)
        {
            this.acceso = acceso;
        }
        public List<Localidad> listarPorProvincia(int idProvincia)
        {
            string consulta = "SELECT l.idLocalidad, l.idProvincia, l.nombreLocalidad, l.activo, p.nombreProvincia " +
                                "FROM Localidades l " +
                                "INNER JOIN Provincias p ON l.idProvincia = p.idProvincia " +
                                "WHERE l.idProvincia = @idProvincia AND l.activo = 1";

            SqlConnection conexion = acceso.ObtenerConexion();
            List<Localidad> localidades = new List<Localidad>();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idProvincia", idProvincia);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    localidades.Add(new Localidad()
                    {
                        IdLocalidad = (int)reader["idLocalidad"],
                        IdProvincia = (int)reader["idProvincia"],
                        NombreLocalidad = reader["nombreLocalidad"].ToString(),
                        Activo = (bool)reader["activo"],
                        NombreProvincia = reader["nombreProvincia"].ToString()
                    });
                }
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }

            return localidades;
        }

        public Localidad? obtenerLocalidadPorId(int idLocalidad) { //porque el ?
            string consulta = "select * from Localidades where idLocalidad=@IdLocalidad and activo=1";
            SqlConnection conexion = acceso.ObtenerConexion();
            

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.Read()) {
                    Localidad localidad = new Localidad()
                    {
                        IdLocalidad = (int)sqlDataReader["idLocalidad"],
                        IdProvincia = (int)sqlDataReader["idProvincia"],
                        NombreLocalidad = sqlDataReader["nombreLocalidad"].ToString(),
                        Activo = (bool)sqlDataReader["activo"],
    
                    };
                    return localidad;
                }
                return null;
                
            }
            finally {
                acceso.CerrarConexion(conexion);
            }
            
            
        }
        public int darDeBajaLocalidad(int idLocalidad) {
            string consulta = "update Localidades set activo=0 where idLocalidad=@IdLocalidad";
            SqlConnection conexion = acceso.ObtenerConexion();
            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("IdLocalidad", idLocalidad);
                int filas = sqlCommand.ExecuteNonQuery();
                return filas;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public int darDeAltaLocalidad(int idLocalidad)
        {
            string consulta = "update Localidades set activo=1 where idLocalidad=@IdLocalidad";
            SqlConnection conexion = acceso.ObtenerConexion();
            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("IdLocalidad", idLocalidad);
                int filas = sqlCommand.ExecuteNonQuery();
                return filas;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }
        }

        public int agregarLocalidad(Localidad localidad) {
            string consulta = "insert into Localidades (idProvincia, nombreLocalidad) values (@idProv, @nombreLocal)";
            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idProv", localidad.IdProvincia);
                sqlCommand.Parameters.AddWithValue("@nombreLocal", localidad.NombreLocalidad);
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
