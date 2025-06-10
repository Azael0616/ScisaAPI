using ScisaAPI.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace ScisaAPI.Utils
{
    public static class Utils
    {        
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
        // Expresiones regulares para detectar patrones maliciosos comunes
        private static readonly Regex[] PatronesMaliciosos =
        {
        // Detección mejorada de XSS (incluye variantes de <script>)
        new Regex(@"<script.*?>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled),
        new Regex(@"<script.*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled), // Etiqueta de apertura sola
        new Regex(@"</script>", RegexOptions.IgnoreCase | RegexOptions.Compiled), // Etiqueta de cierre sola
        new Regex(@"<.*?javascript:.*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"<.*?on\w+=.*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        
        // SQL Injection
        new Regex(@"\b(ALTER|CREATE|DELETE|DROP|EXEC(UTE)?|INSERT( +INTO)?|MERGE|SELECT|UPDATE|UNION( +ALL)?)\b",
                 RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"\b(AND|OR)\b\s*\d+\s*=\s*\d+", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"\b(DECLARE|EXEC|EXECUTE|CAST|CONVERT)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        
        // Otros patrones peligrosos
        new Regex(@"(\b|\s+)(https?|ftp|file):\/\/[^\s]*", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"\b(wget|curl|powershell|cmd|bash|sh)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"\.\.\/|\.\.\\", RegexOptions.Compiled),
        new Regex(@"\b(echo|print|alert|confirm|prompt|document\.cookie)\b",
                 RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"on(mouse|key|load|error|submit)\s*=", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"<\?php|\<\?|\?>", RegexOptions.Compiled)
    };

        // Método para verificar si un string contiene contenido malicioso
        public static bool TieneContenidoMalicioso(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Verificar cada patrón malicioso
            foreach (var pattern in PatronesMaliciosos)
            {
                if (pattern.IsMatch(input))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
