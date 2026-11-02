using API_luceritoStore.Data;
using API_luceritoStore.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Writers;

namespace API_luceritoStore.Services
{
    public class UsuarioService
    {

        private readonly IConfiguration _configuration;
        private readonly UsuariosDatos usuariosDatos;
        public UsuarioService(IConfiguration configuration, UsuariosDatos usuariosDatos)
        {
            _configuration = configuration;
            this.usuariosDatos = usuariosDatos;
        }

        public int AgregarUsuario(Usuario usuario)
        {
            
            if(usuario.NombreUsuario == null || usuario.ClaveUsuario == null || usuario.Correo == null ){
                    return -1; //Datos incompletos
            }

            Usuario user = usuariosDatos.ObtenerUsuarioPorNombre(usuario.NombreUsuario);

            if (user != null)
            {
                return -2; // El usuario ya existe
            }

            if(usuariosDatos.ExisteCorreo(usuario.Correo)) {

                return -3; //ya existe ese correo
            }

            if (usuario.ClaveUsuario.Length < 8)
            {
                return -4; // Clave demasiado corta
            }

            return usuariosDatos.AgregarUsuario(usuario);
        }

        public string Login(Usuario usuario)
        {

            if (usuario.NombreUsuario == null || usuario.ClaveUsuario == null)
            {
                return null; //Datos incompletos
            }

            Usuario user = usuariosDatos.ObtenerUsuarioPorLogin(usuario.NombreUsuario, usuario.ClaveUsuario); 

            if(user == null)
            {
                return null;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, user.NombreUsuario.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }
}
