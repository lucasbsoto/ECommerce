using ECommerce.Application.BackgroundJobs;
using ECommerce.Application.BackgroundJobs.Abstractions;
using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using ECommerce.Application.Services.Abstractions;
using ECommerce.Application.Validations;
using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure._Core;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.ExternalServices;
using ECommerce.Infrastructure.ExternalServices.Abstractions;
using ECommerce.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=ecommerce.db"));

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

builder.Services.Configure<BillingServiceOptions>(
    builder.Configuration.GetSection(BillingServiceOptions.BillingService));

builder.Services.AddHttpClient<IBillingHttpClient, BillingHttpClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BillingServiceOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);

});

builder.Services.AddScoped<IValidator<SaleRequest>, SaleRequestValidator>();
builder.Services.AddScoped<IValidator<CustomerRequest>, CustomerRequestValidator>();
builder.Services.AddScoped<IValidator<ItemRequest>, ItemRequestValidator>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IRepository<Sale>, SaleRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();

builder.Services.AddSingleton<IBillingQueueService, InMemoryBillingQueueService>();
builder.Services.AddScoped<IBillingHttpClient, BillingHttpClient>();
builder.Services.AddHostedService<BillingProcessorService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();