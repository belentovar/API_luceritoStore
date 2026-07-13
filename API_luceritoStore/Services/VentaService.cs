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
                    throw new Exception("No existe el producto");
                }

                if (detalle.Cantidad > producto.Stock) //si no hay suficiente stock
                {
                    throw new Exception("No hay suficiente stock");
                }

                if (detalle.Cantidad <= 0) //se valida la cantidad
                {

                    throw new InvalidOperationException("Error en la cantidad");
                }

                detalle.PrecioUnitario = producto.Precio; //guardo precioU ahora por si dspues cambia el precio del prod

                venta.Total += producto.Precio * detalle.Cantidad; //validar total o devolver total real
            }

            // El descuento de stock ya NO se hace aca. Se movio adentro de la transaccion de
            // VentaDatos.insertarVenta (UPDATE guardado con WHERE stock >= cantidad) para que sea
            // atomico: o se registra la venta y se descuenta el stock juntos, o no pasa nada.
            // Antes se descontaba aca en conexiones sueltas: si fallaba el insert el stock quedaba
            // mal, y ante pedidos simultaneos habia sobreventa (race condition).
            // El chequeo de stock del bucle de arriba queda como validacion temprana/amigable,
            // pero la garantia real ahora la da la base de datos dentro de la transaccion.

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
