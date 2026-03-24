using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly BeachContext _context;

        public ClientesController(BeachContext context)
        {
            _context = context;
        }

        // Clase auxiliar para registrar cliente + usuario
        public class ClienteRegistroRequest
        {
            public string Cedula { get; set; }
            public string NombreCompleto { get; set; }
            public string Telefono { get; set; }
            public string CorreoElectronico { get; set; }
            public string Direccion { get; set; }
            public int IdTipoCedula { get; set; }
            public string Username { get; set; }
            public string Contrasenna { get; set; }
        }

        [HttpGet("Listar")]
        public List<Cliente> Listar()
        {
            return _context.Clientes.ToList();
        }

        [HttpPost("Guardar")]
        public string Guardar(ClienteRegistroRequest request)
        {
            string msj = $"Cliente {request.NombreCompleto} registrado correctamente";

            try
            {
                // Validar si el username ya existe
                if (_context.Usuarios.Any(u => u.Username == request.Username))
                {
                    return $"Error: El nombre de usuario '{request.Username}' ya está en uso.";
                }

                var nuevoCliente = new Cliente
                {
                    Cedula = request.Cedula,
                    NombreCompleto = request.NombreCompleto,
                    Telefono = request.Telefono,
                    CorreoElectronico = request.CorreoElectronico,
                    Direccion = request.Direccion,
                    IdTipoCedula = request.IdTipoCedula
                };

                _context.Clientes.Add(nuevoCliente);
                _context.SaveChanges();

                var nuevoUsuario = new Usuario
                {
                    Username = request.Username,
                    Contrasenna = request.Contrasenna,
                    IdRol = 2, // Rol por defecto: cliente
                    IdCliente = nuevoCliente.IdCliente
                };

                _context.Usuarios.Add(nuevoUsuario);
                _context.SaveChanges();

                var username = User.Identity?.Name ?? "Anonimo";
                var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                _context.Auditorias.Add(new Auditoria
                {
                    Usuario = username,
                    Rol = rol,
                    Fecha = DateTime.Now,
                    TablaAfectada = "Clientes y Usuarios",
                    Accion = "Guardar",
                    IdRegistro = nuevoCliente.IdCliente,
                    Descripcion = $"Se registró el cliente {nuevoCliente.NombreCompleto} y usuario {nuevoUsuario.Username}"
                });

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpGet("BuscarId")]
        public Cliente BuscarId(int pIdCliente)
        {
            var temp = _context.Clientes.FirstOrDefault(x => x.IdCliente == pIdCliente);

            if (temp == null)
            {
                temp = new Cliente
                {
                    IdCliente = pIdCliente,
                    Cedula = "No existe",
                    NombreCompleto = "N/A",
                    Telefono = "N/A",
                    CorreoElectronico = "N/A",
                    Direccion = "N/A"
                };
            }

            return temp;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Modificar")]
        public async Task<string> Modificar(Cliente pCliente)
        {
            string msj = $"Actualizando cliente {pCliente.NombreCompleto}";

            try
            {
                var temp = _context.Clientes.FirstOrDefault(t => t.IdCliente == pCliente.IdCliente);

                if (temp != null)
                {
                    temp.Cedula = pCliente.Cedula;
                    temp.NombreCompleto = pCliente.NombreCompleto;
                    temp.Telefono = pCliente.Telefono;
                    temp.CorreoElectronico = pCliente.CorreoElectronico;
                    temp.Direccion = pCliente.Direccion;
                    temp.IdTipoCedula = pCliente.IdTipoCedula;

                    _context.Clientes.Update(temp);
                    await _context.SaveChangesAsync();

                    var username = User.Identity?.Name ?? "Anonimo";
                    var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = username,
                        Rol = rol,
                        Fecha = DateTime.Now,
                        TablaAfectada = "Clientes",
                        Accion = "Modificar",
                        IdRegistro = temp.IdCliente,
                        Descripcion = $"Se modificó el cliente {temp.NombreCompleto}"
                    });

                    await _context.SaveChangesAsync();

                    msj = $"Cliente {temp.NombreCompleto} actualizado correctamente";
                }
                else
                {
                    msj = $"No existe el cliente con ID {pCliente.IdCliente}";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int pIdCliente)
        {
            string msj = "";

            try
            {
                // Buscar cliente
                var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == pIdCliente);
                if (cliente == null)
                {
                    return $"No existe cliente con ID {pIdCliente}";
                }

                // Buscar usuario relacionado
                var usuario = _context.Usuarios.FirstOrDefault(u => u.IdCliente == pIdCliente);

                // Eliminar usuario si existe
                if (usuario != null)
                {
                    _context.Usuarios.Remove(usuario);

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = User.Identity?.Name ?? "Anonimo",
                        Rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido",
                        Fecha = DateTime.Now,
                        TablaAfectada = "Usuarios",
                        Accion = "Eliminar",
                        IdRegistro = usuario.IdUsuario,
                        Descripcion = $"Se eliminó el usuario relacionado con el cliente {cliente.NombreCompleto}"
                    });
                }

                // Eliminar cliente
                _context.Clientes.Remove(cliente);

                _context.Auditorias.Add(new Auditoria
                {
                    Usuario = User.Identity?.Name ?? "Anonimo",
                    Rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido",
                    Fecha = DateTime.Now,
                    TablaAfectada = "Clientes",
                    Accion = "Eliminar",
                    IdRegistro = cliente.IdCliente,
                    Descripcion = $"Se eliminó el cliente {cliente.NombreCompleto}"
                });

                await _context.SaveChangesAsync();
                msj = $"Cliente {cliente.NombreCompleto} y su usuario fueron eliminados correctamente";
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpGet("BuscarNombre")]
        public List<Cliente> BuscarNombre(string pNombre)
        {
            return _context.Clientes
                .Where(c => c.NombreCompleto.StartsWith(pNombre))
                .ToList();
        }
    } //Cierre class
} //Cierre namespace 





