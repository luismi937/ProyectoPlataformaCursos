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
        public decimal Precio { get; set; }
        public bool AceptaEfectivo { get; set; }
        public bool AceptaTarjeta { get; set; }
        public bool AceptaTransferencia { get; set; }
        public string FormasPagoDisponibles { get; set; } = string.Empty;
        public int NumeroLecciones { get; set; }
        public bool EstaInscrito { get; set; }
        public int ProgresoPercentage { get; set; }
    }
}
