using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Models.Response
{
    public class ResponseCancelarReserva
    {
        public int Reserva_Id { get; set; }
        public string Estado { get; set; }
        public bool Success { get; set; }
        public string Mensaje { get; set; }

    }
}
