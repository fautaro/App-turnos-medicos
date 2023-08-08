using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ProfesionalResponse
    {
        public bool Activo { get; set; }
        public int Profesional_Id { get; set; }
        public string? Profesional { get; set; }

        public string? Titulo { get; set; }

        public string? Descripcion { get; set; }
        public byte[]? Imagen { get; set; } 


    }
}
