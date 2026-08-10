namespace API_luceritoStore.Models
{
    
    public class Producto
    {
        public int IdProducto { get; set; }
        public int IdCategoria { get; set; }
        // Clave de negocio del producto: unica y estable. El nombre no sirve para eso
        // porque puede repetirse entre productos distintos y cambia con el tiempo.
        public string Codigo { get; set; }
        public string NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public string Detalle { get; set; }
        public string ImagenProducto { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
    }
}