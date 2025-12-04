using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class AdminHomeViewModel
    {
        public ICommand GoToLivresCommand { get; }
        public ICommand GoToAjoutLivreCommand { get; }
        public ICommand GoToSuppressionLivreCommand { get; }
        public ICommand GoToUtilisateursCommand { get; }
        public ICommand GoToEvaluationsCommand { get; }

        public AdminHomeViewModel()
        {
            GoToLivresCommand = new Command(async () =>
                await Shell.Current.GoToAsync("ListeLivresPage"));

            GoToAjoutLivreCommand = new Command(async () =>
                await Shell.Current.GoToAsync("AdminAjoutLivrePage"));

            GoToSuppressionLivreCommand = new Command(async () =>
                await Shell.Current.GoToAsync("AdminSuppressionPage"));

            GoToUtilisateursCommand = new Command(async () =>
                await Shell.Current.GoToAsync("AdminUtilisateursPage"));

            GoToEvaluationsCommand = new Command(async () =>
                await Shell.Current.GoToAsync("EvaluationPage"));
        }
    }
}
