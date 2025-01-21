using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class MusicasExtensions
    {
        public static void AddEndPointMusicas(this WebApplication app) 
        {
            /*Músicas*/
            #region Endpoint Musicas
            app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
            {
                IEnumerable<Musica> musicas = dal.Listar();

                ICollection<MusicaResponse> retorno = EntityListToResponseList(musicas);
                
                return Results.Ok(retorno);
            });

            app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
            {
                Musica musica = dal.RecuperaPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));

                if (musica is null) Results.NotFound();

                return Results.Ok(musica);
            });

            app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL < Genero > dalGenero, [FromBody] MusicaRequest musicaRequest) =>
            {
                            
                Musica musica = new Musica(musicaRequest.nome, musicaRequest.artistaId, musicaRequest.anoLancamento)
                {
                    Generos = musicaRequest.generos is not null ? GeneroRequestConverter(musicaRequest.generos, dalGenero) : new List<Genero>()
                };

                dal.Adicionar(musica);
                return Results.Ok();
            });

            app.MapPatch("/Musicas/{id}", ([FromServices] DAL<Musica> dal, [FromBody] Musica musica, int id) =>
            {
                Musica musicaAtual = dal.RecuperaPor(a => a.Id.Equals(id));
                if (musicaAtual is null)
                {
                    Results.NoContent();
                }
                musicaAtual.Nome = musica.Nome;
                musicaAtual.AnoLancamento = musica.AnoLancamento;
                musicaAtual.Artista = musica.Artista;
                dal.Atualizar(musicaAtual);
                return Results.Ok();
            });

            app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) =>
            {
                Musica musica = dal.RecuperaPor(a => a.Id.Equals(id));
                if (musica is null)
                {
                    return Results.NoContent();
                }
                dal.Deletar(musica);
                return Results.Ok();
            });
            #endregion
        }

        private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> generoDAL)
        {
            List<Genero> listaDeGeneros = new List<Genero>();
            foreach(GeneroRequest generoRequest in generos)
            {
                Genero entity = RequestToEnty(generoRequest);
                Genero genero = generoDAL.RecuperaPor(g => g.Nome.Equals(generoRequest.nome));

                if (genero is not null)
                {
                    listaDeGeneros.Add(genero);
                } 
                else
                {
                    listaDeGeneros.Add(entity);
                }

            }
            return listaDeGeneros;
        }

        private static Genero RequestToEnty(GeneroRequest genero)
        {            
                return new Genero()
                {
                    Nome = genero.nome,
                    Descricao = genero.descricao,
                };                       
        }

        private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
        {
            return musicaList.Select(a => EntityToResponse(a)).ToList();
        }

        private static MusicaResponse EntityToResponse(Musica musica)
        {
            DAL<Artista> dalArtista = new DAL<Artista>(new ScreenSoundContext());
            Artista artista = dalArtista.RecuperaPor(a => a.Id.Equals(musica.ArtistaId));

            if (artista is not null)
            {
                musica.Artista = artista;
            }

            return new MusicaResponse(musica.Id, musica.Nome!, musica.Artista!.Id, musica.Artista!.Nome);
        }
    }
}
