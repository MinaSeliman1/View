using System;
using System.IO;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View;

public partial class EvaluationPage : ContentPage
{
    public EvaluationPage()
    {
        InitializeComponent();

        var cheminXml = Path.Combine(AppContext.BaseDirectory, "bibliotheque.xml");
        var service = new BibliothequeXmlService(cheminXml);

        BindingContext = new EvaluationViewModel(service);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EvaluationViewModel vm &&
            vm.ChargerLivresBienNotesCommand.CanExecute(null))
        {
            vm.ChargerLivresBienNotesCommand.Execute(null);
        }
    }

    private async void OnRetourClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminHomePage));
    }
}
