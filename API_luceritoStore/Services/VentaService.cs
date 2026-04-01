using API_luceritoStore.Data;
using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Services
{
    public class VentaService
    {
        public int insertarVenta(Venta venta)
        {
            VentaDatos ventaDatos = new VentaDatos();
            UsuariosDatos usuariosDatos = new UsuariosDatos();

            Producto producto = new Producto();
            ProductosDatos productosDatos = new ProductosDatos();

            if (usuariosDatos.obtenerUsuarioPorId(venta.IdUsuario) == null) //si el usuario no existe
            {
                return 0;
            }

            if (venta.Detalle == null || venta.Detalle.Count == 0) //si la lista no existe o si la lista esta vacia
            {
                return 0;
            }

            venta.Total = 0; //lo reinicio para que no acumule el total que viene del front

            foreach (DetalleVenta detalle in venta.Detalle) {

                int idProducto = detalle.idProducto;
                producto = productosDatos.obtenerProductoPorId(idProducto);

                if (producto == null) //si el producto no existe
                { 
                    return 0;
                }

                if (detalle.Cantidad > producto.Stock) //si no hay suficiente stock
                {
                    return 0;
                }

                if (detalle.Cantidad <= 0) //se valida la cantidad
                {
                    return 0;
                }

                detalle.PrecioUnitario = producto.Precio; //guardo precioU ahora por si dspues cambia el precio del prod

                venta.Total += producto.Precio * detalle.Cantidad; //validar total o devolver total real
            }

            foreach (DetalleVenta detalle in venta.Detalle)
            {
                Producto produ = productosDatos.obtenerProductoPorId(detalle.idProducto);

                produ.Stock -= detalle.Cantidad;
                productosDatos.actualizarProducto(produ);
            }

            venta.Fecha = DateTime.Now; //asignar fecha
            venta.Estado = EstadoVenta.Pendiente;// asignar estado

            return ventaDatos.insertarVenta(venta);
        }

      
        public int actualizarEstadoVenta(int IdVenta, string Estado)
        {
            VentaDatos ventaDatos = new VentaDatos();
            Venta venta = new Venta();

            venta = ventaDatos.obtenerVentaPorId(IdVenta);
            if (venta == null)
            {
                return 0;
            }

            if (Estado != EstadoVenta.Pendiente &&
                Estado != EstadoVenta.Pagado &&
                Estado != EstadoVenta.Enviado &&
                Estado != EstadoVenta.Cancelado)
            {
                return 0;
            }

            return ventaDatos.actualizarEstadoVenta(IdVenta, Estado);
        }
    }
}
