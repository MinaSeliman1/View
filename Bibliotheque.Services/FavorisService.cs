using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheque.Model;

namespace Bibliotheque.Services
{
    public class FavorisService : IFavorisService
    {
        private readonly string _cheminFichier;

        public FavorisService(string cheminFichierFavoris)
        {
            _cheminFichier = cheminFichierFavoris;
        }

        public Task<IList<Livre>> ChargerFavorisAsync(string emailClient)
        {
            // TODO : à implémenter plus tard
            throw new NotImplementedException();
        }

        public Task MettreAJourFavoriAsync(string emailClient, Livre livre)
        {
            // TODO : à implémenter plus tard
            throw new NotImplementedException();
        }
    }
}
