using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class AdminSuppressionViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _service;

        public ObservableCollection<Livre> Livres { get; } = new();

        public ICommand ChargerLivresCommand { get; }
        public ICommand SupprimerLivreCommand { get; }

        public AdminSuppressionViewModel(IBibliothequeXmlService service)
        {
            _service = service;
            ChargerLivresCommand = new Command(async () => await ChargerLivresAsync(), () => !EstEnChargement);
            SupprimerLivreCommand = new Command<Livre>(async l => await SupprimerLivreAsync(l), l => !EstEnChargement && l != null);
        }

        public async Task ChargerLivresAsync()
        {
            if (EstEnChargement) return;

            try
            {
                EstEnChargement = true;
                Livres.Clear();

                var livres = await _service.ChargerLivresAsync();
                foreach (var l in livres)
                    Livres.Add(l);
            }
            finally
            {
                EstEnChargement = false;
                if (ChargerLivresCommand is Command cmd)
                    cmd.ChangeCanExecute();
                if (SupprimerLivreCommand is Command cmd2)
                    cmd2.ChangeCanExecute();
            }
        }

        private async Task SupprimerLivreAsync(Livre? livre)
        {
            if (livre == null || EstEnChargement) return;

            try
            {
                EstEnChargement = true;

                await _service.SupprimerLivreParIsbnAsync(livre.ISBN);

                Livres.Remove(livre);
            }
            finally
            {
                EstEnChargement = false;
                if (SupprimerLivreCommand is Command cmd2)
                    cmd2.ChangeCanExecute();
            }
        }
    }
}
