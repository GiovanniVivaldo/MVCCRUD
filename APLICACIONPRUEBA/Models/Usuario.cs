namespace APLICACIONPRUEBA.Models
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
    }

}
