using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class Reservacion
    {
        //Propiedades
        [Key]
        public int IdReservacion { get; set; }

        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        public int IdPaquete { get; set; }

        public int CantidadNoches { get; set; }

        public int IdMetodoPago { get; set; }

        public decimal Prima { get; set; }
        public decimal Mensualidades { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuestos { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal ValorColones { get; set; }
        public decimal ValorDolares { get; set; }

        public bool PDFEnviado { get; set; }
    } //Cierre class
}
