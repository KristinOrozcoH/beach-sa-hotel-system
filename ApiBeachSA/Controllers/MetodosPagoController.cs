using Microsoft.AspNetCore.Mvc;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetodosPagoController : ControllerBase
    {
        private readonly BeachContext _context;

        public MetodosPagoController(BeachContext context)
        {
            _context = context;
        }

        // Método para listar todos los métodos de pago
        [HttpGet("Listar")]
        public List<MetodoPago> Listar()
        {
            return _context.MetodosPago.ToList();
        }

    } //Cierre class
} //Cierre namespace 


