namespace ScisaAPI.Models
{
    public class PokemonEspecie
    {
        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public PokemonEspecie() { }
        public PokemonEspecie(int id, string nombre)
        {
            this.Id = id;
            this.Nombre = nombre;
        }
    }
}
