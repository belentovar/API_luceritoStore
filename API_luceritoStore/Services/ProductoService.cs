using API_luceritoStore.Data;
using API_luceritoStore.Models;

namespace API_luceritoStore.Services
{
    public class ProductoService
    {


        public int actualizarProducto(Producto producto)
        {
            categoriaDatos catDatos = new categoriaDatos();
            ProductosDatos prodDatos = new ProductosDatos();

            // --- el objeto ---
            if (producto == null)
            {
                throw new InvalidOperationException("El producto no puede ser nulo.");
            }

            // --- los campos ---
            if (string.IsNullOrWhiteSpace(producto.Codigo))
            {
                throw new InvalidOperationException("El codigo del producto es obligatorio.");
            }

            if (producto.Codigo.Length > LargoMaximoCodigo)
            {
                throw new InvalidOperationException("El codigo no puede superar los " + LargoMaximoCodigo + " caracteres.");
            }

            if (string.IsNullOrWhiteSpace(producto.NombreProducto))
            {
                throw new InvalidOperationException("El nombre del producto es obligatorio.");
            }

            if (producto.NombreProducto.Length > LargoMaximoNombre)
            {
                throw new InvalidOperationException("El nombre no puede superar los " + LargoMaximoNombre + " caracteres.");
            }

            // --- los rangos ---
            if (producto.IdProducto <= 0)
            {
                throw new InvalidOperationException("El id del producto es invalido.");
            }

            if (producto.Precio <= 0)
            {
                throw new InvalidOperationException("El precio debe ser mayor a cero.");
            }

            if (producto.Stock < 0)
            {
                throw new InvalidOperationException("El stock no puede ser negativo.");
            }

            if (producto.IdCategoria <= 0)
            {
                throw new InvalidOperationException("El id de categoria es invalido.");
            }

            // --- la base ---
            if (!catDatos.ExisteCategoria(producto.IdCategoria))
            {
                throw new InvalidOperationException("La categoria no existe.");
            }

            if (prodDatos.obtenerProductoPorId(producto.IdProducto) == null)
            {
                throw new InvalidOperationException("El producto no existe.");
            }

            return prodDatos.actualizarProducto(producto);
        }


        public List<Producto> listarProductosPorCategoria(int idCategoria)
        {
            ProductosDatos prodDatos = new ProductosDatos();
            categoriaDatos catDatos = new categoriaDatos();
            if (!catDatos.ExisteCategoria(idCategoria)) {
                throw new Exception("La categoría no existe.");
            }

            return prodDatos.listarProductosPorCategoria(idCategoria);
        }

        // Debe coincidir con el largo de la columna nombreProducto en la base.
        // Verificar en SSMS y ajustar: validar distinto del esquema rechaza nombres
        // que la base acepta, o deja pasar nombres que SQL Server trunca.
        private const int LargoMaximoNombre = 30;

        // Debe coincidir con el largo de la columna codigo en la base.
        private const int LargoMaximoCodigo = 20;

        public int agregarProducto(Producto producto)
        {
            // Las validaciones van de lo barato a lo caro: primero el objeto y sus campos,
            // despues los rangos, y al final las consultas a la base. Asi no se abre una
            // conexion para descubrir un error que se veia sin salir de memoria, y ningun
            // chequeo toca una propiedad de un objeto que todavia no se valido.

            // --- el objeto ---
            if (producto == null)
            {
                throw new InvalidOperationException("El producto no puede ser nulo.");
            }

            // --- los campos ---
            if (string.IsNullOrWhiteSpace(producto.Codigo))
            {
                throw new InvalidOperationException("El codigo del producto es obligatorio.");
            }

            if (producto.Codigo.Length > LargoMaximoCodigo)
            {
                throw new InvalidOperationException("El codigo no puede superar los " + LargoMaximoCodigo + " caracteres.");
            }

            if (string.IsNullOrWhiteSpace(producto.NombreProducto))
            {
                throw new InvalidOperationException("El nombre del producto es obligatorio.");
            }

            if (producto.NombreProducto.Length > LargoMaximoNombre)
            {
                throw new InvalidOperationException("El nombre no puede superar los " + LargoMaximoNombre + " caracteres.");
            }

            // --- los rangos ---
            if (producto.Precio <= 0)
            {
                throw new InvalidOperationException("El precio debe ser mayor a cero.");
            }

            if (producto.Stock < 0)
            {
                throw new InvalidOperationException("El stock no puede ser negativo.");
            }

            if (producto.IdCategoria <= 0)
            {
                throw new InvalidOperationException("El id de categoria es invalido.");
            }

            // --- la base ---
            categoriaDatos categoriaDatos = new categoriaDatos();
            if (!categoriaDatos.ExisteCategoria(producto.IdCategoria))
            {
                throw new InvalidOperationException("La categoria no existe.");
            }

            ProductosDatos productosDatos = new ProductosDatos();
            if (productosDatos.ExisteProducto(producto.Codigo))
            {
                throw new InvalidOperationException("Ya existe un producto con ese codigo.");
            }

            return productosDatos.AgregarProducto(producto);
        }
    }
}
