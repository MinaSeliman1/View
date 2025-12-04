using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Services;
using Bibliotheque.Model;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _service;

        private string _email = string.Empty;
        private string _motDePasse = string.Empty;
        private string _message = string.Empty;

        public LoginViewModel(IBibliothequeXmlService service)
        {
            _service = service;

            SeConnecterCommand = new Command(
                async () => await SeConnecterAsync(),
                () => !EstEnChargement
            );
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string MotDePasse
        {
            get => _motDePasse;
            set => SetProperty(ref _motDePasse, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand SeConnecterCommand { get; }

        private async Task SeConnecterAsync()
        {
            if (EstEnChargement)
                return;

            try
            {
                EstEnChargement = true;
                Message = string.Empty;

                // ===================== CAS ADMIN =====================
                if (Email == "admin@exemple.com" &&
                    MotDePasse == "420-3GP420-3GP" || MotDePasse == "3GP")
                {
                    SessionUtilisateur.CourrielConnecte = Email;
                    SessionUtilisateur.EstAdmin = true;

                    await Shell.Current.GoToAsync("AdminHomePage");
                    return;
                }

                // ===================== CAS UTILISATEUR NORMAL =====================
                var comptes = await _service.ChargerComptesAsync();

                var compte = comptes.FirstOrDefault(c =>
                    c.Email != null &&
                    c.MotDePasse != null &&
                    c.Email.Equals(Email, StringComparison.OrdinalIgnoreCase) &&
                    c.MotDePasse == MotDePasse);

                if (compte != null)
                {
                    SessionUtilisateur.CourrielConnecte = compte.Email;
                    SessionUtilisateur.EstAdmin = false;

                    await Shell.Current.GoToAsync("ListeLivresPage");
                }
                else
                {
                    Message = "Courriel ou mot de passe invalide.";
                }
            }
            finally
            {
                EstEnChargement = false;
                if (SeConnecterCommand is Command cmd)
                    cmd.ChangeCanExecute();
            }
        }
    }
}
