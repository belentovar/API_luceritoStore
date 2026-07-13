namespace API_luceritoStore.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int IdLocalidad { get; set; }
        public string Direccion { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }

        public List<DetalleVenta> Detalle { get; set; }

    }
}
