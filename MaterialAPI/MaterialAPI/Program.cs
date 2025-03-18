using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MaterialAPI.Data;
using MaterialAPI;
using MaterialAPI.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MaterialAPIContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MaterialAPIContext") ?? throw new InvalidOperationException("Connection string 'MaterialAPIContext' not found.")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Host.ConfigureDefaults(args).ConfigureWebHostDefaults(webBuilder =>
//{
//    webBuilder.UseUrls("http://localhost:5003", "https://localhost:5004");
//});
//JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
//{
//    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//};

//    .Converter.Add(options => options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
//builder.Services.Configure<JsonSerializerOptions>(options => options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

//builder.Services.AddControllers().AddJsonOptions(options => 
//{
//    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//});
//JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
//builder.Services.AddOptions<JsonSerializerOptions>().
builder.Services.AddScoped<IService, Service>();

var AllowedOrigins = "_allowedOrigins";
builder.Services.AddCors(options =>
{
    //options.AddPolicy(name: AllowedOrigins, policy => { policy.WithOrigins("http://localhost:3007"); });
    //options.AddPolicy(name: AllowedOrigins, policy => { policy.WithOrigins("http://localhost:3007"); policy.AllowAnyMethod(); });
    options.AddPolicy(name: AllowedOrigins, p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Material")),
    RequestPath = "/Material"
});


app.UseHttpsRedirection();

app.MapCategoryEndpoints();
app.MapBatchEndpoints();


app.UseCors(AllowedOrigins);

app.Run();
