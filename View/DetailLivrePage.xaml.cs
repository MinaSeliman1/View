using System;
using System.IO;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View;

[QueryProperty(nameof(Isbn), "isbn")]
public partial class DetailLivrePage : ContentPage
{
    private readonly DetailLivreViewModel _viewModel;
    private string _isbn = string.Empty;

    public string Isbn
    {
        get => _isbn;
        set
        {
            _isbn = Uri.UnescapeDataString(value ?? string.Empty);
            _ = _viewModel.ChargerLivreParIsbnAsync(_isbn);
        }
    }

    public DetailLivrePage()
    {
        InitializeComponent();

        var cheminXml = Path.Combine(AppContext.BaseDirectory, "bibliotheque.xml");
        var service = new BibliothequeXmlService(cheminXml);

        // Configuration du service Favoris
        var cheminFavoris = Path.Combine(AppContext.BaseDirectory, "favoris.xml");
        var favorisService = new FavorisService(cheminFavoris); // Instanciation du FavorisService

        _viewModel = new DetailLivreViewModel(service, favorisService); // Injection des deux services
        BindingContext = _viewModel;
    }

    private async void OnRetourClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ListeLivresPage");
    }
}