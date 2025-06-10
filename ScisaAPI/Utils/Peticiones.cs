using ScisaAPI.Models;
using System.Text.Json;

namespace ScisaAPI.Utils
{
    public static class Peticiones
    {
        //Busca al pokemon por ID
        public static async Task<Pokemon> Obtener_pokemon_por_ID(HttpClient http, int id)
        {
            Pokemon _pokemon = new Pokemon();
            //Se realiza la petición asincrona
            var respuesta = await http.GetAsync("https://pokeapi.co/api/v2/pokemon/" + id);
            //En caso de que haya error, retorna un objeto vacío.
            if (!respuesta.IsSuccessStatusCode)
                return _pokemon;

            //Se lee la información del JSON
            var contenido = await respuesta.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(contenido);
            var root = doc.RootElement;

            //Se asigna la información al objeto
            string? nombre = root.GetProperty("name").GetString();
            string? imagen = root.GetProperty("sprites").GetProperty("front_default").GetString();            

            _pokemon.Id = id;
            _pokemon.Nombre = nombre != null ? nombre.ToUpper() : "";
            _pokemon.Imagen = imagen != null ? imagen : "";

            return _pokemon;
        }
        //Busca al pokemon por nombre
        public static async Task<Pokemon> Obtener_pokemon_por_Nombre(HttpClient http, string nombrePokemon)
        {
            Pokemon _pokemon = new Pokemon();
            //Se realiza la petición asincrona
            var respuesta = await http.GetAsync("https://pokeapi.co/api/v2/pokemon/" + nombrePokemon);
            //En caso de que haya error, retorna un objeto vacío.
            if (!respuesta.IsSuccessStatusCode)
                return _pokemon;

            //Se lee la información del JSON
            var contenido = await respuesta.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(contenido);
            var root = doc.RootElement;

            //Se asigna la información al objeto
            int id = root.GetProperty("order").GetInt32();
            string? nombre = root.GetProperty("name").GetString();
            string? imagen = root.GetProperty("sprites").GetProperty("front_default").GetString();

            _pokemon.Id = id != 0 ? id : 0;
            _pokemon.Nombre = nombre != null ? nombre.ToUpper() : "";
            _pokemon.Imagen = imagen != null ? imagen : "";

            return _pokemon;
        }
        //Busca al pokemon por especie
        public static async Task<Pokemon> Obtener_pokemon_por_Especie(HttpClient http, int id)
        {
            Pokemon _pokemon = new Pokemon();
            //Se realiza la petición asincrona
            var respuesta = await http.GetAsync("https://pokeapi.co/api/v2/pokemon-species/" + id);
            //En caso de que haya error, retorna un objeto vacío.
            if (!respuesta.IsSuccessStatusCode)
                return _pokemon;

            //Se lee la información del JSON
            var contenido = await respuesta.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(contenido);
            var root = doc.RootElement;

            //Se asigna la información al objeto            
            string? nombre = root.GetProperty("name").GetString();

            _pokemon.Id = id;
            _pokemon.Nombre = nombre != null ? nombre.ToUpper() : "";

            return _pokemon;
        }
        //Muestra las primeras 20 especies de pokemon para el dropdown
        public static async Task<List<PokemonEspecie>> Obtener_filtro_Especies(HttpClient http)
        {
            List<PokemonEspecie> listaDeEspecies = new List<PokemonEspecie>();
            //Se realiza la petición asincrona
            var respuesta = await http.GetAsync("https://pokeapi.co/api/v2/pokemon-species");
            //En caso de que haya error, retorna un objeto vacío.
            if (!respuesta.IsSuccessStatusCode)
                return listaDeEspecies;

            //Se lee la información del JSON            
            int i = 0;

            var contenido = await respuesta.Content.ReadAsStringAsync();

            var datos = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(contenido);

            if (datos == null)
                return listaDeEspecies;

            if (datos.TryGetValue("results", out var resultadosJson))
            {
                foreach (var item in resultadosJson.EnumerateArray())
                {
                    i++;
                    PokemonEspecie _especie = new PokemonEspecie();
                    string? nombre = item.GetProperty("name").GetString();

                    _especie.Nombre = nombre != null ? nombre.ToUpper() : "";
                    _especie.Id = i;
                    listaDeEspecies.Add(_especie);
                }
            }
            return listaDeEspecies;
        }
        //Obtiene mas detalles del pokemon para el modal
        public static async Task<PokemonDetalle> Obtener_detalle_pokemon_por_ID(HttpClient http, int id)
        {
            PokemonDetalle _pokemon = new PokemonDetalle();
            //Se realiza la petición asincrona
            var respuesta = await http.GetAsync("https://pokeapi.co/api/v2/pokemon/" + id);
            //En caso de que haya error, retorna un objeto vacío.
            if (!respuesta.IsSuccessStatusCode)
                return _pokemon;

            //Se lee la información del JSON
            var contenido = await respuesta.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(contenido);
            var root = doc.RootElement;

            //Se asigna la información al objeto
            string? nombre = root.GetProperty("name").GetString();
            string? imagen = root.GetProperty("sprites").GetProperty("front_default").GetString();
            int altura = root.GetProperty("height").GetInt32();
            int peso = root.GetProperty("weight").GetInt32();
            var habilidades = root.GetProperty("abilities");
            string? habilidad = "";
            if(habilidades.ValueKind != JsonValueKind.Null)
            {
                habilidad = root.GetProperty("abilities")[0].GetProperty("ability").GetProperty("name").GetString();
            }

            _pokemon.Id = id;
            _pokemon.Nombre = nombre != null ? nombre.ToUpper() : "";
            _pokemon.Altura = altura != 0 ? altura: 0;
            _pokemon.Peso = peso != 0 ? peso : 0;
            _pokemon.Habilidad = habilidad != null ? habilidad.ToUpper() : "";
            _pokemon.Imagen = imagen != null ? imagen : "";

            return _pokemon;
        }
    }
}
