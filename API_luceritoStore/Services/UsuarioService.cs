using API_luceritoStore.Data;
using API_luceritoStore.Models;
using API_luceritoStore.Models.Errores;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El nombre de usuario, la clave y el correo son obligatorios.");
            }

            Usuario user = usuariosDatos.ObtenerUsuarioPorNombre(usuario.NombreUsuario);

            if (user != null)
            {
                throw new NegocioException(ErrorNegocio.Duplicado, "Ya existe un usuario con ese nombre.");
            }

            if(usuariosDatos.ExisteCorreo(usuario.Correo)) {

                throw new NegocioException(ErrorNegocio.Duplicado, "Ya existe un usuario con ese correo.");
            }

            if (usuario.ClaveUsuario.Length < 8)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "La clave debe tener al menos 8 caracteres.");
            }

            return usuariosDatos.AgregarUsuario(usuario);
        }

        public string Login(Usuario usuario)
        {

            if (usuario.NombreUsuario == null || usuario.ClaveUsuario == null)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El nombre de usuario y la clave son obligatorios.");
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
