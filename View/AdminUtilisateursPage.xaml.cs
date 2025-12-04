using System;
using System.IO;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View;

public partial class AdminUtilisateursPage : ContentPage
{
    public AdminUtilisateursPage()
    {
        InitializeComponent();

        var cheminXml = Path.Combine(AppContext.BaseDirectory, "bibliotheque.xml");
        var xmlService = new BibliothequeXmlService(cheminXml);

        var vm = new AdminUtilisateursViewModel(xmlService);
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminUtilisateursViewModel vm &&
            vm.ChargerComptesCommand.CanExecute(null))
        {
            vm.ChargerComptesCommand.Execute(null);
        }
    }

    private async void OnRetourClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminHomePage));

    }
}
