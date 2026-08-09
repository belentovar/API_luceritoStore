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
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string rol = string.IsNullOrWhiteSpace(usuario.Rol) ? "cliente" : usuario.Rol.ToLower();

            string consulta = "insert into Usuarios (nombreUsuario, claveUsuario, correo, rol) " +
                              "values (@nombreU, @claveU, @correoU, @rol)";

            try{

                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@nombreU", usuario.NombreUsuario);
                sqlCommand.Parameters.AddWithValue("@claveU", claveHasheada);
                sqlCommand.Parameters.AddWithValue("@correoU", usuario.Correo);
                sqlCommand.Parameters.AddWithValue("@rol", rol);
                int filasAfectadas = sqlCommand.ExecuteNonQuery();
                return filasAfectadas;
            }
            catch (Exception) {
                throw;
            }
            finally{
                acceso.CerrarConexion(conexion);
            }
            
        }

        public int DarDeBajaUsuario(int idUsuario)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            string consulta = "update Usuarios set activo = 0 where idUsuario = @idUsua";


            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
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
                acceso.CerrarConexion(conexion);
            }
        }

        public int DarDeAltaUsuario(int idUsuario)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            string consulta = "update Usuarios set activo = 1 where idUsuario = @idUsua";


            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idUsua", idUsuario);
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

        public Usuario? obtenerUsuarioPorId(int idUsuario)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "select * from Usuarios where idUsuario = @idUsua";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@idUsua", idUsuario);
                SqlDataReader sdr = sqlCommand.ExecuteReader();

                if (sdr.Read())
                {
                    Usuario usuario = new Usuario()
                    {
                        IdUsuario = (int)sdr["idUsuario"],
                        NombreUsuario = sdr["nombreUsuario"].ToString(),
                        ClaveUsuario = sdr["claveUsuario"].ToString(),
                        Correo = sdr["correo"].ToString(),
                        Rol = sdr["rol"].ToString(),
                        Activo = (bool)sdr["activo"]
                    };
                    return usuario;
                }
                return null;
            }
            catch (Exception) {
                throw;
            }
            finally
            {
                acceso.CerrarConexion(conexion);
            }

        }

        public Usuario ObtenerUsuarioPorNombre(string nombreUsuario)
        {
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();

            string consulta = "SELECT * FROM Usuarios WHERE nombreUsuario = @nombreU";

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@nombreU", nombreUsuario);

                SqlDataReader sdr = sqlCommand.ExecuteReader();

                if (sdr.Read())
                {
                    return new Usuario()
                    {
                        IdUsuario = (int)sdr["idUsuario"],
                        NombreUsuario = sdr["nombreUsuario"].ToString(),
                        Correo = sdr["correo"].ToString(),
                        Rol = sdr["rol"].ToString(),
                        Activo = (bool)sdr["activo"]
                    };
                    // ClaveUsuario se deja sin mapear a proposito: este metodo se usa para saber si
                    // el usuario existe, y para eso no hace falta cargar el hash de la clave en memoria.
                    // El unico que necesita el hash es ObtenerUsuarioPorLogin, para verificarlo.
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

        public Usuario ObtenerUsuarioPorLogin(string nombreUsuario, string claveUsuario)
        {
            string consulta = "select idUsuario, nombreUsuario, claveUsuario, correo from Usuarios where nombreUsuario = @nombreU and activo = 1";
            AccesoDatos acceso = new AccesoDatos();
            SqlConnection conexion = acceso.ObtenerConexion();
            Usuario usuario = null;

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
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
                acceso.CerrarConexion(conexion);
            } 
        }

        public bool ExisteCorreo(string correo)
        {
            AccesoDatos acceso = new AccesoDatos();
            string consulta = "SELECT TOP 1 1 FROM Usuarios WHERE correo = @correoUsuario";

            //El primer 1 Le dice a la base de datos: "Detente en cuanto encuentres la primera coincidencia".
            //El segundo, le decimos a SQL: "No me traigas datos reales (strings, fechas, etc.), solo devuélveme el número 1".

            SqlConnection conexion = acceso.ObtenerConexion();

            try
            {
                SqlCommand sqlCommand = new SqlCommand(consulta, conexion);
                sqlCommand.Parameters.AddWithValue("@correoUsuario", correo);
               
                object resultado = sqlCommand.ExecuteScalar();

                return resultado != null; //si es 1
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
