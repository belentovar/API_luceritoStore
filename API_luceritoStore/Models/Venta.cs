namespace API_luceritoStore.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }

        public List<DetalleVenta> Detalle { get; set; }

    }
}
