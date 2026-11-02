using API_luceritoStore.Data;
using API_luceritoStore.Models;
using API_luceritoStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Preparador - caja vacía donde vas a ir metiendo todo lo que tu app necesita antes de abrir
var builder = WebApplication.CreateBuilder(args);

// Aviso que voy a usar controllers y que prepare todo lo necesario para eso
builder.Services.AddControllers();

// Registro las clases en el contenedor de inyeccion de dependencias.
// AddScoped = crea una instancia nueva por cada pedido HTTP y la reusa durante todo ese pedido.
builder.Services.AddScoped<AccesoDatos>();
builder.Services.AddScoped<categoriaDatos>();
builder.Services.AddScoped<DetalleVentaDatos>();
builder.Services.AddScoped<CarritoDatos>();
builder.Services.AddScoped<CarritoProductoDatos>();
builder.Services.AddScoped<LocalidadDatos>();
builder.Services.AddScoped<ProductosDatos>();
builder.Services.AddScoped<ProvinciaDatos>();
builder.Services.AddScoped<UsuariosDatos>();
builder.Services.AddScoped<VentaDatos>();
builder.Services.AddScoped<favoritosDatos>();

builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<VentaService>();


// Estos 2 son para el Swagger (página web que .NET te genera sola)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// "cuando llegue un token, verificá que esté firmado con MI clave secreta. No me importa quién lo emitió ni para quién es."
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


// Agarrás la caja que fuiste llenando y construís la aplicación de verdad. (dspues de esta linea NO se pueden agregar mas servicios)
var app = builder.Build();

// Si estoy en mi computadora desarrollando, activá Swagger.
// El if está porque no querés que la documentación de tu API sea pública en producción.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Si alguien entra por http:// (inseguro), lo redirige a https:// (cifrado).
app.UseHttpsRedirection();

//Autenticación = ¿QUIÉN SOS?
app.UseAuthentication();

//Autorización = ¿PODÉS HACER ESTO?
app.UseAuthorization();

// Conectá las URLs con los métodos de mis controllers.
app.MapControllers();

// Abrí la persiana y quedate atendiendo.
app.Run();



