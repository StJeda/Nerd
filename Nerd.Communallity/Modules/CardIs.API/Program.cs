using Nerd.CardIs.API.Endpoints;
using Nerd.CardIs.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

builder.AddCardIsServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddCardIsEndpoints();

app.UseHttpsRedirection();

app.Run();
