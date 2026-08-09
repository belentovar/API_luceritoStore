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

            if(producto == null) {
                return 0; //si el producto esta vacio
            }
            if (producto.IdProducto <= 0) return -1;
            if (producto.Precio <= 0) return -2; //si el precio es menor igual a cero
            if (producto.Stock < 0) return -3; //si el stock es negativo
            if (string.IsNullOrWhiteSpace(producto.NombreProducto)) return -4; // si es null, Empty, White Space
            if (producto.NombreProducto.Length > 25) return -5;

            if (!catDatos.ExisteCategoria(producto.IdCategoria))
            {
                return -6; // La categoría enviada no es válida
            }

            Producto? productoExistente = prodDatos.obtenerProductoPorId(producto.IdProducto);

            if (productoExistente == null)
            {
                return -7; // El producto no existe
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

        public int agregarProducto(int idCarrito, int idProducto, int cantidad)
        { 
            
        }
    }
}
