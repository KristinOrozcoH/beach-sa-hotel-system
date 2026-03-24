using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiBeachSA.Model
{
    public class Reservacion
    {
        [Key]
        public int IdReservacion { get; set; }

        public int IdCliente { get; set; }

        public int IdPaquete { get; set; }

        public int IdMetodoPago { get; set; }

        public int CantidadNoches { get; set; }

        [JsonIgnore]
        public decimal Mensualidades { get; set; }

        [JsonIgnore]
        public decimal Prima { get; set; }

        [JsonIgnore]
        public decimal Descuento { get; set; }

        [JsonIgnore]
        public decimal Impuestos { get; set; }

        [JsonIgnore]
        public decimal MontoTotal { get; set; }

        [JsonIgnore]
        public decimal TipoCambio { get; set; }

        [JsonIgnore]
        public decimal ValorColones { get; set; }

        [JsonIgnore]
        public decimal ValorDolares { get; set; }

        [JsonIgnore]
        public bool PDFEnviado { get; set; }
    } //Cierre class 
} //Cierre namespace

