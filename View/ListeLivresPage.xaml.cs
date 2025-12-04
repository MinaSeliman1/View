using System;
using System.IO;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View
{
    public partial class ListeLivresPage : ContentPage
    {
        private readonly ListeLivresViewModel _viewModel;

        public ListeLivresPage()
        {
            InitializeComponent();

            var cheminXml = Path.Combine(AppContext.BaseDirectory, "bibliotheque.xml");
            var service = new BibliothequeXmlService(cheminXml);

            _viewModel = new ListeLivresViewModel(service);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // cacher le bouton retour pour les clients
            BtnRetour.IsVisible = SessionUtilisateur.EstAdmin;

            if (_viewModel.ChargerLivresCommand.CanExecute(null))
            {
                _viewModel.ChargerLivresCommand.Execute(null);
            }
        }

        private async void OnRetourClicked(object sender, EventArgs e)
        {
            if (SessionUtilisateur.EstAdmin)
            {
                // admin : retour vers l’espace admin
                await Shell.Current.GoToAsync("AdminHomePage");
            }
            else
            {
                // au cas où : client → retour à la page de login
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not Livre livre)
                return;

            var isbnEnc = Uri.EscapeDataString(livre.ISBN ?? string.Empty);
            await Shell.Current.GoToAsync($"{nameof(DetailLivrePage)}?isbn={isbnEnc}");

            if (sender is CollectionView cv)
                cv.SelectedItem = null;
        }
    }
}
