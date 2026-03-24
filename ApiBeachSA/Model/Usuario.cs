using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; 

namespace ApiBeachSA.Model
{
    //Programación POO
    public class Usuario
    {
        //Propiedades
        [Key]
        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public string Contrasenna { get; set; }

        [JsonIgnore]
        public int IdRol { get; set; }
        public int? IdCliente { get; set; }
    } //Cierre class
} //Cierre namespace



