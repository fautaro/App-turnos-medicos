using BusinessEntity.Models.Request;
using BusinessEntity.Models.Response;
using BusinessEntity.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TurnoMedico.Models;

namespace TurnoMedico.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ValidationService _validationService;
        private ReservaService _reservaService;

        public HomeController(ILogger<HomeController> logger, ValidationService validationService, ReservaService reservaService)
        {
            _logger = logger;
            _validationService = validationService;
            _reservaService = reservaService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task PagarTurno()
        {

        }

        public async Task ConfirmarTurno(RequestDatosTurno turno)
        {
           if (await _validationService.validateReserva(turno))
            {
                ResponseDatosTurno reserva = await _reservaService.GuardarReserva(turno);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}