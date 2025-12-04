using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class EvaluationViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _service;

        public ObservableCollection<Livre> LivresBienNotes { get; } = new();

        public ICommand ChargerLivresBienNotesCommand { get; }

        public EvaluationViewModel(IBibliothequeXmlService service)
        {
            _service = service;
            ChargerLivresBienNotesCommand =
                new Command(async () => await ChargerLivresBienNotesAsync(), () => !EstEnChargement);
        }

        public async Task ChargerLivresBienNotesAsync()
        {
            if (EstEnChargement) return;

            try
            {
                EstEnChargement = true;
                LivresBienNotes.Clear();

                var tous = await _service.ChargerLivresAsync();

                foreach (var livre in tous)
                {
                    if (livre.MoyenneEvaluation >= 4.0 
                        & livre.NombreEvaluations > 0)
                        LivresBienNotes.Add(livre);
                }
            }
            finally
            {
                EstEnChargement = false;
                if (ChargerLivresBienNotesCommand is Command cmd)
                    cmd.ChangeCanExecute();
            }
        }
    }
}
