using Microsoft.AspNetCore.Mvc;
using AppWebBeachSA.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AppWebBeachSA.Controllers
{
    public class ClientesController : Controller
    {
        private ApiWebBeachSA _client = null;
        private HttpClient _api = null;

        public ClientesController()
        {
            _client = new ApiWebBeachSA();
            _api = _client.IniciarAPI();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Cliente> listado = new List<Cliente>();
            HttpResponseMessage response = await _api.GetAsync("Clientes/Listar");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Cliente>>(result);
            }

            return View(listado);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

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

        [HttpPost]
        public async Task<IActionResult> Create([Bind] ClienteRegistroRequest cliente)
        {
            var respuesta = await _api.PostAsJsonAsync("Clientes/Guardar", cliente);

            if (respuesta.IsSuccessStatusCode)
            {
                var mensaje = await respuesta.Content.ReadAsStringAsync();
                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "No se logró registrar el cliente";
                return View(cliente);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Cliente cliente = new Cliente();
            HttpResponseMessage response = await _api.GetAsync($"Clientes/BuscarId?pIdCliente={id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                cliente = JsonConvert.DeserializeObject<Cliente>(result);
            }

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind] Cliente cliente)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var respuesta = await _api.PutAsJsonAsync("Clientes/Modificar", cliente);

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Cliente actualizado correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al actualizar el cliente.";
                return View(cliente);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Cliente cliente = new Cliente();
            HttpResponseMessage response = await _api.GetAsync($"Clientes/BuscarId?pIdCliente={id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                cliente = JsonConvert.DeserializeObject<Cliente>(result);
            }

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await _api.DeleteAsync($"Clientes/Eliminar?pIdCliente={id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Cliente eliminado correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el cliente.";
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuscarNombre(string nombre)
        {
            List<Cliente> resultados = new List<Cliente>();

            HttpResponseMessage response = await _api.GetAsync($"Clientes/BuscarNombre?pNombre={nombre}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                resultados = JsonConvert.DeserializeObject<List<Cliente>>(result);
            }

            return View("Index", resultados);
        }

        private AuthenticationHeaderValue AutorizacionToken()
        {
            var token = HttpContext.Session.GetString("token");
            return string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
