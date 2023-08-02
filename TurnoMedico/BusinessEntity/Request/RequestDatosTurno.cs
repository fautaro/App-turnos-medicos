using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Models.Request
{
    public class RequestDatosTurno
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Profesional { get; set; }
        public int ProfesionalId { get; set; }



    }
}