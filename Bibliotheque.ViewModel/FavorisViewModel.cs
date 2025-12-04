using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class FavorisViewModel : BaseViewModel
    {
        private readonly IFavorisService _favorisService;

        private string _emailClient = string.Empty;

        public FavorisViewModel(IFavorisService favorisService)
        {
            _favorisService = favorisService;
            Favoris = new ObservableCollection<Livre>();
            ChargerFavorisCommand = new Command(async () => await ChargerFavorisAsync(), () => !EstEnChargement);
            // Lecture de l'email de l'utilisateur connecté au moment de l'instanciation
            EmailClient = SessionUtilisateur.CourrielConnecte ?? string.Empty;
        }

        public string EmailClient
        {
            get => _emailClient;
            set => SetProperty(ref _emailClient, value);
        }

        public ObservableCollection<Livre> Favoris { get; }

        public ICommand ChargerFavorisCommand { get; }

        public async Task ChargerFavorisAsync()
        {
            if (EstEnChargement || string.IsNullOrWhiteSpace(EmailClient))
                return;

            try
            {
                EstEnChargement = true;
                Favoris.Clear();

                var livres = await _favorisService.ChargerFavorisAsync(EmailClient);
                foreach (var livre in livres)
                    Favoris.Add(livre);
            }
            finally
            {
                EstEnChargement = false;
                if (ChargerFavorisCommand is Command cmd)
                    cmd.ChangeCanExecute();
            }
        }
    }
}