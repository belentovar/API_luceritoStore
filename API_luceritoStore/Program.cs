using API_luceritoStore.Data;
using API_luceritoStore.Models;
//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

DetalleVentaDatos detalleVentaDatos = new DetalleVentaDatos();
List<Venta> ventas = new List<Venta>();

ventas = detalleVentaDatos.listarDetalleVenta(1);

foreach (Venta venta in ventas)
{
    Console.WriteLine("IdUsuario: " + venta.IdUsuario);

    foreach(DetalleVenta detalleVenta in venta.Detalle)
    {
        Console.WriteLine("Cantidad: " + detalleVenta.Cantidad);
        Console.WriteLine("Nombre Producto: " + detalleVenta.Producto.NombreProducto);
        Console.WriteLine("precio: " + detalleVenta.Producto.Precio);
    }
}

