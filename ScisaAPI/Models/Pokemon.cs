namespace ScisaAPI.Models
{
    public class Pokemon
    {
        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public Pokemon() { }
        public Pokemon(int id, string nombre, string imagen) {
            this.Id = id;
            this.Nombre = nombre;
            this.Imagen = imagen;
        }
    }
}
