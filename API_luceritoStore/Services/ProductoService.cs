using API_luceritoStore.Data;
using API_luceritoStore.Models;
using API_luceritoStore.Models.Errores;

namespace API_luceritoStore.Services
{
    public class ProductoService
    {
        private readonly categoriaDatos categoriaDatos;
        private readonly ProductosDatos productosDatos;

        public ProductoService(categoriaDatos categoriaDatos, ProductosDatos productosDatos)
        {
            this.categoriaDatos = categoriaDatos;
            this.productosDatos = productosDatos;
        }
        public int actualizarProducto(Producto producto)
        {

            // --- el objeto ---
            if (producto == null)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El producto no puede ser nulo.");
            }

            // --- los campos ---
            if (string.IsNullOrWhiteSpace(producto.Codigo))
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El codigo del producto es obligatorio.");
            }

            if (producto.Codigo.Length > LargoMaximoCodigo)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El codigo no puede superar los " + LargoMaximoCodigo + " caracteres.");
            }

            if (string.IsNullOrWhiteSpace(producto.NombreProducto))
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El nombre del producto es obligatorio.");
            }

            if (producto.NombreProducto.Length > LargoMaximoNombre)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El nombre no puede superar los " + LargoMaximoNombre + " caracteres.");
            }

            // --- los rangos ---
            if (producto.IdProducto <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El id del producto es invalido.");
            }

            if (producto.Precio <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El precio debe ser mayor a cero.");
            }

            if (producto.Stock < 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El stock no puede ser negativo.");
            }

            if (producto.IdCategoria <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El id de categoria es invalido.");
            }

            // --- la base ---
            if (!categoriaDatos.ExisteCategoria(producto.IdCategoria))
            {
                throw new NegocioException(ErrorNegocio.NoEncontrado, "La categoria no existe.");
            }

            if (productosDatos.obtenerProductoPorId(producto.IdProducto) == null)
            {
                throw new NegocioException(ErrorNegocio.NoEncontrado, "El producto no existe.");
            }

            return productosDatos.actualizarProducto(producto);
        }


        public List<Producto> listarProductosPorCategoria(int idCategoria)
        {
            if (!categoriaDatos.ExisteCategoria(idCategoria)) {
                throw new NegocioException(ErrorNegocio.NoEncontrado, "La categoría no existe.");
            }

            return productosDatos.listarProductosPorCategoria(idCategoria);
        }

    
        private const int LargoMaximoNombre = 30;
        private const int LargoMaximoCodigo = 20;

        public int agregarProducto(Producto producto)
        {

            // --- el objeto ---
            if (producto == null)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El producto no puede ser nulo.");
            }

            // --- los campos ---
            if (string.IsNullOrWhiteSpace(producto.Codigo))
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El codigo del producto es obligatorio.");
            }

            if (producto.Codigo.Length > LargoMaximoCodigo)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El codigo no puede superar los " + LargoMaximoCodigo + " caracteres.");
            }

            if (string.IsNullOrWhiteSpace(producto.NombreProducto))
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El nombre del producto es obligatorio.");
            }

            if (producto.NombreProducto.Length > LargoMaximoNombre)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El nombre no puede superar los " + LargoMaximoNombre + " caracteres.");
            }

            // --- los rangos ---
            if (producto.Precio <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El precio debe ser mayor a cero.");
            }

            if (producto.Stock < 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El stock no puede ser negativo.");
            }

            if (producto.IdCategoria <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El id de categoria es invalido.");
            }

            // --- la base ---
            if (!categoriaDatos.ExisteCategoria(producto.IdCategoria))
            {
                throw new NegocioException(ErrorNegocio.NoEncontrado, "La categoria no existe.");
            }

            if (productosDatos.ExisteProducto(producto.Codigo))
            {
                throw new NegocioException(ErrorNegocio.Duplicado, "Ya existe un producto con ese codigo.");
            }

            return productosDatos.AgregarProducto(producto);
        }

        public List<Producto> listarProductos()
        {
            return productosDatos.listarProductos();
        }

        public Producto? obtenerProductoPorId(int idProducto)
        {
            if (idProducto <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El id del producto es invalido.");
            }

            return productosDatos.obtenerProductoPorId(idProducto);
        }

        public List<Producto> listarProductosPorNombre(string nombreProduc)
        {
            if (string.IsNullOrWhiteSpace(nombreProduc))
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El nombre a buscar es obligatorio.");
            }

            return productosDatos.listarProductosPorNombre(nombreProduc.Trim());
        }

        public int darDeBajaProducto(int idProducto)
        {
            if (idProducto <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El id del producto es invalido.");
            }

            if (productosDatos.obtenerProductoPorId(idProducto) == null)
            {
                throw new NegocioException(ErrorNegocio.NoEncontrado, "El producto no existe.");
            }

            return productosDatos.darDeBajaProducto(idProducto);
        }

        public int darDeAltaProducto(int idProducto)
        {
            if (idProducto <= 0)
            {
                throw new NegocioException(ErrorNegocio.DatosInvalidos, "El id del producto es invalido.");
            }


            if (productosDatos.obtenerProductoPorId(idProducto) == null)
            {
                throw new NegocioException(ErrorNegocio.NoEncontrado, "El producto no existe.");
            }

            return productosDatos.darDeALtaProducto(idProducto);
        }
    }
}
