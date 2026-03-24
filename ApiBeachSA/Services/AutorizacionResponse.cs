namespace APIBeachSA.Services
{
    public class AutorizacionResponse
    {
        // Token generado
        public string Token { get; set; }

        // Fecha de expiración del token
        public DateTime Expira { get; set; }

        // Rol asignado al usuario autenticado
        public string Rol { get; set; }

        // Nombre del usuario
        public string Nombre { get; set; }

    } //Cierre class
} //Cierre namespace

