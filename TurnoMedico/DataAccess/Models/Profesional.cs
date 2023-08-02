using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Profesional
    {
        public int Profesional_Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Alias { get; set; }
        public int Profesion_Id { get; set; }
        public int Usuario_Id { get; set; }
        public bool? Activo { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Imagen { get; set; } // Nueva propiedad para la imagen

    }

}