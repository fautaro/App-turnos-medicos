using BusinessEntity.Models.Request;
using BusinessEntity.Models.Response;
using DataAccess.Models;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore;

namespace BusinessEntity.Services
{
    public class ReservaService
    {
        private readonly DbWrapper _dbWrapper;
        private readonly ValidationService _validationService;
        private readonly TokenService _tokenService;


        public async Task<ResponseDatosTurno> GuardarReserva(RequestDatosTurno datosTurno)
        {
            try
            {
                ResponseDatosTurno response = new ResponseDatosTurno();

                if (await _validationService.validateReserva(datosTurno))
                {
                    Turno turno = new Turno()
                    {
                        Nombre = datosTurno.Cliente,
                        Apellido = "Completar", //TODO: Pasar apellido desde la pantalla separado del nombre
                        Email = datosTurno.Email,
                        FechaHora = Convert.ToDateTime(datosTurno.Fecha + datosTurno.Hora),
                        Profesional_Id = datosTurno.ProfesionalId,
                        Telefono = datosTurno.Telefono,
                        Activo = true,
                        Token = _tokenService.GenerateGuidToken()

                    };
                    var TurnoGeneradoDB = _dbWrapper.AddTurno(turno);

                    response.Success = true;
                    response.Reserva_Id = TurnoGeneradoDB.Result.Turno_Id;
                    response.Estado = "C";
                    response.Mensaje = "Turno guardado Correctamente";
                    response.TurnoConfirmado = new ResponseTurnoConfirmado()
                    {
                        Cliente = datosTurno.Cliente, //TODO: Pasar apellido desde la pantalla separado del nombre
                        Email = datosTurno.Email,
                        Fecha = datosTurno.Fecha,
                        Hora = datosTurno.Hora,
                        Profesional = datosTurno.Profesional, //TODO: Pasar Profesional_Id desde el controller, tomando en cuenta la url 
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



        public async Task<ResponseCancelarReserva> CancelarReserva(RequestCancelarReserva turno)
        {
            try
            {
                ResponseCancelarReserva response = new ResponseCancelarReserva();

                if (await _validationService.ValidateCancelarReserva(turno))
                {
                    await _dbWrapper.CancelarTurno(turno.Reserva_Id);

                    response.Success = true;
                    response.Estado = "Cancelada";
                    response.Mensaje = $"La reserva {turno.Reserva_Id} ha sido cancelada correctamente";


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
                ResponseCancelarReserva response = new ResponseCancelarReserva();

                response.Success = true;
                response.Mensaje = $"Hubo un error al cancelar la reserva";
                return response;
            }

        }
    }
}
