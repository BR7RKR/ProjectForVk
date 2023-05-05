using Microsoft.OpenApi.Models;
using ProjectForVk.Infrastructure.Database.DateOnlySupport;
using ProjectForVk.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize DateOnly as strings
    x.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    x.JsonSerializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());
});
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

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