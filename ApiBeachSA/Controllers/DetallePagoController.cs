using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DetallePagoController : ControllerBase
    {
        private readonly BeachContext _context = null;

        public DetallePagoController(BeachContext pContext)
        {
            _context = pContext;
        }

        [HttpGet("Listar")]
        public List<DetallePago> Listar()
        {
            return _context.DetallesPago.ToList();
        }

        [Authorize]
        [HttpPost("Guardar")]
        public string Guardar(DetallePago temp)
        {
            string msj = $"Detalle de pago registrado correctamente";

            try
            {
                _context.DetallesPago.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpGet("BuscarId")]
        public DetallePago BuscarId(int pIdDetallePago)
        {
            var temp = _context.DetallesPago.FirstOrDefault(x => x.IdDetallePago == pIdDetallePago);

            if (temp == null)
            {
                temp = new DetallePago
                {
                    IdDetallePago = pIdDetallePago,
                    NumeroCheque = "No existe",
                    Banco = "N/A"
                };
            }

            return temp;
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int pIdDetallePago)
        {
            string msj = "";

            try
            {
                var temp = _context.DetallesPago.FirstOrDefault(d => d.IdDetallePago == pIdDetallePago);

                if (temp == null)
                {
                    msj = $"No existe detalle de pago con ID {pIdDetallePago}";
                }
                else
                {
                    _context.DetallesPago.Remove(temp);
                    await _context.SaveChangesAsync();
                    msj = $"Detalle de pago eliminado correctamente";
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
        public async Task<string> Modificar(DetallePago pDetalle)
        {
            string msj = $"Actualizando detalle de pago";

            try
            {
                var temp = _context.DetallesPago.FirstOrDefault(t => t.IdDetallePago == pDetalle.IdDetallePago);

                if (temp != null)
                {
                    temp.NumeroCheque = pDetalle.NumeroCheque;
                    temp.Banco = pDetalle.Banco;

                    _context.DetallesPago.Update(temp);
                    await _context.SaveChangesAsync();

                    msj = $"Detalle de pago actualizado correctamente";
                }
                else
                {
                    msj = $"No existe el detalle con ID {pDetalle.IdDetallePago}";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }
    } //Cierre class
} //Cierre namespace 


