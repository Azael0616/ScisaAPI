using ScisaAPI.Models;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace ScisaAPI.Utils
{
    public static class Utils
    {        
        public static async Task<Pokemon> Obtener_pokemon_por_ID(HttpClient http, int id)
        {
            Pokemon _pokemon = new Pokemon();
            //Se realiza la petición
            var respuesta = await http.GetAsync("https://pokeapi.co/api/v2/pokemon/" + id);
            //En caso de que haya error, retorna un objeto vacío.
            if (!respuesta.IsSuccessStatusCode)
                return _pokemon;

            //Se lee la información del JSON
            var contenido = await respuesta.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(contenido);
            var root = doc.RootElement;

            //Se asigna la información al objeto
            var nombre = root.GetProperty("name").GetString();
            var imagen = root.GetProperty("sprites").GetProperty("front_default").GetString();

            _pokemon.Id = id;
            _pokemon.Nombre = nombre != null ? nombre.ToUpper() : "";
            _pokemon.Imagen = imagen != null ? imagen : "";

            return _pokemon;
        }
        public static async Task<Pokemon> Obtener_pokemon_por_Nombre(HttpClient http, string nombrePokemon)
        {
            Pokemon _pokemon = new Pokemon();
            //Se realiza la petición
            var respuesta = await http.GetAsync("https://pokeapi.co/api/v2/pokemon/" + nombrePokemon);
            //En caso de que haya error, retorna un objeto vacío.
            if (!respuesta.IsSuccessStatusCode)
                return _pokemon;

            //Se lee la información del JSON
            var contenido = await respuesta.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(contenido);
            var root = doc.RootElement;

            //Se asigna la información al objeto
            var id = root.GetProperty("id").GetString();
            var nombre = root.GetProperty("name").GetString();
            var imagen = root.GetProperty("sprites").GetProperty("front_default").GetString();

            _pokemon.Id = id != null ? int.Parse(id) : 0;
            _pokemon.Nombre = nombre != null ? nombre.ToUpper() : "";
            _pokemon.Imagen = imagen != null ? imagen : "";

            return _pokemon;
        }
        public static async Task<List<PokemonEspecie>> Obtener_filtro_Especies(HttpClient http)
        {
            List<PokemonEspecie> listaDeEspecies = new List<PokemonEspecie>();
            //Se realiza la petición
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
                    var nombre = item.GetProperty("name").GetString();

                    _especie.Nombre = nombre != null ? nombre.ToUpper() : "";
                    _especie.Id = i;
                    listaDeEspecies.Add(_especie);
                }
            }
            return listaDeEspecies;
        }
        public static void GuardarObjetoEnSession<T>(this ISession session, string key, T value)
        {
            //Convierte la información del objeto en JSON
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        public static T? ObtenerObjetoDeSession<T>(this ISession session, string key)
        {
            //Convierte la información del JSON en objeto
            var json = session.GetString(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}
