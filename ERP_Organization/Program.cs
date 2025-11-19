using ERP_Organization.Services.UserService;
using ERP_OrganizationService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers only (no Swagger)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // optional
    });

// 2. DbContext
builder.Services.AddDbContext<OrganizationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OrganizationConnection")
        ?? throw new InvalidOperationException("Connection string 'OrganizationConnection' not found.")
    ));

// 3. Your UserService (inherits BaseService<User>)
builder.Services.AddScoped<IUserService, UserService>();

// 4. MassTransit + RabbitMQ – FULLY WORKING (this is the correct way in .NET 8)
builder.Services.AddMassTransit(x =>
{
    // Add your consumers here later (e.g., OrderCreated ? InventoryService)
    // x.AddConsumer<SomeConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbit = builder.Configuration.GetSection("RabbitMQ");

        cfg.Host(rabbit["Host"] ?? "localhost", "/", h =>
        {
            h.Username(rabbit["Username"] ?? "guest");
            h.Password(rabbit["Password"] ?? "guest");
        });

        // Optional: Configure retry, dead-letter, etc.
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Pipeline – No Swagger, No Auth Middleware (Ocelot handles JWT)
app.UseHttpsRedirection();
app.MapControllers();

app.Run();