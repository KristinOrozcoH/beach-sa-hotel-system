using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservacionesController : ControllerBase
    {
        private readonly BeachContext _context;

        public ReservacionesController(BeachContext pContext)
        {
            _context = pContext;
        }

        [HttpGet("Listar")]
        public List<Reservacion> Listar()
        {
            return _context.Reservaciones.ToList();
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost("Guardar")]
        public string Guardar(Reservacion temp)
        {
            string msj = $"Reservación registrada correctamente";

            try
            {
                var claimIdCliente = User.Claims.FirstOrDefault(c => c.Type == "IdCliente")?.Value;

                if (string.IsNullOrEmpty(claimIdCliente) || temp.IdCliente.ToString() != claimIdCliente)
                    return "No puede registrar reservaciones para otro cliente.";

                var paquete = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == temp.IdPaquete);
                var metodo = _context.MetodosPago.FirstOrDefault(m => m.IdMetodoPago == temp.IdMetodoPago);

                if (paquete == null || metodo == null)
                    return "Error: Paquete o método de pago no encontrado";

                decimal subtotal = paquete.PrecioPorNoche * temp.CantidadNoches;
                decimal descuento = 0;

                if (temp.CantidadNoches >= 13)
                    descuento = subtotal * 0.05m;
                else if (temp.CantidadNoches >= 6)
                    descuento = subtotal * 0.03m;
                else if (temp.CantidadNoches >= 3)
                    descuento = subtotal * 0.01m;

                if (metodo.AplicaDescuento)
                    descuento += subtotal * 0.02m;

                decimal impuesto = (subtotal - descuento) * 0.13m;
                decimal total = subtotal - descuento + impuesto;

                decimal prima = total * paquete.PorcentajePrima;
                decimal mensualidad = (total - prima) / paquete.Mensualidades;
                decimal tipoCambio = 540.00m;

                temp.Prima = Math.Round(prima, 2);
                temp.Mensualidades = Math.Round(mensualidad, 2);
                temp.Descuento = Math.Round(descuento, 2);
                temp.Impuestos = Math.Round(impuesto, 2);
                temp.MontoTotal = Math.Round(total, 2);
                temp.TipoCambio = tipoCambio;
                temp.ValorColones = Math.Round(total, 2);
                temp.ValorDolares = Math.Round(total / tipoCambio, 2);
                temp.PDFEnviado = false;

                _context.Reservaciones.Add(temp);
                _context.SaveChanges();

                // Si requiere detalle de cheque, lo crea con valores por defecto
                if (metodo.RequiereDetalleCheque)
                {
                    var detalle = new DetallePago
                    {
                        IdReservacion = temp.IdReservacion,
                        NumeroCheque = "Por definir",
                        Banco = "Por definir"
                    };
                    _context.DetallesPago.Add(detalle);
                    _context.SaveChanges();
                }

                var usuario = User.Identity?.Name ?? "Anonimo";
                var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                _context.Auditorias.Add(new Auditoria
                {
                    Usuario = usuario,
                    Rol = rol,
                    Fecha = DateTime.Now,
                    TablaAfectada = "Reservaciones",
                    Accion = "Guardar",
                    IdRegistro = temp.IdReservacion,
                    Descripcion = $"Se registró una nueva reservación para el cliente {temp.IdCliente}"
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
        public Reservacion BuscarId(int pIdReservacion)
        {
            var temp = _context.Reservaciones.FirstOrDefault(x => x.IdReservacion == pIdReservacion);

            if (temp == null)
            {
                temp = new Reservacion
                {
                    IdReservacion = pIdReservacion,
                    MontoTotal = 0,
                    ValorColones = 0,
                    ValorDolares = 0
                };
            }

            return temp;
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int pIdReservacion)
        {
            string msj = "";

            try
            {
                var temp = _context.Reservaciones.FirstOrDefault(r => r.IdReservacion == pIdReservacion);

                if (temp == null)
                {
                    msj = $"No existe reservación con ID {pIdReservacion}";
                }
                else
                {
                    _context.Reservaciones.Remove(temp);
                    await _context.SaveChangesAsync();

                    var usuario = User.Identity?.Name ?? "Anonimo";
                    var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = usuario,
                        Rol = rol,
                        Fecha = DateTime.Now,
                        TablaAfectada = "Reservaciones",
                        Accion = "Eliminar",
                        IdRegistro = temp.IdReservacion,
                        Descripcion = $"Se eliminó la reservación con ID {temp.IdReservacion}"
                    });

                    await _context.SaveChangesAsync();

                    msj = $"Reservación eliminada correctamente";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [Authorize]
        [HttpPut("ActualizarEstadoPDF")]
        public async Task<string> ActualizarEstadoPDF(int pIdReservacion)
        {
            string msj = "";

            try
            {
                var temp = _context.Reservaciones.FirstOrDefault(r => r.IdReservacion == pIdReservacion);

                if (temp != null)
                {
                    temp.PDFEnviado = true;
                    _context.Reservaciones.Update(temp);
                    await _context.SaveChangesAsync();

                    var usuario = User.Identity?.Name ?? "Anonimo";
                    var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Desconocido";

                    _context.Auditorias.Add(new Auditoria
                    {
                        Usuario = usuario,
                        Rol = rol,
                        Fecha = DateTime.Now,
                        TablaAfectada = "Reservaciones",
                        Accion = "Modificar",
                        IdRegistro = temp.IdReservacion,
                        Descripcion = $"Se marcó como enviado el PDF para la reservación {temp.IdReservacion}"
                    });

                    await _context.SaveChangesAsync();

                    msj = $"Estado PDF actualizado para reservación {temp.IdReservacion}";
                }
                else
                {
                    msj = $"No existe reservación con ID {pIdReservacion}";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [Authorize]
        [HttpGet("Factura/{pIdReservacion}")]
        public IActionResult VerFactura(int pIdReservacion)
        {
            var temp = _context.Reservaciones.FirstOrDefault(x => x.IdReservacion == pIdReservacion);

            if (temp == null)
                return NotFound($"No existe reservación con ID {pIdReservacion}");

            var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
            var idClienteClaim = User.Claims.FirstOrDefault(c => c.Type == "IdCliente")?.Value;

            // Si es cliente, solo puede ver su propia factura
            if (rol == "Cliente" && (string.IsNullOrEmpty(idClienteClaim) || temp.IdCliente.ToString() != idClienteClaim))
            {
                return StatusCode(403, "No tiene permiso para ver esta factura.");
            }

            // Buscar si existe detalle de pago para esta reservación
            var detalleCheque = _context.DetallesPago
                .Where(d => d.IdReservacion == pIdReservacion)
                .Select(d => new
                {
                    d.NumeroCheque,
                    d.Banco
                })
                .FirstOrDefault();

            var factura = new
            {
                temp.IdReservacion,
                temp.IdCliente,
                temp.IdPaquete,
                temp.IdMetodoPago,
                temp.CantidadNoches,
                temp.Mensualidades,
                temp.Prima,
                temp.Descuento,
                temp.Impuestos,
                temp.MontoTotal,
                temp.TipoCambio,
                temp.ValorColones,
                temp.ValorDolares,
                temp.PDFEnviado,
                DetalleCheque = detalleCheque // se incluye solo si existe
            };

            return Ok(factura);
        }
    } //Cierre class 
} //Cierre namespace
