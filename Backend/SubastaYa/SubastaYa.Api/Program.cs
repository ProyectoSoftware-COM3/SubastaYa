using Microsoft.EntityFrameworkCore;
using SubastaYa.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Configuracion de la base de datos.
builder.Services.AddDbContext<SubastaYaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Referencia para el Hash Se corrio esta linea una sola vez, se copio el resultado impreso por consola, y se borro.
// Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Test123!"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

//Para usar el DbSeeder, se crea un scope para obtener el contexto de la base de datos y luego se llama al método SeedAsync.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SubastaYaDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.Run();
