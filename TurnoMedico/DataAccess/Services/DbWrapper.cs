using DataAccess.Context;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{

    public class DbWrapper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DateTime fechaActual = DateTime.Now;


        public DbWrapper(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // Agregar un nuevo turno
        public async Task<Turno> AddTurno(Turno turno)
        {
            try
            {
                await _dbContext.Turno.AddAsync(turno);
                await _dbContext.SaveChangesAsync();

                return turno;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Eliminar un turno
        public async Task<bool> CancelarTurno(int TurnoId)
        {
            try
            {
                var TurnoACancelar = await _dbContext.Turno.Where(e => e.Turno_Id == TurnoId).FirstOrDefaultAsync();
                TurnoACancelar.Activo = false;
                _dbContext.Turno.Update(TurnoACancelar);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }



        // Metodos para Capa de validación

        //Metodo que devuelve true si el usuario ya tiene un turno para la semana seleccionada
        public async Task<bool> GetTurnosSemana(int profesional_Id, string email, DateTime fecha, DayOfWeek diaTurno, DateTime primerDiaSemana, DateTime ultimoDiaSemana)
        {
            try
            {
                // Verificar si existe algún turno en la misma semana y con el mismo correo electrónico
                var turnoExistente = await _dbContext.Turno
                    .AnyAsync(e => e.Profesional_Id == profesional_Id &&
                                   e.Email == email &&
                                   e.FechaHora >= primerDiaSemana && e.FechaHora <= ultimoDiaSemana);

                return turnoExistente;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<Turno>> GetTurnosReservados(int profesional_Id)
        {
            try
            {
                var TurnosReservados = await _dbContext.Turno.Where(e => e.Profesional_Id == profesional_Id && e.Activo && e.FechaHora > fechaActual).ToListAsync();

                return TurnosReservados;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<Horario>> GetHorariosPermitidos(int profesional_Id)
        {

            try
            {
                var HorariosPermitidos = await _dbContext.Horario.Where(e => e.Profesional_Id == profesional_Id).ToListAsync();

                return HorariosPermitidos;


            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<AgendaBloqueada>> GetAgendaBloqueada(int profesional_Id)
        {
            try
            {

                List<AgendaBloqueada> agendaBloqueadaList = await _dbContext.AgendaBloqueada
                    .Where(e => e.Profesional_Id == profesional_Id && e.Activo && e.FechaDesde > fechaActual)
                    .ToListAsync();

                // Devolver la lista obtenida
                return agendaBloqueadaList;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<Turno> CheckTurno(DateTime fechaHora, int profesionalId)
        {
            try
            {
                var TurnoRepetido = await _dbContext.Turno.Where(e => e.FechaHora == fechaHora && e.Profesional_Id == profesionalId).FirstOrDefaultAsync();

                return TurnoRepetido;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<Turno> CheckToken(int TurnoId, string token)
        {
            try
            {
                var TurnoACancelar = await _dbContext.Turno.Where(e => e.Token == token).FirstOrDefaultAsync();

                return TurnoACancelar;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<Turno> CheckReservaId(int TurnoId, string token)
        {
            try
            {
                var TurnoACancelar = await _dbContext.Turno.Where(e => e.Turno_Id == TurnoId).FirstOrDefaultAsync();

                return TurnoACancelar;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public async Task<Profesional> GetProfesional(string profesional)
        {
            try
            {
                var Profesional = await _dbContext.Profesional.Where(e => e.Alias == profesional).FirstOrDefaultAsync();

                return Profesional;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
