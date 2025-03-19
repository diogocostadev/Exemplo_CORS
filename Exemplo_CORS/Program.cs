var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuração de CORS detalhada
builder.Services.AddCors(options =>
{
    // Política permissiva para testes - NÃO USE EM PRODUÇÃO!
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    // Política restritiva para ambiente de produção
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder.WithOrigins("https://app.seudominio.com",
                           "https://admin.seudominio.com")
               .WithMethods("GET", "POST", "PUT", "DELETE")
               .WithHeaders("Authorization", "Content-Type")
               .AllowCredentials(); // Permite envio de cookies
    });

    // Política específica para ambiente de desenvolvimento
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
