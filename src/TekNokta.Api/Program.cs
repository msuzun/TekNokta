using Microsoft.OpenApi;
using TekNokta.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

const string allowAllCorsPolicy = "AllowAll";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ı şu formatta girin: Bearer {token}"
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, externalResource: null),
            new List<string>()
        }
    });
});
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowAllCorsPolicy, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(allowAllCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
