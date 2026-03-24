using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiBeachSA.Model;

namespace ApiBeachSA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly BeachContext _context = null;

        public RolesController(BeachContext pContext)
        {
            _context = pContext;
        }

        [HttpGet("Listar")]
        public List<Rol> Listar()
        {
            return _context.Roles.ToList();
        }

        [Authorize]
        [HttpPost("Guardar")]
        public string Guardar(Rol temp)
        {
            string msj = $"Rol {temp.NombreRol} guardado correctamente";
            try
            {
                _context.Roles.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException.Message;
            }
            return msj;
        }

        [HttpGet("BuscarId")]
        public Rol BuscarId(int pIdRol)
        {
            var temp = _context.Roles.FirstOrDefault(x => x.IdRol == pIdRol);

            if (temp == null)
            {
                temp = new Rol
                {
                    IdRol = pIdRol,
                    NombreRol = "No existe"
                };
            }

            return temp;
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int pIdRol)
        {
            string msj = "";

            try
            {
                var temp = _context.Roles.FirstOrDefault(r => r.IdRol == pIdRol);

                if (temp == null)
                {
                    msj = $"No existe el rol con ID {pIdRol}";
                }
                else
                {
                    _context.Roles.Remove(temp);
                    await _context.SaveChangesAsync();
                    msj = $"Rol {temp.NombreRol} eliminado correctamente";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException.Message;
            }

            return msj;
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<string> Modificar(Rol pRol)
        {
            string msj = $"Actualizando rol {pRol.NombreRol}";

            try
            {
                var temp = _context.Roles.FirstOrDefault(t => t.IdRol == pRol.IdRol);

                if (temp != null)
                {
                    temp.NombreRol = pRol.NombreRol;

                    _context.Roles.Update(temp);
                    await _context.SaveChangesAsync();

                    msj = $"Rol {temp.NombreRol} actualizado correctamente";
                }
                else
                {
                    msj = $"No existe el rol con ID {pRol.IdRol}";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException.Message;
            }

            return msj;
        }

        [HttpGet("BuscarNombre")]
        public List<Rol> BuscarNombre(string pNombreRol)
        {
            return _context.Roles
                .Where(r => r.NombreRol.StartsWith(pNombreRol))
                .ToList();
        }
    } //Cierre class
} //Cierre namespace 

