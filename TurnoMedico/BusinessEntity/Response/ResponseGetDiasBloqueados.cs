using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ResponseGetDiasBloqueados
    {
        public bool Success { get; set; }
        public List<string>? DiasBloqueados { get; set; }


    }
}
