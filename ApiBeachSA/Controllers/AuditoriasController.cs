using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditoriasController : ControllerBase
    {
        private readonly BeachContext _context;

        public AuditoriasController(BeachContext pContext)
        {
            _context = pContext;
        } 

        [Authorize]
        [HttpGet("Listar")]
        public List<Auditoria> Listar()
        {
            return _context.Auditorias
                .OrderByDescending(a => a.Fecha)
                .ToList();
        }

        [Authorize]
        [HttpGet("BuscarPorUsuario")]
        public List<Auditoria> BuscarPorUsuario(string usuario)
        {
            return _context.Auditorias
                .Where(a => a.Usuario.Contains(usuario))
                .OrderByDescending(a => a.Fecha)
                .ToList();
        }
    } //Cierre class
} //Cierre namespace 

