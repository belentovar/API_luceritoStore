namespace API_luceritoStore.Models
{
    public class CarritoProducto
    {
        public int idCarritoProducto { get; set; }
        public int idCarrito { get; set; }
        public int idProducto { get; set; }
        public int cantidad { get; set; }
        public Producto Producto { get; set; }
    }
}
