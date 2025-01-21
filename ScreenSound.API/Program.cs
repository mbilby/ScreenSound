using Microsoft.EntityFrameworkCore;
using ScreenSound.API.Endpoints;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ScreenSoundContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:ScreenSoundDB"])
        .UseLazyLoadingProxies();
});
builder.Services.AddTransient<DAL<Artista>>();
builder.Services.AddTransient<DAL<Musica>>();
builder.Services.AddTransient<DAL<Genero>>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json
    .JsonOptions>(opt =>
    {
        opt.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;       
    });

WebApplication app = builder.Build();

//Endpoints
app.AddEndPointArtistas();
app.AddEndPointMusicas();

//SWAGGER
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
