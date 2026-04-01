using API_luceritoStore.Models;
using BCrypt.Net;
using Microsoft.Data.SqlClient;

namespace API_luceritoStore.Data
{
    public class UsuariosDatos
    {

        public int AgregarUsuario(Usuario usuario)
        {
            string claveHasheada = BCrypt.Net.BCrypt.HashPassword(usuario.ClaveUsuario); /// SE ENCRIPTA LA CLAVE 
                                                                                         /// PARA RESPETAR LA PRIVACIDAD DEL USUARIO
            AccesoDatos conexion = new AccesoDatos();

            string consulta = "insert into Usuarios (nombreUsuario, claveUsuario, correo) " +
                              "values (@nombreU, @claveU, @correoU)";

            try{
                
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
                sqlCommand.Parameters.AddWithValue("@nombreU", usuario.NombreUsuario);
                sqlCommand.Parameters.AddWithValue("@claveU", claveHasheada);
                sqlCommand.Parameters.AddWithValue("@correoU", usuario.Correo);
                int filasAfectadas = sqlCommand.ExecuteNonQuery();
                return filasAfectadas;
            }
            catch (Exception) {
                throw;
            }
            finally{
                conexion.CerrarConexion();
            }
            
        }

        public int eliminarUsuario(int idUsuario)
        {
            AccesoDatos conexion = new AccesoDatos();
            string consulta = "delete from Usuarios where idUsuario = @idUsua";


            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
                sqlCommand.Parameters.AddWithValue("@idUsua", idUsuario);
                int filasAfectadas = sqlCommand.ExecuteNonQuery();
                return filasAfectadas;
            }
            catch(Exception)
            { 
                throw; 
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        public Usuario obtenerUsuarioPorId(int idUsuario)
        {
            AccesoDatos conexion = new AccesoDatos();
            Usuario usuario = new Usuario();
            string consulta = "select * from Usuarios where idUsuario = @idUsua";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
                sqlCommand.Parameters.AddWithValue("@idUsua", idUsuario);
                SqlDataReader sdr = sqlCommand.ExecuteReader();

                while (sdr.Read())
                {
                    usuario = new Usuario()
                    {
                        IdUsuario = (int)sdr["idUsuario"],
                        NombreUsuario = sdr["nombreUsuario"].ToString(),
                        ClaveUsuario = sdr["claveUsuario"].ToString(),
                        Correo = sdr["correo"].ToString()
                    };
                }
                return usuario;
            }
            catch (Exception) {
                throw;
            }
            finally
            {
                conexion.CerrarConexion();
            }

        }


        public Usuario ObtenerUsuarioPorLogin(string nombreUsuario, string claveUsuario)
        {
            string consulta = "select idUsuario, nombreUsuario, claveUsuario, correo from Usuarios where nombreUsuario = @nombreU";
            AccesoDatos conexion = new AccesoDatos();
            Usuario usuario = new Usuario();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion.ObtenerConexion());
                sqlCommand.Parameters.AddWithValue("@nombreU", nombreUsuario);

                SqlDataReader sdr = sqlCommand.ExecuteReader();

                if (sdr.Read())
                {
                    string hashGuardado = sdr["claveUsuario"].ToString();

                    bool claveCorrecta = BCrypt.Net.BCrypt.Verify(claveUsuario, hashGuardado);

                    if(claveCorrecta) {

                        usuario = new Usuario()
                        {
                            IdUsuario = (int)sdr["idUsuario"],
                            NombreUsuario = sdr["nombreUsuario"].ToString(),
                            Correo = sdr["correo"].ToString()
                        };
                    }
                    
                }
                return usuario;
            }
            catch(Exception) {
                throw;
            }
            finally
            {
                conexion.CerrarConexion();
            } 
        }


    }
}
