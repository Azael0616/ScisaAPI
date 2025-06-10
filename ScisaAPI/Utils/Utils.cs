using ScisaAPI.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace ScisaAPI.Utils
{
    public static class Utils
    {                
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
