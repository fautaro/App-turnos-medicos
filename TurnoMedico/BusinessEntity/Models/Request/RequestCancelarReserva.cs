using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Models.Request
{
    public class RequestCancelarReserva
    {
        public int Reserva_Id { get; set; }
        public string Token { get; set; }

    }
}
