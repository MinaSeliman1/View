using System;
using System.IO;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View;

public partial class AdminSuppressionPage : ContentPage
{
    public AdminSuppressionPage()
    {
        InitializeComponent();

        var cheminXml = Path.Combine(AppContext.BaseDirectory, "bibliotheque.xml");
        var service = new BibliothequeXmlService(cheminXml);

        BindingContext = new AdminSuppressionViewModel(service);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminSuppressionViewModel vm &&
            vm.ChargerLivresCommand.CanExecute(null))
        {
            vm.ChargerLivresCommand.Execute(null);
        }
    }

    private async void OnRetourClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminHomePage));

    }
}
