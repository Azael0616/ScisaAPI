namespace ScisaAPI.Models
{
    public class PokemonDetalle
    {
        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public int Altura { get; set; } = 0;
        public int Peso { get; set; } = 0;
        public string Habilidad { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
    }
}
