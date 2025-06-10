using ScisaAPI.Models;

namespace ScisaAPI.Utils.Interfaces
{
    public interface IServicioCorreo
    {
        Task EnviarCorreo(List<Pokemon> lista);
    }
}
