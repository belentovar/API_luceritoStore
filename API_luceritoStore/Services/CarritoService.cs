using API_luceritoStore.Data;
using API_luceritoStore.Models;

namespace API_luceritoStore.Services
{
    public class CarritoService
    {
        private readonly UsuariosDatos usuariosDatos;
        private readonly CarritoDatos carritoDatos;
        private readonly CarritoProductoDatos carritoProductoDatos;
        private readonly ProductosDatos productosDatos;

        public CarritoService(UsuariosDatos usuariosDatos, CarritoDatos carritoDatos, CarritoProductoDatos carritoProductoDatos, ProductosDatos productosDatos)
        {
            this.usuariosDatos = usuariosDatos;
            this.carritoDatos = carritoDatos;
            this.carritoProductoDatos = carritoProductoDatos;
            this.productosDatos = productosDatos;
        }
        public Carrito obtenerOCrearCarrito(int idUsuario)
        {

            if (idUsuario <= 0)
            {
                throw new InvalidOperationException("El id de usuario es invalido.");
            }

            if (!usuariosDatos.ExisteUsuarioPorId(idUsuario))
            {
                throw new InvalidOperationException("El idUsuario no existe");
            }

            Carrito? carrito = carritoDatos.obtenerCarritoPorUsuario(idUsuario);

            if (carrito == null)
            {
                int idNuevo = carritoDatos.crearCarrito(idUsuario);
                carrito = new Carrito() { idCarrito = idNuevo, idUsuario = idUsuario };
            }

            return carrito;
        }

        public List<CarritoProducto> verCarrito(int idUsuario)
        {

            // No se revalida el usuario: obtenerOCrearCarrito ya lo hace, y repetirlo
            // aca serian dos consultas de mas en cada llamada.
            Carrito carrito = obtenerOCrearCarrito(idUsuario);

            return carritoProductoDatos.obtenerProductosCarrito(carrito.idCarrito);
        }

        public int agregarProducto(int idUsuario, int idProducto, int cantidad)
        {

            if (idProducto <= 0)
            {
                throw new InvalidOperationException("El id del producto es invalido.");
            }

            if (cantidad <= 0)
            {
                throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
            }

            Carrito carrito = obtenerOCrearCarrito(idUsuario);

            Producto? producto = productosDatos.obtenerProductoPorId(idProducto);
            if (producto == null)
            {
                throw new InvalidOperationException("El producto no existe.");
            }

            CarritoProducto? existente = null;

            foreach (CarritoProducto cp in carritoProductoDatos.obtenerProductosCarrito(carrito.idCarrito))
            {
                if (cp.idProducto == idProducto)
                {
                    existente = cp;
                    break;
                }
            }

            int cantidadFinal = cantidad;
            if (existente != null)
            {
                cantidadFinal = existente.cantidad + cantidad;
            }

            if (cantidadFinal > producto.Stock)
            {
                throw new InvalidOperationException("Stock insuficiente.");
            }

            if (existente == null)
            {
                return carritoProductoDatos.agregarProducto(carrito.idCarrito, idProducto, cantidad);
            }

            return carritoProductoDatos.actualizarCantidad(existente.idCarritoProducto, cantidadFinal);
        }

        public int actualizarCantidad(int idUsuario, int idCarritoProducto, int cantidad)
        {

            if (cantidad < 0)
            {
                throw new InvalidOperationException("La cantidad no puede ser negativa.");
            }

            Carrito carrito = obtenerOCrearCarrito(idUsuario);
            CarritoProducto carritoProducto = obtenerCarritoProducto(carrito.idCarrito, idCarritoProducto);

            // Cantidad 0 se interpreta como "sacalo del carrito"
            if (cantidad == 0)
            {
                return carritoProductoDatos.eliminarProducto(idCarritoProducto);
            }

            Producto? producto = productosDatos.obtenerProductoPorId(carritoProducto.idProducto);
            if (producto == null)
            {
                throw new InvalidOperationException("El producto no existe.");
            }

            // Aca la cantidad es absoluta: reemplaza a la anterior, asi que se compara
            // tal cual contra el stock. En agregarProducto es relativa y hay que sumar.
            if (cantidad > producto.Stock)
            {
                throw new InvalidOperationException("Stock insuficiente.");
            }

            return carritoProductoDatos.actualizarCantidad(idCarritoProducto, cantidad);
        }

        public int eliminarProducto(int idUsuario, int idCarritoProducto)
        {

            Carrito carrito = obtenerOCrearCarrito(idUsuario);

            // Se llama aunque no se use el resultado: es la verificacion de pertenencia,
            // y si el renglon no es de este carrito lanza antes de borrar nada.
            obtenerCarritoProducto(carrito.idCarrito, idCarritoProducto);

            return carritoProductoDatos.eliminarProducto(idCarritoProducto);
        }

        public int vaciarCarrito(int idUsuario)
        {

            Carrito carrito = obtenerOCrearCarrito(idUsuario);

            return carritoDatos.vaciarCarrito(carrito.idCarrito);
        }


        private CarritoProducto obtenerCarritoProducto(int idCarrito, int idCarritoProducto)
        {
            if (idCarritoProducto <= 0)
            {
                throw new InvalidOperationException("El id del item del carrito es invalido.");
            }


            foreach (CarritoProducto cp in carritoProductoDatos.obtenerProductosCarrito(idCarrito))
            {
                if (cp.idCarritoProducto == idCarritoProducto)
                {
                    return cp;
                }
            }

            throw new InvalidOperationException("El producto no esta en el carrito.");
        } 

        public decimal obtenerTotalCarrito(int idUsuario)
        {

            decimal total = 0;
            List<CarritoProducto> carritoProductos = verCarrito(idUsuario);

            foreach (CarritoProducto item in carritoProductos)
            {
                total += item.cantidad * item.Producto.Precio;
            }

            return total;
        }
    }
}
