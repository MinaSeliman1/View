using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheque.Model;

namespace Bibliotheque.Services
{
    public interface IFavorisService
    {
        Task<IList<Livre>> ChargerFavorisAsync(string emailClient);
        Task MettreAJourFavoriAsync(string emailClient, Livre livre);
    }
}
