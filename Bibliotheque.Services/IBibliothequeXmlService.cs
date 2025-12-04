using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheque.Model;

namespace Bibliotheque.Services
{
    public interface IBibliothequeXmlService
    {
        Task<IList<Livre>> ChargerLivresAsync();
        Task<IList<Compte>> ChargerComptesAsync();

        Task<Livre?> ObtenirLivreParIsbnAsync(string isbn);

        Task AjouterLivreAsync(Livre livre);
        Task SupprimerLivreParIsbnAsync(string isbn);

        Task MettreAJourEvaluationAsync(string isbnLivre, string emailClient, int nouvelleNote);
    }
}
