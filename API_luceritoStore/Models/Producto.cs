namespace API_luceritoStore.Models
{
    
    public class Producto
    {
        public int IdProducto { get; set; }
        public int IdCategoria { get; set; }
        public string NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public string Detalle { get; set; }
        public string ImagenProducto { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
    }
}