using System.ComponentModel.DataAnnotations;

namespace ProyectoPlataformaCursos.Models.ViewModels
{
    public class AdminCursoPagoViewModel
    {
        public int IdCurso { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Range(0, 999999)]
        public decimal Precio { get; set; }

        public bool Activo { get; set; }

        public bool AceptaEfectivo { get; set; }

        public bool AceptaTarjeta { get; set; }

        public bool AceptaTransferencia { get; set; }
    }
}