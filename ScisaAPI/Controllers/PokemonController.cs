using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScisaAPI.Models;
using ScisaAPI.Models.Filtros;
using ScisaAPI.Utils;
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
            List<PokemonEspecie> listaEspecies = new List<PokemonEspecie>();
            //Validación si ya está cargada la lista de especies
            var listaEspeciesEnSesion = HttpContext.Session.ObtenerObjetoDeSession<List<PokemonEspecie>>("ListaEspecies");
            if(listaEspeciesEnSesion != null && listaEspeciesEnSesion?.Count>0)
            {
                listaEspecies = listaEspeciesEnSesion;
            }
            else
            {
                //Obtiene las primeras 20 especies para el dropdown
                listaEspecies = await Utils.Utils.Obtener_filtro_Especies(_http);
                //Guardar la lista en una variable de sesion
                HttpContext.Session.GuardarObjetoEnSession("ListaEspecies", listaEspecies);
            }                            
            ViewBag.ListaEspecies = listaEspecies;
            //En caso de tener Nombre o EspecieID, hará busqueda nuevamente. Nota: en caso de tener ambos campos, el Nombre tiene PRIORIDAD
            if (filtro.Nombre != null && filtro.Nombre.Trim() != "")
            {
                Pokemon _pokemon = await Utils.Utils.Obtener_pokemon_por_Nombre(_http, filtro.Nombre.Trim().ToLower());
                lista.Clear();
                if(_pokemon.Nombre != "" && _pokemon.Nombre != null)
                {
                    lista.Add(_pokemon);
                    HttpContext.Session.GuardarObjetoEnSession("PokemonPorBusqueda", lista);
                }
                else
                {
                    //Limpia la variable de sesión de pokémon encontrado por busqueda
                    HttpContext.Session.Remove("PokemonPorBusqueda");
                }
            }
            else if(filtro.EspecieID != 0)
            {
                Pokemon _pokemon = await Utils.Utils.Obtener_pokemon_por_ID(_http, filtro.EspecieID);
                lista.Clear();
                if (_pokemon.Nombre != "" && _pokemon.Nombre != null)
                {
                    lista.Add(_pokemon);
                    HttpContext.Session.GuardarObjetoEnSession("PokemonPorBusqueda", lista);
                }
                else
                {
                    //Limpia la variable de sesión de pokémon encontrado por busqueda
                    HttpContext.Session.Remove("PokemonPorBusqueda");
                }
            }
            else
            {
                //Limpia la variable de sesión de pokémon encontrado por busqueda
                HttpContext.Session.Remove("PokemonPorBusqueda");
                //Validación si ya está cargada la lista de pokemones
                var listaPokemonEnSesion = HttpContext.Session.ObtenerObjetoDeSession<List<Pokemon>>("ListaPokemon");
                if (listaPokemonEnSesion != null && listaPokemonEnSesion?.Count > 0)
                {
                    lista = listaPokemonEnSesion;
                }
                else
                {
                    //Obtiene los primeros 20 pokemon
                    for (int i = 0; i < 20; i++)
                    {
                        Pokemon _pokemon = await Utils.Utils.Obtener_pokemon_por_ID(_http, (i + 1));
                        lista.Add(_pokemon);
                    }
                    //Guardar la lista en una variable de sesion
                    HttpContext.Session.GuardarObjetoEnSession("ListaPokemon", lista);
                }
            }                                       
            //Para la paginación
            int total = lista.Count;
            int registrosPorPagina = 5;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)registrosPorPagina);
            ViewBag.PaginaActual = filtro.Pagina;

            List<Pokemon> listaPaginada = lista.Skip((filtro.Pagina - 1) * registrosPorPagina).Take(registrosPorPagina).ToList();

            HttpContext.Session.GuardarObjetoEnSession("ListaPokemonPaginada", listaPaginada);

            return View(listaPaginada);
        }
        [HttpGet]
        public IActionResult ExportarExcel()
        {
            byte[] datos;
            //Validación si hay pokemon por busqueda
            var listaPokemonEnSesion = HttpContext.Session.ObtenerObjetoDeSession<List<Pokemon>>("PokemonPorBusqueda");
            if(listaPokemonEnSesion == null)
            {
                //Validación para obtener el listado pokemon de la sesion
                listaPokemonEnSesion = HttpContext.Session.ObtenerObjetoDeSession<List<Pokemon>>("ListaPokemonPaginada");
            }
            if(listaPokemonEnSesion != null && listaPokemonEnSesion.Count != 0)
            {
                //Si existe un pokemon por busqueda, sólo se exportará ese, en caso contrario, se exportarán los que se encuentren en la paginación actual
                datos = Utils.ExportarExcel.ExportarListadoPokemon(listaPokemonEnSesion);
                return File(datos,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ListadoPokemon.xlsx");
            }
            else
            {
                //En caso que no haya información
                TempData["Alerta"] = "No hay datos para exportar";
                return RedirectToAction("Listado");
            }                
        }
    }
}
