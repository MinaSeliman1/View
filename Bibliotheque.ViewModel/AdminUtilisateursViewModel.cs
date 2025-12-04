using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class AdminUtilisateursViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _bibliothequeService;
        private string _message = string.Empty;

        public AdminUtilisateursViewModel(IBibliothequeXmlService bibliothequeService)
        {
            _bibliothequeService = bibliothequeService;
            ComptesClients = new ObservableCollection<Compte>();
            ChargerComptesCommand = new Command(async () => await ChargerComptesAsync(), () => !EstEnChargement);
        }

        public ObservableCollection<Compte> ComptesClients { get; }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand ChargerComptesCommand { get; }

        public async Task ChargerComptesAsync()
        {
            if (EstEnChargement)
                return;

            try
            {
                EstEnChargement = true;
                ComptesClients.Clear();

                var comptes = await _bibliothequeService.ChargerComptesAsync();
                foreach (var c in comptes)
                {
                    c.Role = RoleCompte.Client;
                    ComptesClients.Add(c);
                }
            }
            finally
            {
                EstEnChargement = false;
                if (ChargerComptesCommand is Command cmd)
                    cmd.ChangeCanExecute();
            }
        }
    }
}
