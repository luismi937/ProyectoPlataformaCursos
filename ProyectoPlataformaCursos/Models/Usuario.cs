using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPlataformaCursos.Models
{
    [Table("Usuarios")]
    public class Usuario : IdentityUser<int>
    {
        [Key]
        [Column("IdUsuario")]
        public override int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public override string Email { get; set; } = string.Empty;

        public override string? PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "ALUMNO";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
        public virtual ICollection<Progreso> Progresos { get; set; } = new List<Progreso>();
    }
}
