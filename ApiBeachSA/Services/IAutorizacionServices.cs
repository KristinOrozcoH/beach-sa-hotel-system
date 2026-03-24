using APIBeachSA.Services; 
using ApiBeachSA.Model;

namespace ApiBeachSA.Services
{
    public interface IAutorizacionServices
    {
        Task<AutorizacionResponse> DevolverToken(Usuario usuario);
    } //Cierre interface
} //Cierre namespace 

