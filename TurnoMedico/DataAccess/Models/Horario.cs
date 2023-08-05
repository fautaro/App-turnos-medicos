using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Horario
    {
        public int Horario_Id { get; set; }
        public int Profesional_Id { get; set; }

        public TimeSpan Hora { get; set; }

    }
}
