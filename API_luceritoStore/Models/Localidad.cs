namespace API_luceritoStore.Models
{
    public class Localidad
    {
        public int IdLocalidad { get; set; }
        public int IdProvincia { get; set; }
        public string NombreLocalidad { get; set; }

        // opcional: si querés traer el nombre de la provincia junto con la localidad
        public string NombreProvincia { get; set; }
        public bool Activo { get; set; }
    }
}
