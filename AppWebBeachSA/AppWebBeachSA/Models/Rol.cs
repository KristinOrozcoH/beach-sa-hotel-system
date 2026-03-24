using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class Rol
    {
        //Propiedades
        [Key]
        public int IdRol { get; set; }

        public string NombreRol { get; set; }
    } //Cierre class
}
