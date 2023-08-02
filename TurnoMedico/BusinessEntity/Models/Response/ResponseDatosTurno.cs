using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Models.Response
{
    public class ResponseDatosTurno
    {
        public int Reserva_Id { get; set; }
        public string Estado { get; set; }
        public bool Success { get; set; }
        public string Mensaje { get; set; }

        public ResponseTurnoConfirmado TurnoConfirmado { get; set; }
    }
    

    public class ResponseTurnoConfirmado
    {
        public string? Cliente { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public string? Profesional { get; set; }

    }
}
