using Microsoft.AspNetCore.Mvc;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TipoCedulaController : ControllerBase
    {
        private readonly BeachContext _context;

        public TipoCedulaController(BeachContext context)
        {
            _context = context;
        }

        // Listar tipos de cédula
        [HttpGet("Listar")]
        public List<TipoCedula> Listar()
        {
            return _context.TiposCedula.ToList();
        }
    } //Cierre class
} //Cierre namespace 

