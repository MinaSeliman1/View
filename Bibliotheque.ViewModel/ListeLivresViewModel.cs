using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class ListeLivresViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _service;

        public ObservableCollection<Livre> Livres { get; } = new();

        public ICommand ChargerLivresCommand { get; }

        public ListeLivresViewModel(IBibliothequeXmlService service)
        {
            _service = service;
            ChargerLivresCommand = new Command(async () => await ChargerLivresAsync(), () => !EstEnChargement);
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
            }
        }
    }
}
