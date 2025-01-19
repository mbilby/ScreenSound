using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class ArtistasExtensions
    {
        public static void AddEndPointArtistas(this WebApplication app) 
        {
            /*Artistas*/
            #region Endpoint Artistas
            app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
            {
                ICollection<ArtistaResponse> artistas = EntityListToResponseList(dal.Listar());
                return Results.Ok(artistas);
            });

            app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
            {
                Artista artista = dal.RecuperaPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));

                if (artista is null) Results.NotFound();

                return Results.Ok(artista);
            });

            app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
            {
                Artista artista = new Artista(artistaRequest.nome, artistaRequest.bio, artistaRequest.fotoPerfil);
                dal.Adicionar(artista);
                return Results.Ok();
            });

            app.MapPatch("/Artistas/{id}", ([FromServices] DAL<Artista> dal, [FromBody] Artista artista, int id) =>
            {
                Artista artistaAtual = dal.RecuperaPor(a => a.Id.Equals(id));
                if (artistaAtual is null)
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
            #endregion

        }

        private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
        {
            return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
        }

        private static ArtistaResponse EntityToResponse(Artista artista)
        {
            return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
        }
    }
}
