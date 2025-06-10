using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScisaAPI.Models;
using ScisaAPI.Models.Filtros;
using System.Linq;
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
            //Obtiene las primeras 20 especies para el dropdown
            List<PokemonEspecie> listaEspecies = await Utils.Utils.Obtener_filtro_Especies(_http);
            ViewBag.ListaEspecies = listaEspecies;
            //Obtiene los primeros 10 pokemon
            for (int i = 0; i < 20; i++)
            {
                Pokemon _pokemon = await Utils.Utils.Obtener_pokemon_por_ID(_http, (i + 1));
                lista.Add(_pokemon);
            }            

            //Para la paginación
            int total = lista.Count;
            int registrosPorPagina = 5;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)registrosPorPagina);
            ViewBag.PaginaActual = filtro.Pagina;

            List<Pokemon> listaPaginada = lista.Skip((filtro.Pagina - 1) * registrosPorPagina).Take(registrosPorPagina).ToList();

            return View(listaPaginada);
        }
    }
}
