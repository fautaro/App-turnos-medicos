using BusinessEntity.Models.Request;
using BusinessEntity.Response;
using DataAccess.Models;
using DataAccess.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Services
{
    public class ValidationService
    {
        private DbWrapper _dbWrapper;

        public ValidationService(DbWrapper dbWrapper)
        {
            _dbWrapper = dbWrapper;
        }

        public async Task<bool> validateReserva(RequestDatosTurno turno)
        {
            if (turno == null || turno.Fecha == null || turno.Hora == null || turno.Nombre == null || turno.Apellido == null || turno.ProfesionalId == null || turno.Email == null) return false;

            DateTime FechaHora = DateTime.ParseExact($"{turno.Fecha} {turno.Hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);


            // Comprobar si el turno ya existe

            var TurnoExistente = await _dbWrapper.CheckTurno(FechaHora, turno.ProfesionalId);

            if (TurnoExistente != null) return false;

            var HorarioBloqueado = await _dbWrapper.GetAgendaBloqueada(turno.ProfesionalId);

            // Comprobar si la fecha y hora proporcionadas están dentro de algún rango de horario bloqueado
            foreach (var bloqueo in HorarioBloqueado)
            {
                if (FechaHora >= bloqueo.FechaDesde && FechaHora <= bloqueo.FechaHasta)
                {
                    return false;
                }
            }

            //Compruebo que la fecha del turno sea mayor al momento actual donde se desea guardar el turno
            if (FechaHora < DateTime.Now)
            {
                return false;
            }

            // Verificar si la Fecha y hora del turno es sábado o domingo
            if (FechaHora.DayOfWeek == DayOfWeek.Saturday || FechaHora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }


            // Verificar si los minutos son diferentes de 0 o 30
            if (FechaHora.Minute % 30 != 0)
            {
                return false;
            }


            //El turno es correcto
            return true;


        }


        public async Task<bool> ValidateCancelarReserva(RequestCancelarTurno turno)
        {
            if (turno == null || turno.Token == null || turno.Turno_Id == null) return false;


            var Token = await _dbWrapper.CheckToken(turno.Turno_Id, turno.Token);
            var Reserva = await _dbWrapper.CheckReservaId(turno.Turno_Id, turno.Token);


            if (Token == null || Reserva == null) return false;

            if (Token.Equals(Reserva)) return true;

            return false;
        }


        public async Task<ProfesionalResponse> ValidateProfesional(string profesional)
        {

            ProfesionalResponse response = new ProfesionalResponse();
            var Profesional = await _dbWrapper.GetProfesional(profesional);

            if (Profesional == null || Profesional.Activo.Equals(false))
            {
                response.Activo = false;
                response.Profesional_Id = null;
                return response;

            }

            response.Titulo = Profesional.Titulo;
            response.Descripcion = Profesional.Descripcion;
            response.Profesional_Id = Profesional.Profesional_Id;
            response.Activo = Profesional.Activo == true ? true : false;
            response.Imagen = Profesional.Imagen;
            response.Profesional = $"{Profesional.Nombre} {Profesional.Apellido}";

            return response;
        }

    }
}
