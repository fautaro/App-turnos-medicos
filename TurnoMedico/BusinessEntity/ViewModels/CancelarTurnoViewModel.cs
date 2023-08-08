namespace BusinessEntity.ViewModels
{
    public class CancelarTurnoViewModel
    {
        public bool Success { get; set; }
        public string? Mensaje { get; set; }
        public string? Profesional { get; set; }
        public string? FechaHora { get; set; }
        public string? Paciente { get; set; }
        public int Profesional_Id { get; set; }
        public string? Token { get; set; }


    }
}
