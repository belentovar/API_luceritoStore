using API_luceritoStore.Data;
using API_luceritoStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_luceritoStore.Services
{
    public class VentaService
    {
        private readonly VentaDatos ventaDatos;
        private readonly UsuariosDatos usuariosDatos;
        private readonly ProductosDatos productosDatos;
        private readonly LocalidadDatos localidadDatos;
        private readonly DetalleVentaDatos detalleVentaDatos;
        private readonly CarritoService carritoService;

        public VentaService(CarritoService carritoService, VentaDatos ventaDatos, UsuariosDatos usuariosDatos, ProductosDatos productosDatos, LocalidadDatos localidadDatos, DetalleVentaDatos detalleVentaDatos)
        {
            this.ventaDatos = ventaDatos;
            this.usuariosDatos = usuariosDatos;
            this.productosDatos = productosDatos;
            this.localidadDatos = localidadDatos;
            this.detalleVentaDatos = detalleVentaDatos;
            this.carritoService = carritoService;
        }

        public int checkoutDesdeCarrito(int idUsuario, string metodoPago, string tipoEntrega, string direccion, int idLocalidad)
        {
            List<CarritoProducto> carritoProductos = carritoService.verCarrito(idUsuario);

            if (carritoProductos == null || carritoProductos.Count == 0)
            {
                throw new InvalidOperationException("El carrito esta vacio, no se puede generar la venta.");
            }

            List<DetalleVenta> detalles = new List<DetalleVenta>();

            foreach (CarritoProducto item in carritoProductos)
            {
                DetalleVenta detalle = new DetalleVenta()
                {
                    idProducto = item.idProducto,
                    Cantidad = item.cantidad
                };

                detalles.Add(detalle);
            }

            Venta venta = new Venta()
            {
                IdUsuario = idUsuario,
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                MetodoPago = metodoPago,
                TipoEntrega = tipoEntrega,
                Direccion = direccion,
                IdLocalidad = idLocalidad,
                Detalle = detalles
            };

            int idVenta = insertarVenta(venta);

            carritoService.vaciarCarrito(idUsuario);

            return idVenta;
        }
        public int insertarVenta(Venta venta)
        {
            Producto producto = new Producto();

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


            // --- Validacion del envio ---
            // El metodo de pago NO se valida aca: ya lo garantiza el CHECK constraint en la BDD.
            string tipoEntrega = venta.TipoEntrega?.ToLower();

            if (tipoEntrega != "envio" && tipoEntrega != "retiro")
            {
                throw new InvalidOperationException("Tipo de entrega invalido");
            }

            if (tipoEntrega == "envio")
            {
                // Envio: la direccion es obligatoria.
                if (string.IsNullOrWhiteSpace(venta.Direccion))
                {
                    throw new InvalidOperationException("La direccion es obligatoria para envio");
                }

                // La localidad tiene que existir Y estar activa. obtenerLocalidadPorId ya filtra
                // activo=1, asi que si devuelve null es porque no existe o esta dada de baja.
                
                if (localidadDatos.obtenerLocalidadPorId(venta.IdLocalidad) == null)
                {
                    throw new InvalidOperationException("La localidad no existe o no esta habilitada para envio");
                }
            }
            else // retiro: no se envia a ningun lado, se limpian los datos de envio.
            {
                venta.Direccion = null;
                venta.IdLocalidad = 0; // VentaDatos convierte 0 -> DBNull
            }

            venta.Fecha = DateTime.Now; //asignar fecha
            venta.Estado = EstadoVenta.Pendiente;// asignar estado

            return ventaDatos.insertarVenta(venta);
        }

      
        public int actualizarEstadoVenta(int IdVenta, string Estado)
        {
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

        public Venta? obtenerVentaPorId(int idVenta)
        {
            // Se consulta DetalleVentaDatos y no VentaDatos: VentaDatos.obtenerVentaPorId trae solo
            // la cabecera y deja Detalle como lista vacia, asi que la venta salia sin los productos
            // vendidos (total sin renglones). obtenerVentaConDetalle resuelve cabecera y detalle en
            // una sola consulta con JOIN.
            // Tampoco hace falta chequear existencia antes: obtenerVentaConDetalle ya devuelve null
            // cuando la venta no existe. Consultar primero para preguntar si existe era un viaje
            // extra a la base para traer la misma fila dos veces.
            if (idVenta <= 0)
            {
                return null;
            }

            return detalleVentaDatos.obtenerVentaConDetalle(idVenta);
        }
    }
}
