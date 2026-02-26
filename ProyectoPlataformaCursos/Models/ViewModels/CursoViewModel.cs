namespace ProyectoPlataformaCursos.Models.ViewModels
{
    public class CursoViewModel
    {
        public int IdCurso { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string NombreProfesor { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        public int NumeroLecciones { get; set; }
        public bool EstaInscrito { get; set; }
        public int ProgresoPercentage { get; set; }
    }
}
