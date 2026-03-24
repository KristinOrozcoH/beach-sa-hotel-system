using System.ComponentModel.DataAnnotations;

namespace ApiBeachSA.Model
{
    //Programación POO
    public class TipoCedula
    {
        //Propiedades
        [Key]
        public int IdTipoCedula { get; set; }

        public string Descripcion { get; set; }
    } //Cierre class
} //Cierre namespace

