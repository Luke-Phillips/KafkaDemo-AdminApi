using AdminApi.Data;
using AdminApi.Kafka;
using AdminApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Numbers", async () => {
    using (var db = new AdminApiContext())
    {
        return (await db.SavedNumbers.ToListAsync()).OrderBy(n => n.Value);
    }
})
.WithName("GetNumbers")
.WithOpenApi();

app.MapPut("/Numbers", async (int number) => {
    using (var db = new AdminApiContext())
    {
        SavedNumber? numberToBan = await db.SavedNumbers.FirstOrDefaultAsync(n => n.Value == number);
        if (numberToBan == null)
        {
            return Results.BadRequest("Number does not exist");
        }
        if (numberToBan.IsBanned)
        {
            return Results.BadRequest("Number is already banned");
        }
        numberToBan.IsBanned = true;
        await db.SaveChangesAsync();
        KafkaProducer.Produce("numbers", "banNumber", numberToBan.Value.ToString());
    }

    return Results.Ok();
})
.WithName("BanNumber")
.WithOpenApi();

app.Run();