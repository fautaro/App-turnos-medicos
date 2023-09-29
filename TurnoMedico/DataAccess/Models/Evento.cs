using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Evento
    {
        public int Evento_Id { get; set; }
        public string Usuario_Id { get; set; }
        public string Detalle { get; set; }
        public string Entidad { get; set; }
        public DateTime? FechaHora { get; set; }
    }
}
