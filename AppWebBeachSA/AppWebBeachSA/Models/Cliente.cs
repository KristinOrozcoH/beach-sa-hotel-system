using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class Cliente
    {
        //Propiedades
        [Key]
        public int IdCliente { get; set; }

        public int IdTipoCedula { get; set; }

        public string Cedula { get; set; }

        public string NombreCompleto { get; set; }

        public string Telefono { get; set; }

        public string CorreoElectronico { get; set; }

        public string Direccion { get; set; }
    }
}
