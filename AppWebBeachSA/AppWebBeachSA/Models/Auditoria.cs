using System.ComponentModel.DataAnnotations;

namespace AppWebBeachSA.Models
{
    public class Auditoria
    {
        [Key]
        public int IdAuditoria { get; set; }

        public string Usuario { get; set; }

        public string Rol { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string TablaAfectada { get; set; }

        public string Accion { get; set; } // Insertar, Modificar, Eliminar

        public int? IdRegistro { get; set; }

        public string Descripcion { get; set; }
    } //Cierre class
}
