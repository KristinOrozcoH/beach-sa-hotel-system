using Microsoft.AspNetCore.Mvc;

using AppWebBeachSA.Models;
using Newtonsoft;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;



namespace AppWebBeachSA.Controllers
{
    public class PaquetesController : Controller
    {
        private ApiWebBeachSA _client = null;

        private HttpClient _api = null;
        //////////////////////////////////////////////////////////////////////////////////


        public PaquetesController()
        {
            _client = new ApiWebBeachSA();

            _api = _client.IniciarAPI();
        }

        public async Task<IActionResult> Index()
        {
            List<Paquete> listado = new List<Paquete>();

            HttpResponseMessage response = await _api.GetAsync("Paquetes/Listar");

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Paquete>>(result);
            }
            return View(listado);
        }
        //////////////////////////////////////////////////////////////////////////////////


        //Métodos encargados de almacenar los datos para un paquete
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] Paquete pPaquete)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var agregar = _api.PostAsJsonAsync("Paquetes/Guardar", pPaquete);

            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error, no se logró almacena el paquete";

                return View(pPaquete);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////


        //Método encargado de generar el token de autorización
        private AuthenticationHeaderValue AutorizacionToken()
        {
            var token = HttpContext.Session.GetString("Token");

            AuthenticationHeaderValue authentication = null;

            if (token != null && token.Length != 0)
            {
                authentication = new AuthenticationHeaderValue("Bearer", token);
            }
            return authentication;
        }
        //////////////////////////////////////////////////////////////////////////////////


        //Método encargado de eliminar un paquete por medio del idPaquete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage delete = await _api.DeleteAsync($"Paquetes/Eliminar?pIdPaquete={id}");

            if (delete.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
        //////////////////////////////////////////////////////////////////////////////////


        //Métodos encargados de realizar el proceso de edicion
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Paquete temp = new Paquete();

            HttpResponseMessage buscar = await _api.GetAsync($"Paquetes/BuscarId?pIdPaquete={id}");

            if (buscar.IsSuccessStatusCode)
            {
                var result = buscar.Content.ReadAsStringAsync().Result;

                temp = JsonConvert.DeserializeObject<Paquete>(result);
            }
            return View(temp);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind] Paquete temp)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage modificar = await _api.PutAsJsonAsync<Paquete>("Paquetes/Modificar", temp);

            if (modificar.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(temp);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////


        //Método encargado de mostrar el detalle de datos para un paquete
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Paquete temp = new Paquete();

            HttpResponseMessage buscar = await _api.GetAsync($"Paquetes/BuscarId?pIdPaquete={id}");

            if (buscar.IsSuccessStatusCode)
            {
                var result = buscar.Content.ReadAsStringAsync().Result;

                temp = JsonConvert.DeserializeObject<Paquete>(result);
            }

            return View(temp);
        }
        //////////////////////////////////////////////////////////////////////////////////
    }//class
}//namespace