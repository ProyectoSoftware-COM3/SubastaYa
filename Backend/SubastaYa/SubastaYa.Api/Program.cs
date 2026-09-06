using Microsoft.EntityFrameworkCore;
using SubastaYa.Application;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Infrastructure.Persistence;
using SubastaYa.Infrastructure.Repositories;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>cfg.RegisterServicesFromAssemblyContaining<AssemblyReference>());



//Configuracion de la base de datos.
builder.Services.AddDbContext<SubastaYaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

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
