using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ResponseGetHorasDisponibles
    {
        public bool Success { get; set; }
        public List<string>? HorasDisponibles { get; set; }
    }
}
