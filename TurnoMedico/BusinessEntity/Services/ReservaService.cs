using antlr;
using BusinessEntity.Models.Request;
using BusinessEntity.Models.Response;
using BusinessEntity.Request;
using BusinessEntity.Response;
using BusinessEntity.ViewModels;
using DataAccess.Models;
using DataAccess.Services;
using System.Globalization;

namespace BusinessEntity.Services
{
    public class ReservaService
    {
        private DbWrapper _dbWrapper;
        private ValidationService _validationService;
        private TokenService _tokenService;
        private MailService _mailService;
        private readonly DateTime fechaActual = DateTime.Now;


        public ReservaService(DbWrapper dbWrapper, ValidationService validationService, TokenService tokenService, MailService mailService)
        {
            _dbWrapper = dbWrapper;
            _validationService = validationService;
            _tokenService = tokenService;
            _mailService = mailService; 
        }




        public async Task<ResponseGetDiasBloqueados> GetDiasBloqueados(RequestGetDiasBloqueados request)
        {
            ResponseGetDiasBloqueados response = new ResponseGetDiasBloqueados();

            try
            {

                bool DiaActualDisponible = await _validationService.ValidarDiaActual(request.Profesional_Id);

                var agendaBloqueada = _dbWrapper.GetAgendaBloqueada(request.Profesional_Id).Result;

                if (agendaBloqueada != null)
                {
                    response.DiasBloqueados = new List<string>();
                    foreach (var rangoBloqueado in agendaBloqueada)
                    {
                        var DiasBloqueados = await _validationService.ConvertToArrayFechas(rangoBloqueado.FechaDesde, rangoBloqueado.FechaHasta);
                        response.DiasBloqueados.AddRange(DiasBloqueados);

                    }
                }

                //Se obtienen los días que tienen todos los turnos completos y se agregan a la lista de días bloqueados
                response.DiasBloqueados.AddRange(await _validationService.GetDiasTurnosCompletos(request.Profesional_Id));

                if (!DiaActualDisponible)
                {
                    response.DiasBloqueados.Add(fechaActual.ToString("yyyy/MM/dd"));
                }
                //Ordeno lista de días bloqueados
                response.DiasBloqueados.Sort();
                response.Success = true;

                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.DiasBloqueados = null;

                return response;
            }
        }


        public async Task<ResponseGetHorasDisponibles> GetHorasDisponibles(RequestGetHorasDisponibles request)
        {
            ResponseGetHorasDisponibles response = new ResponseGetHorasDisponibles();

            try
            {
                var GetHorariosDisponibles = await _validationService.GetHorasDisponibles(request);

                response.Success = true;
                response.HorasDisponibles = GetHorariosDisponibles;
                response.HorasDisponibles.Sort();

                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.HorasDisponibles = null;

                return response;
            }
        }

        public async Task<ResponseDatosTurno> GuardarReserva(RequestDatosTurno datosTurno)
        {
            try
            {
                ResponseDatosTurno response = new ResponseDatosTurno();

                if (await _validationService.validateReserva(datosTurno))
                {
                    if (!await _validationService.validateUsuarioTieneTurno(datosTurno))
                    {
                        response.Success = false;
                        response.Estado = "E";
                        response.Mensaje = "Tu mail ya tiene un turno existente en la semana seleccionada";

                    }
                    else
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
                        response.TurnoConfirmado = new ResponseTurnoConfirmado()
                        {
                            Cliente = $"{datosTurno.Nombre} {datosTurno.Apellido}",
                            Email = datosTurno.Email,
                            Fecha = datosTurno.Fecha,
                            Hora = datosTurno.Hora,
                            Profesional = datosTurno.Profesional,
                            Telefono = datosTurno.Telefono

                        };
                        await _mailService.EnviarMailConfirmacionTurno(datosTurno.Email, datosTurno.Profesional, datosTurno.ProfesionalId, TurnoGeneradoDB.FechaHora.ToString("dd/MM/yyyy HH:mm"), TurnoGeneradoDB.FechaHora, TurnoGeneradoDB.Token);
                    }
                }
                else
                {
                    response.Success = false;
                    response.Estado = "E";
                    response.Mensaje = "Ocurrió un error al guardar el turno. Verifique los datos ingresados";
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

        public async Task<CancelarTurnoViewModel> GetCancelacionTurno(string Token, ProfesionalResponse Profesional)
        {
            CancelarTurnoViewModel response = new CancelarTurnoViewModel();
            var Turno = await _dbWrapper.CheckToken(Token);

            if (Turno != null)
            {
                response.Profesional = Profesional.Profesional;
                response.Paciente = $"{Turno.Nombre} {Turno.Apellido}";
                response.FechaHora = Turno.FechaHora.ToString("dd/MM/yyyy HH:mm");
                response.Profesional_Id = Turno.Profesional_Id;
                response.Token = Turno.Token;
                response.Success = true;

            } else
            {
                response.Success = false;
                response.Mensaje = "No hemos podido encontrar el turno que deseas cancelar";
            }

            return response;
        }

        public async Task<ResponseCancelarTurno> CancelarReserva(RequestCancelarTurno request)
        {
            ResponseCancelarTurno response = new ResponseCancelarTurno();

            try
            {
                ResponseCancelarTurno Validacion = await _validationService.ValidateCancelarReserva(request);


                if (Validacion.Success)
                {
                    await _dbWrapper.CancelarTurno(request.Token);

                    response.Success = true;
                    response.Mensaje = $"El turno ha sido cancelado correctamente";
                }
                else
                {
                    response.Success = Validacion.Success;
                    response.Mensaje = Validacion.Mensaje;

                }
                return response;
            }
            catch (Exception ex)
            {

                response.Success = true;
                response.Mensaje = $"Hubo un error al cancelar la reserva";
                return response;
            }
        }
    }
}
