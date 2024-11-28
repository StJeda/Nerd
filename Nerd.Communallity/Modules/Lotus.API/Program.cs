using Nerd.Lotus.API.Endpoints;
using Nerd.Lotus.API.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Lotus.API.xml"));
});

builder.AddLotusServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddLotusEndpoints();

app.UseHttpsRedirection();

app.Run();
