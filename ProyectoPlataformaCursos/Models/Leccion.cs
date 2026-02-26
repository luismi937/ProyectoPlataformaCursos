using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPlataformaCursos.Models
{
    [Table("Lecciones")]
    public class Leccion
    {
        [Key]
        [Column("IdLeccion")]
        public int IdLeccion { get; set; }

        [Required]
        [Column("IdCurso")]
        public int IdCurso { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(4000)]
        public string Contenido { get; set; } = string.Empty;

        public int Orden { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [ForeignKey("IdCurso")]
        public virtual Curso? Curso { get; set; }

        public virtual ICollection<Progreso> Progresos { get; set; } = new List<Progreso>();
    }
}
