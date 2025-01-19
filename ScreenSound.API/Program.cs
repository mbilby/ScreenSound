using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ScreenSoundContext>();
builder.Services.AddTransient<DAL<Artista>>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json
    .JsonOptions>(opt =>
    {
        opt.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;       
    });

var app = builder.Build();

app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
{    
    return Results.Ok(dal.Listar());
});

app.MapGet("/Artistas/{nome}", ([FromServices] DAL < Artista > dal, string nome) =>
{
    Artista artista =  dal.RecuperaPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));

    if (artista is null) Results.NotFound();

    return Results.Ok(artista);
});

app.MapPost("/Artistas", ([FromServices] DAL < Artista > dal, [FromBody]Artista artista) =>
{    
    dal.Adicionar(artista);
    return Results.Ok();
});

app.MapPatch("/Artistas/{id}", ([FromServices] DAL<Artista> dal, [FromBody] Artista artista, int id) =>
{
    Artista artistaAtual = dal.RecuperaPor(a => a.Id.Equals(id));
    if(artistaAtual is null)
    {
        Results.NoContent();
    }
    artistaAtual.Nome = artista.Nome;
    artistaAtual.Bio = artista.Bio;
    artistaAtual.FotoPerfil = artista.FotoPerfil;
    dal.Atualizar(artistaAtual);
    return Results.Ok();
});

app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) =>
{
    Artista artista = dal.RecuperaPor(a => a.Id.Equals(id));
    if (artista is null)
    {
        return Results.NoContent();
    }
    dal.Deletar(artista);
    return Results.Ok();
});

app.Run();
