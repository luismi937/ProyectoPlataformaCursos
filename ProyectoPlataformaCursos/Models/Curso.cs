using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPlataformaCursos.Models
{
    [Table("Cursos")]
    public class Curso
    {
        [Key]
        [Column("IdCurso")]
        public int IdCurso { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Column("IdProfesor")]
        public int IdProfesor { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        [ForeignKey("IdProfesor")]
        public virtual Usuario? Profesor { get; set; }

        public virtual ICollection<Leccion> Lecciones { get; set; } = new List<Leccion>();
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
