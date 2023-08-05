using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Request
{
    public class RequestGetHorasDisponibles
    {
        public int Profesional_Id { get; set; }
        public string Fecha { get; set; }
    }
}
