var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configura��o de CORS detalhada
builder.Services.AddCors(options =>
{
    // Pol�tica permissiva para testes - N�O USE EM PRODU��O!
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    // Pol�tica restritiva para ambiente de produ��o
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder.WithOrigins("https://app.seudominio.com",
                           "https://admin.seudominio.com")
               .WithMethods("GET", "POST", "PUT", "DELETE")
               .WithHeaders("Authorization", "Content-Type")
               .AllowCredentials(); // Permite envio de cookies
    });

    // Pol�tica espec�fica para ambiente de desenvolvimento
    options.AddPolicy("DevPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000",
                           "http://localhost:8080")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevPolicy");
    app.MapOpenApi();
}
else
{
    app.UseCors("ProductionPolicy");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
