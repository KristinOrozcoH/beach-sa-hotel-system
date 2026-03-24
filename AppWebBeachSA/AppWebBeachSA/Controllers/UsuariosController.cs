using AppWebBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AppWebBeachSA.Controllers
{
    public class UsuariosController : Controller
    {
        private ApiWebBeachSA _client = null;
        private HttpClient _api = null;

        public UsuariosController()
        {
            _client = new ApiWebBeachSA();
            _api = _client.IniciarAPI();
        }

        // Listado de Usuarios
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Usuario> lista = new List<Usuario>();

            HttpResponseMessage response = await _api.GetAsync("Usuarios/Listar");

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadAsStringAsync();
                lista = JsonConvert.DeserializeObject<List<Usuario>>(resultado);
            }

            return View(lista);
        }

        // Crear Usuario - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Crear Usuario - POST
        [HttpPost]
        public async Task<IActionResult> Create([Bind] Usuario pUsuario)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await _api.PostAsJsonAsync("Usuarios/Registrar", pUsuario);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            TempData["Error"] = "Error al crear el usuario";
            return View(pUsuario);
        }

        // Editar Usuario - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Usuario usuario = new Usuario();

            HttpResponseMessage response = await _api.GetAsync($"Usuarios/BuscarId?pIdUsuario={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);
            }

            return View(usuario);
        }

        // Editar Usuario - POST
        [HttpPost]
        public async Task<IActionResult> Edit([Bind] Usuario usuario)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await _api.PutAsJsonAsync("Usuarios/Modificar", usuario);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            TempData["Error"] = "Error al modificar el usuario";
            return View(usuario);
        }

        // Detalle Usuario
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Usuario usuario = new Usuario();

            HttpResponseMessage response = await _api.GetAsync($"Usuarios/BuscarId?pIdUsuario={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);
            }

            return View(usuario);
        }

        // Eliminar Usuario
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await _api.DeleteAsync($"Usuarios/Eliminar?pIdUsuario={id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            TempData["Error"] = "No se pudo eliminar el usuario";
            return NotFound();
        }

        // Método para recuperar token de sesión
        private AuthenticationHeaderValue AutorizacionToken()
        {
            var token = HttpContext.Session.GetString("token");
            return string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind] Usuario usuario)
        {
            //Variable para almacenar los datos del token
            AutorizacionResponse autorizacion = null;

            //datos 
            usuario.Username = usuario.Username;
            usuario.Contrasenna = usuario.Contrasenna;

            //se valida si hay datos
            if (usuario == null)
            {
                TempData["MensajeLogin"] = "Usuario o contraseña incorrectos";
                return View(usuario);
            } //se ejecuta el método autenticar para la entrega del token
            HttpResponseMessage response = await _api.PostAsJsonAsync<Usuario>(
                "Usuarios/Autenticar", usuario);

            Console.WriteLine(usuario.Username);
            Console.WriteLine(usuario.Contrasenna);
            Console.WriteLine(response.StatusCode);


            //se verifica si el código es 200 ok
            if (response.IsSuccessStatusCode)
            {  //se realiza lectura de los datos en formato JSON
                var resultado = response.Content.ReadAsStringAsync().Result;
                //Se convierte los datos en un objeto
                autorizacion = JsonConvert.DeserializeObject<AutorizacionResponse>(resultado);
            }

            //Se valida los datos de autorizacion
            if (autorizacion != null && autorizacion.Resultado == true)
            {
                //se instancia la identidad a iniciar sesión
                var identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                //Se rellenan los datos
                identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Username));

                //se crea la identidad principal
                var principal = new ClaimsPrincipal(identity);
                //se inicia sesión
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //se almacena el token otorgado
                HttpContext.Session.SetString("token", autorizacion.Token);

                //se ubica al usuario dentro del listado libros
                return RedirectToAction("Index", "Libros");
            }
            else
            {
                return View(usuario);
            }
        }


        //Método encargado de realizar el proceso de cierre sesión
        public async Task<IActionResult> Logout()
        {
            //se cierra la sesión
            await HttpContext.SignOutAsync();

            //Se ubica la usuario dentro del formulario principal
            return RedirectToAction("Index", "Home");
        }
    }
}
