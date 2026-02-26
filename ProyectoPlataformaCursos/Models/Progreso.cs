using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPlataformaCursos.Models
{
    [Table("Progreso")]
    public class Progreso
    {
        [Key]
        [Column("IdProgreso")]
        public int IdProgreso { get; set; }

        [Required]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("IdLeccion")]
        public int IdLeccion { get; set; }

        public bool Completado { get; set; } = false;

        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.Now;

        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("IdLeccion")]
        public virtual Leccion? Leccion { get; set; }
    }
}
