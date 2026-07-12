namespace API_luceritoStore.Models
{
    public class Carrito
    {
        public int idCarrito { get; set; }
        public int idUsuario { get; set; }
        public Usuario Usuario { get; set; }

    }
}
