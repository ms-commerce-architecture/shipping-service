using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using shipping_service_backend.Data;
using shipping_service_backend.Kafka;
using shipping_service_backend.Mapper;
using shipping_service_backend.Services;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);


   

    // ─── AJOUTER CETTE LIGNE ICI ──────────────────────────────────────────
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());
    // ──────────────────────────────────────────────────────────────────────

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddDbContext<AppDbContext>(option =>
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IShipmentService, ShipmentService>();
    builder.Services.AddScoped<ShipmentMapper>();
    builder.Services.AddScoped<TrackingEventMapper>();
    builder.Services.AddEndpointsApiExplorer();


    builder.Services.AddHostedService<OrderEventConsumer>();

    var app = builder.Build();

    // Optionnel mais recommandé : Enregistre proprement le passage des requêtes HTTP (GET/POST)
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

   
    app.UseAuthorization();
    app.MapControllers();
   

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
