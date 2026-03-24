using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class DetallePago
    {
        //Propiedades
        [Key]
        public int IdDetallePago { get; set; }

        public int IdReservacion { get; set; }

        public string NumeroCheque { get; set; }

        public string Banco { get; set; }
    } //Cierre class
}
