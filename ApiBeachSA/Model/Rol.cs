using System.ComponentModel.DataAnnotations;

namespace ApiBeachSA.Model
{
    //Programación POO
    public class Rol
    {
        //Propiedades
        [Key]
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
    } //Cierre class
} //Cierre namespace