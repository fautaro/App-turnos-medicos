using BusinessEntity.Models.Request;
using BusinessEntity.Models.Response;
using BusinessEntity.Request;
using BusinessEntity.Response;
using DataAccess.Models;
using DataAccess.Services;
using Microsoft.AspNetCore.Components.Forms;
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
        private readonly DateTime fechaActual = DateTime.Now;


        public ValidationService(DbWrapper dbWrapper)
        {
            _dbWrapper = dbWrapper;
        }



        #region Guardar Turno


        //Método para validar que el mail ya no tenga un turno seleccionado en la semana

        public async Task<bool> validateUsuarioTieneTurno(RequestDatosTurno turno)
        {
            DateTime FechaHora = DateTime.ParseExact($"{turno.Fecha} {turno.Hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

            //Comprobar que ese mail no se encuentre asociado a un turno vigente existente
            // Obtener el día de la semana del turno solicitado
            DayOfWeek diaTurno = FechaHora.DayOfWeek;

            // Obtener la fecha del primer día de la semana (domingo) en el que se encuentra el turno solicitado
            DateTime primerDiaSemana = FechaHora.Date.AddDays(-(int)diaTurno);

            // Obtener la fecha del último día de la semana (sábado) en el que se encuentra el turno solicitado
            DateTime ultimoDiaSemana = primerDiaSemana.AddDays(6);
            if (await _dbWrapper.GetTurnosSemana(turno.ProfesionalId, turno.Email, FechaHora, diaTurno, primerDiaSemana, ultimoDiaSemana))
            {
                return false;
            }
            return true;
        }

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
        public async Task<ResponseCancelarTurno> ValidateCancelarReserva(RequestCancelarTurno turno)
        {
            ResponseCancelarTurno response = new ResponseCancelarTurno();

            if (turno == null || turno.Token == null || turno.Profesional_Id == null)
            {
                response.Success = false;
                response.Mensaje = "El turno que intentas cancelar no ha sido encontrado.";
                return response;
            };


            var TurnoDb = await _dbWrapper.CheckToken(turno.Token);


            if (TurnoDb == null || !TurnoDb.Activo)
            {
                response.Success = false;
                response.Mensaje = "El turno que intentas cancelar no ha sido encontrado.";
                return response;
            };

            if (TurnoDb.FechaHora < fechaActual)
            {
                response.Success = false;
                response.Mensaje = "El turno que intentas cancelar ya ha caducado.";
                return response;
            };

            response.Success = true;
            return response;


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
                response.Profesional_Id = 0;
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
            // Obtener TurnosReservados y HorariosPermitidos
            var TurnosReservados = await _dbWrapper.GetTurnosReservados(profesional_Id);
            var HorariosPermitidos = await _dbWrapper.GetHorariosPermitidos(profesional_Id);

            // Crear una lista para almacenar los días bloqueados
            var ListaDiasBloqueados = new List<string>();

            // Agrupar los turnos reservados por día
            var turnosPorDia = TurnosReservados.GroupBy(turno => turno.FechaHora.Date);

            foreach (var grupo in turnosPorDia)
            {
                var fecha = grupo.Key;

                // Obtener los minutos de los horarios permitidos para el día
                var minutosHorariosPermitidos = HorariosPermitidos.Select(horario => horario.Hora.Hours * 60 + horario.Hora.Minutes);

                // Obtener los minutos de los horarios reservados para el día
                var minutosHorariosReservados = grupo.Select(turno => turno.FechaHora.Hour * 60 + turno.FechaHora.Minute);

                // Verificar si todos los horarios permitidos para el día están reservados
                if (minutosHorariosPermitidos.All(hora => minutosHorariosReservados.Contains(hora)))
                {
                    ListaDiasBloqueados.Add(fecha.ToString("yyyy/MM/dd"));
                }
            }

            return ListaDiasBloqueados;
        }




        public async Task<List<string>> GetHorasDisponibles(RequestGetHorasDisponibles request)
        {
            var response = new List<string>();
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
            if (fechaBuscada.Equals(fechaActual.Date))
            {
                response = GetHorariosPermitidos
                .Where(horario => !horariosReservadosDelDia.Contains(horario.Hora) && horario.Hora > fechaActual.TimeOfDay)
                .Select(horario => horario.Hora.ToString(@"hh\:mm"))
                .ToList();
            }
            else
            {
                response = GetHorariosPermitidos
                .Where(horario => !horariosReservadosDelDia.Contains(horario.Hora))
                .Select(horario => horario.Hora.ToString(@"hh\:mm"))
                .ToList();
            }

            return response;
        }


        public async Task<bool> ValidarDiaActual(int Profesional_Id)
        {
            //Horarios que trabaja el profesional todos los días
            var GetHorariosPermitidos = await _dbWrapper.GetHorariosPermitidos(Profesional_Id);
            var GetTurnosDelDia = await _dbWrapper.GetTurnosReservadosDelDia(Profesional_Id);

            // Filtrar horarios permitidos eliminando los horarios ya reservados
            var horariosLibres = GetHorariosPermitidos
                .Where(horario => !GetTurnosDelDia.Any(turno =>
                    turno.FechaHora.TimeOfDay == horario.Hora &&
                    turno.FechaHora.Date == fechaActual.Date))
                .ToList();

            // Filtrar horarios permitidos eliminando los horarios en el pasado
            var horariosLibresSinPasado = horariosLibres
                .Where(horario => fechaActual.TimeOfDay < horario.Hora)
                .ToList();

            // Validar si hay al menos un horario libre para el día actual
            var hayHorariosLibres = horariosLibresSinPasado.Any();

            return hayHorariosLibres;

        }


        #endregion Metodos Load Aplicación


    }
}
