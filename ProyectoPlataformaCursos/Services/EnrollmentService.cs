using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Models.ViewModels;
using ProyectoPlataformaCursos.Repositories.Interfaces;

namespace ProyectoPlataformaCursos.Services
{
    public class EnrollmentService
    {
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IProgresoRepository _progresoRepository;
        private readonly StripePaymentService _stripePaymentService;

        public EnrollmentService(
            IInscripcionRepository inscripcionRepository,
            ICursoRepository cursoRepository,
            IProgresoRepository progresoRepository,
            StripePaymentService stripePaymentService)
        {
            _inscripcionRepository = inscripcionRepository;
            _cursoRepository = cursoRepository;
            _progresoRepository = progresoRepository;
            _stripePaymentService = stripePaymentService;
        }

        public async Task<(bool Success, string Message)> InscribirUsuarioAsync(
            int idUsuario,
            int idCurso,
            string? metodoPago,
            string? cardHolderName,
            string? cardNumber,
            string? cardExpiry,
            string? cardCvc,
            string? transferAccountHolder,
            string? transferIban,
            string? transferBankName,
            string? transferReference)
        {
            if (await _inscripcionRepository.ExistsAsync(idUsuario, idCurso))
            {
                return (false, "Ya estás inscrito en este curso");
            }

            var curso = await _cursoRepository.GetByIdAsync(idCurso);
            if (curso == null)
            {
                return (false, "El curso no existe");
            }

            string metodoNormalizado = string.IsNullOrWhiteSpace(metodoPago)
                ? string.Empty
                : metodoPago.Trim().ToUpperInvariant();

            if (curso.Precio > 0)
            {
                var metodosDisponibles = new Dictionary<string, bool>
                {
                    ["EFECTIVO"] = curso.AceptaEfectivo,
                    ["TARJETA"] = curso.AceptaTarjeta,
                    ["TRANSFERENCIA"] = curso.AceptaTransferencia
                };

                if (!metodosDisponibles.TryGetValue(metodoNormalizado, out var permitido) || !permitido)
                {
                    return (false, "Selecciona un método de pago válido para este curso");
                }

                if (metodoNormalizado == "TARJETA")
                {
                    if (string.IsNullOrWhiteSpace(cardHolderName)
                        || string.IsNullOrWhiteSpace(cardNumber)
                        || string.IsNullOrWhiteSpace(cardExpiry)
                        || string.IsNullOrWhiteSpace(cardCvc))
                    {
                        return (false, "Completa todos los datos de pago con tarjeta para continuar");
                    }

                    var stripeResult = await _stripePaymentService.CreateAndConfirmPaymentIntentAsync(
                        curso.Precio,
                        cardHolderName,
                        cardNumber,
                        cardExpiry,
                        cardCvc,
                        $"Inscripción curso {curso.Titulo} (Id:{curso.IdCurso})");

                    if (!stripeResult.Success)
                    {
                        return (false, stripeResult.Message);
                    }
                }

                if (metodoNormalizado == "TRANSFERENCIA")
                {
                    if (string.IsNullOrWhiteSpace(transferAccountHolder)
                        || string.IsNullOrWhiteSpace(transferIban)
                        || string.IsNullOrWhiteSpace(transferBankName)
                        || string.IsNullOrWhiteSpace(transferReference))
                    {
                        return (false, "Completa todos los datos de transferencia");
                    }
                }
            }
            else
            {
                metodoNormalizado = "SIN_COSTE";
            }

            var inscripcion = new Inscripcion
            {
                IdUsuario = idUsuario,
                IdCurso = idCurso,
                FechaInscripcion = DateTime.Now,
                Estado = "ACTIVO",
                MetodoPago = metodoNormalizado,
                ImportePagado = curso.Precio
            };

            await _inscripcionRepository.CreateAsync(inscripcion);
            return (true, curso.Precio > 0
                ? $"Pago registrado ({metodoNormalizado}) e inscripción completada"
                : "Te has inscrito exitosamente al curso");
        }

        public async Task<IEnumerable<CursoViewModel>> GetMisCursosAsync(int idUsuario)
        {
            var inscripciones = await _inscripcionRepository.GetByUsuarioAsync(idUsuario);
            var viewModels = new List<CursoViewModel>();

            foreach (var inscripcion in inscripciones)
            {
                if (inscripcion.Curso != null)
                {
                    var numLecciones = inscripcion.Curso.Lecciones?.Count ?? 0;
                    var completadas = 0;

                    if (numLecciones > 0)
                    {
                        completadas = await _progresoRepository.GetCompletedCountAsync(idUsuario, inscripcion.IdCurso);
                    }

                    var viewModel = new CursoViewModel
                    {
                        IdCurso = inscripcion.Curso.IdCurso,
                        Titulo = inscripcion.Curso.Titulo,
                        Descripcion = inscripcion.Curso.Descripcion,
                        NombreProfesor = inscripcion.Curso.Profesor != null ?
                            $"{inscripcion.Curso.Profesor.Nombre} {inscripcion.Curso.Profesor.Apellidos}" : "Sin asignar",
                        FechaCreacion = inscripcion.Curso.FechaCreacion,
                        Activo = inscripcion.Curso.Activo,
                        Precio = inscripcion.Curso.Precio,
                        AceptaEfectivo = inscripcion.Curso.AceptaEfectivo,
                        AceptaTarjeta = inscripcion.Curso.AceptaTarjeta,
                        AceptaTransferencia = inscripcion.Curso.AceptaTransferencia,
                        FormasPagoDisponibles = string.Join(", ",
                            new[]
                            {
                                inscripcion.Curso.AceptaEfectivo ? "Efectivo" : null,
                                inscripcion.Curso.AceptaTarjeta ? "Tarjeta" : null,
                                inscripcion.Curso.AceptaTransferencia ? "Transferencia" : null
                            }.Where(x => x != null)),
                        NumeroLecciones = numLecciones,
                        EstaInscrito = true,
                        ProgresoPercentage = numLecciones > 0 ? (int)((completadas * 100.0) / numLecciones) : 0
                    };

                    viewModels.Add(viewModel);
                }
            }

            return viewModels;
        }
    }
}
