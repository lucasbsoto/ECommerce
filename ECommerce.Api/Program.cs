using ECommerce.Application.BackgroundJobs;
using ECommerce.Application.BackgroundJobs.Abstractions;
using ECommerce.Application.Services;
using ECommerce.Application.Services.Abstractions;
using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.ExternalServices;
using ECommerce.Infrastructure.ExternalServices.Abstractions;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore; // Adicione esta linha no topo do arquivo

var builder = WebApplication.CreateBuilder(args);

// 1. Infraestrutura (EF Core + SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=ecommerce.db"));

// Cria o DB se não existir (apenas para o desafio com SQLite)
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

builder.Services.AddHttpClient<IBillingHttpClient, BillingHttpClient>(client =>
{
     client.BaseAddress = new Uri("https://sti3-faturamento.azurewebsites.net/api/vendas"); // Boa prática em projetos reais
});

// 2. Repositório
builder.Services.AddScoped<IRepository<Sale>, SaleRepository>();

// 3. Aplicação/Serviços
builder.Services.AddScoped<SaleService>();
builder.Services.AddSingleton<IBillingQueueService, InMemoryBillingQueueService>(); // Implementar esta classe
builder.Services.AddScoped<IBillingHttpClient, BillingHttpClient>(); // Implementar esta classe
builder.Services.AddHostedService<BillingProcessorService>(); // Fase 2

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();