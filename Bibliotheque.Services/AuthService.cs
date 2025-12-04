using System;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheque.Model;

namespace Bibliotheque.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBibliothequeXmlService _bibliothequeService;

        // la seule facon pour ce connecter en tant qu'Admin
        private const string AdminEmail = "admin@exemple.com";
        private const string AdminMotDePasse = "420-3GP || 3GP";

        public AuthService(IBibliothequeXmlService bibliothequeService)
        {
            _bibliothequeService = bibliothequeService;
        }

        // retourne resultat de l'authentification 
        public async Task<ResultatAuthentification> AuthentifierAsync(string email, string motDePasse)
        {
            var resultat = new ResultatAuthentification();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(motDePasse))
            {
                resultat.EstValide = false;
                resultat.Message = "Veuillez entrer un courriel et un mot de passe."; // si resultat pas bon
                return resultat;
            }

            email = email.Trim();

            // Cas administrateur
            if (string.Equals(email, AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                if (motDePasse == AdminMotDePasse)
                {
                    resultat.EstValide = true;
                    resultat.Compte = new Compte
                    {
                        Email = AdminEmail,
                        MotDePasse = AdminMotDePasse,
                        Nom = "Administrateur",
                        Prenom = "Principal",
                        Role = RoleCompte.Administrateur
                    };
                    return resultat;
                }

                resultat.EstValide = false;
                resultat.Message = "Mot de passe administrateur incorrect.";
                return resultat;
            }

            // Cas client 
            var comptes = await _bibliothequeService.ChargerComptesAsync();

            var compte = comptes.FirstOrDefault(c =>
                string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase));

            if (compte == null)
            {
                resultat.EstValide = false;
                resultat.Message = "Aucun compte trouvé avec ce courriel.";
                return resultat;
            }

            if (!string.IsNullOrEmpty(compte.MotDePasse) 
                &
                !string.Equals(compte.MotDePasse, motDePasse))
            {
                resultat.EstValide = false;
                resultat.Message = "Mot de passe incorrect.";
                return resultat;
            }

            // conpte defini comme client et non admin
            compte.Role = RoleCompte.Client;

            resultat.EstValide = true;
            resultat.Compte = compte;
            resultat.Message = string.Empty;
            return resultat;
        }
    }
}
