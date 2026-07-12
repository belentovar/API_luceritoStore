namespace API_luceritoStore.Models
{
    public class DetalleVenta
    {
        public int IdDetalleVenta { get; set; }

        public int IdVenta { get; set; }
        public Producto Producto { get; set; }
        public int idProducto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        
    }
}
