using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiBeachSA.Model;
using ApiBeachSA.Services;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly BeachContext _context;
        private readonly IAutorizacionServices _autorizacionServices;

        public UsuariosController(BeachContext context, IAutorizacionServices autorizacionServices)
        {
            _context = context;
            _autorizacionServices = autorizacionServices;
        }

        [HttpPost("Autenticar")]
        public async Task<IActionResult> Autenticar(Usuario usuario)
        {
            var autorizado = await _autorizacionServices.DevolverToken(usuario);
            return autorizado == null ? Unauthorized() : Ok(autorizado);
        }

        [HttpGet("Listar")]
        public List<Usuario> Listar()
        {
            return _context.Usuarios.ToList();
        }


        [HttpGet("BuscarId")]
        public Usuario BuscarId(int pIdUsuario)
        {
            var temp = _context.Usuarios.FirstOrDefault(x => x.IdUsuario == pIdUsuario);

            if (temp == null)
            {
                temp = new Usuario
                {
                    IdUsuario = pIdUsuario,
                    Username = "No existe",
                    Contrasenna = "N/A"
                };
            }

            return temp;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int pIdUsuario)
        {
            string msj = "";

            try
            {
                var temp = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == pIdUsuario);

                if (temp == null)
                {
                    msj = $"No existe usuario con ID {pIdUsuario}";
                }
                else
                {
                    _context.Usuarios.Remove(temp);
                    await _context.SaveChangesAsync();

                    var username = User.Identity?.Name ?? "Anonimo";
                    var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = username,
                        Rol = rol,
                        Fecha = DateTime.Now,
                        TablaAfectada = "Usuarios",
                        Accion = "Eliminar",
                        IdRegistro = temp.IdUsuario,
                        Descripcion = $"Se eliminó el usuario {temp.Username}"
                    });

                    await _context.SaveChangesAsync();
                    msj = $"Usuario {temp.Username} eliminado correctamente";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Modificar")]
        public async Task<string> Modificar(Usuario pUsuario)
        {
            string msj = $"Actualizando usuario {pUsuario.Username}";

            try
            {
                var temp = _context.Usuarios.FirstOrDefault(t => t.IdUsuario == pUsuario.IdUsuario);

                if (temp != null)
                {
                    temp.Username = pUsuario.Username;
                    temp.Contrasenna = pUsuario.Contrasenna;
                    temp.IdRol = pUsuario.IdRol;
                    temp.IdCliente = pUsuario.IdCliente;

                    _context.Usuarios.Update(temp);
                    await _context.SaveChangesAsync();

                    var username = User.Identity?.Name ?? "Anonimo";
                    var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = username,
                        Rol = rol,
                        Fecha = DateTime.Now,
                        TablaAfectada = "Usuarios",
                        Accion = "Modificar",
                        IdRegistro = temp.IdUsuario,
                        Descripcion = $"Se modificó el usuario {temp.Username}"
                    });

                    await _context.SaveChangesAsync();
                    msj = $"Usuario {temp.Username} actualizado correctamente";
                }
                else
                {
                    msj = $"No existe el usuario con ID {pUsuario.IdUsuario}";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpGet("BuscarUsername")]
        public List<Usuario> BuscarUsername(string pUsername)
        {
            return _context.Usuarios
                .Where(u => u.Username.StartsWith(pUsername))
                .ToList();
        }
    } //Cierre class
} //Cierre namespace 