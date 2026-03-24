using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class Paquete
    {
        //Propiedades
        [Key]
        public int IdPaquete { get; set; }

        public string NombrePaquete { get; set; }

        public string Destino { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public decimal PrecioPorNoche { get; set; }

        public decimal PorcentajePrima { get; set; }

        public int Mensualidades { get; set; }
    } //Cierre class
}
