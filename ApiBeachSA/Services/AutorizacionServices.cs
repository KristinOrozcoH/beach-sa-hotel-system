using ApiBeachSA.Model;
using APIBeachSA.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiBeachSA.Services
{
    public class AutorizacionServices : IAutorizacionServices
    {
        private readonly BeachContext _context;
        private readonly IConfiguration _configuration;

        public AutorizacionServices(BeachContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AutorizacionResponse> DevolverToken(Usuario usuario)
        {
            var existe = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Username == usuario.Username &&
                    u.Contrasenna == usuario.Contrasenna);

            if (existe == null)
                return null;

            var nombreRol = await _context.Roles
                .Where(r => r.IdRol == existe.IdRol)
                .Select(r => r.NombreRol)
                .FirstOrDefaultAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existe.Username),
                new Claim(ClaimTypes.Role, nombreRol ?? "Desconocido")
            };

            if (existe.IdCliente.HasValue)
            {
                claims.Add(new Claim("IdCliente", existe.IdCliente.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(8);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new AutorizacionResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expira = expiration,
                Rol = nombreRol,
                Nombre = existe.Username
            };
        }
    } //Cierre class 
} //Cierre namespace 