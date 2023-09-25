using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Notificacion
    {
        public int Notificacion_Id { get; set; }

        public int Profesional_Id { get; set; }

        public string Titulo { get; set; }

        public string? Descripcion { get; set; }

        public bool Leido { get; set; }
        public DateTime? FechaHoraEvento { get; set; }

        public bool? Eliminado { get; set; }

    }
}
