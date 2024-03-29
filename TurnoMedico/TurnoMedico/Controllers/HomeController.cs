﻿using BusinessEntity.Models.Request;
using BusinessEntity.Models.Response;
using BusinessEntity.Request;
using BusinessEntity.Response;
using BusinessEntity.Services;
using BusinessEntity.ViewModels;
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
        public async Task<ResponseGetHorasDisponibles> GetHorasDisponibles([FromBody] RequestGetHorasDisponibles request)
        {
            try
            {
                var Response = await _reservaService.GetHorasDisponibles(request);

                return Response;


            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<ResponseGetDiasBloqueados> GetDiasBloqueados([FromBody] RequestGetDiasBloqueados request)
        {
            try
            {
                var Response = await _reservaService.GetDiasBloqueados(request);

                return Response;

            }
            catch (Exception ex)
            {

                throw;
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

        [HttpGet]
        [Route("{profesional}/cancelarturno")]

        public async Task<IActionResult> CancelarTurno(string profesional, string token)
        {
            CancelarTurnoViewModel Response = new CancelarTurnoViewModel();

            try
            {
                var Profesional = await _validationService.ValidateProfesional(profesional);

                if (Profesional != null && Profesional.Activo)
                {
                    Response = await _reservaService.GetCancelacionTurno(token, Profesional);
                }
                else
                {
                    Response.Success = false;
                    Response.Mensaje = "No se pudo cancelar el turno";
                }
                return View(Response);

            }
            catch (Exception ex)
            {
                Response.Success = false;
                Response.Mensaje = "No se pudo cancelar el turno";

                throw;
            }

        }

        [HttpPost]
        public async Task<ResponseCancelarTurno> ConfirmCancelarTurno([FromBody] RequestCancelarTurno request)
        {
            ResponseCancelarTurno response = new ResponseCancelarTurno();

            try
            {
                if (request != null)
                {
                    response = await _reservaService.CancelarReserva(request);
                } else
                {
                    response.Success = false;
                    response.Mensaje = "Ocurrió un error al cancelar la reserva";
                }
                return response;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Mensaje = "Ocurrió un error al cancelar la reserva";
                return response;
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("NotFound");
        }
    }
}