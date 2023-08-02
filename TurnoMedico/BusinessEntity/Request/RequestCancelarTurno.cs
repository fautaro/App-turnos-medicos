using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Models.Request
{
    public class RequestCancelarTurno
    {
        public int Turno_Id { get; set; }
        public string Token { get; set; }

    }
}
