using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using People.Api.ApiModels;
using People.Api.Exceptions;
using People.Api.Mappings;
using People.Api.Services;
using People.Api.Services.Interfaces;
using People.Data.Context;
using People.Data.Exceptions;
using System.ComponentModel;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddTransient<IPersonService, PersonService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (exceptionHandlerFeature != null)
        {
            string message = "";

            var exception = exceptionHandlerFeature.Error;

            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case ArgumentNullException argumentNullException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    message = "Parameter is required";
                    break;

                case ArgumentOutOfRangeException argumentOutOfRangeException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    message = "Invalid parameter";
                    break;

                case InvalidNameException invalidNameException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    message = "Name can't be empty";
                    break;

                case InvalidPersonIdException invalidPersonIdException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    message = "Invalid person's id";
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    message = "Internal Server Error";
                    break;
            }

            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                MoreInfo = exceptionHandlerFeature.Error.Message
            });
        }
    });

});


app.MapGet("/health", () =>
    {
        Results.Ok();
    })
    .WithName("HealthCheck")
    .WithSummary("Health Check")
    .WithDescription("Returns HTTP 200 OK liveness checks.")
    .WithOpenApi();


app.MapPost("/Add", async ([Description("The person information to be added.")] NewPerson newPerson, IPersonService personService) =>
    {
        var person = await personService.AddPersonAsync(newPerson);
        return PersonMapping.DBPersonToPersonApi(person);
    })
    .WithName("AddNewPerson")
    .WithSummary("Add a new person")
    .WithDescription("This API adds a new person into the database.")
    .WithOpenApi();

app.MapPut("/Update", async ([Description("The person information to be updated.")] PersonApi updatePerson, IPersonService personService) =>
    {
        var person = await personService.UpdatePersonAsync(updatePerson);
        return PersonMapping.DBPersonToPersonApi(person);
    })
    .WithName("UpdatePerson")
    .WithSummary("Updates an existing person")
    .WithDescription("This API updates an existing person in the database.")
    .WithOpenApi();

app.MapDelete("/Delete", async ([Description("The id of the person to be deleted.")] int id, IPersonService personService) =>
    {
        await personService.DeletePersonAsync(id);
        Results.Ok();
    })
    .WithName("DeletePerson")
    .WithSummary("Deletes an existing person")
    .WithDescription("This API deletes an existing person from the database.")
    .WithOpenApi();

app.MapGet("/List", async (IPersonService personService) =>
    {
        var list = await personService.GetAllAsync();
        return PersonMapping.DBPersonToPersonApi(list).ToList();
    })
    .WithName("ListAllPerson")
    .WithSummary("Listing all existing persons")
    .WithDescription("This API lists all existing persons in the database.")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

