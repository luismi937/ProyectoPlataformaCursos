namespace ProyectoPlataformaCursos.Models.ViewModels
{
    public class LeccionViewModel
    {
        public int IdLeccion { get; set; }
        public int IdCurso { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public string PreguntaEvaluacion { get; set; } = string.Empty;
        public int Orden { get; set; }
        public bool Completado { get; set; }
        public bool PuedeAcceder { get; set; }
        public string TituloCurso { get; set; } = string.Empty;
    }
}
