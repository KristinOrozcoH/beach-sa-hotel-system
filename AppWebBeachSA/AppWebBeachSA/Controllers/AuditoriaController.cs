using Microsoft.AspNetCore.Mvc;

using AppWebBeachSA.Models;
using Newtonsoft;
using Newtonsoft.Json;
using System.Net.Http;


namespace AppWebBeachSA.Controllers
{
    public class AuditoriaController : Controller
    {
        private ApiWebBeachSA _client = null;

        private HttpClient _api = null;

        public AuditoriaController()
        {
            _client = new ApiWebBeachSA();

            _api = _client.IniciarAPI();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Auditoria> listado = new List<Auditoria>();

            HttpResponseMessage response = await _api.GetAsync("Auditorias/Listar");

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Auditoria>>(result);
            }
            return View(listado);
        }


        [HttpGet]
        public async Task<IActionResult> BuscarPorUsuario(string pUsuario)
        {
            if (string.IsNullOrWhiteSpace(pUsuario))
            {
                return View("Index", new List<Auditoria>());
            }

            HttpResponseMessage buscar = await _api.GetAsync($"Auditoria/BuscarPorUsuario?usuario={pUsuario}");

            if (buscar.IsSuccessStatusCode)
            {
                var contenido = await buscar.Content.ReadAsStringAsync();
                var auditorias = JsonConvert.DeserializeObject<List<Auditoria>>(contenido);
                return View("Index", auditorias);
            }

            return View("Index", new List<Auditoria>());
        }

    }
}