using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class MetodoPago
    {
        //Propiedades
        [Key]
        public int IdMetodoPago { get; set; }

        public string NombreMetodo { get; set; }

        public bool AplicaDescuento { get; set; }

        public bool RequiereDetalleCheque { get; set; }
    }
}
