using System.ComponentModel.DataAnnotations;

namespace ApiBeachSA.Model
{
    //Programación POO
    public class DetallePago
    {
        //Propiedades
        [Key]
        public int IdDetallePago { get; set; }

        public int IdReservacion { get; set; }

        public string NumeroCheque { get; set; }

        public string Banco { get; set; }
    } //Cierre class
} //Cierre namespace
