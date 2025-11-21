using ERP_AuthService.Services.AuthService;
using ERP_AuthService.Services.JwtTokenService;
using ERP_Clients.Services.OrganizationClient;
using ERP_Models.Entities.Data.ERP_AuthService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Register Clients
builder.Services.AddHttpClient<IOrganizationClient, OrganizationClient>((sp, client) =>
{
    var url = sp.GetRequiredService<IConfiguration>()["ServiceUrls:OrganizationService"]
              ?? throw new InvalidOperationException("Missing OrganizationService URL");
    client.BaseAddress = new Uri(url);
});

// Register Services
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();