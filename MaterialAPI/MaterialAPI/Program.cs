using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MaterialAPI.Data;
using MaterialAPI;
using MaterialAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MaterialAPIContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MaterialAPIContext") ?? throw new InvalidOperationException("Connection string 'MaterialAPIContext' not found.")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IService, Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCategoryEndpoints();

app.Run();
