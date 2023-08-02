using BusinessEntity.Models.Request;
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
        private readonly DbWrapper _dbWrapper;

        public async Task<bool> validateReserva(RequestDatosTurno turno)
        {
            if (turno == null || turno.Fecha == null || turno.Hora == null || turno.Cliente == null || turno.Profesional == null || turno.Email == null) return false;

            DateTime FechaHora = DateTime.ParseExact($"{turno.Fecha} {turno.Hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

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

            return true;

        }


        public async Task<bool> ValidateCancelarReserva(RequestCancelarReserva turno)
        {
            if (turno == null || turno.Token == null || turno.Reserva_Id == null) return false;


                var Token = await _dbWrapper.CheckToken(turno.Reserva_Id, turno.Token);
                var Reserva = await _dbWrapper.CheckReservaId(turno.Reserva_Id, turno.Token);


            if (Token == null || Reserva == null) return false;

            if (Token.Equals(Reserva)) return true;

            return false;
        }

    }
}
