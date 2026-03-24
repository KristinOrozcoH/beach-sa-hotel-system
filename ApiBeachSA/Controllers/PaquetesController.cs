using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaquetesController : ControllerBase
    {
        private readonly BeachContext _context;

        public PaquetesController(BeachContext pContext)
        {
            _context = pContext;
        }

        [HttpGet("Listar")]
        public List<Paquete> Listar()
        {
            return _context.Paquetes.ToList();
        }

        [Authorize]
        [HttpPost("Guardar")]
        public string Guardar(Paquete temp)
        {
            string msj = $"Paquete {temp.NombrePaquete} registrado correctamente";

            try
            {
                _context.Paquetes.Add(temp);
                _context.SaveChanges();

                var usuario = User.Identity?.Name ?? "Anonimo";
                var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                _context.Auditorias.Add(new Auditoria
                {
                    Usuario = usuario,
                    Rol = rol,
                    Fecha = DateTime.Now,
                    TablaAfectada = "Paquetes",
                    Accion = "Guardar",
                    IdRegistro = temp.IdPaquete,
                    Descripcion = $"Se registró el paquete {temp.NombrePaquete}"
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
        public Paquete BuscarId(int pIdPaquete)
        {
            var temp = _context.Paquetes.FirstOrDefault(x => x.IdPaquete == pIdPaquete);

            if (temp == null)
            {
                temp = new Paquete
                {
                    IdPaquete = pIdPaquete,
                    NombrePaquete = "No existe",
                    Destino = "N/A",
                    FechaInicio = null,
                    FechaFin = null,
                    PrecioPorNoche = 0,
                    PorcentajePrima = 0,
                    Mensualidades = 0
                };
            }

            return temp;
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int pIdPaquete)
        {
            string msj = "";

            try
            {
                var temp = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == pIdPaquete);

                if (temp == null)
                {
                    msj = $"No existe paquete con ID {pIdPaquete}";
                }
                else
                {
                    _context.Paquetes.Remove(temp);
                    await _context.SaveChangesAsync();

                    var usuario = User.Identity?.Name ?? "Anonimo";
                    var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = usuario,
                        Rol = rol,
                        Fecha = DateTime.Now,
                        TablaAfectada = "Paquetes",
                        Accion = "Eliminar",
                        IdRegistro = temp.IdPaquete,
                        Descripcion = $"Se eliminó el paquete {temp.NombrePaquete}"
                    });

                    await _context.SaveChangesAsync();

                    msj = $"Paquete {temp.NombrePaquete} eliminado correctamente";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<string> Modificar(Paquete pPaquete)
        {
            string msj = $"Actualizando paquete {pPaquete.NombrePaquete}";

            try
            {
                var temp = _context.Paquetes.FirstOrDefault(t => t.IdPaquete == pPaquete.IdPaquete);

                if (temp != null)
                {
                    temp.NombrePaquete = pPaquete.NombrePaquete;
                    temp.Destino = pPaquete.Destino;
                    temp.FechaInicio = pPaquete.FechaInicio;
                    temp.FechaFin = pPaquete.FechaFin;
                    temp.PrecioPorNoche = pPaquete.PrecioPorNoche;
                    temp.PorcentajePrima = pPaquete.PorcentajePrima;
                    temp.Mensualidades = pPaquete.Mensualidades;

                    _context.Paquetes.Update(temp);
                    await _context.SaveChangesAsync();

                    var usuario = User.Identity?.Name ?? "Anonimo";
                    var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = usuario,
                        Rol = rol,
                        Fecha = DateTime.Now,
                        TablaAfectada = "Paquetes",
                        Accion = "Modificar",
                        IdRegistro = temp.IdPaquete,
                        Descripcion = $"Se modificó el paquete {temp.NombrePaquete}"
                    });

                    await _context.SaveChangesAsync();

                    msj = $"Paquete {temp.NombrePaquete} actualizado correctamente";
                }
                else
                {
                    msj = $"No existe el paquete con ID {pPaquete.IdPaquete}";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpGet("BuscarNombre")]
        public List<Paquete> BuscarNombre(string pNombre)
        {
            return _context.Paquetes
                .Where(p => p.NombrePaquete.StartsWith(pNombre))
                .ToList();
        }
    } //Cierre class 
} //Cierre namespace 



