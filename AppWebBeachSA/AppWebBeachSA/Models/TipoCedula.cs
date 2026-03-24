using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class TipoCedula
    {
        //Propiedades
        [Key]
        public int IdTipoCedula { get; set; }

        public string Descripcion { get; set; }
    } //Cierre class
}
