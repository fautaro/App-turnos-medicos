using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class AgendaBloqueada
    {
        public int AgendaBloqueada_Id { get; set; }

        public int Profesional_Id { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public bool Activo { get; set; }

    }
}
