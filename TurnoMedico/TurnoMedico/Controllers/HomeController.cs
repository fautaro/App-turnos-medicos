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

        public async Task<IActionResult> Index(string profesional)
        {
            if (string.IsNullOrEmpty(profesional))
            {
                return View("NotFound");
            }

            var Profesional = await _validationService.ValidateProfesional(profesional);

            if (Profesional != null && Profesional.Activo)
            {
                return View(Profesional);

            }
            else
            {
                return View("NotFound");
            }
        }

        [HttpPost]
        public async Task<ResponseDatosTurno> ConfirmarTurno([FromBody] RequestDatosTurno turno)
        {
            try
            {
                //await Task.Delay(4000);

                var Response = await _reservaService.GuardarReserva(turno);

                return Response;


            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<ResponseCancelarTurno> CancelarTurno([FromBody] RequestCancelarTurno turno)
        {
            try
            {
                //await Task.Delay(4000);

                var Response = await _reservaService.CancelarReserva(turno);

                return Response;

            }
            catch (Exception ex)
            {

                throw;
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


//var response = new ResponseDatosTurno()
//{
//    Reserva_Id = 100,
//    Estado = "C",
//    Success = true,
//    TurnoConfirmado = new ResponseTurnoConfirmado()
//    {
//        Cliente = turno.Nombre + turno.Apellido,
//        Telefono = turno.Telefono,
//        Email = turno.Email,
//        Fecha = turno.Fecha,
//        Hora = turno.Hora,
//        Profesional = turno.Profesional
//    }
//};