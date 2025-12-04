using System.Threading.Tasks;
using Bibliotheque.Model;

namespace Bibliotheque.Services
{
    public interface IAuthService
    {
        Task<ResultatAuthentification> AuthentifierAsync(string email, string motDePasse);
    }
}
