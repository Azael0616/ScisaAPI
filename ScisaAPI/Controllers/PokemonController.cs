using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScisaAPI.Models;
using ScisaAPI.Models.Filtros;
using System.Text.Json;

namespace ScisaAPI.Controllers
{
    public class PokemonController : Controller
    {
        private readonly HttpClient _http;
        public PokemonController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
        }
        //Petición principal
        [HttpGet]
        public async Task<IActionResult> Listado(Filtro filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            List<PokemonEspecie> listaEspecies = await Utils.Utils.Obtener_filtro_Especies(_http);
            ViewBag.ListaEspecies = listaEspecies;
            for (int i = 0; i < 10; i++)
            {
                Pokemon _pokemon = await Utils.Utils.Obtener_pokemon_por_ID(_http, (i + 1));
                lista.Add(_pokemon);
            }
            
            return View(lista);
        }
    }
}
