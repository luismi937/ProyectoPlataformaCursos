using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPlataformaCursos.Models
{
    [Table("Inscripciones")]
    public class Inscripcion
    {
        [Key]
        [Column("IdInscripcion")]
        public int IdInscripcion { get; set; }

        [Required]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("IdCurso")]
        public int IdCurso { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "ACTIVO";

        [StringLength(20)]
        public string MetodoPago { get; set; } = "SIN_COSTE";

        [Column(TypeName = "decimal(10,2)")]
        public decimal ImportePagado { get; set; } = 0;

        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("IdCurso")]
        public virtual Curso? Curso { get; set; }
    }
}
