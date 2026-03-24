using APIBeachSA.Model;

namespace APIBeachSA.Services
{
    public interface IAutorizacionServices
    {
        Task<AutorizacionResponse> DevolverToken(Usuario usuario);
    } //
} //Cierre namespace

