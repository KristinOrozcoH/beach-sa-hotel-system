using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class Usuario
    {
        [Key]
        [Required(ErrorMessage = "No puede dejar el id del Usuario en blanco")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "No puede dejar el username en blanco")]
        [DataType(DataType.Text)]
        [StringLength(100)]
        public string Username { get; set; }
    

        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Contrasenna { get; set; }
        
    } 
}
