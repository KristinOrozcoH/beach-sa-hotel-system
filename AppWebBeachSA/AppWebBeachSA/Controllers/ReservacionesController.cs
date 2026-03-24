using Microsoft.AspNetCore.Mvc;
using AppWebBeachSA.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AppWebBeachSA.Controllers
{
    public class ReservacionesController : Controller
    {
        private ApiWebBeachSA _client = null;
        private HttpClient _api = null;

        public ReservacionesController()
        {
            _client = new ApiWebBeachSA();
            _api = _client.IniciarAPI(); 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Reservacion> listado = new List<Reservacion>();
            HttpResponseMessage response = await _api.GetAsync("Reservaciones/Listar");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Reservacion>>(result);
            }

            return View(listado);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] Reservacion reservacion)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var respuesta = await _api.PostAsJsonAsync("Reservaciones/Guardar", reservacion);

            if (respuesta.IsSuccessStatusCode)
            {
                var mensaje = await respuesta.Content.ReadAsStringAsync();
                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "No se logró registrar la reservación";
                return View(reservacion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Reservacion reservacion = new Reservacion();
            HttpResponseMessage response = await _api.GetAsync($"Reservaciones/BuscarId?pIdReservacion={id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                reservacion = JsonConvert.DeserializeObject<Reservacion>(result);
            }

            return View(reservacion);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await _api.DeleteAsync($"Reservaciones/Eliminar?pIdReservacion={id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Reservación eliminada correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "No se logró eliminar la reservación.";
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarcarPDF(int id)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await _api.PutAsync($"Reservaciones/ActualizarEstadoPDF?pIdReservacion={id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Estado PDF actualizado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar el estado PDF.";
            }

            return RedirectToAction("Index");
        }

        private AuthenticationHeaderValue AutorizacionToken()
        {
            var token = HttpContext.Session.GetString("token");
            return string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }
    }
}