using BusinessEntity.Models.Request;
using BusinessEntity.Request;
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



        #region Guardar Turno
        //Método para validar si el turno que se intenta guardar es válida
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
        #endregion Guardar Turno


        #region Cancelar Turno
        //Método para validar el turno que se intenta cancelar
        public async Task<bool> ValidateCancelarReserva(RequestCancelarTurno turno)
        {
            if (turno == null || turno.Token == null || turno.Turno_Id == null) return false;


            var Token = await _dbWrapper.CheckToken(turno.Turno_Id, turno.Token);
            var Reserva = await _dbWrapper.CheckReservaId(turno.Turno_Id, turno.Token);


            if (Token == null || Reserva == null) return false;

            if (Token.Equals(Reserva)) return true;

            return false;
        }
        #endregion Cancelar Turno



        #region Metodos Load Aplicación
        //Método para validar que el profesional recibido por querystring turnos.com/profesional esté activo y sea válido
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


        //Método para convertir los campos FechaDesde y FechaHasta de la base de datos en un array de fechas
        public async Task<List<string>> ConvertToArrayFechas(DateTime desde, DateTime hasta)
        {
            List<string> resultado = new List<string>();
            DateTime fechaActual = desde;

            while (fechaActual <= hasta)
            {
                resultado.Add(fechaActual.ToString("yyyy/MM/dd"));
                fechaActual = fechaActual.AddDays(1);
            }

            return resultado;
        }


        //Método para validar los dias completos segun los turnos cargados. Si un prof. trabaja 2 horarios por dia, y dichos turnos estan completos, entonces el dia esta completo
        public async Task<List<string>> GetDiasTurnosCompletos(int profesional_Id)
        {
            //Get TurnosReservados y HorariosPermitidos
            var TurnosReservados = await _dbWrapper.GetTurnosReservados(profesional_Id);
            var HorariosPermitidos = await _dbWrapper.GetHorariosPermitidos(profesional_Id);



            //Lógica para generar los días bloqueados
            var ListaDiasBloqueados = new List<string>();

            var turnosPorDia = TurnosReservados.GroupBy(turno => turno.FechaHora.Date);

            foreach (var fecha in turnosPorDia.Select(grupo => grupo.Key))
            {
                var todosLosHorariosReservados = false;

                var horariosDelDia = HorariosPermitidos.Select(horario => horario.Hora);

                foreach (var horarioReservado in turnosPorDia.First(grupo => grupo.Key == fecha))
                {
                    // Obtener solo la hora del turno reservado
                    var horaReservada = horarioReservado.FechaHora.TimeOfDay;

                    // Verificar si la hora del turno reservado está en los horarios permitidos para el día
                    if (!horariosDelDia.Contains(horaReservada))
                    {
                        // Si hay algún horario no reservado, entonces no está completo
                        todosLosHorariosReservados = true;
                        break;
                    }
                }

                if (todosLosHorariosReservados)
                {
                    ListaDiasBloqueados.Add(fecha.ToString("yyyy/MM/dd"));
                }
            }
            return ListaDiasBloqueados;
        }


        public async Task<List<string>> GetHorasDisponibles(RequestGetHorasDisponibles request)
        {

            //Convierto el string en pantalla a un objeto DateTime
            DateTime fechaBuscada = DateTime.ParseExact(request.Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //Horarios que trabaja el profesional todos los días
            var GetHorariosPermitidos = await _dbWrapper.GetHorariosPermitidos(request.Profesional_Id);
            var GetHorariosReservados = await _dbWrapper.GetTurnosReservados(request.Profesional_Id);

            // Obtener los horarios reservados del día en formato TimeSpan
            var horariosReservadosDelDia = GetHorariosReservados
                .Where(turno => turno.FechaHora.Date == fechaBuscada.Date)
                .Select(turno => turno.FechaHora.TimeOfDay)
                .ToList();

            // Obtener los horarios disponibles (horarios permitidos que no están reservados)
            var horariosDisponibles = GetHorariosPermitidos
                .Where(horario => !horariosReservadosDelDia.Contains(horario.Hora))
                .Select(horario => horario.Hora.ToString(@"hh\:mm"))
                .ToList();

            return horariosDisponibles;
        }




        #endregion Metodos Load Aplicación


    }
}
