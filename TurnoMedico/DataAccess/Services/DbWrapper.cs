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

        public DbWrapper(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public List<AgendaBloqueada> GetAgendaBloqueada(int profesional_Id)
        {
            try
            {
                List<AgendaBloqueada> agendaBloqueadaList = _dbContext.AgendaBloqueada.Where(e => e.Profesional_Id == profesional_Id && e.Activo).ToList();

                // Devolver la lista obtenida
                return agendaBloqueadaList;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Agregar un nuevo turno
        public Turno AddTurno(Turno turno)
        {
            try
            {
                _dbContext.Turno.Add(turno);
                _dbContext.SaveChanges();

                return turno;
            }
            catch (Exception ex)
            {

                throw;
            }


        }


        // Eliminar un turno
        public bool CancelarTurno(Turno turno)
        {
            try
            {
                var TurnoACancelar = _dbContext.Turno.Where(e => e.Turno_Id == turno.Turno_Id).FirstOrDefault();
                TurnoACancelar.Activo = false;

                _dbContext.Turno.Update(TurnoACancelar);

                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }
    }
}
