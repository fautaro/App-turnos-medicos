using BusinessEntity.Models.Request;
using BusinessEntity.Models.Response;
using BusinessEntity.Request;
using BusinessEntity.Response;
using DataAccess.Models;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BusinessEntity.Services
{
    public class ReservaService
    {
        private DbWrapper _dbWrapper;
        private ValidationService _validationService;
        private TokenService _tokenService;


        public ReservaService(DbWrapper dbWrapper, ValidationService validationService, TokenService tokenService)
        {
            _dbWrapper = dbWrapper;
            _validationService = validationService;
            _tokenService = tokenService;

        }

        public async Task<ResponseGetDiasBloqueados> GetDiasBloqueados(RequestGetDiasBloqueados request)
        {

            try
            {
                var agendaBloqueada = _dbWrapper.GetAgendaBloqueada(request.Profesional_Id);


                ResponseGetDiasBloqueados response = new ResponseGetDiasBloqueados()
                {
                    Success = true,
                    DiasBloqueados = new List<string> { "2023/08/15", "2023/08/16", "2023/08/23" }
                };

                response.DiasBloqueados.Sort();

                return response;

            }
            catch (Exception ex)
            {
                ResponseGetDiasBloqueados response = new ResponseGetDiasBloqueados()
                {
                    Success = false,
                    //HorasDisponibles = null
                };
                return response;
            }


            //return new List<string> { "15:00", "16:00", "17:00", "11:00", "10:00" };
        }

        public async Task<ResponseGetHorasDisponibles> GetHorasDisponibles(RequestGetHorasDisponibles request)
        {

            try
            {
                ResponseGetHorasDisponibles response = new ResponseGetHorasDisponibles()
                {
                    Success = true,
                    HorasDisponibles = new List<string> { "15:00", "16:00", "17:00", "11:00", "10:00" }
                };

                response.HorasDisponibles.Sort();

                return response;

            }
            catch (Exception ex)
            {
                ResponseGetHorasDisponibles response = new ResponseGetHorasDisponibles()
                {
                    Success = false,
                    HorasDisponibles = null
                };
                return response;
            }


            //return new List<string> { "15:00", "16:00", "17:00", "11:00", "10:00" };
        }

        public async Task<ResponseDatosTurno> GuardarReserva(RequestDatosTurno datosTurno)
        {
            try
            {
                ResponseDatosTurno response = new ResponseDatosTurno();

                if (await _validationService.validateReserva(datosTurno))
                {
                    DateTime FechaHora = DateTime.ParseExact($"{datosTurno.Fecha} {datosTurno.Hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                    Turno turno = new Turno()
                    {
                        Nombre = datosTurno.Nombre,
                        Apellido = datosTurno.Apellido,
                        Email = datosTurno.Email,
                        FechaHora = FechaHora,
                        Profesional_Id = datosTurno.ProfesionalId,
                        Telefono = datosTurno.Telefono,
                        Activo = true,
                        Token = _tokenService.GenerateGuidToken()

                    };
                    var TurnoGeneradoDB = await _dbWrapper.AddTurno(turno);

                    response.Success = true;
                    response.Reserva_Id = TurnoGeneradoDB.Turno_Id;
                    response.Estado = "C";
                    response.Mensaje = "Turno guardado Correctamente";
                    response.TurnoConfirmado = new ResponseTurnoConfirmado()
                    {
                        Cliente = $"{datosTurno.Nombre} {datosTurno.Apellido}",
                        Email = datosTurno.Email,
                        Fecha = datosTurno.Fecha,
                        Hora = datosTurno.Hora,
                        Profesional = datosTurno.Profesional,
                        Telefono = datosTurno.Telefono

                    };

                }
                else
                {
                    response.Success = false;
                    response.Estado = "E";
                    response.Mensaje = "Ocurrió un error al guardar el turno";


                }
                return response;
            }
            catch (Exception ex)
            {
                ResponseDatosTurno response = new ResponseDatosTurno();

                response.Success = false;
                response.Estado = "E";
                response.Mensaje = $"Ocurrió un error al guardar el turno: {ex.Message}";

                return response;
            }

        }



        public async Task<ResponseCancelarTurno> CancelarReserva(RequestCancelarTurno turno)
        {
            try
            {
                ResponseCancelarTurno response = new ResponseCancelarTurno();

                if (await _validationService.ValidateCancelarReserva(turno))
                {
                    await _dbWrapper.CancelarTurno(turno.Turno_Id);

                    response.Success = true;
                    response.Estado = "Cancelada";
                    response.Mensaje = $"La reserva {turno.Turno_Id} ha sido cancelada correctamente";


                }
                else
                {
                    response.Success = true;
                    response.Mensaje = $"Hubo un error al validar la reserva";

                }

                return response;
            }
            catch (Exception ex)
            {
                ResponseCancelarTurno response = new ResponseCancelarTurno();

                response.Success = true;
                response.Mensaje = $"Hubo un error al cancelar la reserva";
                return response;
            }

        }

    }
}
