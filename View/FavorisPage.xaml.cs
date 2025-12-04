using System;
using System.IO;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View;

public partial class FavorisPage : ContentPage
{
    public FavorisPage()
    {
        InitializeComponent();

        // On peut utiliser un fichier séparé pour les favoris si tu veux
        var cheminFavoris = Path.Combine(AppContext.BaseDirectory, "favoris.xml");
        var favorisService = new FavorisService(cheminFavoris);

        var vm = new FavorisViewModel(favorisService);
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is FavorisViewModel vm &&
            vm.ChargerFavorisCommand.CanExecute(null))
        {
            vm.ChargerFavorisCommand.Execute(null);
        }
    }
}
